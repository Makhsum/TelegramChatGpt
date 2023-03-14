using ChatGpt.Domain.Models;

namespace ChatGpt.Application.Common.Abstractions.ServiceInterfaces;

public interface IQuestionService
{
    Task SendMessageAsync(QuestionMessage msg);
}