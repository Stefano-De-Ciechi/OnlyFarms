# Descrizione dei topic MQTT:

# Attuatori:

ogni attuatore conosce l'id della coltivazione di cui fa parte (valore fisso impostato all'interno del programma)

## Publisher:
1.  **conferma dei comandi**: "crops/{cropId}/actuators/commands-confirmation"
    - {cropId=int,actuatorId=int,command=["ON" | "OFF"]}

2. **utilizzi di acqua**: "crops/{cropId}/waterUsages"
    - {cropId=int,consumedQuantity=int}

## Subscriber:
1. **comandi**: "crops/{cropId}/actuators/commands"
    - {command=["ON" | "OFF"]}

2. **sincronizzazione**: "crops/components/sync"
    - {"Mattina" | "Pomeriggio" | "Sera" | "TURN_OFF"}


# Sensori:

ogni sensore conosce l'id della coltivazione di cui fa parte (valore fisso impostato all'interno del programma)

## Publisher:
1. **invio delle misurazioni**: "crops/{cropId}/sensors/{sensorId}/measurements"
    - {cropId=int,sensorId=int,sensorType=["Temperature" | "Humidity"],value=int,measuringUnit=["C° | %"]}

## Subscriber:
1. **sincronizzazione**: "crops/components/sync"
    - {"Mattina" | "Pomeriggio" | "Sera" | "TURN_OFF"}


# Gestori IoT:

## Publisher:
1. **comandi**: "crops/{cropId}/actuators/commands"
    - {command=["ON" | "OFF"]}
    - per cropId si usa quello ricevuto dalle misurazioni dei sensori sulla stesa coltivazione

2. **sincronizzazione**: "crops/components/sync"
    - {"Mattina" | "Pomeriggio" | "Sera" | "TURN_OFF"}

## Subscriber:

sfruttata la wildcard + per ascoltare contemporaneamente tutte le coltivazioni dell'azienda

1.  **conferma dei comandi**: "crops/+/actuators/commands-confirmation"
    - {cropId=int,actuatorId=int,command=["ON" | "OFF"]}

2. **utilizzi di acqua**: "crops/+/waterUsages"
    - {cropId=int,consumedQuantity=int}

1. **invio delle misurazioni**: "crops/+/sensors/+/measurements"
    - {cropId=int,sensorId=int,sensorType=["Temperature" | "Humidity"] value=int,measuringUnit=["C° | %"]}