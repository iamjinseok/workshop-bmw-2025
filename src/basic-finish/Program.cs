// See https://aka.ms/new-console-template for more information
using Microsoft.SemanticKernel;

Console.WriteLine("Hello, Semantic Kernel!");

var deployname = "gpt-4o-mini";
var endpoint = "https://blazormeetup.openai.azure.com";
var apiKey = "f09bbed482e84d30999f935effe34430";

var kernel = Kernel.CreateBuilder()
							.AddAzureOpenAIChatClient( deployname, endpoint, apiKey)
							.Build();

var prompt = "Semantic Kernel이 무엇인가요?";
Console.WriteLine( prompt );

var response = await kernel.InvokePromptAsync(prompt);
Console.WriteLine(response);