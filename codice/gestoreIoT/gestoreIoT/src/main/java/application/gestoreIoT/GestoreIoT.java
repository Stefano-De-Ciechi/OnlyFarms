package application.gestoreIoT;

import org.eclipse.paho.client.mqttv3.IMqttDeliveryToken;
import org.eclipse.paho.client.mqttv3.IMqttMessageListener;
import org.eclipse.paho.client.mqttv3.IMqttToken;
import org.eclipse.paho.client.mqttv3.MqttCallback;

import java.time.*;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.ExecutionException;

import org.eclipse.paho.client.mqttv3.MqttClient;
import org.eclipse.paho.client.mqttv3.MqttConnectOptions;
import org.eclipse.paho.client.mqttv3.MqttException;
import org.eclipse.paho.client.mqttv3.MqttMessage;



public class GestoreIoT implements MqttCallback{
	
	private String broker = "tcp://localhost:1883";
	private MqttClient clientR = new MqttClient(broker, MqttClient.generateClientId());
	private MqttClient clientS = new MqttClient(broker, MqttClient.generateClientId());
    private MqttConnectOptions connOpts = new MqttConnectOptions();    
    private CompletableFuture<Void> dataReceived = new CompletableFuture<>();

	private double temperature;
	private double humidity;
	private String data;
	
	
	public GestoreIoT() throws MqttException{
		clientR.setCallback(this);
		clientS.setCallback(this);
		connOpts.setCleanSession(true);
		connOpts.setKeepAliveInterval(1000);
		clientR.connect(connOpts);
		clientS.connect(connOpts);

	}
	
	
	public void setTemperature(double temperature) {
		this.temperature = temperature;
	}
	
	public void setHumidity(double humidity) {
		this.humidity = humidity;
	}
	
	public double getTemperature() {
		return this.temperature;
	}
	
	public double getHumidity() {
		return this.humidity;
	}
	
	public void setData(String data) {
		this.data = data;
	}
	
	public String getData() {
		return this.data;
	}
	
	public void receiveMessage(String dataTopic, final String commandTopic) throws MqttException, InterruptedException{
        clientR.subscribe(dataTopic, new IMqttMessageListener() {
			@Override
			public void messageArrived(String topic, MqttMessage message) throws Exception {
				System.out.println(topic);			    
			    String received = new String(message.getPayload());
			    setData(received);			    
			    
			}
		});
        
	}
	
	private int extractValue(String data, String fieldName) {
	/*setTemperature(extractValue(getData(), "Temperatura"));
			    setHumidity(extractValue(getData(), "Umidità"));*/
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
        mqttMessage.setRetained(false);
        
        clientS.publish(topic, mqttMessage);
	}
	
	/*public void closeConnection() throws MqttException {
	    client.disconnect();
	    client.close();
	}*/


	
    public void disconnect() {
        try {
            // Disconnect from the broker
            clientR.disconnect();;;
     

            // Close the client
            clientR.close();
        } catch (Exception e) {
            e.printStackTrace();
        }
    }


	@Override
	public void connectionLost(Throwable cause) {
		// TODO Auto-generated method stub
		
	}


	@Override
	public void messageArrived(String topic, MqttMessage message) throws Exception {
		// TODO Auto-generated method stub
		
	}


	@Override
	public void deliveryComplete(IMqttDeliveryToken token) {
		// TODO Auto-generated method stub
		
	}

}
