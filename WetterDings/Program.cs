using System.Text.Json;
using System.Configuration;
using System.Collections.Specialized;

namespace WetterDings
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var appid = ConfigurationManager.AppSettings.Get("appid");

            if (string.IsNullOrWhiteSpace(appid) || appid is null)
            {
                Console.WriteLine("appid not set in App.config");
                return;
            }

            Console.Write("Enter a city name: ");
            var city = Console.ReadLine();

            if (city is null)
            {
                Console.WriteLine("Something went wrong with your weather :(");
                return;
            }
            else if (string.IsNullOrWhiteSpace(city))
            {
                Console.WriteLine("Next time enter a city name :)");
                return;
            }

            var weather = await GetCurrentWeatherAsync(city, appid);

            if (weather is null)
            {
                return;
            }

            PrintWeather(weather);
        }

        static void PrintWeather(WeatherResponse w)
        {
            const string _C = "°C";
            var localDate = DateTime.UnixEpoch.AddSeconds(w.dt).AddSeconds(w.timezone);

            Console.WriteLine($"City: {w.name} {w.coord.lat} {w.coord.lon}");
            Console.WriteLine($"Local DateTime: {localDate}");
            Console.WriteLine($"Temperature: {w.main.temp + _C} Feels Like: {w.main.feels_like + _C}");
            Console.WriteLine($"Max/Min Temperature: {w.main.temp_max + _C}/{w.main.temp_min + _C}");
            Console.WriteLine($"Weather:");
            foreach (var f in w.weather)
            {
                Console.WriteLine($"{f.description}");
            }
        }

        static async Task<WeatherResponse?> GetCurrentWeatherAsync(string city, string appid)
        {
            using HttpClient http = new();

            var response = await http.GetAsync($"https://api.openweathermap.org/data/2.5/weather?units=metric&q={city}&appid={appid}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Failed to get weather.");
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<WeatherResponse>(content);
        }
    }


    public class WeatherResponse
    {
        public Coord coord { get; set; }
        public Weather[] weather { get; set; }
        public string _base { get; set; }
        public Main main { get; set; }
        public int visibility { get; set; }
        public Wind wind { get; set; }
        public Rain rain { get; set; }
        public Clouds clouds { get; set; }
        public int dt { get; set; }
        public Sys sys { get; set; }
        public int timezone { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int cod { get; set; }
    }

    public class Coord
    {
        public float lon { get; set; }
        public float lat { get; set; }
    }

    public class Main
    {
        public float temp { get; set; }
        public float feels_like { get; set; }
        public float temp_min { get; set; }
        public float temp_max { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
        public int sea_level { get; set; }
        public int grnd_level { get; set; }
    }

    public class Wind
    {
        public float speed { get; set; }
        public int deg { get; set; }
        public float gust { get; set; }
    }

    public class Rain
    {
        public float _1h { get; set; }
    }

    public class Clouds
    {
        public int all { get; set; }
    }

    public class Sys
    {
        public int type { get; set; }
        public int id { get; set; }
        public string country { get; set; }
        public int sunrise { get; set; }
        public int sunset { get; set; }
    }

    public class Weather
    {
        public int id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }

}