package application.gestoreIoT;

import org.eclipse.paho.client.mqttv3.*;

import java.nio.charset.StandardCharsets;
import java.util.HashMap;
import java.util.InputMismatchException;
import java.util.Map;
import java.util.Scanner;

public class GestoreIoT implements MqttCallback {

    // TODO rimuovere sta costante quando si esegue la demo
    private static final boolean SEND_DATA_TO_REST_API = false;     // impostare a false per evitare di "intasare" il DB
    private MqttClient mqttClient;
    private final RestApiClient apiClient;
    private final HashMap<Integer, String> lastActuatorCommandPerCrop;

    public GestoreIoT(int farmingCompanyId, String jwtToken) {

        this.apiClient = new RestApiClient(farmingCompanyId, jwtToken);

        final String brokerUrl = "tcp://localhost:1883";

        try {
            this.mqttClient = new MqttClient(brokerUrl, "gestore IoT di compagnia agricola n." + farmingCompanyId, null);   // null per non avere persistenza dei messaggi
        } catch (MqttException e) {
            System.err.println("could not create an mqtt client");
            System.exit(-1);
        }

        MqttConnectOptions options = new MqttConnectOptions();
        options.setCleanSession(true);
        options.setAutomaticReconnect(true);
        //options.setKeepAliveInterval(1000);

        try {
            this.mqttClient.setCallback(this);
            this.mqttClient.connect(options);

            // i messaggi sul topic sono di questo tipo: {cropId=int,sensorId=int,sensorType=["Temperature" | "Humidity"],value=int,measuringUnit=["C°" | "%"]}
            String MEASUREMENTS_TOPIC = "crops/+/sensors/+/measurements";
            this.mqttClient.subscribe(MEASUREMENTS_TOPIC);
            System.out.println("subscribed to topic " + MEASUREMENTS_TOPIC);

        } catch (MqttException e) {
            System.err.println("could not connect to broker with url = " + brokerUrl + "\ncheck if mosquitto is running on port 1883");
            System.exit(-1);
        }

        this.lastActuatorCommandPerCrop = new HashMap<>();

    }

    public void showMenu() {

        Scanner scanner = new Scanner(System.in);
        int choice;

        do {
            System.out.println("\nInserire il periodo del giorno da simulare:\n1) Mattina\n2) Pomeriggio\n3) Sera\n0) concludi simulazione\n\nscelta: ");

            try {
                choice = scanner.nextInt();

            } catch (InputMismatchException e) {
                scanner.next();     // consuma l'input errato ed evita loop infiniti
                choice = -1;
            }

            switch (choice) {
                case 0:
                    // invia un messaggio a tutti gli emulatori connessi per terminarne l'esecuzione
                    sendComponentsSyncCommand("TURN_OFF");
                    break;
                case 1:
                    sendComponentsSyncCommand("Mattina");
                    break;
                case 2:
                    sendComponentsSyncCommand("Pomeriggio");
                    break;
                case 3:
                    sendComponentsSyncCommand("Sera");
                    break;
                default:
                    System.err.println("invalid choice");
                    break;
            }
        } while(choice != 0);

        try {
            this.mqttClient.disconnect();
            this.mqttClient.close();
        } catch (MqttException e) {
            System.err.println("could not disconnect gracefully, ending the program anyway");
            System.exit(-1);
        }

        System.out.println("closing connection with the broker");
        scanner.close();
    }

    @Override
    public void connectionLost(Throwable throwable) {
        System.err.println("connection to broker lost, check if mosquitto is still running on port 1883");
        System.exit(-1);
    }

    @Override
    public void messageArrived(String topic, MqttMessage mqttMessage) throws Exception {
        String message = new String(mqttMessage.getPayload(), StandardCharsets.UTF_8);      // decodifica il messaggio in formato UFT-8 per evitare di avere b'<messagio>', b sta per byte

        System.out.println("received message:" + message + " on topic: " + topic);

        if (topic.matches("crops/\\d/sensors/\\d/measurements")) {      // qui ho dovuto usare una regular expression per poter comprendere tutti i topic con lo stesso formato ma valori numerici diversi; \\d indica un qualunque numero

            // values contiene: {cropId=int,sensorId=int,sensorType=["Temperature" | "Humidity"],value=int,measuringUnit=["C°" | "%"]}
            Map<String, Object> values = parseSensorMeasurement(message);       // per accedere ai valori usare values.get("sensorId") o values.get("value") ecc., necessario fare un cast ad Integer o String

            if (values == null) {
                System.err.println("received an invalid message format from topic: " + topic);
                return;
            }

            // chiamata alla RestApi per registrare la misurazione letta dal sensore
            if (!sendSensorMeasurementToApi(values)) {
                System.err.println("could not send measurement to Rest API, verify that it is running on http://localhost:5234 and that you have a valid JwtToken");
            }

            // chiamata alla RestApi per ottenere il valore di umidita' ideale della crop
            int cropId = (Integer) values.get("cropId");
            int idealHumidity = getCropIdealHumidity(cropId);

            if (idealHumidity == -1) {
                System.err.println("could not retrieve humidity information from the Rest API, verify that it is running on http://localhost:5234 and that you have a valid JwtToken");
                return;
            }

            int currentHumidity = (int) values.get("value");
            String sensorType = (String) values.get("sensorType");
            String command;

            /*
            // logica per attivare o disattivare gli attuatori
            if (sensorType.equals("Humidity") && currentHumidity < idealHumidity) {
                command = "ON";
                MqttMessage m = new MqttMessage(command.getBytes());
                this.mqttClient.publish("crops/ + cropId + /actuators/commands", m);     // facciamo che tutti gli attuatori della crop ricevano il comando
            } else if (sensorType.equals("Humidity") && currentHumidity >= idealHumidity) {
                command = "OFF";
                MqttMessage m = new MqttMessage(command.getBytes());
                this.mqttClient.publish("crops/ + cropId + /actuators/commands", m);
            }*/

            // logica per attivare o disattivare gli attuatori (publish sul topic "crops/+/actuators/commands" di un comando "ON" o "OFF")
            if (sensorType.equals("Humidity")) {
                MqttMessage m;
                command = (currentHumidity < idealHumidity) ? "ON" : "OFF";

                if (lastActuatorCommandPerCrop.containsKey(cropId)) {

                    /* lastActuatorCommandPerCrop immagazzina l'ultimo comando ricevuto, se il nuovo comando e' uguale al precedente non lo si invia */
                    if (!lastActuatorCommandPerCrop.get(cropId).equals(command)) {
                        lastActuatorCommandPerCrop.put(cropId, command);
                        m = new MqttMessage(command.getBytes());
                        this.mqttClient.publish("crops/" + cropId + "/actuators/commands", m);     // facciamo che tutti gli attuatori della crop ricevano il comando
                    }
                }
                else {
                    lastActuatorCommandPerCrop.put(cropId, command);
                    m = new MqttMessage(command.getBytes());
                    this.mqttClient.publish("crops/" + cropId + "/actuators/commands", m);
                }

            }

            /* per essere sicuri che un attuatore abbia ricevuto un comando, si puo' fare come negli esempi del corso e fare in modo che l'attuatore rimandi indietro il comando ricevuto
            * e solo allora si puo' mandare la registrazione del comando alla Rest Api (serve un topic nuovo e il gestore deve ascoltare anche su quel topic) */

            // chiamata alla RestApi per registrare il comando mandato all'attuatore
            if (!sendCommandToAllActuatorsToAPi(cropId, "" /* TODO qua passare il comando "ricevuto indietro" dall'attuatore */)) {
                System.err.println("could not send command to Rest API, verify that the application is running on http://localhost:5234 and that you have a valid JwtToken");
            }
        }

    }

