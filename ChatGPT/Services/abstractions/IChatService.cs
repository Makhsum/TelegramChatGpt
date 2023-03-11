using ChatGPT.Models;

namespace ChatGPT.Services.abstractions;

public interface IChatService
{
    Task SendMessageAsync(QuestionMessage msg);
    
}