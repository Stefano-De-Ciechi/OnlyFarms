import paho.mqtt.client as mqtt
import os

# ===== CONFIGURAZIONE =====
# TODO assegnare in modo "statico" i valori di crop_id e actuator_id in base a quale attuatore nel DB si vuole emulare

crop_id = 3          # nel DB e' una crop creata per test
actuator_id = 2      # nel DB e' un attuatore creato per test

file_path = os.path.dirname(os.path.abspath(__file__))

COMMANDS_TOPIC = f"crops/{crop_id}/actuators/commands"

# ==========================

def on_connect(client: mqtt.Client, userdata, flags, rc, properties=None):
    print("sucessfully connected to broker")
    client.subscribe(topic=COMMANDS_TOPIC, qos=2)       # QualityOfService = 2 --> "Exactly one delivery" (verso il broker, no messaggi duplicati)

def on_message(client: mqtt.Client, userdata, msg: mqtt.MQTTMessage):
    message = msg.payload.decode("utf-8")
    print(f"received message on topic {msg.topic} :: {message}")
    
    # TODO inserire qui qualche tipo di visualizzazione dell'attuatore che si attiva, es. accendere lampadina sull'emulatore philips hue
    # TODO inserire qui la modifica dei valori di umidita' sul file che emula il campo (in un loop che ogni tot. secondi aggiorna i valori nel file json)

    with open(os.path.join(file_path, "actuatorCommands.txt"), "a+") as output:
        # TODO se si hanno piu' emulatori di un sensore attivi si puo
        output.write(f"actuator n.{actuator_id}: {str(message)}\n")

if __name__ == "__main__":

    client = mqtt.Client(
        client_id=f"emulatore attuatore n.{actuator_id} coltivazione n.{crop_id}", 
        userdata=None, 
        protocol=mqtt.MQTTv5,
        transport="tcp"
    )

    client.on_connect = on_connect
    client.on_message = on_message

    try:
        client.connect(
            host="localhost",
            port=1883,
            clean_start=True        # ignora la cronologia di messaggi inviati prima della connessione
        )

        client.loop_forever()

    except ConnectionRefusedError:
        print("could not connect to broker, verify that mosquitto is running on port 1883")
        exit(-1)

    except KeyboardInterrupt:
        client.disconnect()
        client.loop_stop()
        print("\nclosing connection with the broker")

