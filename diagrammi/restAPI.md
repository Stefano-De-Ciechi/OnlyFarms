===== Definizione REST API =====

# ===== UTENTI =====

## GET /utenti/:id
restituisce un singolo utente in base all'id

Response 200
body :
{
    "tipo" : string,
    "nomeUtente" : string,
    "autorizzazioni" : [
        string : string
    ]
}

Response 400 Bad Request
id inesistente
[errorResponse](#error-response)

Response 401 Unauthorized
autenticazione necessaria
[errorResponse](#error-response)

Response 403 Forbidden
endpoint accessibile solo da utenti amministratori
[errorResponse](#error-response)

Response 500 Server Error
[errorResponse](#error-response)

---------------------------------------------

## GET /utenti
restuisce la lista di utenti presenti nel sistema

Response 200
body :
{
    "utenti" : [
        {
            "tipo" : string,
            "nomeUtente" : string,
            "autorizzazioni" : [
                string : string
            ]
        },
        ...
    ]
}

Response 401 Unauthorized
autenticazione necessaria
[errorResponse](#error-response)

Response 403 Forbidden
endpoint accessibile solo da utenti amministratori
[errorResponse](#error-response)

Response 500 Server Error
[errorResponse](#error-response)

----------------------------------------------

## POST /utenti
aggiunta di un nuovo utente al sistema

Request
body :
{
    "tipo" : string,
    "nomeUtente" : string,
    "password" : string
    "autorizzazioni" : [
        string : string
    ]
}

Response 200
body :
{
    "id" : int      (generato dal DB)
    "tipo" : string,
    "nomeUtente" : string,
    "autorizzazioni" : [
        string : string
    ]
}

Response 400 Bad Request
errori nel body della richiesta o body inesistente
[errorResponse](#error-response)

Response 500 Server Error
[errorResponse](#error-response)

----------------------------------------------

## PUT /utenti/:id
modifica delle autorizzazioni di un utente (solo utente amministratore)

Request
body :
{
    "autorizzazioni" : [
        string : string
    ]
}

Response 200
body :
{
    "id" : int      (generato dal DB)
    "tipo" : string,
    "nomeUtente" : string,
    "autorizzazioni" : [
        string : string
    ]
}

Response 400 Bad Request
id inesistente o errori nel body della richiesta o body inesistente
[errorResponse](#error-response)

Response 401 Unauthorized
autenticazione necessaria
[errorResponse](#error-response)

Response 403 Forbidden
endpoint accessibile solo da utenti amministratori
[errorResponse](#error-response)

Response 500 Server Error
[errorResponse](#error-response)

----------------------------------------------

## DELETE /utenti/:id
rimozione di un utente dal sistema (solo utente amministratore)

Response 200
body :
{
    "id" : int      (generato dal DB)
    "tipo" : string,
    "nomeUtente" : string,
    "autorizzazioni" : [
        string : string
    ]
}

Response 400 Bad Request
id inesistente
[errorResponse](#error-response)

Response 401 Unauthorized
autenticazione necessaria
[errorResponse](#error-response)

Response 403 Forbidden
endpoint accessibile solo da utenti amministratori
[errorResponse](#error-response)

Response 500 Server Error
[errorResponse](#error-response)

# ===== AZIENDE =====
// TODO non abbiamo piu' un singolo endpoint per le aziende ma due separati in base al tipo (idrica o agricola) --> gli endpoint pero' sono pressoche' identici

## GET /aziende/:id
restituisce una singola azienda in base all'id

Response 200
body : 
{
    "tipo" : string,
    "nome" : string,
    "indirizzo" : string,
    "riservaIdrica" : float
}

Response 400 Bad Request
id inesistente
[errorResponse](#error-response)

Response 401 Unauthorized
autenticazione necessaria
[errorResponse](#error-response)

Response 500 Server Error
[errorResponse](#error-response)

---------------------------------------------------

## GET /aziende?tipo=idriche|agricole
restituisce una lista con tutte le aziende

Response 200
body : 
{
    "aziende" : [
        {
            "tipo" : string,
            "nome" : string,
            "indirizzo" : string,
            "riservaIdrica" : float
        }, 
        ...
    ]
}

Response 401 Unauthorized
autenticazione necessaria
[errorResponse](#error-response)

Response 500 Server Error
[errorResponse](#error-response)

---------------------------------------------------

## POST /aziende   
aggiunta di una nuova azienda nel sistema

Request
body : 
{
    "tipo" : string,
    "nome" : string,
    "indirizzo" : string,
    "riservaIdrica" : float
}

Response 200
body : 
{
    "id" : int      (generato dal DB)
    "tipo" : string,
    "nome" : string,
    "indirizzo" : string,
    "riservaIdrica" : float
}

Response 400 Bad Request
errori nel body della richiesta o body inesistente
[errorResponse](#error-response)

Response 401 Unauthorized
autenticazione necessaria
[errorResponse](#error-response)

Response 500 Server Error
[errorResponse](#error-response)

-----------------------------------------------------------

## PUT /aziende/:id
aggiorna un'azienda dato il suo ID ed i dati aggiornati

Request
body : 
{
    "tipo" : string,
    "nome" : string,
    "indirizzo" : string,
    "riservaIdrica" : float
}

Response 200
body : 
{
    "id" : int      (generato dal DB)
    "tipo" : string,
    "nome" : string,
    "indirizzo" : string,
    "riservaIdrica" : float
}

Response 400 Bad Request
id inesistente o errori nel body della richiesta o body inesistente
[errorResponse](#error-response)

Response 401 Unauthorized
autenticazione necessaria
[errorResponse](#error-response)

Response 500 Server Error
[errorResponse](#error-response)

-------------------------------------------------------------

## DELETE /aziende/:id
elimina un'azienda dal sistema dato il suo ID

Response 200
body : 
{
    "id" : int      (generato dal DB)
    "tipo" : string,
    "nome" : string,
    "indirizzo" : string,
    "riservaIdrica" : float
}

Response 400 Bad Request
id inesistente
[errorResponse](#error-response)

Response 401 Unauthorized
autenticazione necessaria
[errorResponse](#error-response)

Response 403 Forbidden
endpoint accessibile solo da utenti amministratori
[errorResponse](#error-response)

Response 500 Server Error
[errorResponse](#error-response)

# ===== COLTIVAZIONI =====

## GET /aziende/:id/coltivazioni/:id
restituisce una singola coltivazione di un'azienda in base all'id

Response 200
body : 
{
    "superficie" : float,
    "tipoDiIrrigazione" : string,
    "fabbisognoIdrico" : float,
    "umiditaIdeale" : float
}

Response 400 Bad Request
id inesistente dell'azienda o della coltivazione
[errorResponse](#error-response)

Response 401 Unauthorized
autenticazione necessaria
[errorResponse](#error-response)

Response 403 Forbidden
endpoint accessibile solo da utenti di tipo "gestore agricolo" appartenenti all'azienda in questione
[errorResponse](#error-response)

Response 500 Server Error
[errorResponse](#error-response)

---------------------------------------------------

## GET /aziende/:id/coltivazioni/
restituisce una lista con tutte le coltivazioni di un'azienda

Response 200
body : 
{
    "coltivazioni" : [
        {
            "superficie" : float,
            "tipoDiIrrigazione" : string,
            "fabbisognoIdrico" : float,
            "umiditaIdeale" : float
        },
        ...
    ]
}

Response 400 Bad Request
id inesistente dell'azienda
[errorResponse](#error-response)

Response 401 Unauthorized
autenticazione necessaria
[errorResponse](#error-response)

Response 403 Forbidden
endpoint accessibile solo da utenti di tipo "gestore agricolo" appartenenti all'azienda in questione
[errorResponse](#error-response)

Response 500 Server Error
[errorResponse](#error-response)

---------------------------------------------------

## POST /aziende/:id/coltivazioni   
aggiunta nuova coltivazione ad un'azienda, restituisce ID generato dal DB

Request
body : 
{
    "superficie" : float,
    "tipoDiIrrigazione" : string,
    "fabbisognoIdrico" : float,
    "umiditaIdeale" : float
}

Response 200
body : 
{
    {
        "id" : int,     (generato dal DB)
        "superficie" : float,
        "tipoDiIrrigazione" : string,
        "fabbisognoIdrico" : float,
        "umiditaIdeale" : float
    }
}

Response 400 Bad Request in caso di parametri sbagliati o omessi
id inesistente dell'azienda o errori nel body della richiesta o body inesistente
[errorResponse](#error-response)

Response 401 Unauthorized
autenticazione necessaria
[errorResponse](#error-response)

Response 403 Forbidden
endpoint accessibile solo da utenti di tipo "gestore agricolo" appartenenti all'azienda in questione
[errorResponse](#error-response)

Response 500 Server Error
[errorResponse](#error-response)

-----------------------------------------------------------

## PUT /aziende/:id/coltivazioni/:id
aggiorna i dati di una coltivazione di un'azienda dati ID e dati aggiornati

Request
body : 
{
    "superficie" : float,
    "tipoDiIrrigazione" : string,
    "fabbisognoIdrico" : float,
    "umiditaIdeale" : float
}

Response 200
body : 
{
    "id" : int      (generato dal DB)
    "superficie" : float,
    "tipoDiIrrigazione" : string,
    "fabbisognoIdrico" : float,
    "umiditaIdeale" : float
}

Response 400 Bad Request
id inesistente dell'azienda o della coltivazione o errori nel body della richiesta o body inesistente
[errorResponse](#error-response)

Response 401 Unauthorized
autenticazione necessaria
[errorResponse](#error-response)

Response 403 Forbidden
endpoint accessibile solo da utenti di tipo "gestore agricolo" appartenenti all'azienda in questione
[errorResponse](#error-response)

Response 500 Server Error
[errorResponse](#error-response)

-------------------------------------------------------------

## DELETE /aziende/:id/coltivazioni/:id
elimina una coltivazione di un'azienda dal sistema dato il suo ID

Response 200
body : 
{
    "id" : int      (generato dal DB)
    "superficie" : float,
    "tipoDiIrrigazione" : string,
    "fabbisognoIdrico" : float,
    "umiditaIdeale" : float
}

Response 400 Bad Request
id inesistente dell'azienda o della coltivazione
[errorResponse](#error-response)

Response 401 Unauthorized
autenticazione necessaria
[errorResponse](#error-response)

Response 403 Forbidden
endpoint accessibile solo da utenti di tipo "gestore agricolo" appartenenti all'azienda in questione
[errorResponse](#error-response)

Response 500 Server Error
[errorResponse](#error-response)

# ===== SENSORI =====

## GET /aziende/:id/coltivazioni/:id/sensori/:id
restituisce un sensore di una coltivazione di un'azienda

Response 200
body : 
{
    "tipo" : string         (es. sensore di temperatura o di umidita')
}

Response 400 Bad Request
id dell'azienda, id della coltivazione o id del sensore inesistenti
[errorResponse](#error-response)

Response 401 Unauthorized
autenticazione necessaria
[errorResponse](#error-response)

Response 403 Forbidden
endpoint accessibile solo da utenti di tipo "gestore agricolo" appartenenti all'azienda in questione
[errorResponse](#error-response)

Response 500 Server Error
[errorResponse](#error-response)

--------------------------------------------------------

## GET /aziende/:id/coltivazioni/:id/sensori
restituisce tutti i sensori di una coltivazione di un'azienda

Response 200
body :
{
    "sensori" : [
        {
            "id" : int,
            "tipo" : string     (es. sensore di temperatura o di umidita')
        },
        ...
    ]
}

Response 400 Bad Request
id dell'azienda, id della coltivazione inesistenti
[errorResponse](#error-response)

Response 401 Unauthorized
autenticazione necessaria
[errorResponse](#error-response)

Response 403 Forbidden
endpoint accessibile solo da utenti di tipo "gestore agricolo" appartenenti all'azienda in questione
[errorResponse](#error-response)

Response 500 Server Error
[errorResponse](#error-response)

--------------------------------------------------------

## POST /aziende/:id/coltivazioni/:id/sensori
registra un nuovo sensore di una coltivazione di un'azienda

Request
body :
{
    "tipo" : string
}

Response 200
body :
{
    "id" : int,
    "tipo" : string
}

Response 400 Bad Request
id dell'azienda, id della coltivazione inesistenti
[errorResponse](#error-response)

Response 401 Unauthorized
autenticazione necessaria
[errorResponse](#error-response)

Response 403 Forbidden
endpoint accessibile solo da utenti di tipo "gestore agricolo" appartenenti all'azienda in questione
[errorResponse](#error-response)

Response 500 Server Error
[errorResponse](#error-response)

--------------------------------------------------------

## DELETE /aziende/:id/coltivazioni/:id/sensori/:id
rimuove un sensore da una coltivazione di un'azienda (verra' cancellato anche lo storico delle sue misurazioni?)

Response 200
body : 
{
    "id" : int,
    "tipo" : string     (es. sensore di temperatura o di umidita')
}

Response 400 Bad Request
id dell'azienda, id della coltivazione o id del sensore inesistenti
[errorResponse](#error-response)

Response 401 Unauthorized
autenticazione necessaria
[errorResponse](#error-response)

Response 403 Forbidden
endpoint accessibile solo da utenti di tipo "gestore agricolo" appartenenti all'azienda in questione
[errorResponse](#error-response)

Response 500 Server Error
[errorResponse](#error-response)

# ===== MISURAZIONI =====

## GET /aziende/:id/coltivazioni/:id/sensori/:id/misurazioni
restituisce una lista di misurazioni di un singolo sensore da una singola coltivazione di un'azienda

Response 200
body : 
{
    "misurazioni" : [
        {
            "timestamp" : string,
            "valoreMisurato" : float,
            "unitaDiMisura" : string
        },
        ...
    ]
}

Response 400 Bad Request
id dell'azienda, id della coltivazione o id del sensore inesistenti
[errorResponse](#error-response)

Response 401 Unauthorized
autenticazione necessaria
[errorResponse](#error-response)

Response 403 Forbidden
endpoint accessibile solo da utenti di tipo "gestore agricolo" appartenenti all'azienda in questione
[errorResponse](#error-response)

Response 500 Server Error
[errorResponse](#error-response)

# ===== ATTUATORI =====

## GET /aziende/:id/coltivazioni/:id/attuatori/:id
restituisce un attuatore di una coltivazione di un'azienda

Response 200
body : 
{
    "tipo" : string         (es. irrigatore)
}

Response 400 Bad Request
id dell'azienda, id della coltivazione o id dell'attuatore inesistenti
[errorResponse](#error-response)

Response 401 Unauthorized
autenticazione necessaria
[errorResponse](#error-response)

Response 403 Forbidden
endpoint accessibile solo da utenti di tipo "gestore agricolo" appartenenti all'azienda in questione
[errorResponse](#error-response)

Response 500 Server Error
[errorResponse](#error-response)

--------------------------------------------------------

## GET /aziende/:id/coltivazioni/:id/attuatori
restituisce tutti gli attuatori di una coltivazione di un'azienda

Response 200
body :
{
    "attuatori" : [
        {
            "id" : int,
            "tipo" : string     (es. irrigatore)
        },
        ...
    ]
}

Response 400 Bad Request
id dell'azienda, id della coltivazione inesistenti
[errorResponse](#error-response)

Response 401 Unauthorized
autenticazione necessaria
[errorResponse](#error-response)

Response 403 Forbidden
endpoint accessibile solo da utenti di tipo "gestore agricolo" appartenenti all'azienda in questione
[errorResponse](#error-response)

Response 500 Server Error
[errorResponse](#error-response)

--------------------------------------------------------

## POST /aziende/:id/coltivazioni/:id/attuatori
registra un nuovo attuatore di una coltivazione di un'azienda

Request
body :
{
    "tipo" : string
}

Response 200
body :
{
    "id" : int,
    "tipo" : string
}

Response 400 Bad Request
id dell'azienda, id della coltivazione inesistenti o errori nel body della richiesta o body inesistente
[errorResponse](#error-response)

Response 401 Unauthorized
autenticazione necessaria
[errorResponse](#error-response)

Response 403 Forbidden
endpoint accessibile solo da utenti di tipo "gestore agricolo" appartenenti all'azienda in questione
[errorResponse](#error-response)

Response 500 Server Error
[errorResponse](#error-response)

--------------------------------------------------------

## DELETE /aziende/:id/coltivazioni/:id/attuatori/:id
rimuove un attuatore da una coltivazione di un'azienda (verra' cancellato anche lo storico dei comandi/stati?)

Response 200
body : 
{
    "id" : int,
    "tipo" : string     (es. sensore di temperatura o di umidita')
}

Response 400 Bad Request
id dell'azienda, id della coltivazione o id dell'attuatore inesistenti
[errorResponse](#error-response)

Response 401 Unauthorized
autenticazione necessaria
[errorResponse](#error-response)

Response 403 Forbidden
endpoint accessibile solo da utenti di tipo "gestore agricolo" appartenenti all'azienda in questione
[errorResponse](#error-response)

Response 500 Server Error
[errorResponse](#error-response)

# ===== COMANDI =====

## GET /aziende/:id/coltivazioni/:id/attuatori/:id/comandi
restituisce lo storico degli stati di un attuatore di una coltivazione di un'azienda

Response 200
body :
{
    "stati" : [
        {
            "timestamp" : string,
            "stato" : boolean
        },
        ...
    ]
}

Response 400 Bad Request
id dell'azienda, id della coltivazione o id dell'attuatore inesistenti
[errorResponse](#error-response)

Response 401 Unauthorized
autenticazione necessaria
[errorResponse](#error-response)

Response 403 Forbidden
endpoint accessibile solo da utenti di tipo "gestore agricolo" appartenenti all'azienda in questione
[errorResponse](#error-response)

Response 500 Server Error
[errorResponse](#error-response)

-----------------------------------------------

## POST /aziende/:id/coltivazioni/:id/attuatori/:id/comandi

Request
body :
{
    "stato" : boolean
}

Response 200
body :
{
    "id" : int,
    "timestamp" : string,
    "stato" : boolean
}

Response 400 Bad Request
id dell'azienda, id della coltivazione o id dell'attuatore inesistenti
[errorResponse](#error-response)

Response 401 Unauthorized
autenticazione necessaria
[errorResponse](#error-response)

Response 403 Forbidden
endpoint accessibile solo da utenti di tipo "gestore agricolo" appartenenti all'azienda in questione
[errorResponse](#error-response)

Response 500 Server Error
[errorResponse](#error-response)

# ===== PRENOTAZIONI =====

## GET /aziende/:id/prenotazioni/:id
restituisce una prenotazione in base all'id

Response 200
body :
{
    "timestamp" : string,
    "quantitaPrenotata" : float,
    "costoTransazione" : float,
    "prenotazioneAttiva" : boolean
}

Response 400 Bad Request
id dell'azienda o della prenotazione inesistenti
[errorResponse](#error-response)

Response 401 Unauthorized
autenticazione necessaria
[errorResponse](#error-response)

Response 403 Forbidden
endpoint accessibile solo da utenti di tipo "gestore agricolo" appartenenti all'azienda in questione o da utenti
di tipo "gestore idrico" appartenenti all'azienda idrica che rifornisce l'azienda idrica in questione
[errorResponse](#error-response)

Response 500 Server Error
[errorResponse](#error-response)

--------------------------------------------------

## GET /aziende/:id/prenotazioni
restituisce una lista di prenotazioni

Response 200
body :
{
    "id" : int      (generato automaticamente dal DB)
    "timestamp" : string,
    "quantitaPrenotata" : float,
    "costoTransazione" : float,
    "prenotazioneAttiva" : boolean
}

Response 400 Bad Request
id dell'azienda inesistente
[errorResponse](#error-response)

Response 401 Unauthorized
autenticazione necessaria
[errorResponse](#error-response)

Response 403 Forbidden
endpoint accessibile solo da utenti di tipo "gestore agricolo" appartenenti all'azienda in questione o da utenti
di tipo "gestore idrico" appartenenti all'azienda idrica che rifornisce l'azienda idrica in questione
[errorResponse](#error-response)

Response 500 Server Error
[errorResponse](#error-response)

----------------------------------------------------

## POST /aziende/:id/prenotazioni
richiesta di una nuova prenotazione

Request
body :
{
   "quantitaPrenotata" : string 
}

Response 200
body :
{
    "timestamp" : string,
    "quantitaPrenotata" : float,
    "costoTransazione" : float,
    "prenotazioneAttiva" : boolean
}

Response 400 Bad Request
id dell'azienda o della prenotazione inesistenti
[errorResponse](#error-response)

Response 401 Unauthorized
autenticazione necessaria
[errorResponse](#error-response)

Response 403 Forbidden
endpoint accessibile solo da utenti di tipo "gestore agricolo" appartenenti all'azienda in questione
[errorResponse](#error-response)

Response 500 Server Error
[errorResponse](#error-response)

----------------------------------------------------

## PUT /aziende/:id/prenotazioni/:id
modifica lo stato di una prenotazione in base all'id

Request
body :
{
    "quantitaPrenotata" : float,
    "prenotazioneAttiva" : boolean
}

Response 200
body :
{
    "timestamp" : string,
    "quantitaPrenotata" : float,
    "costoTransazione" : float,
    "prenotazioneAttiva" : boolean
}

Response 400 Bad Request
id dell'azienda o della prenotazione inesistenti
[errorResponse](#error-response)

Response 401 Unauthorized
autenticazione necessaria
[errorResponse](#error-response)

Response 403 Forbidden
endpoint accessibile solo da utenti di tipo "gestore agricolo" appartenenti all'azienda in questione o da utenti
di tipo "gestore idrico" appartenenti all'azienda idrica che rifornisce l'azienda idrica in questione
[errorResponse](#error-response)

Response 500 Server Error
[errorResponse](#error-response)

----------------------------------------------------

# ===== CONSUMI =====

## GET /aziende/:id/consumi
restituisce dati sui consumi di un'azienda (possibile query sul giorno esatto o su un itervallo di giorni)

Response 200
body :
{
    "consumi" : [
        {
            "timestamp" : string,
            "quantitaConsumata" : float
        },
        ...
    ]
}

Response 400 Bad Request
id dell'azienda inesistente
[errorResponse](#error-response)

Response 401 Unauthorized
autenticazione necessaria
[errorResponse](#error-response)

Response 403 Forbidden
endpoint accessibile solo da utenti di tipo "gestore agricolo" appartenenti all'azienda in questione o utenti
di tipo "gestore idrico" appartenenti all'azienda idrica che rifornisce l'azienda agricola in questione
[errorResponse](#error-response)

Response 500 Server Error
[errorResponse](#error-response)

-----------------------------------------------

## POST /aziende/:id/consumi
registra un nuovo consumo relativo ad un giorno per un'azienda

Request
body :
{
    "timestamp" : string,
    "quantitaConsumata" : float
}

Response 200
body :
{
    "id" : int,
    "timestamp" : string,
    "quantitaConsumata" : float
}

Response 400 Bad Request
id dell'azienda inesistente o errori nel body della richiesta o body inesistente
[errorResponse](#error-response)

Response 401 Unauthorized
autenticazione necessaria
[errorResponse](#error-response)

Response 403 Forbidden
endpoint accessibile solo da utenti di tipo "gestore agricolo" appartenenti all'azienda in questione
[errorResponse](#error-response)

Response 500 Server Error
[errorResponse](#error-response)

# ===== SERVIZIO METEO =====

## GET /meteo?posizione=...
restituisce le previsioni meteo per una certa localita' (e' possibile specificare un giorno preciso o una settimana)

Response 200
body : 
{
    "timestamp" : string,
    "descrizione" : string,     (es. soleggiato, pioggia leggera ecc.)
    "temperatura" : float,
    "umidita" : float
}

Response 500 Server Error
[errorResponse](#error-response)

-----------------------------------------------------------------------------------
-----------------------------------------------------------------------------------

# OGGETTI COMUNI

## ERROR RESPONSE

body :
{
    "error message" : string
}