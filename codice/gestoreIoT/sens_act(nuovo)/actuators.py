import time
import paho.mqtt.client as mqtt
import threading

import sensors

#from sensors import cropId



"""
def connect_mqtt() -> mqtt:
    def on_connect(client, userdata, flags):
        client = mqtt.Client(client_id="", userdata=None, protocol=mqtt.MQTTv5, transport="tcp")
        client.on_connect = on_connect
        client.connect("localhost", 1883, 60)
        return client


def subscribe(client: mqtt):
    def on_message(client, userdata, msg):
        print(f"Received `{msg.payload.decode()}` from `{msg.topic}` topic")

    client.subscribe(f"1/actuators/command")
    client.on_message = on_message


def run():
    client = connect_mqtt()
    subscribe(client)
    client.loop_forever()


run()
"""



def on_connect(client, userdata, flags, rc, properties=None):
    print("ID")
    print("Connected with result code " + str(rc))
        # Subscribing in on_connect() means that if we lose the connection and
        # reconnect then subscriptions will be renewed.
    client.subscribe(f"1/actuators/command", qos=0);


    # The callback for when a PUBLISH message is received from the server.
def on_message(client, userdata, msg):
    print("Received message on topic " + msg.topic + ": " + str(msg.payload))
    writeFile = open("actCommands.txt", "a")
    writeFile.write(str(msg.payload) + "\n")


client = mqtt.Client(client_id="", userdata=None, protocol=mqtt.MQTTv5, transport="tcp")

client.on_connect = on_connect
client.on_message = on_message
client.connect("localhost", 1883, 60)
client.loop_forever()


"""l'attuatore riceve il messaggio di attiva insieme riceve anche l'umidità ideale. L'attuatore legge l'umiditò attuale 
ed entra in un ciclo finchè quella umidità non è raggiunta e la segna sul file. """