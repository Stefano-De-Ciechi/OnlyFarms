package prototypes;

import org.springframework.http.*;
import org.springframework.web.client.ResourceAccessException;
import org.springframework.web.client.RestTemplate;

public class ClientHttp {
    // token per il sottosistema IoT generato direttamente dalla pagina web del profilo FarmManager: farm1@test.com  Pa$$w0rd
    //private final String jwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJSb2xlcyI6ImlvdFN1YlN5c3RlbSIsImV4cCI6MTczMTE2MjUwNH0.TG-TWZW2T3VQFOIKgyaNfrk7nvEgEUyTTxFGqMaI9k4";
    private final String jwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6Ijg3MjEwYjViLTZkZDgtNDQxYy1hNzhiLWUyZDJiYTBjZTAyYSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJmYXJtMUB0ZXN0LmNvbSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6ImZhcm0xQHRlc3QuY29tIiwiQXNwTmV0LklkZW50aXR5LlNlY3VyaXR5U3RhbXAiOiJBRUpYREU0SkhaNE9VWjUyQkhZN0VSRFJLSlI3RTZWNyIsImFtciI6InB3ZCIsIlJvbGVzIjoiaW90U3ViU3lzdGVtIiwiZXhwIjoxNzMxMTczMDAxfQ.tLgiuzypgfRiNqKe8gjyvColE50Xnxfj6jKkY1wZUI8";
    private final int farmingCompanyId = 1;
    private final String baseUrl = "http://localhost:5234/api/v1/farmingCompanies/" + farmingCompanyId + "/";
    private final RestTemplate rest;
    private final HttpHeaders headers;

    public ClientHttp() {
        rest = new RestTemplate();
        headers = new HttpHeaders();

        //headers.setContentType(MediaType.APPLICATION_JSON);
        headers.set("Accept", "application/json");
        headers.setBearerAuth(jwtToken);
    }

    public void testGetRequests() {
        String url = baseUrl + "crops/";

        try {
            HttpEntity<String> httpEntity = new HttpEntity<>(headers);
            ResponseEntity<String> response = rest.exchange(url, HttpMethod.GET, httpEntity, String.class);
            String jsonRes = response.getBody();
            System.out.println(jsonRes);
        } catch (ResourceAccessException e) {
            System.err.println(e.getMessage());
        }
    }

    public void testPostRequests() {
        String url = baseUrl + "crops/1/" + "actuators/1/commands/";

        try {
            headers.setContentType(MediaType.APPLICATION_JSON);
            String jsonBody = "{\"state\":\"on\"}";

            HttpEntity<String> request = new HttpEntity<>(jsonBody, headers);
            ResponseEntity<String> response = rest.postForEntity(url, request, String.class);
            String jsonRes = response.getBody();
            System.out.println(jsonRes);

        } catch (ResourceAccessException e) {
            System.err.println(e.getMessage());
        }
    }

    public static void main(String[] args) {
        ClientHttp client = new ClientHttp();
        client.testGetRequests();
        //client.testPostRequests();
    }

}

