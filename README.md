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
