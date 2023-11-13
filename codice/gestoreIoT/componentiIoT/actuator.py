import paho.mqtt.client as mqtt
import os
import json
from sys import exit, argv
from threading import Event

# ===== CONFIGURAZIONE =====

crop_id = 3          # nel DB e' una crop creata per test
actuator_id = 2      # nel DB e' un attuatore creato per test

sleep_interval = 10
humidity_increment = 1
water_usage_increment = 500

crop_file_name = "datiSensore.json"
commands_file_name = "actuatorCommands.txt"

COMMANDS_TOPIC = f"crops/{crop_id}/actuators/commands"
COMMANDS_CONFIRMATION_TOPIC = f"crops/{crop_id}/actuators/commands-confirmation"
SYNC_TOPIC = "crops/components/sync"
WATER_USAGE_TOPIC = f"crops/{crop_id}/waterUsages"

# ==========================

file_path = os.path.dirname(os.path.abspath(__file__))
crop_file = os.path.join(file_path, crop_file_name)
commands_file = os.path.join(file_path, commands_file_name)

time_of_day_values = ["Mattina", "Pomeriggio", "Sera"]
time_of_day = None

waterUsageWasSent = False       # flag usato per inviare gli utilizzi di acqua una sola volta (quando time_of_day diventa "Sera")

update_data = Event()
stop_simulation = Event()

def on_connect(client: mqtt.Client, userdata, flags, rc, properties=None):
    print("sucessfully connected to broker")
    client.subscribe(topic=COMMANDS_TOPIC, qos=2)       # QualityOfService = 2 --> "Exactly one delivery" (verso il broker, no messaggi duplicati)
    client.subscribe(topic=SYNC_TOPIC, qos=2)

def on_message(client: mqtt.Client, userdata, msg: mqtt.MQTTMessage):
    message = msg.payload.decode("utf-8")
    print(f"received message on topic {msg.topic} :: {message}")

    if msg.topic == SYNC_TOPIC:

        if message == "TURN_OFF":
            stop_simulation.set()
            update_data.set()
            print(f"\nending the simulation")

        global time_of_day
        time_of_day = message

    # invia la conferma di ricezione del comando al gestore IoT
    if msg.topic == COMMANDS_TOPIC and message in ["ON", "OFF"]:

        client.publish(
            topic=COMMANDS_CONFIRMATION_TOPIC,
            payload=f"cropId={crop_id},actuatorId={actuator_id},command={message}",
            qos=2,
            retain=False
        )
    
    # ogni attuatore stampa il proprio stato sul file ogni volta che viene cambiato
    with open(os.path.join(file_path, commands_file), "a+") as output:
        output.write(f"crop n. {crop_id} actuator n.{actuator_id}: {str(message)}\n")

    # attivazione / disattivazione attuatore
    if time_of_day in time_of_day_values and time_of_day != "Sera":

        if message == 'ON':
            update_data.set()     # reset del valore del segnale

        elif message == 'OFF':
            update_data.clear()
            print("reached ideal humidity value")

    if time_of_day in time_of_day_values and time_of_day == "Sera":
        if not update_data.is_set():
            update_data.set()

def read_water_usage() -> int:
    with open(crop_file, 'r') as file:
        data = json.load(file)
        return int(data["WaterUsage"])

def update_crop_data():
    global waterUsageWasSent

    while not stop_simulation.is_set():

        update_data.wait()      # e' come una sleep che attende di essere interrotta dal segnale

        if time_of_day in time_of_day_values and time_of_day == "Sera":

            if not waterUsageWasSent:
                msg = f"cropId={crop_id},consumedQuantity={read_water_usage()}"
                
                client.publish(
                    topic=WATER_USAGE_TOPIC,
                    payload=msg,
                    qos=2,
                    retain=False
                )

                waterUsageWasSent = True    # non inviare piu' water usage per oggi

            print("sleeping")
            stop_simulation.wait(sleep_interval)
            continue        # passa all'iterazione successiva del while

        if update_data.is_set() and not stop_simulation.is_set():
            with open(crop_file, 'r') as file:
                data = json.load(file)

            if time_of_day != "Sera":
                data[time_of_day]['Humidity'] += humidity_increment
                data["WaterUsage"] += water_usage_increment
                
                with open(crop_file, "w+", encoding='utf-8') as f:
                    json.dump(data, f, indent=4, ensure_ascii=False)

                print(f"humidity value increased to {data[time_of_day]['Humidity']}")

        stop_simulation.wait(sleep_interval)

    client.disconnect()
    client.loop_stop()
    print("\nclosing connection with the broker")

if __name__ == "__main__":

    # controlla subito se il file .json che emula il campo esiste o meno
    if not os.path.exists(crop_file) or not os.path.isfile(crop_file):
        print("can't find crop simulation file")
        exit(-1)


    if (len(argv) != 3):
        print(f"no values passed from cli, using cropId={crop_id} and actuatorId={actuator_id}")
    else:
        crop_id = int(argv[1])
        actuator_id = int(argv[2])
        COMMANDS_TOPIC = f"crops/{crop_id}/actuators/commands"
        COMMANDS_CONFIRMATION_TOPIC = f"crops/{crop_id}/actuators/commands-confirmation"
        WATER_USAGE_TOPIC = f"crops/{crop_id}/waterUsages"
        print(f"using values passed from cli, cropId={crop_id} and actuatorId={actuator_id}")

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

        # loop_start esegue il client mqtt su un thread separato, la variabile client rimane comunque accessibile (e' possible chiamare il metodo client.publish)
        client.loop_start()

        update_crop_data()

    except ConnectionRefusedError:
        print("could not connect to broker, verify that mosquitto is running on port 1883")
        exit(-1)

    except (KeyboardInterrupt, SystemExit):
        client.disconnect()
        client.loop_stop()
        print("\nclosing connection with the broker")

