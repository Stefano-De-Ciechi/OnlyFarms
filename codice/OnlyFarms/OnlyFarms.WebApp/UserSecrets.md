Per far funzionare l'applicazione bisogna impostare gli user-secrets:

- da Visual Studio:         tasto dx. sul progetto OnlyFarms.RestApi -> Manage User Secrets (o "Gestisci Segreti Utente")
- da JetBrains Rider:       tasto dx. sul progetto OnlyFarms.RestApi -> tools -> .NET User Secrets
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
    "JwtSecretKey":"Kro4Q/8z83ZkOGtqZhCurrik7ocDgXifL2qL2scbG6E=",
    "Authentication" :
    {
        "Google" :
        {
            "ClientId" : "",
            "ClientSecret" : ""
        }
    },
    "Admin" :
    {
        "UserName" : "admin@admin.com",
        "Password" : "Pa$$w0rd"
    }
}

```

ClientId e ClientSecret di Google generati seguendo il tutorial: https://learn.microsoft.com/it-it/aspnet/core/security/authentication/social/google-logins?view=aspnetcore-7.0

per generarli ho usato l'account google dell'universita'; NON condividere questi dati con nessuno
