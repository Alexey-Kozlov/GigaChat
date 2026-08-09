using GigaChat.DTO;

record ChatRequest(string model, List<ChatMessage> messages);