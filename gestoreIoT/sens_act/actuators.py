import paho.mqtt.client as mqtt

def on_connect(client, userdata, flags, rc, properties=None):
    print("Connected with result code "+str(rc))

    # Subscribing in on_connect() means that if we lose the connection and
    # reconnect then subscriptions will be renewed.
    client.subscribe("actuators/command", qos=0);

# The callback for when a PUBLISH message is received from the server.
def on_message(client, userdata, msg):
    print("Received message on topic " + msg.topic + ": " + str(msg.payload))
    writeFile = open("actCommands.txt", "a")
    writeFile.write(str(msg.payload)+"\n")


client = mqtt.Client(client_id="", userdata=None, protocol=mqtt.MQTTv5, transport="tcp")
client.on_connect = on_connect
client.on_message = on_message


client.connect("localhost", 1883, 60)
client.loop_forever()


