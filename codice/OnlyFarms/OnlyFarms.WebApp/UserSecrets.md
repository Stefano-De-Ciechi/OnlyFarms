Per far funzionare l'applicazione bisogna impostare gli user-secrets:

- da Visual Studio:         tasto dx. sul progetto OnlyFarms.RestApi -> Manage User Secrets (o "Gestisci Segreti Utente")
- da JetBrains Rider:       tasto dx. sul progetto OnlyFarms.RestApi -> tools -> .NET User Secrets
- da terminale:             cd nella directory del progetto OnlyFarms.RestApi -> dotnet user-secrets init -> salvare in
  un file secrets.json l'oggetto json definito sotto -> eseguire comando   cat ./secrets.json | dotnet user-secrets set

incollare l'oggetto json:

{
    "Admin" :
    {
        "UserName" : "admin@admin.com",
        "Password" : "Pa$$w0rd"
    }
}