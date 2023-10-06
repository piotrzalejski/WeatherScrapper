using Microsoft.Extensions.Configuration;

namespace WebScraper
{
    class Weather
    {
        static async Task Main(string[] args)
        {
            var weatherData = await WeatherScraper.AsyncScrapeWeather();

            Console.WriteLine($"Temperature: {weatherData.Temp}");
            Console.WriteLine($"Condition: {weatherData.Condition}");
            Console.WriteLine($"Location: {weatherData.Location}");

            Speech.SpeakResp($"Currently in {weatherData.City} it is {weatherData.Temp} and {weatherData.Condition}");

            Console.WriteLine("\n-----ChatGPT-----");

            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var chatGptApiClient = new ChatGptApiClient(config);
            var clothingRecommendation = new ClothingRecommendationService(chatGptApiClient);

            string recommendation = await clothingRecommendation.GetClothingRecommendationAsync(weatherData.Temp, weatherData.Condition);
            Console.WriteLine($"GPT API RESPONSE: {recommendation}");

            Speech.SpeakResp(recommendation);

            Console.ReadLine();
        }

        
    }
}