package application.gestoreIoT;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertTrue;

import org.junit.Test;

/* IMPORTANTE: prima di eseguire i test assicurarsi che il programma in codice/OnlyFarms/OnlyFarms.RestApi sia stato avviato e sia raggiungibile all'indirizzo http://localhost:5234 */
public class AppTest 
{
    /* nel DB e' stata creata una farmingCompany di test con ID=3 ed una sola crop con ID=3; la farmingCompany NON e' legata a nessun account ma i suoi dati sono accessibili da qualunque utente nella pagina "Crops" cambiando nell'url il cropId con 3 */
    private final String jwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6Ijg3MjEwYjViLTZkZDgtNDQxYy1hNzhiLWUyZDJiYTBjZTAyYSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJmYXJtMUB0ZXN0LmNvbSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6ImZhcm0xQHRlc3QuY29tIiwiQXNwTmV0LklkZW50aXR5LlNlY3VyaXR5U3RhbXAiOiJBRUpYREU0SkhaNE9VWjUyQkhZN0VSRFJLSlI3RTZWNyIsImFtciI6InB3ZCIsIlJvbGVzIjoiaW90U3ViU3lzdGVtIiwiZXhwIjoxNzMxMTczMDAxfQ.tLgiuzypgfRiNqKe8gjyvColE50Xnxfj6jKkY1wZUI8";
    private final RestApiClient client = new RestApiClient(3, jwtToken);
    private final int testCropId = 3;

    @Test
    public void testGetIdealHumidity() {
        int expected = 70;
        int actual = client.getCropIdealHumidity(testCropId);
        assertEquals(expected, actual);
    }

    @Test
    public void testPostActuatorCommand() {
        int testActuatorId = 2;
        assertTrue(client.sendActuatorCommand(testCropId, testActuatorId, "test ON"));
    }

    @Test
    public void testPostSensorMeasurement() {
        int testSensorId = 3;
        assertTrue(client.sendSensorMeasurement(testCropId, testSensorId, 11, "test measure"));
    }

    @Test
    public void testPostCommandToAllActuators() {
        assertTrue(client.sendCommandToAllActuators(testCropId, "TEST ALL ON"));
    }

    @Test
    public void testPostWaterUsage() {
        assertTrue(client.sendWaterUsage(5000));
    }
}
