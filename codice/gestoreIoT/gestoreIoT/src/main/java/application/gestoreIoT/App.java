package application.gestoreIoT;

import org.eclipse.paho.client.mqttv3.IMqttMessageListener;
import org.eclipse.paho.client.mqttv3.MqttClient;
import org.eclipse.paho.client.mqttv3.MqttException;
import org.eclipse.paho.client.mqttv3.MqttMessage;
import org.eclipse.paho.client.mqttv3.persist.MemoryPersistence;

public class App 
{
	 public static void main(String[] args) throws MqttException {
	        String broker = "tcp://localhost:1883";
	        String dataTopic = "sensors/data";
	        String commandTopic = "actuators/command";

	        MqttClient client = new MqttClient(broker, MqttClient.generateClientId(), new MemoryPersistence());
	        client.connect();

	        client.subscribe(dataTopic, new IMqttMessageListener() {
				@Override
				public void messageArrived(String topic, MqttMessage message) throws Exception {
				    String data = new String(message.getPayload());
				    // Process sensor data
				    System.out.println("Received sensor data: " + data);
				}
			});
	       

	        

	        String command = "Activate";
	        
	        MqttMessage mqttMessage = new MqttMessage(command.getBytes());
	        mqttMessage.setQos(0);
	        
	        client.publish(commandTopic, mqttMessage);
	        
	        client.disconnect();
			client.close();
	 }
	 
	
}
