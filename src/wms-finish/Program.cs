using Microsoft.Extensions.AI;
using wms_finish.Components;
using Azure;
using Azure.AI.OpenAI;
using System.ClientModel;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using wms_finish.Sqlite;
using wms_finish;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

#pragma warning disable SKEXP0001
var kernel = Kernel.CreateBuilder().
                    AddAzureOpenAIChatCompletion("gpt-4o-mini", "https://blazormeetup.openai.azure.com", "f09bbed482e84d30999f935effe34430")
                    .Build();
kernel.ImportPluginFromType<InventoryPlugin>();

var chatService = kernel.GetRequiredService<IChatCompletionService>();
IChatClient chatClient = chatService.AsChatClient();
#pragma warning restore SKEXP0001

builder.Services.AddChatClient(chatClient).UseFunctionInvocation().UseLogging();
builder.Services.AddSingleton<Kernel>(kernel);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.UseStaticFiles();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// ป๙วร DB
using (var context = new FactoryContext())
{
	context.initailize();
}

app.Run();
