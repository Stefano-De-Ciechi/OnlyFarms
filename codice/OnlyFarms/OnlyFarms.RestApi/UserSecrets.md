Per far funzionare l'applicazione bisogna impostare gli user-secrets:

- da Visual Studio:         tasto dx. sul progetto OnlyFarms.RestApi -> Manage User Secrets (o "Gestisci Segreti Utente")
- da JetBrains Rider:       tasto dx. sul progetto OnlyFarms.RestApi -> tools -> .NET User Secrets
- da terminale:             cd nella directory del progetto OnlyFarms.RestApi -> dotnet user-secrets init -> salvare in 
 un file secrets.json l'oggetto json definito sotto -> eseguire comando   cat ./secrets.json | dotnet user-secrets set

incollare l'oggetto json:

{
    "JwtSecretKey":"Kro4Q/8z83ZkOGtqZhCurrik7ocDgXifL2qL2scbG6E=",
    "Authentication:Schemes:Bearer:SigningKeys":[
        {
            "Id":"134741b3",
            "Issuer":"dotnet-user-jwts",
            "Value":"Kro4Q/8z83ZkOGtqZhCurrik7ocDgXifL2qL2scbG6E=",
            "Length":32
        }
    ]
}

JwtSecretKey l'ho aggiunta seguendo l'esempio del corso di AppWeb, ma per ora non viene usata, perche' seguendo un tutorial
di Microsoft viene aggiunto in automatico la parte "Authentication:Schemas...." e il programma sfrutta quelle chiavi

token generati per test:
chiave utilizzata:      Kro4Q/8z83ZkOGtqZhCurrik7ocDgXifL2qL2scbG6E=
- role=admin:           eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6InN0ZWZhbm8iLCJzdWIiOiJzdGVmYW5vIiwianRpIjoiYTFhMTcxNjMiLCJyb2xlIjoiYWRtaW4iLCJhdWQiOlsiaHR0cDovL2xvY2FsaG9zdDoxNzg1OSIsImh0dHBzOi8vbG9jYWxob3N0OjQ0MzQ0IiwiaHR0cDovL2xvY2FsaG9zdDo1MjM0IiwiaHR0cHM6Ly9sb2NhbGhvc3Q6NzA1OCJdLCJuYmYiOjE2OTYyNDU3MzEsImV4cCI6MTcwNDE5NDUzMSwiaWF0IjoxNjk2MjQ1NzMzLCJpc3MiOiJkb3RuZXQtdXNlci1qd3RzIn0.ub6JZVIVBnw9O9HZ3FDnTSj07gkRLafDcpWSJZR7ouU
- role=farmManager:     eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6InN0ZWZhbm8iLCJzdWIiOiJzdGVmYW5vIiwianRpIjoiODE5MDc2YWIiLCJyb2xlIjoiZmFybU1hbmFnZXIiLCJhdWQiOlsiaHR0cDovL2xvY2FsaG9zdDoxNzg1OSIsImh0dHBzOi8vbG9jYWxob3N0OjQ0MzQ0IiwiaHR0cDovL2xvY2FsaG9zdDo1MjM0IiwiaHR0cHM6Ly9sb2NhbGhvc3Q6NzA1OCJdLCJuYmYiOjE2OTYyNTUzNjIsImV4cCI6MTcwNDIwNDE2MiwiaWF0IjoxNjk2MjU1MzYzLCJpc3MiOiJkb3RuZXQtdXNlci1qd3RzIn0.FQA15rrFhpwAvDJNRGlS0G72l2tWHHagu1XXypJz_wM
- role=waterManager:    eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6InN0ZWZhbm8iLCJzdWIiOiJzdGVmYW5vIiwianRpIjoiYWM5ZTlmYWUiLCJyb2xlIjoid2F0ZXJNYW5hZ2VyIiwiYXVkIjpbImh0dHA6Ly9sb2NhbGhvc3Q6MTc4NTkiLCJodHRwczovL2xvY2FsaG9zdDo0NDM0NCIsImh0dHA6Ly9sb2NhbGhvc3Q6NTIzNCIsImh0dHBzOi8vbG9jYWxob3N0OjcwNTgiXSwibmJmIjoxNjk2MjU1MzY5LCJleHAiOjE3MDQyMDQxNjksImlhdCI6MTY5NjI1NTM3MCwiaXNzIjoiZG90bmV0LXVzZXItand0cyJ9.y-ugd0f7Zr-ciEKp3c_Op-pgFHaoUGD72KCJhzJIWys
- role=iotSubSystem:    eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6InN0ZWZhbm8iLCJzdWIiOiJzdGVmYW5vIiwianRpIjoiYThhMjg1ZTEiLCJyb2xlIjoiaW90U3Vic3lzdGVtIiwiYXVkIjpbImh0dHA6Ly9sb2NhbGhvc3Q6MTc4NTkiLCJodHRwczovL2xvY2FsaG9zdDo0NDM0NCIsImh0dHA6Ly9sb2NhbGhvc3Q6NTIzNCIsImh0dHBzOi8vbG9jYWxob3N0OjcwNTgiXSwibmJmIjoxNjk2MjU1MzgwLCJleHAiOjE3MDQyMDQxODAsImlhdCI6MTY5NjI1NTM4MSwiaXNzIjoiZG90bmV0LXVzZXItand0cyJ9.ZGTawl5C8QRlRVxuX7oGOaMsbW-HDSjy3pAJqF9WA0c

se per qualche motivo i token non dovessero funzionare, si possono generare entrando nella directory OnlyFarms.RestApi e usando il comando:
    dotnet user-jwts create --role [admin | farmManager | waterManager | iotSubsystem]      (inserire uno solo dei 4 ruoli!)

se si vogliono inserire dei claim aggiuntivi, oltre al ruolo:
    dotnet user-jwts create --role [admin | farmManager | waterManager | iotSubsystem] --claim "<name>=<value>" --claim "<name>=<value>" ...