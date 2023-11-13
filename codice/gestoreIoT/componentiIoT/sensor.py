import paho.mqtt.client as mqtt
import os
import json
from sys import exit, argv
from threading import Event

# ===== CONFIGURAZIONE =====
# TODO assegnare in modo "statico" i valori di crop_id e sensor_id in base a quale sensore nel DB si vuole emulare

crop_id = 3          # nel DB e' una crop creata per test
sensor_id = 3        # nel DB e' un sensore creato per test
sensor_type = "Humidity"     # "Temperature" | "Humidity"
crop_file_name = "datiSensore.json"     # nome del file che emula il campo

sleep_interval = 10     # n. di secondi di pausa tra due letture di misurazioni dal campo / file + invio messaggio su mqtt

MEASUREMENTS_TOPIC = f"crops/{crop_id}/sensors/{sensor_id}/measurements"
SYNC_TOPIC = "crops/components/sync"      # topic usato per inviare una stringa di sincronizzazione che indica al sensore in quale parte della giornata ci si trovi

# ==========================

file_path = os.path.dirname(os.path.abspath(__file__))
crop_file = os.path.join(file_path, crop_file_name)

time_of_day_values = ["Mattina", "Pomeriggio", "Sera"]
time_of_day = None

stop_simulation = Event()       # evento usato per terminare l'esecuzione del programma

def on_connect(client: mqtt.Client, userdata, flags, rc, properties=None):
    print("sucessfully connected to broker")
    client.subscribe(topic=SYNC_TOPIC, qos=2)       # QualityOfService = 2 --> "Exactly one delivery" (verso il broker, no messaggi duplicati)
    
def on_message(client: mqtt.Client, userdata, msg: mqtt.MQTTMessage):
    message = msg.payload.decode("utf-8")
    print(f"received message on topic {msg.topic} :: {message}")

    if msg.topic == SYNC_TOPIC:

        if message == "TURN_OFF":
            print(f"\nending the simulation")
            stop_simulation.set()       # interrompe il loop di read_crop_data() (ma solo al termine della prossima sleep)
        
        global time_of_day
        time_of_day = message


def read_crop_data():

        while not stop_simulation.is_set():
            
            if time_of_day in time_of_day_values and time_of_day == "Sera":
                print("sleeping")
                stop_simulation.wait(sleep_interval)
                continue        # passa all'iterazione successiva del while

            if time_of_day in time_of_day_values:
                with open(os.path.join(file_path, crop_file_name), "r") as crop_file:
                    crop_data = json.load(crop_file)

                value = crop_data[time_of_day][sensor_type]
                measuring_unit = "C°" if sensor_type == "Temperature" else "%"
                
                print(f"read measurement from crop: {value} {measuring_unit}")
                message = f"cropId={crop_id},sensorId={sensor_id},sensorType={sensor_type},value={value},measuringUnit={measuring_unit}"
                
                client.publish(
                    MEASUREMENTS_TOPIC,
                    payload=str(message),
                    qos=2,       # "Exactly one delivery"
                    retain=False
                )

                print(f'published message "{message}" on topic: {MEASUREMENTS_TOPIC}')

            stop_simulation.wait(sleep_interval)        # funziona come una sleep, ma puo' essere interrotta se l'evento viene "triggerato" con il metodo .set()

        client.disconnect()
        client.loop_stop()
        print("\nclosing connection with the broker")

if __name__ == "__main__":

    # controlla subito se il file .json che emula il campo esiste o meno
    if not os.path.exists(crop_file) or not os.path.isfile(crop_file):
        print("can't find crop simulation file")
        exit(-1)

    if (len(argv) != 4):
        print(f"no values passed from cli, using cropId={crop_id}, sensorId={sensor_id}, sensorType={sensor_type}")
    else:
        crop_id = int(argv[1])
        sensor_id = int(argv[2])
        sensor_type = str(argv[3])
        MEASUREMENTS_TOPIC = f"crops/{crop_id}/sensors/{sensor_id}/measurements"
        print(f"using values passed from cli, cropId={crop_id}, sensorId={sensor_id}, sensorType={sensor_type}")

    client = mqtt.Client(
        client_id=f"emulatore sensore n.{sensor_id} coltivazione n.{crop_id}",
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

        # loop_start esegue il client mqtt su un thread separato, la variabile client rimane comunque accessibile (e' possible chiamare il metodo client.publish)
        client.loop_start()
        
        # qui inizia il loop di lettura dei dati (l'oggetto client si puo' ancora usare e fa sempre riferimento al client mqtt)
        read_crop_data()

    except ConnectionRefusedError:
        print("could not connect to broker, verify that mosquitto is running on port 1883")
        exit(-1)

    except KeyboardInterrupt:     # TODO ogni tanto quando si interrompe il programma compare un errore del tipo "Exception ignored in: <module 'threading'>", ma il programma termina comunque e chiude tutte le connesioni
        client.disconnect()
        client.loop_stop()
        print("\nclosing connection with the broker")
