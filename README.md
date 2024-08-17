# OnlyFarms 

**ATTENZIONE**: alcune funzionalita' del progetto (come l'autenticazione tramite account google) potrebbero non funzionare piu' correttamente, in quanto per generare i segreti utente (tipo ClientID e ClientSecret) era stato utilizzato l'account Google universitario probabilmente non piu' attivo.

# Informazioni:
Progetto sviluppato assieme ad altri tre studenti per due corsi universitari durante l'anno accademico 2023 / 2024, che comprende un'applicazione web scritta in C# + ASP.NET Core e una REST API (sempre in C# + .NET Core) a supporto di un gestore di dispositivi IoT scritto in Java, piu' alcuni emulatori di sensori e attuatori scritti in Python. Il progetto e' strutturato con un'architettura a microservizi.

# ===== Parte del corso di Applicazioni Web =====
La relazione sulle scelte implementative si trova nel file "Relazione.md" nella cartella /codice/OnlyFarms/OnlyFarms.WebApp

## Istruzioni per il setup

### Cosa serve:
- per poter eseguire il progetto e' necessario installare [.NET Core 7.0](https://dotnet.microsoft.com/en-us/download) (il download dovrebbe contenere SDK+Runtime+ASP.NET)

### Setup iniziale:
1) clonare questa repository
2) impostare i segreti utente per i due progetti:
   1) entrare nelle cartelle "codice/OnlyFarms/Onlyfarms.RestApi" e "codice/OnlyFarms/Onlyfarms.WebApp" e seguire le istruzioni nei due file UserSecrets.md
   
**nota**: i vari pacchetti NuGet usati vengono scaricati in automatico quando si esegueno le build dei progetti

### Esecuzione:
**PREMESSA**: l'applicazione e' stata sviluppata e testata prevalentemente su sistemi linux e macOs

**ATTENZIONE**:
su windows + Visual Studio si potrebbe presentare un problema all'avvio, in cui viene lanciata un'eccezione su alcune pagine "Scaffolded" della sezione Identity mancanti (che in realta' sono presenti);
se il problema dovesse verificarsi anche dopo una "clean build" provare ad avviare l'applicazione da terminale (nel nostro caso ha funzionato).

Per lanciare la **WebApp** da terminale:
1) cd codice/OnlyFarms/OnlyFarms.WebApp
2) dotnet build
3) dotnet run --launch-profile https

Per lanciare la **Rest API** da terminale:
1) cd codice/OnlyFarms/OnlyFarms.RestApi
2) dotnet build
3) dotnet run --launch-profile https

la documentazione della REST API (generata con Swagger) puo' essere visitata navigando agli url http://localhost:5234/swagger oppure https://localhost:7058/swagger

se si decide di eseguire i progetti da qualche IDE (tipo Visual Studio o Rider) assicurarsi di eseguire il profilo HTTPS

### Credenziali di accesso per provare il sistema:

la password e' la stessa per tutti gli utenti: Pa$$w0rd

le email da utilizzare sono:

| Email           | Ruolo | Stato |
|-----------------| --- | --- |
| admin@admin.com | Admin | attivo |
| farm1@test.com  | FarmManager | attivo |
| farm2@test.com  | FarmManager | in attesa di attivazione dall'admin |
| water1@test.com | WaterManager | attivo |
| water2@test.com | WaterManager | attivo |



# ===== parte del corso Progettazione e Implementazione di Sistemi Software In Rete (PISSIR) =====

## Istruzioni per il setup

**ATTENZIONE**: per eseguire le parti di RestApi e WebApp fare riferimento alla [Parte di Applicazioni Web](#parte-di-applicazioni-web)

### Cosa serve:

**Premessa** ogni sistema operativo ha i propri metodi di installazione, cercare quello piu' corretto per ogni dipendenza indicata (es. package manager su linux, download diretto per windows, brew per macOs ecc.
)
- [maven](https://maven.apache.org/download.cgi) (build system per Java), l'ultima versione dovrebbe essere compatibile ma nel caso non dovesse funzionare scaricare una delle versioni 3.8.x
- [Java versione 17](https://www.oracle.com/java/technologies/javase/jdk17-archive-downloads.html) (verificare con il comando "java --version" che sia effetivamente in uso nel sistema la versione 17, altrimenti maven build potrebbe fallire)
- [Python 3.11](https://www.python.org/downloads/release/python-3110/) o superiore + [pip - package installer for python](https://pypi.org/project/pip/) (dovrebbe essere compreso nell'installazione)
- [Eclipse Mosquitto](https://mosquitto.org/download/) come broker MQTT

### Setup iniziale:

#### Gestore IoT (Java):
1) entrare nella cartella "codice/gestoreIoT/gestoreIoT" (verificare che sia presente il file *pom.xml*)
2) eseguire il build del progetto (dovrebbe scaricare in automatico le librerie) con il comando "mvn clean install -DskipTests"
3) se il build non va a buon fine verificare di star usando Java 17!

#### Componenti IoT (Python):
1) entrare nella cartella "codice/gestoreIoT/componentiIoT"
2) *opzionale*: creare un [virtual environment](https://docs.python.org/3/library/venv.html) se non si vuole installare la libreria "globalmente"
3) eseguire il comando "pip install paho-mqtt" (unica libreria esterna)

