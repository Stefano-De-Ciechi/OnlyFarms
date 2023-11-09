package application.gestoreIoT;

import org.eclipse.paho.client.mqttv3.IMqttMessageListener;
import java.time.*;
import org.eclipse.paho.client.mqttv3.MqttClient;
import org.eclipse.paho.client.mqttv3.MqttException;
import org.eclipse.paho.client.mqttv3.MqttMessage;
import org.eclipse.paho.client.mqttv3.persist.MemoryPersistence;

public class GestoreIoT {
	
	private String broker = "tcp://localhost:1883";
	private MqttClient client = new MqttClient(this.broker, MqttClient.generateClientId());
	private int temperature;
	private int humidity;
	
	
	public GestoreIoT() throws MqttException{
		client.connect();
	}
	
	
	public void setTemperature(int temperature) {
		this.temperature = temperature;
	}
	
	public void setHumidity(int humidity) {
		this.humidity = humidity;
	}
	
	public int getTemperature() {
		return this.temperature;
	}
	
	
	public void receiveMessage(String dataTopic, final String commandTopic) throws MqttException{
        client.subscribe(dataTopic, new IMqttMessageListener() {
			@Override
			public void messageArrived(String topic, MqttMessage message) throws Exception {
			    String data = new String(message.getPayload());
			    System.out.println("Received sensor data: " + data);
			    LocalDateTime date = LocalDateTime.now();
			    setTemperature(extractValue(data, "Temperatura"));
			    setHumidity(extractValue(data, "Humidity"));
			    
			}
		});
        
		if(getTemperature() == 18) {
			sendCommand("Deactivate\n", commandTopic);
			setTemperature(0);
		}
        
        
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
	
	public void sendCommand(String command, String topic) throws MqttException{
        
        MqttMessage mqttMessage = new MqttMessage(command.getBytes());
        mqttMessage.setQos(0);
        
        client.publish(topic, mqttMessage);
	}

}
