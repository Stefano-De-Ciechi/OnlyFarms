# ===== IDEE =====

- si potrebbe fare in modo che ogni FarmingCompany abbia inizialmente la propria WaterSupply = 0, e man mano che vengono aggiunte Coltivazioni, 
  le loro WaterNeed vengano sommate alla WaterSupply; in questo modo una WaterCompany ha bisogno solamente di controllare che la somma di WaterSupply
  di tutte le "aziende clienti" sia inferiore alla propria WaterSupply
- si dovrebbero aggiungere delle query ai vari endpoint (es. selezionare tutte le aziende agricole di una citta', o tutte le misurazioni di una certa data o in un certo intervallo di tempo)

# ===== DUBBI =====

- non so se sia un problema di entity framework o una mia implementazione sbagliata, ma ora come ora se si esegue una GET request di una 
  qualsiasi risorsa che contiene delle relazioni 1:N, gli array di tali risorse NON vengono riempiti (es. eseguendo una richiesta a 
  farmingCompanies/1 gli array crops, waterUsages e reservations sono tutti vuoti, ma se si esegue una GET a farmingCompanies/1/crops e 
  poi si esegue nuovamente la prima richiesta, il campo crops viene "magicamente" riempito con i valori giusti (ma i sotto-array sono 
  comunque vuoti)) --> in realta' pensandoci su e' meglio così (che sia un bug o una feature), visto che ricevere sempre tutti gli elementi
  interni di ogni risorsa sarebbe uno spreco di risorse assurdo (es. se si vuole l'elenco di aziende agricole NON ci interessano anche tutti
  i loro campi, i loro sensori, le loro misure, ecc. )