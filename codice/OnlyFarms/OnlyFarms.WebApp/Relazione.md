le istruzioni per il setup sono contenute nel README.md nella cartella principale della repository
(il file contiene anche le credenziali di accesso per diversi tipi di utente)

# Note Significative:

pur non facendo parte dell'esame di Applicazioni Web, anche la parte di REST API e' stata implementata in .NET Core; 
se il docente e' interessato puo' dare un'occhiata anche a quel progetto (in codice/OnlyFarms/OnlyFarms.RestApi)

alla consegna di questa parte del progetto, la "logica" del Gestore IoT (richiesta per l'esame di PISSIR) non e'
ancora stata terminata e quindi tutti i dati nel DB relativi a sensori e attuatori sono stati inseriti manualmente e non seguono
dei comportamenti sensati (es. non c'e' una vera e propria correlazione tra le misurazioni di umidita' e lo stato dei sensori in un 
determinato istante di tempo)

### Differenze con le specifiche:

"Distinguere le due tipologie di utenti: gestori del servizio idrico (GSI), gestori delle singole aziede (GA)."

nella nostra applicazione sono presenti 3 tipi di utente:
  - Admin 
  - FarmManager (GA)
  - WaterManager (GSI)

Possono esistere multipli utenti per ogni ruolo, ma ogni utente FarmManager/WaterManager e' legato 
ad una sola azienda e viceversa (relazione 1:1 tra utente e azienda)

---

"Prevedere, durante la fase di setup del sistema, almeno un utente con ruolo “GSI”,
automaticamente abilitato al sistema."

nel nostro caso, l'utente automaticamente abilitato al sistema e' l'utente Admin

---

"La richiesta di adesione al servizio da parte di un’azienda avviene attraverso la compilazione di
un apposito form di registrazione che richiede il nome dell’azienda e l’indirizzo mail (user
name OAuth2). Le richieste di adesione possono essere valutate da un qualunque utente con
ruolo “GSI” che può quindi decidere se accettare o meno. In caso di accettazione, l’utente,
indicato nel form, potrà autenticarsi e diventerà il gestore della relativa azienda."

nel nostro caso le richieste di adesione sono valutate dagli utenti di tipo Admin

---

"Proteggere le API REST con token JWT generabili da un’apposita pagina, previa autenticazione
dell’utente che deve possedere il ruolo “GSI”."

nel nostro caso qualunque utente autorizzato al sistema, dopo aver eseguito il login, puo' generare
dei token JWT per poter avanzare richieste alla Rest API (i FarmManager possono anche generare un
token da passare al proprio sottosistema IoT)

# Scelte implementative:

## Front-End:
La parte "visiva" dell'applicazione utilizza Razor Pages + Bootstrap

Tutte le pagine usano layout abbastanza standard e semplici (immagine di copertina, div con classi "row" e "col" per posizionare i dati in una griglia e tabelle per visualizzare i dati)

Alcune pagine fanno uso del componente "Modal" di Bootstrap per far apparire dei form, in quei casi
sono stati aggiunti dei piccoli script in JavaScript per prelevare dinamicamente i dati in base al "componente trigger" premuto
e passarli al backend con i flag "asp-for" ecc. 
(vedere ad esempio OnlyFarms.WebApp/Pages/FarmManager/Reservations/Reservation.cshtml)

Per la parte "Identity" abbiamo sfruttato lo Scaffolding delle pagine Login, ExternalLogin, Logout e Registration, modificandole leggermente non sul lato estetico ma solo funzionale
(es. aggiunti dei campi aggiuntivi al form di registrazione)

Quando un utente esegue il login viene reindirizzato alla pagina "profilo" della propria azienda (se e' un Admin viene reindirizzato alla pagina da cui puo' gestire gli accessi alla piattaforma)

nota: abbiamo provato ad impostare un font esterno per avere un look uniforme tra le varie piattaforme e browser, ma a quanto pare il font non viene sempre usato
(quando il font viene applicato il sito risulta leggermente piu' gradevole alla vista, con le scritte spaziate in maniera uniforme tra di loro, mentre con altri font di default rimane tutto un po' piu' schiacciato)

## Back-End:

L'applicazione e' strutturata in 3 progetti separati:
- OnlyFarms.Core contiene la definizione dei dati e delle varie repository (usato il Repository Pattern) + la definizione del DataContext + qualche metodo accessorio (es. extension points) e qualche valore costante
- OnlyFarms.WebApp contiene la definizione delle varie Razor Pages
- OnlyFarms.RestApi (anche se non fa parte del progetto d'esame di questo corso) contiene il DB + la definizione degli endpoint + generazione pagina Swagger

### Gestione dei dati:
L'approccio alla gestione dei dati e' di tipo Code-First + Repository Pattern.

L'ORM utilizzato e' Entity Framework.

Il DB e' stato posizionato nel progetto OnlyFarms.RestApi perche' inizialmente avevamo pensato di accedere ai dati solo tramite richieste HTTP; in seguito pero'
abbiamo optato per un accesso diretto dei dati dalla WebApp al DB (nella nostra architettura i due programmi WebApp e RestApi girano sullo stesso server e quindi hanno entrambi
la possibilita' di accesso diretto al file OnlyFarms.Sqlite)

Sarebbe comunque possibile (sfruttando l'astrazione del Repository Pattern) creare nuove repository che interagiscano con la Rest API tramite un HttpClient (rimpiazzando cosi' l'accesso diretto dato da Entity Framework)

Le tabelle del DB (Sqlite) sono state create partendo dalle classi modello definite in OnlyFarms.Core/models/ e la coerenza tra tabelle e modelli e' stata
mantenuta applicando una serie di Migrazioni (e' possibile vederne la lista eseguendo il comando "dotnet ef migrations list" dalla cartella OnlyFarms.RestApi)

RestApi e WebApp sfruttano le stesse repository per l'accesso ai dati, ma il progetto WebApp contiene in piu' la definizione della repository per
la generazione di Token JWT

Alcune repository (e alcuni endpoint della RestApi) sfruttano i tipi generici per ridurre al minimo la duplicazione di codice.

### Sicurezza / Accesso ai dati:
L'applicazione sfrutta una logica di accesso ai dati tramite Autenticazione + Autorizzazione "semplificata":

- Autenticazione: un utente puo' eseguire l'accesso al profilo della propria azienda tramite username+password o tramite OAuth 2 (accesso con account Google)
- Autorizzazione: si sfrutta un approccio di tipo Claim-Based per assegnare ad ogni utente un attributo che ne indica il tipo (Admin, FarmManager, WaterManager), ad ogni pagina e' stata applicata
una policy che controlla se l'utente corrente possegga o meno un claim con il tipo adeguato.

E' "semplificata" perche' teoricamente un utente di tipo FarmManager puo' accedere alle risorse di altri utenti dello stesso tipo se ne conosce gli id e leggere/modificare i dati (stessa cosa per gli utenti WaterManager).
Per risolvere questo problema (e avere accesso esclusivo alle risorse legate al proprio profilo) si dovrebbero implementare degli Authorization-handler piu' complessi con controlli sugli id delle risorse coinvolte
e tabelle del DB in cui compaiono campi aggiuntivi che fanno riferimento al proprio proprietario (es. colonna OwnerId).

Per associare un utente alla propria azienda nella tabella AspNetUsers del DB sono state aggiunte 3 colonne, companyId, companyType e companyName. 
CompanyType viene usato come "discriminante" per capire a quale tabella (FarmingCompanies o WaterCompanies) associare companyId, ma questa logica e' implementata "manualmente" nelle varie pagine 
(es. se l'utente loggato e' di tipo WaterManager, si usera' companyId all'interno della repository delle WaterCompanies)

Per rendere la registrazione di un utente effettiva, l'admin agisce sul campo EmailConfirmed della tabella AspNetUsers (visto che non abbiamo la conferma via email)