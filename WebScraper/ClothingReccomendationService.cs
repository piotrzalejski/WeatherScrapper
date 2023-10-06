using System.Text.Json;
using static GptModel;

public class ClothingRecommendationService
{
	private readonly ChatGptApiClient? _chatGptApiClient;


	public ClothingRecommendationService(ChatGptApiClient chatGptApiClient)
	{
		_chatGptApiClient = chatGptApiClient;
	}

	public async Task<string> GetClothingRecommendationAsync(string temp, string condition)
	{
		// create a message to ask ChatGPT based on the weather
		string message = $"What should I wear today? It is {temp} and {condition}. Please provide a short answer.";

		// get response from ChatGPT
		string response = await _chatGptApiClient!.GetAsyncResp(message);
		//Console.WriteLine(response);

		var respObject = JsonSerializer.Deserialize<ChatGPTResponse>(response);

		Choice[] choices = respObject.choices;
		string content = choices[0].message.content;
		return content;
	}
}

