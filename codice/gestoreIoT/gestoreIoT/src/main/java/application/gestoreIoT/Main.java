package application.gestoreIoT;

import org.eclipse.paho.client.mqttv3.MqttException;

public class Main {

	public static void main(String[] args) throws MqttException {
		 GestoreIoT gIT = new GestoreIoT();
		 String dataTopic = "sensors/data";
		 String commandTopic = "actuators/command";
		 String cropId = "1/";
		
		 gIT.sendCommand("1", dataTopic);
		 
		while(true) {
			 gIT.receiveMessage(cropId.concat(dataTopic), cropId.concat(commandTopic));

		 }
	}

}
