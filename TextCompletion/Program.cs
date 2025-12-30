using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
//using OpenAI.Chat;
using System.ClientModel;
using System.Text.Json.Serialization;

// get credentials from user secrets
IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

var credential = new ApiKeyCredential(config["GitHubModels:Token"] ?? throw new InvalidOperationException("Missing configuration: GitHubModels:Token."));
var options = new OpenAIClientOptions()
{
    Endpoint = new Uri("https://models.github.ai/inference")
};

// create a chat client
IChatClient client =
    new OpenAIClient(credential, options).GetChatClient("openai/gpt-4o-mini").AsIChatClient();
#region Basic Completion
//// send prompt and get response
string basicprompt = "What is AI ? How AI works? explain with step by step. Also provide the solution architecture";
Console.WriteLine($"user >>> {basicprompt}");

ChatResponse response = await client.GetResponseAsync(basicprompt);

Console.WriteLine($"assistant >>> {response}");
Console.WriteLine($"Tokens used: in={response.Usage?.InputTokenCount}, out={response.Usage?.OutputTokenCount}");
#endregion

#region"streaming"
string streamingprompt = "What is AI ? explain max 200 word";
Console.WriteLine($"user >>> {streamingprompt}");

var responseStream = client.GetStreamingResponseAsync(streamingprompt);
await foreach (var message in responseStream)
{
    Console.Write(message.Text);
}

#endregion
