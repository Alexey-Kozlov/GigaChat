namespace GigaChat.DTO;
record ChatResponse(List<Choice> Choices);
record Choice(ChatMessage Message);