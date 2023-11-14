package application.gestoreIoT;

import org.eclipse.paho.client.mqttv3.*;

import java.nio.charset.StandardCharsets;
import java.util.HashMap;
import java.util.InputMismatchException;
import java.util.Map;
import java.util.Scanner;

public class GestoreIoT implements MqttCallback {

    // TODO impostare la costante a true quando si esegue la demo
    private static final boolean SEND_DATA_TO_REST_API = true;     // impostare a false per evitare di "intasare" il DB
    private MqttClient mqttClient;
    private final RestApiClient apiClient;
    private final HashMap<Integer, String> lastActuatorCommandPerCrop;
    private final HashMap<Integer, Integer> lastConsumedQuantityPerCrop;
    private final int availableWaterSupply;
    private int waterLimit;

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

        try {
            this.mqttClient.setCallback(this);
            this.mqttClient.connect(options);

            // i messaggi sul topic sono di questo tipo: {cropId=int,sensorId=int,sensorType=["Temperature" | "Humidity"],value=int,measuringUnit=["C°" | "%"]}
            String MEASUREMENTS_TOPIC = "crops/+/sensors/+/measurements";
            String WATER_USAGES_TOPIC = "crops/+/waterUsages";
            String COMMANDS_CONFIRMATION_TOPIC = "crops/+/actuators/commands-confirmation";

            this.mqttClient.subscribe(MEASUREMENTS_TOPIC);
            System.out.println("subscribed to topic " + MEASUREMENTS_TOPIC);

            this.mqttClient.subscribe(WATER_USAGES_TOPIC);
            System.out.println("subscribed to topic " + WATER_USAGES_TOPIC);

            this.mqttClient.subscribe(COMMANDS_CONFIRMATION_TOPIC);
            System.out.println("subscribed to topic " + COMMANDS_CONFIRMATION_TOPIC);

        } catch (MqttException e) {
            System.err.println("could not connect to broker with url = " + brokerUrl + "\ncheck if mosquitto is running on port 1883");
            System.exit(-1);
        }

        this.lastActuatorCommandPerCrop = new HashMap<>();
        this.lastConsumedQuantityPerCrop = new HashMap<>();

        this.availableWaterSupply = getAvailableWaterSupply();
        this.waterLimit = availableWaterSupply;
        System.out.println("\ntotale acqua disponibile per oggi (in base alle prenotazioni attive): " + availableWaterSupply + " litri");
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

        Map<String, Object> values = parseMessageValues(message);       // per accedere ai valori usare values.get("sensorId") o values.get("value") ecc., necessario fare un cast ad Integer o String

        if (values == null) {
            System.err.println("received an invalid message format from topic: " + topic);
            return;
        }

