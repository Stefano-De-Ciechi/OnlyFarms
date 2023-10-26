Per far funzionare l'applicazione bisogna impostare gli user-secrets:

- da Visual Studio:         tasto dx. sul progetto OnlyFarms.RestApi -> Manage User Secrets (o "Gestisci Segreti Utente")
- da JetBrains Rider:       tasto dx. sul progetto OnlyFarms.RestApi -> tools -> .NET User Secrets
- da terminale:             cd nella directory del progetto OnlyFarms.RestApi -> dotnet user-secrets init -> salvare in
  un file secrets.json l'oggetto json definito sotto -> eseguire comando   cat ./secrets.json | dotnet user-secrets set

incollare l'oggetto json:

{
    "Authentication" :
    {
        "Google" :
        {
            "ClientId" : "913049808548-g7p8m5ccc2d6l1fr7mrq2om9mpm28l1j.apps.googleusercontent.com",
            "ClientSecret" : "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"
        }
    },
    "Admin" :
    {
        "UserName" : "admin@admin.com",
        "Password" : "Pa$$w0rd"
    }
}

ClientId e ClientSecret di Google generati seguendo il tutorial: https://learn.microsoft.com/it-it/aspnet/core/security/authentication/social/google-logins?view=aspnetcore-7.0
per generarli ho usato l'account google dell'universita'; NON condividere questi dati con nessuno