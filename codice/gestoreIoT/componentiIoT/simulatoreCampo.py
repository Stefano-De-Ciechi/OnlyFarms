import json
import os
from random import randint


def SetValues():
    # serve a trovare il file anche quando il programma viene eseguito partendo da un'altra cartella
    filePath = os.path.dirname(os.path.abspath(__file__)) 
    
    fileName = os.path.join(filePath, "datiSensore.json")

    with open(fileName, "r") as f:
        data = json.load(f)

    # genera i valori random in base al momento della giornta
    mattina_temperatura = randint(8, 15)
    mattina_umidita = randint(40, 45)
    pomeriggio_temperatura = randint(20, 25)
    pomeriggio_umidita = randint(45, 50)
    sera_temperatura = randint(17, 20)
    sera_umidita = randint(50, 55)
    #sera_acqua_rimanente = data["Sera"]["Acqua rimanente"]

    data.update({
        "Mattina": {
            "Temperature": mattina_temperatura,
            "Humidity": mattina_umidita
        },
        "Pomeriggio": {
            "Temperature": pomeriggio_temperatura,
            "Humidity": pomeriggio_umidita
        },
        "WaterUsage" : 0    # ogni giorno l'utilizzo dell'acqua viene resettato

    })
    # scrittura sul file json, encoding e ensure ascii servono per stampare i caratteri speciali
    with open(fileName, "w+", encoding='utf-8') as f:
        json.dump(data, f, indent=4, ensure_ascii=False)


if __name__ == "__main__":
    SetValues()