        // qui ho dovuto usare una regular expression per poter comprendere tutti i topic con lo stesso formato ma valori numerici diversi; \\d indica un qualunque numero
        if (topic.matches("crops/\\d/sensors/\\d/measurements")) {

            // qui values contiene: {cropId=int,sensorId=int,sensorType=["Temperature" | "Humidity"],value=int,measuringUnit=["C°" | "%"]}

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

            // logica per attivare o disattivare gli attuatori (publish sul topic "crops/+/actuators/commands" di un comando "ON" o "OFF")

            int totalConsumedWater = getTotalWaterUsage();
            //System.err.println("totalConsumedWater: " + totalConsumedWater);
            int lastConsumedQuantity = 0;

            if (lastConsumedQuantityPerCrop.containsKey(cropId)) {
                lastConsumedQuantity = lastConsumedQuantityPerCrop.get(cropId);
                //System.err.println("lastConsumedQuantity: " + lastConsumedQuantity);
            }

            if (lastActuatorCommandPerCrop.containsKey(cropId) && lastActuatorCommandPerCrop.get(cropId).equals("ON")) {
                waterLimit = (availableWaterSupply - totalConsumedWater - lastConsumedQuantity);
            }
            
            System.out.println("acqua rimanente: " + (availableWaterSupply - totalConsumedWater) + " / " + availableWaterSupply);

            if (sensorType.equals("Humidity")) {
                MqttMessage m;
                command = (currentHumidity < idealHumidity && waterLimit > 0) ? "ON" : "OFF";

                if (waterLimit < lastConsumedQuantity) {
                    System.out.println("RISERVA IDRICA AL LIMITE");
                }

                if (lastActuatorCommandPerCrop.containsKey(cropId)) {
                    // lastActuatorCommandPerCrop immagazzina l'ultimo comando ricevuto, se il nuovo comando e' uguale al precedente non lo si invia (cosi' si evitano messaggi duplicati anche nel DB)
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

            System.out.println("\n=======================\n");

        }

        if (topic.matches("crops/\\d/waterUsages")) {
            // qui values contiene: {cropId=int,waterUsage=int}

            int cropId = (int) values.get("cropId");
            int waterUsage = (int) values.get("consumedQuantity");
            int spilledWater = (int) values.get("spilledWater");

            lastConsumedQuantityPerCrop.put(cropId, spilledWater);

            if (!sendWaterUsageToApi(cropId, waterUsage)) {
                System.err.println("could not send waterUsage to Rest API, verify that the application is running on http://localhost:5234 and that you have a valid JwtToken");
                return;
            }
        }

        /* questo topic e' usato per confermare la ricezione di un comando da parte di un attuatore (il comando viene rispedito al gestore IoT, e al suo arrivo viene anche registrato nel DB con una chiamata alla rest api */
        if (topic.matches("crops/\\d/actuators/commands-confirmation")) {

            // qui values contiene {cropId=int,actuatorId=int,command=String}

            int cropId = (int) values.get("cropId");
            int actuatorId = (int) values.get("actuatorId");
            String command = (String) values.get("command");

            if (!lastActuatorCommandPerCrop.get(cropId).equals(command)) {
                System.err.println("mismatch between command previously sent and confirmation received; try to reset the simulation");
                return;
            }

            // chiamata alla RestApi per registrare il comando mandato all'attuatore
            if (!sendActuatorCommandToApi(cropId, actuatorId, command)) {
                System.err.println("could not send command to Rest API, verify that the application is running on http://localhost:5234 and that you have a valid JwtToken");
            }
        }
    }

    @Override
    public void deliveryComplete(IMqttDeliveryToken iMqttDeliveryToken) {

    }

    private Map<String, Object> parseMessageValues(String message) {
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

    private int getAvailableWaterSupply() {
        return this.apiClient.getAvailableWaterSupply();
    }

    // consumedWaterTMP e' usata solo quando l'accesso alla rest api e' disattivato (per emulare i valori di WaterUsage che incrementano man mano)
    static int consumedWaterTMP = 0;
    private int getTotalWaterUsage() {
        if (!SEND_DATA_TO_REST_API) {
            return consumedWaterTMP;
        }
        return this.apiClient.getTotalWaterUsage(null);     // null perche' ci interessa l'utilizzo di oggi
    }

    private boolean sendSensorMeasurementToApi(Map<String, Object> values) {
        if (!SEND_DATA_TO_REST_API) {
            System.out.println("emulating a POST request to api for sensor measurement with body: " + values);
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

    private boolean sendActuatorCommandToApi(int cropId, int actuatorId, String command) {
        if (!SEND_DATA_TO_REST_API) {
            System.out.println("emulating a POST request to api for actuator " + actuatorId + " with command = " + command);
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

    private boolean sendWaterUsageToApi(int cropId, int waterUsage) {
        if (!SEND_DATA_TO_REST_API) {
            System.out.println("emulating a POST request to api with waterUsage = " + waterUsage);
            consumedWaterTMP += waterUsage;
            return true;
        }

        return this.apiClient.sendWaterUsage(cropId, waterUsage);
    }

    public static void main(String[] args) {
        GestoreIoT gestoreIoT = new GestoreIoT(1, "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6Ijg3MjEwYjViLTZkZDgtNDQxYy1hNzhiLWUyZDJiYTBjZTAyYSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJmYXJtMUB0ZXN0LmNvbSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6ImZhcm0xQHRlc3QuY29tIiwiQXNwTmV0LklkZW50aXR5LlNlY3VyaXR5U3RhbXAiOiJBRUpYREU0SkhaNE9VWjUyQkhZN0VSRFJLSlI3RTZWNyIsImFtciI6InB3ZCIsIlJvbGVzIjoiaW90U3ViU3lzdGVtIiwiZXhwIjoxNzMxMTczMDAxfQ.tLgiuzypgfRiNqKe8gjyvColE50Xnxfj6jKkY1wZUI8");    // nel DB, farmingCompanyId=3 fa riferimento ad una farmingCompany creata per eseguire test
        gestoreIoT.showMenu();
    }
}