### Esecuzione della demo:

per questa parte e' necessario prima avere eseguito il setup della [parte di applicazioni web](#parte-di-applicazioni-web), specialmente la parte in cui si settano i **segreti utente** (altrimenti i token jwt usati nella rest api non funzioneranno)

**PREMESSA**: l'applicazione e' stata sviluppata e testata prevalentemente su sistemi linux e macOs

*l'esecuzione della demo richiede un buon numero di finestre di terminale aperte, un solo schermo potrebbe non essere sufficiente per visualizzare tutto*

1) avviare un'istanza di **mosquitto** (comando "mosquitto" in un terminale; in alcuni sistemi potrebbe essere gia' attivo e restituire un errore del tipo "porta 1883 gia' in uso)

2) avviare la **rest api**:
   - cd "codice/OnlyFarms/Onlyfarms.RestApi"
   - "dotnet build"
   - "dotnet run --launch-profile https"

   nella cartella e' presente un file **CLEAN_OnlyFarms.sqlite**, rimuovere CLEAN_ e sostituire con il file originale per avere un DB senza "valori spazzatura" potenzialmente rimasti da qualche test

3) avviare la **web app** (solo se si vuole anche la parte grafica):
   - cd "codice/OnlyFarms/Onlyfarms.WebApp"
   - "dotnet build"
   - "dotnet run --launch-profile https"

4) avviare gli emulatori dei **componenti IoT**:
   - cd "codice/gestoreIoT/componentiIoT"
   - eseguire il comando "python3 simulatoreCampo.py", che assegna dei valori random all'interno del file *cropData.json* e azzera i dati residui da esecuzioni precedenti
   - aprire (e tenere aperto) il file *cropData.json* per vedere come i valori cambiano in base allo stato degli attuatori
   - aprire (e tenere aperto) ed eventualmente cancellare il contenuto del file *actuatorCommands.txt* (questo file serve ad avere una rappresentazione visiva dello stato degli attuatori)
   - in un nuovo terminale (nella stessa directory) eseguire il comando "python3 actuator.py"
   - in un nuovo terminale (nella stessa directory) eseguire il comando "python3 sensor.py"

5) avviare il **gestore IoT**:
   - cd "codice/gestoreIoT/gestoreIoT" (deve essere presente il file *pom.xml*)
   - eseguire il comando: mvn exec:java -Dexec.mainClass="application.gestoreIoT.GestoreIoT"

### Funzionamento della demo:

la demo simula una giornata per la FarmingCompany di id=1 accessibile nella web app con le credenziali:
- username: farm1@test.com
- password: Pa$$w0rd

viene simulata una sola coltivazione (con id=1) al cui interno sono presenti un irrigatore (id=1) e un sensore di umidita' (id=1)

alcuni valori parametrici di sensori e attuatori possono essere modificati direttamente nei file python per cambiarne un po' il funzionamento (es. tempo di sleep tra due letture o quantita' di acqua erogata)

dal menu del **gestore IoT** si possono simulare le 3 parti della giornata:
- "Mattina" e "Pomeriggio" : sensori e attuatori agiscono su due set differenti di valori di temperatura e umidita'; ogni misurazione o cambio di stato viene inviato alla RestApi per essere registrato nel DB (se si sta usando l'applicazione web aggiornando la pagina si possono vedere i nuovi valori)
- "Sera" : sensori ed attuatori sono disattivati (entrano in sleep) e si invia alla Rest Api il valore di acqua consumata dalla coltivazione per quella giornata
- se si vuole ricominciare la simulazione senza terminare tutti i programmi e riavviarli si può eseguire di nuovo il comando "python3 simulatoreCampo.py" e ricominciare dalla "Mattina" o dal "Pomeriggio" (**nota**: i timestamp riportano comuqnue il giorno corrente, e la Sera il valore di acqua consumata verra' sovrascritto)
- l'ultima opzione "concludi simulazione" invia a tutti i componenti IoT un messaggio di "TURN_OFF" per interrompere automaticamente l'esecuzione del programma in python

e' possibile simulare altre aziende e coltivazioni modificando i valori dei vari id nel main di *GestoreIoT.java* e nei due file *actuator.py* e *sensor.py* (verificando prima che tali componenti esistano nel DB)
