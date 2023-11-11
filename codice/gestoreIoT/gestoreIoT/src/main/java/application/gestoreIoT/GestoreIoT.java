package application.gestoreIoT;

import org.eclipse.paho.client.mqttv3.*;

import java.nio.charset.StandardCharsets;
import java.util.HashMap;
import java.util.InputMismatchException;
import java.util.Map;
import java.util.Scanner;

public class GestoreIoT implements MqttCallback {

    private MqttClient mqttClient;
    private final RestApiClient apiClient;
    private final String COMMANDS_TOPIC = "crops/+/actuators/commands";             // publisher {value=["on" | "off"]}
    private final String SYNC_TOPIC = "crops/sensors/sync";
    private final String MEASUREMENTS_TOPIC = "crops/+/sensors/+/measurements";

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
        //options.setKeepAliveInterval(1000);

        try {
            this.mqttClient.setCallback(this);
            this.mqttClient.connect(options);

            // subscriber {cropId=int,sensorId=int,sensorType=["Temperature" | "Humidity"],value=int,measuringUnit=["C°" | "%"]}

            this.mqttClient.subscribe(MEASUREMENTS_TOPIC);
            System.out.println("subscribed to topic " + MEASUREMENTS_TOPIC);

        } catch (MqttException e) {
            System.err.println("could not connect to broker with url = " + brokerUrl + "\ncheck if mosquitto is running on port 1883");
            System.exit(-1);
        }

    }

    public void showMenu() {

        Scanner scanner = new Scanner(System.in);
        int choice = -1;

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
                    // TODO sarebbe bello inviare un messaggio mqtt a tutti i componenti python e farli terminare
                    break;
                case 1:
                    sendSensorsSyncCommand("Mattina");
                    break;
                case 2:
                    sendSensorsSyncCommand("Pomeriggio");
                    break;
                case 3:
                    sendSensorsSyncCommand("Sera");
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
                System.err.println("could not send measurement to Rest API, verify that the application is running on http://localhost:5234 and that you have a valid JwtToken");
            }

            // chiamata alla RestApi per ottenere il valore di umidita ideale della crop
            int cropId = (Integer) values.get("cropId");
            int idealHumidity = apiClient.getCropIdealHumidity(cropId);
            int currentHumidity = (Integer) values.get("value");

            // TODO inserire qui la logica per attivare o disattivare i sensori
            /* deve essere qualcosa tipo
            if (values.get("sensorType").equals("Humidity") && values.get("value") < [umidita ideale] {
                this.mqttClient.publish("crops/ + cropId + "/actuators/commands" con messaggio di stato "ON")     // facciamo che tutti gli attuatori della crop ricevano il comando
            }
            else {
                stessa cosa di sopra ma con messaggio "OFF"
            }
            */

            /* per essere sicuri che un attuatore abbia ricevuto un comando, si puo' fare come negli esempi del corso e fare in modo che l'attuatore rimandi indietro il comando ricevuto
            * e solo allora si puo' mandare la registrazione del comando alla Rest Api (serve un topic nuovo e il gestore deve ascoltare anche su quel topic) */

            // chiamata alla RestApi per registrare il comando mandato all'attuatore
            if (!sendCommandToAllActuatorsToAPi(cropId, "")) {
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

    private boolean sendSensorMeasurementToApi(Map<String, Object> values) {

        int cropId = (Integer) values.get("cropId");
        int sensorId = (Integer) values.get("sensorId");
        int value = (Integer) values.get("value");
        String measuringUnit = (String) values.get("measuringUnit");

        return this.apiClient.sendSensorMeasurement(cropId, sensorId, value, measuringUnit);
    }

    private boolean sendCommandToAllActuatorsToAPi(int cropId, String command) {
        return this.apiClient.sendCommandToAllActuators(cropId, command);
    }

    private void sendSensorsSyncCommand(String timeOfDay) {
        // timeOfDay e' una stringa contenente "Mattina", "Pomeriggio" o "Sera"
        MqttMessage message = new MqttMessage(timeOfDay.getBytes());
        message.setQos(2);      // exactly one delivery

        try {
            // publisher {["Mattina" | "Pomerigio" | "Sera"]}

            this.mqttClient.publish(SYNC_TOPIC, message);

        } catch (MqttException e) {
            System.err.println("could not send the mqtt sync message");
        }
    }

    public static void main(String[] args) {
        GestoreIoT gestoreIoT = new GestoreIoT(3, null);    // nel DB, farmingCompanyId=3 fa riferimento ad una farmingCompany creata per eseguire test
        gestoreIoT.showMenu();
    }
}
