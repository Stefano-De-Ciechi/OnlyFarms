import json
import threading
import time

import paho.mqtt.client as mqtt

message_received_event = threading.Event()


def on_connect(client, userdata, flags, rc, properties=None):
    print("Connected with result code "+str(rc))
    client.subscribe("sensors/data", qos=0);

def load_data(filename):
    try:
        with open(filename,'r') as file:
            data = json.load(file)
        return data
    except FileNotFoundError:
        return None

cropId = None
def on_message(client, userdata, msg):
    print("Received cropId on topic " + msg.topic + ": " + str(msg.payload))
    global cropId
    cropId = str(msg.payload)

    message_received_event.set()


def on_publish(client, userdata, mid):
    print("Message published")

client = mqtt.Client(client_id="", userdata=None, protocol=mqtt.MQTTv5, transport="tcp")
client.on_connect = on_connect
client.on_publish = on_publish
client.on_message = on_message

client.connect("localhost", 1883, 60)
client_thread = threading.Thread(target=client.loop_forever)
client_thread.start()


while cropId is None:
    time.sleep(1)


def publish_message():

    #ogni 10 secondi vengono mandati i dati del file datiSensori
    while True:
        dayTime = input("Select the time of the day that you like to receive data from: [Mattina, Pomeriggio, Sera]");
        filename = "datiSensore.json"
        weatherData = load_data(filename)


        if weatherData:
            if dayTime in weatherData:
                data = weatherData[dayTime]
                client.publish(f"{cropId[2]}/sensors/data", payload=str(data), qos=0, retain=False)
        else:
            print("è null")

        #while True:
         #   client.publish("sensors/data", payload="ciao", qos=0, retain=False)
          #  time.sleep(10)


#viene inizializzato un nuovo thread per spedire periodicamente informazioni
publish_thread = threading.Thread(target=publish_message)
publish_thread.start()


#client.loop_forever()



