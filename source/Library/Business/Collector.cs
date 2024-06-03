
using System.Net.Http.Json;

namespace Library.Business
{
    public class Collector
    {
        public static readonly HttpClient httpClient = new() { BaseAddress = new("http://sensor-manager") };

        public static Task<List<Sensor>?> GetAll()
        {
            return httpClient.GetFromJsonAsync<List<Sensor>>("/MapSensors");
        }

        public static Task<Sensor?> GetStatusSensor(string name)
        {
            return httpClient.GetFromJsonAsync<Sensor>($"/StatusSensor/{name}");
        }
    }
}
