using ChatGpt.Application.Services;
using ChatGpt.Domain.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ChatGpt.TelegramBot;

public class ChatService
{
   private readonly QuestionService _service;
   private readonly QuestionMessage _questionMessage;

   public ChatService(QuestionService service,QuestionMessage questionMessage)
   {
      _service = service;
      _questionMessage = questionMessage;
   }
   private Dictionary<string, DateTime> userLastMessage = new Dictionary<string, DateTime>();
   public async Task SendMessage(ITelegramBotClient botClient, Update update)
   {
      if (update.Message is not { } message)
         return;
      if (message.Text is not { } messageText)
         return;
      var chatId = message.Chat.Id;
      try
      {
         if (messageText != "/start")
         {
            if (messageText.Contains("/img"))
            {
               return;
            }

            if (!IsTimeOut(message.From.Id.ToString()))
            {
               await botClient.SendTextMessageAsync(message.Chat.Id, "Вам нужно подождать 15 секунд перед тем, как отправить следующее сообщение!");
               return;
            }else
            {
               var a =  await  botClient.SendTextMessageAsync(chatId, "⌛");
               _questionMessage.prompt = messageText;
               await     _service.SendMessageAsync(_questionMessage);
               await      botClient.DeleteMessageAsync(chatId,a.MessageId );
               await botClient.SendTextMessageAsync(chatId,_service.GetResult(),replyToMessageId:message.MessageId);
            }
         }else
         {
                 
            await botClient.SendTextMessageAsync(chatId, "Добрый день напишите свой вопрос!");

         }
      }
      catch (Telegram.Bot.Exceptions.ApiRequestException ex)when( ex.Message.Contains("bot was blocked by the user"))
      {
         Console.WriteLine("bot was blocked by the user");
         return;
      }

   }
   private bool IsTimeOut(string userId)
   {
      DateTime lastMessageTime;
        
      if (userLastMessage.TryGetValue(userId, out lastMessageTime))
      {
         if ((DateTime.Now - lastMessageTime).TotalSeconds <= 15) 
         {
            return false;
         }
      }
        
      userLastMessage[userId] = DateTime.Now; 
        
      return true;
   }
}