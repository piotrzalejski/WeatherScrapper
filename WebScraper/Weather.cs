using HtmlAgilityPack;
using System.Speech.Synthesis;


namespace WebScraper
{
    class Weather
    {
        static async Task Main(string[] args)
        {
            String url = "https://weather.com/weather/today/l/35bf30c24f7d2e3d25bbb6f0399afcd2f256c0f54829bea5dc94dee1eacd9cfa";
            var htmlDocument = await GetHtmlDocAsync(url);

            var temp = GetTemperature(htmlDocument);
            Console.WriteLine($"Temperature: {temp}");

            var condition = GetCondition(htmlDocument);
            Console.WriteLine($"Condition: {condition}");

            var location = GetLocation(htmlDocument);
            string[] parts = location.Split(",");
            string cityName = parts[0];
            Console.WriteLine($"Location: {location}");

            SpeakWeather(cityName, temp, condition);
        }
        static async Task<HtmlDocument> GetHtmlDocAsync(string url)
        {
            using var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            return htmlDocument;
        }

        static string GetTemperature(HtmlDocument htmlDocument)
        {
            var tempElement = htmlDocument.DocumentNode.SelectSingleNode("//span[@data-testid='TemperatureValue']");
            return tempElement?.InnerText.Trim();
        }

        static string GetLocation(HtmlDocument htmlDocument)
        {
            var locElement = htmlDocument.DocumentNode.SelectSingleNode("//h1[contains(@class,'CurrentConditions--location')]");
            return locElement?.InnerText.Trim();
        }

        static string GetCondition(HtmlDocument htmlDocument)
        {
            var condElement = htmlDocument.DocumentNode.SelectSingleNode("//div[contains(@class,'CurrentConditions--phrase')]");
            return condElement?.InnerText.Trim();
        }

        static void SpeakWeather(string location, string temp, string condition)
        {
            using var synth = new SpeechSynthesizer();

            //configure audio output
            synth.SetOutputToDefaultAudioDevice();
            synth.SelectVoiceByHints(VoiceGender.Neutral);

            //speak
            synth.Speak($"Currently in {location} it is {temp} and {condition}");
        }
    }
}