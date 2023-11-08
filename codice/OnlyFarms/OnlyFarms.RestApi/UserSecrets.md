Per far funzionare l'applicazione bisogna impostare gli user-secrets:

- da Visual Studio:         tasto dx. sul progetto OnlyFarms.RestApi > Manage User Secrets (o "Gestisci Segreti Utente")
- da JetBrains Rider:       tasto dx. sul progetto OnlyFarms.RestApi > tools > .NET User Secrets
- da terminale: 
  1) cd codice/OnlyFarms/OnlyFarms.RestApi
  2) verificare se nella cartella e' presente il file secrets.json, altrimenti crearne uno ed incollare l'oggetto json definito sotto 
  3) dotnet user-secrets init
  4)
     - linux/macOs: cat ./secrets.json | dotnet user-secrets set
     - windows: type .\secrets.json | dotnet user-secrets set

incollare l'oggetto json seguente (ignorare le stringhe di markdown ```json se visibili):

```json
{
    "JwtSecretKey":"Kro4Q/8z83ZkOGtqZhCurrik7ocDgXifL2qL2scbG6E="
}
```

i token JWT possono essere generati direttamente dalla WebApp (dopo aver fatto il setup anche di quel progetto, vedi README.md nella cartella iniziale della repository)
eseguire l'accesso con uno degli account di prova (o crearne uno proprio) e poi andare nella pagina/sezione API Tokens