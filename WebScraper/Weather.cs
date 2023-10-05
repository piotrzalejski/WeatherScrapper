using HtmlAgilityPack;
using System.Speech.Synthesis;


namespace WebScraper
{
    class Weather
    {
        static void Main(string[] args)
        {
            // Send get request to weather.com
            String url = "https://weather.com/weather/today/l/35bf30c24f7d2e3d25bbb6f0399afcd2f256c0f54829bea5dc94dee1eacd9cfa";
            var httpClient = new HttpClient();
            var html = httpClient.GetStringAsync(url).Result;
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            // Get the temp
            var tempElement = htmlDocument.DocumentNode.SelectSingleNode("//span[@data-testid='TemperatureValue']");
            var temperature = tempElement.InnerText.Trim();
            Console.WriteLine($"Temperature: {temperature}");

            // Get the conditions
            var condElement = htmlDocument.DocumentNode.SelectSingleNode("//div[contains(@class,'CurrentConditions--phrase')]");
            var condition = condElement.InnerText.Trim();
            Console.WriteLine($"Condition: {condition}");

            // Get the location
            var locElement = htmlDocument.DocumentNode.SelectSingleNode("//h1[contains(@class,'CurrentConditions--location')]");
            var location = locElement.InnerText.Trim();
            string[] parts = location.Split(',');
            string cityName = parts[0];
            Console.WriteLine($"Location: {location}");

            // initialize new instance of SpeechSynthesizer
            SpeechSynthesizer synth = new SpeechSynthesizer();

            // configure audio output
#pragma warning disable CA1416
            synth.SetOutputToDefaultAudioDevice();
            synth.SelectVoiceByHints(VoiceGender.Female);

            // Speak
            synth.Speak(textToSpeak: $"Currently in {cityName} it is {temperature} and {condition}");
#pragma warning restore CA1416

        }
    }
}