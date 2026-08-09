using GigaChat.DTO;
using GigaChat.GigaChat;
using Microsoft.Extensions.Configuration;

namespace GigaChat.Dialog;

public class Dialog
{
    private List<ChatMessage> chatMessages = new List<ChatMessage>();
    private readonly GigaChatSendRequest gigaChatSendRequest;
    private readonly IConfiguration configuration;
    public Dialog(GigaChatSendRequest _gigaChatSendRequest, IConfiguration _configuration)
    {
        gigaChatSendRequest = _gigaChatSendRequest;
        configuration = _configuration;
    }

    public void ProcessDialog(AccessTokenResponseDTO accessToken)
    {
        //выбор собеседника
        Console.Write("Выбор собеседника (1 - dotnet, 2 - юмор, 3 - энциклопедия): ");
        var chatType = Console.ReadLine();
        switch (chatType)
        {
            case "1":
                chatMessages.Add(new("system", configuration["AssistentType:DotNet"]));
                break;
            case "2":
                chatMessages.Add(new("system", configuration["AssistentType:Humor"]));
                break;
            case "3":
                chatMessages.Add(new("system", configuration["AssistentType:Common"]));
                break;
            default:
                return;
        }
        while (true)
        {
            Console.Write("Введите ваше сообщение ('выход' для выхода): ");
            var userInput = Console.ReadLine();
            if (userInput == "выход") break;
            chatMessages.Add(new ChatMessage("user", userInput));
            var answer = gigaChatSendRequest.AskGigaChat(accessToken, chatMessages);
            chatMessages.Add(new ChatMessage("assistant", answer));
            Console.WriteLine($"GigaChat ответил: {answer}");
        }
    }
}