package application.gestoreIoT;

import java.util.Scanner;

import org.eclipse.paho.client.mqttv3.MqttMessage;

public class Main {

	public static void main(String[] args) throws Exception {
		Scanner scanner = new Scanner(System.in);

		 GestoreIoT gIT = new GestoreIoT();
		 String dataTopic = "command/sensors";
		 String dataTopicSend = "sensors/data/send/#";
		 
		 
		 String messagePayload = "payload";

		 MqttMessage mqttMessage = new MqttMessage();
		 mqttMessage.setPayload(messagePayload.getBytes());
		 
		 
		 String commandTopic = "actuators/command";
		 String scelta = "";
		 
		do {
			 System.out.println("Inserisci il periodo del giorno di cui preferisci ricevere le misurazioni");
			 System.out.println("Mattina, Pomeriggio, Sera");
			 System.out.println("Inserisci 0 per concludere la visualizzazione");
			 scelta = scanner.nextLine();
			 
			 
			gIT.sendCommand(scelta, dataTopic);
			//gIT.messageArrived(dataTopicSend, mqttMessage);
			gIT.receiveMessage(dataTopicSend, commandTopic);
			
			Thread.sleep(1000);
			
			System.out.println("Dati da mandare all'act" +gIT.getData());
			gIT.sendCommand(gIT.getData(), commandTopic);
			
		}while(!scelta.equals("0"));
		
		gIT.disconnect();
		System.exit(0);
		

		scanner.close();
	}

}
