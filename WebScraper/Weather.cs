using HtmlAgilityPack;
using System.Speech.Synthesis;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text;
using System.Net.Http.Json;

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

            SpeakWeather($"Currently in {cityName} it is {temp} and {condition}");

            // WIP
            // CHAT GPT Response?

            Console.WriteLine("Hey ChatGPT");
            string recommendation = await GetClothingRecommendationAsync(temp, condition);
            Console.WriteLine($"GPT API RESPONSE: {recommendation}");

            using var synth = new SpeechSynthesizer();

            synth.Speak(recommendation);

            Console.ReadLine();
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

        static void SpeakWeather(string message)
        {
            using var synth = new SpeechSynthesizer();

            //configure audio output
            synth.SetOutputToDefaultAudioDevice();
            synth.SelectVoiceByHints(VoiceGender.Neutral);

            //speak
            synth.Speak(message);
        }

        static async Task<string> GetGPTRespAsync(string message)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            string apiKey = configuration.GetSection("OpenAI")["ApiKey"];
            string apiUrl = "https://api.openai.com/v1/chat/completions";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = "You are a helpful assistant"
                    },
                    new
                    {
                        role = "user",
                        content = message
                    }
                },
                max_tokens = 50
            };

            // serialize the request body to JSON
            var jsonReqBody = JsonSerializer.Serialize(requestBody);

            var content = new StringContent(jsonReqBody,Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseContent);
                throw new Exception($"ChatGPT APi Request Failed: {response.StatusCode}");
            }
        }

        static async Task<string> GetClothingRecommendationAsync(string temp, string condition)
        {
            // create a message to ask ChatGPT based on the weather
            string message = $"What should I wear today? It is {temp} and {condition}. Please provide a short answer.";

            // get response from ChatGPT
            string response = await GetGPTRespAsync(message);
            Console.WriteLine(response);

            var respObject = JsonSerializer.Deserialize<ChatGPTResponse>(response);

            string content = respObject.choices[0].message.content;
            return content;
        }

        public class ChatGPTResponse
        {
            public string id { get; set; }
            public string @object { get; set; }
            public long created { get; set; }
            public string model { get; set; }
            public Choice[] choices { get; set; }
            public Usage usage { get; set; }
        }

        public class Choice
        {
            public int index { get; set; }
            public Message message { get; set; }
            public string finish_reason { get; set; }
        }

        public class Message
        {
            public string role { get; set; }
            public string content { get; set; }
        }

        public class Usage
        {
            public int prompt_tokens { get; set; }
            public int completion_tokens { get; set; }
            public int total_tokens { get; set; }
        }
    }
}