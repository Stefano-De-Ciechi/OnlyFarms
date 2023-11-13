# AA22-23-Gruppo13

# ===== parte di Applicazioni Web =====
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



# ===== PISSIR =====

## Istruzioni per il setup

### Cosa serve:
- per poter eseguire il corretto funzionamento del progetto riguardante il gestoreIoT, occorre utilizzare la versione 17 di Java e la versione 11 o superiore di Python.

### Setup iniziale:
1) Clonare questa repository.
2) Installare Eclipse Paho MQTT Python attraverso l'utilizzo del terminale e del comando: "pip install paho-mqtt".
3) Successivamente sarà necessario installare e avviare il broker mosquitto. All'interno dei sistemi Windows si dovrà eseguire un download dal sito ufficiale "mosquitto.org", per quanto riguarda i sistemi macOS, l'installazione può avvenire attraverso l'utilizzo del terminale e del comando: "brew install mosquitto".
4) Una volta che completati i passaggi precedenti sarà necessaria anche l'installazione di Maven. Quest'ultima avviene attraverso il download dal sito "Apache Maven" all'interno del quale vi sono presenti tutte le versioni per i diversi sistemi operativi. 
Una volta completati questi passagi l'ambiente è pronto all'uso per il lancio dell'applicazione.

### Esecuzione

## Premessa: L'applicazione è stata sviluppata e testata all'interno dei sistemi Linux e masOS.

1. Avviare mosquito attraverso l'utilizzo del comando "mosquitto" all'interno del terminale.
2. Fare in modo che la Rest API sia attiva:
   -Per attivarla seguire i passaggi:
      - "cd codice/OnlyFarms/OnlyFarms.RestApi"
      - "dotnet build"
      - "dotnet run --launch-profile https"

3. Infine avviare la classe GestoreIoT di Java e successivamente i file di Python seguendo le indicazioni da terminale.
