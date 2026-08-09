using GigaChat.Dialog;
using GigaChat.GigaChat;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var customConfig = new ConfigurationBuilder().AddInMemoryCollection().Build();
builder.Configuration.AddConfiguration(customConfig);
builder.Services.AddTransient<AccessToken>();
builder.Services.AddTransient<GigaChatSendRequest>();
builder.Services.AddTransient<Dialog>();
using IHost host = builder.Build();

var accessToken = host.Services.GetRequiredService<AccessToken>();
var token = accessToken.GetAccessToken();
var dialog = host.Services.GetRequiredService<Dialog>();
dialog.ProcessDialog(token);

//await host.RunAsync();