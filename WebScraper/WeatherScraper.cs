using HtmlAgilityPack;

public class WeatherScraper
{
    private const string WeatherUrl = "https://weather.com/weather/today/l/35bf30c24f7d2e3d25bbb6f0399afcd2f256c0f54829bea5dc94dee1eacd9cfa";

    public static async Task<WeatherData> AsyncScrapeWeather()
    {
        var htmlDocument = await GetHtmlDocumentAsync(WeatherUrl);
        var temperature = GetTemperature(htmlDocument);
        var condition = GetCondition(htmlDocument);
        var location = GetLocation(htmlDocument);
        var cityName = location.Split(",")[0];

        return new WeatherData
        {
            Temp = temperature,
            Condition = condition,
            Location = location,
            City = cityName
        };
    }

    private static async Task<HtmlDocument> GetHtmlDocumentAsync(string url)
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
        return tempElement.InnerText.Trim();
    }

    static string GetLocation(HtmlDocument htmlDocument)
    {
        var locElement = htmlDocument.DocumentNode.SelectSingleNode("//h1[contains(@class,'CurrentConditions--location')]");
        return locElement.InnerText.Trim();
    }

    static string GetCondition(HtmlDocument htmlDocument)
    {
        var condElement = htmlDocument.DocumentNode.SelectSingleNode("//div[contains(@class,'CurrentConditions--phrase')]");
        return condElement.InnerText.Trim();
    }

    public class WeatherData
    {
        public string Temp { get; set; }
        public string City { get; set; }
        public string Location { get; set; }
        public string Condition { get; set; }
}
}

