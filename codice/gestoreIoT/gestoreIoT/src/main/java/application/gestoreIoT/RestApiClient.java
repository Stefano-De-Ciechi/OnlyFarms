package application.gestoreIoT;

import com.google.gson.Gson;
import org.springframework.http.*;
import org.springframework.web.client.ResourceAccessException;
import org.springframework.web.client.RestTemplate;

import java.util.HashMap;
import java.util.Map;

// TODO aggiungere un metodo per inviare alla Rest API la water usage della giornata (solo la sera)

public class RestApiClient {
    private final String apiUrl;
    //private final HttpHeaders headers;
    private final Gson jsonConverter;
    private final int farmingCompanyId;
    private final String jwtToken;
    private final String errorMessage = "check that the rest api is running on http://localhost:5234";

    public RestApiClient(int farmingCompanyId, String jwtToken) {
        this.farmingCompanyId = farmingCompanyId;
        this.jwtToken = jwtToken;

        this.apiUrl = "http://localhost:5234/api/v1/farmingCompanies/" + farmingCompanyId + "/crops/";

        this.jsonConverter = new Gson();
    }

    /* esegue una GET request
       restituisce il valore "umidita' ideale" di una coltivazione dato il suo id
    */
    public int getCropIdealHumidity(int cropId) {
        RestTemplate client = new RestTemplate();
        HttpHeaders headers = new HttpHeaders();
        headers.set("Accept", "application/json");
        headers.setBearerAuth(jwtToken);

        String url = apiUrl + cropId + "/";
        int idealHumidity;
        HttpEntity<String> request = new HttpEntity<>(headers);

        try {
            ResponseEntity<String> response = client.exchange(url, HttpMethod.GET, request, String.class);
            String jsonResponse = response.getBody();

            if (jsonResponse == null) {
                return -1;
            }

            Map<?, ?> map = jsonConverter.fromJson(jsonResponse, Map.class);
            idealHumidity = ((Double) map.get("idealHumidity")).intValue();

        } catch (ResourceAccessException e) {
            System.err.println(e.getMessage() + "\n" + errorMessage);
            return -1;
        }

        return idealHumidity;
    }

    /* esegue una POST request
       manda un messaggio con un json body {"value" : int, "measuringUnit" : String}
       all url /api/v1/farmingCompanies/{farmingCompanyId}/crops/{cropId}/sensors/{sensorId}/measurements
    */
    public boolean sendSensorMeasurement(int cropId, int sensorId, int value, String measuringUnit) {
        RestTemplate client = new RestTemplate();
        HttpHeaders headers = new HttpHeaders();
        headers.set("Accept", "application/json");
        headers.setContentType(MediaType.APPLICATION_JSON);
        headers.setBearerAuth(jwtToken);

        String url = apiUrl + cropId + "/sensors/" + sensorId + "/measurements/";

        HashMap<String, Object> body = new HashMap<>();
        body.put("value", value);
        body.put("measuringUnit", measuringUnit);

        String jsonBody = jsonConverter.toJson(body);
        HttpEntity<String> request = new HttpEntity<>(jsonBody, headers);

        try {
            ResponseEntity<String> response = client.exchange(url, HttpMethod.POST, request, String.class);
            String jsonResponse = response.getBody();

            if (jsonResponse == null) {
                return false;
            }

            Map<?, ?> map = jsonConverter.fromJson(jsonResponse, Map.class);
            if (map.size() == 6) {      // il body della response deve contenere 6 elementi (id, timestamp, value, measuringUnit, componentId e sensorId)
                return true;
            }

        } catch (ResourceAccessException e) {
            System.err.println(e.getMessage() + "\n" + errorMessage);
            return false;
        }

        return false;

    }

