package application.gestoreIoT;

import org.eclipse.paho.client.mqttv3.IMqttMessageListener;
import java.time.*;
import org.eclipse.paho.client.mqttv3.MqttClient;
import org.eclipse.paho.client.mqttv3.MqttException;
import org.eclipse.paho.client.mqttv3.MqttMessage;
import org.eclipse.paho.client.mqttv3.persist.MemoryPersistence;

public class GestoreIoT {
	
	private String broker = "tcp://localhost:1883";

	
	private MqttClient client = new MqttClient(this.broker, MqttClient.generateClientId(), new MemoryPersistence());;
	
	public GestoreIoT() throws MqttException{
		client.connect();
	}
	
	
	public void receiveMessage(String dataTopic) throws MqttException{
        client.subscribe(dataTopic, new IMqttMessageListener() {
			@Override
			public void messageArrived(String topic, MqttMessage message) throws Exception {
			    String data = new String(message.getPayload());
			    System.out.println("Received sensor data: " + data);
			    LocalDateTime date = LocalDateTime.now();
			    
			   int temperature = extractValue(data, "Temperatura");
			   int humidity = extractValue(data, "Umidità");
			   
			   if(temperature == 18) {
				   sendCommand("Deactivate "+ date);
			   }
			   
			   
			   
			}
		});
        
        
	}
	
	private int extractValue(String data, String fieldName) {
	    int fieldIndex = data.indexOf("'" + fieldName + "': ");
	    if (fieldIndex != -1) {
	        int startIndex = fieldIndex + ("'" + fieldName + "': ").length();
	        int endIndex = data.indexOf(",", startIndex);
	        
	        if (endIndex == -1) {
	            endIndex = data.indexOf("}", startIndex);
	        }

	        if (startIndex != -1 && endIndex != -1) {
	            String value = data.substring(startIndex, endIndex);
	            return Integer.parseInt(value);
	        }
	    }
	    
	    return 0; 
	}
	
	public void sendCommand(String command) throws MqttException{
        
        MqttMessage mqttMessage = new MqttMessage(command.getBytes());
        mqttMessage.setQos(0);
        
        client.publish("actuators/command", mqttMessage);
	}

	 public static void main(String[] args) throws MqttException {
		 GestoreIoT gIT = new GestoreIoT();
		 String dataTopic = "sensors/data";
		 String commandTopic = "actuators/command";
		 
		 while(true) {
			 gIT.receiveMessage(dataTopic);

		 }
	
	 }

}
