using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

public class ChatGptApiClient
{
    private readonly string? apiKey;
    private readonly HttpClient httpClient;

    public ChatGptApiClient(IConfiguration configuration)
    {
        apiKey = configuration.GetSection("OpenAI")["ApiKey"];
        httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
    }

    public async Task<string> GetAsyncResp(string message)
    {

        string apiUrl = "https://api.openai.com/v1/chat/completions";

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

        var content = new StringContent(jsonReqBody, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(apiUrl, content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }
        else
        {
            throw new Exception($"ChatGPT APi Request Failed: {response.StatusCode}");
        }
    }
}