    /* esegue una POST request
       manda un messaggio con un json body {"state" : String}
       all url /api/v1/farmingCompanies/{farmingCompanyId}/crops/{cropId}/actuators/{actuatorId}/commands
    */
    public boolean sendActuatorCommand(int cropId, int actuatorId, String state) {
        RestTemplate client = new RestTemplate();
        HttpHeaders headers = new HttpHeaders();
        headers.set("Accept", "application/json");
        headers.setContentType(MediaType.APPLICATION_JSON);
        headers.setBearerAuth(jwtToken);

        String url = apiUrl + cropId + "/actuators/" + actuatorId + "/commands/";

        HashMap<String, Object> body = new HashMap<>();
        body.put("state", state);

        String jsonBody = jsonConverter.toJson(body);
        HttpEntity<String> request = new HttpEntity<>(jsonBody, headers);

        try {
            ResponseEntity<String> response = client.exchange(url, HttpMethod.POST, request, String.class);
            String jsonResponse = response.getBody();

            if (jsonResponse == null) {
                return false;
            }

            Map<?, ?> map = jsonConverter.fromJson(jsonResponse, Map.class);
            if (map.size() == 5) {      // il body della response deve contenere 6 elementi (id, timestamp, value, measuringUnit, componentId e sensorId)
                return true;
            }

        } catch (ResourceAccessException e) {
            System.err.println(e.getMessage() + "\n" + errorMessage);
            return false;
        }

        return false;
    }

    /* esegue una POST request
       manda un messaggio con un json body {"state" : state}
       all url /api/v1/farmingCompanies/{farmingCompanyId}/crops/{cropId}/actuators/commands
       cioe' un comando unico per tutti gli attuatori di una coltivazione
    */
    public boolean sendCommandToAllActuators(int cropId, String state) {
        RestTemplate client = new RestTemplate();
        HttpHeaders headers = new HttpHeaders();
        headers.set("Accept", "application/json");
        headers.setContentType(MediaType.APPLICATION_JSON);
        headers.setBearerAuth(jwtToken);

        String url = apiUrl + cropId + "/actuators/commands/";      // endpoint per mandare lo stesso comando a tutti gli attuatori di una coltivazione

        HashMap<String, Object> body = new HashMap<>();
        body.put("state", state);

        String jsonBody = jsonConverter.toJson(body);
        HttpEntity<String> request = new HttpEntity<>(jsonBody, headers);

        try {
            ResponseEntity<String> response = client.exchange(url, HttpMethod.POST, request, String.class);
            HttpStatusCode status = response.getStatusCode();

            if (status.is2xxSuccessful()) {
                return true;
            }

        } catch (ResourceAccessException e) {
            System.err.println(e.getMessage() + "\n" + errorMessage);
            return false;
        }

        return false;
    }

    /* esegue una POST request
       manda un messaggio con un json body {"consumedQuantity" : int}
       all url /api/v1/farmingCompanies/{farmingCompanyId}/waterUsages
       per registrare l'utilizzo di acqua del giorno corrente (se ne e' gia' stato inviato uno i suoi valori vengono sovrascritti
    */
    public boolean sendWaterUsage(int cropId, int consumedQuantity) {
        RestTemplate client = new RestTemplate();
        HttpHeaders headers = new HttpHeaders();
        headers.set("Accept", "application/json");
        headers.setContentType(MediaType.APPLICATION_JSON);
        headers.setBearerAuth(jwtToken);

        String url = "http://localhost:5234/api/v1/farmingCompanies/" + farmingCompanyId + "/crops/" + cropId + "/waterUsages/";

        HashMap<String, Object> body = new HashMap<>();
        body.put("consumedQuantity", consumedQuantity);

        String jsonBody = jsonConverter.toJson(body);
        HttpEntity<String> request = new HttpEntity<>(jsonBody, headers);

        try {
            ResponseEntity<String> response = client.exchange(url, HttpMethod.POST, request, String.class);
            HttpStatusCode status = response.getStatusCode();

            if (status.is2xxSuccessful()) {
                return true;
            }

        } catch (ResourceAccessException e) {
            System.err.println(e.getMessage() + "\n" + errorMessage);
            return false;
        }

        return false;
    }
}

