# ===== IDEE =====

- per ora ho fatto in modo che ogni FarmingCompany abbia inizialmente la propria WaterSupply = 0, e man mano che vengono aggiunte Coltivazioni, 
  le loro WaterNeed vengano sommate alla WaterSupply; in questo modo una WaterCompany ha bisogno solamente di controllare che la somma di WaterSupply
  di tutte le "aziende clienti" sia inferiore alla propria WaterSupply

# ===== DUBBI =====

- sensori ed attuatori necessitano anche loro di autenticazione? Piu' che loro, e' il sottosistema IoT ad averne bisogno

# ===== SEMPLIFICAZIONI =====

- invece di avere una relazione 1:N tra le aziende ed i propri utenti amministratori sarebbe decisamente meno complicato avere una relazione
  1:1, questo perche' nella traccia di esame della parte di Applicazioni Web si parla di "form di registrazione" con nome azienda e email utente;
  per evitare cose complicate come il dover indicare subito piu' utenti registrati o meno, si iniziera' con UN SOLO utente per azienda
  (senza la possibilita' di cancellazione, per il momento); in futuro, se rimane tempo, si potra' pensare a come aggiungere / rimuovere
  utenti alle varie aziende (nel DB verra' mantenuto il campo "UniqueCompanyCode" che serviva proprio allo scopo di far registrare piu' utenti)

- contrariamente a quanto scritto nella traccia di esame di Applicazioni Web, l'accettazione / rifiuto delle varie richieste di adesione delle
  aziende al sistema NON sara' piuì gestita dagli utenti 'GSI' (Gestori del Servizio Idrico) ma dall'UNICO (per ora) utente Admin della piattaforma
  (questo perche' nella piattaforma saranno presenti piu' aziende idriche)