    @Override
    public void deliveryComplete(IMqttDeliveryToken iMqttDeliveryToken) {

    }

    private Map<String, Object> parseSensorMeasurement(String message) {
        // message e' una stringa con il seguente formato:      {cropId=int,sensorId=int,sensorType=["Temperature" | "Humidity"],value=int,measuringUnit=["C°" | "%"]}
        Map<String, Object> result = new HashMap<>();

        String[] keyValuePairs = message.split(",");

        for (String pair : keyValuePairs) {
            String[] parts = pair.split("=");

            if (parts.length != 2) {
                return null;        // TODO gestire meglio? lanciare qualche eccezione di formato invalido
            }

            String key = parts[0];
            String value = parts[1];

            Object typedValue;

            if (valueIsInteger(value)) {
                typedValue = Integer.parseInt(value);
            }
            else {
                typedValue = value;
            }

            result.put(key, typedValue);
        }

        return result;
    }

    private static boolean valueIsInteger(String value) {
        try {
            Integer.parseInt(value);
            return true;
        } catch (NumberFormatException e) {
            return false;
        }
    }

    private int getCropIdealHumidity(int cropId) {
        return this.apiClient.getCropIdealHumidity(cropId);
    }

    private boolean sendSensorMeasurementToApi(Map<String, Object> values) {
        if (!SEND_DATA_TO_REST_API) {
            System.out.println("emulating a POST request to api with body: " + values);
            return true;
        }

        int cropId = (Integer) values.get("cropId");
        int sensorId = (Integer) values.get("sensorId");
        int value = (Integer) values.get("value");
        String measuringUnit = (String) values.get("measuringUnit");

        return this.apiClient.sendSensorMeasurement(cropId, sensorId, value, measuringUnit);
    }

    private boolean sendCommandToAllActuatorsToAPi(int cropId, String command) {
        if (!SEND_DATA_TO_REST_API) {
            System.out.println("emulating a POST request to api with command = " + command);
            return true;
        }

        return this.apiClient.sendCommandToAllActuators(cropId, command);
    }

    private void sendComponentsSyncCommand(String syncCommand) {
        // syncCommand e' una stringa contenente "Mattina", "Pomeriggio", "Sera" o "TURN_OFF"
        MqttMessage message = new MqttMessage(syncCommand.getBytes());
        message.setQos(2);      // exactly one delivery

        try {
            // publish {["Mattina" | "Pomerigio" | "Sera"]}
            String SYNC_TOPIC = "crops/components/sync";
            this.mqttClient.publish(SYNC_TOPIC, message);

        } catch (MqttException e) {
            System.err.println("could not send the mqtt sync message");
        }
    }

    public static void main(String[] args) {
        GestoreIoT gestoreIoT = new GestoreIoT(3, "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6Ijg3MjEwYjViLTZkZDgtNDQxYy1hNzhiLWUyZDJiYTBjZTAyYSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJmYXJtMUB0ZXN0LmNvbSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6ImZhcm0xQHRlc3QuY29tIiwiQXNwTmV0LklkZW50aXR5LlNlY3VyaXR5U3RhbXAiOiJBRUpYREU0SkhaNE9VWjUyQkhZN0VSRFJLSlI3RTZWNyIsImFtciI6InB3ZCIsIlJvbGVzIjoiaW90U3ViU3lzdGVtIiwiZXhwIjoxNzMxMTczMDAxfQ.tLgiuzypgfRiNqKe8gjyvColE50Xnxfj6jKkY1wZUI8");    // nel DB, farmingCompanyId=3 fa riferimento ad una farmingCompany creata per eseguire test
        gestoreIoT.showMenu();
    }
}
