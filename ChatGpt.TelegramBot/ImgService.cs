using ChatGpt.Domain.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ChatGpt.TelegramBot;

public class ImgService
{
    private readonly Application.Services.ImageService _service;
    private readonly Image _image;
   
    private Dictionary<string, DateTime> userLastMessage = new Dictionary<string, DateTime>();

    public ImgService(Application.Services.ImageService service,Image image)
    {
        _service = service;
        _image = image;
       
    }
     public async Task SendMessage(ITelegramBotClient botClient, Update update)
       {
          if (update.Message is not { } message)
             return;
          if (message.Text is not { } messageText)
             return;
          var chatId = message.Chat.Id;
          try
          {
             if (messageText.Contains("/img"))
             {
               messageText = messageText.Replace("/img", "");
             }

             if (!IsTimeOut(message.From.Id.ToString()))
                {
                   await botClient.SendTextMessageAsync(message.Chat.Id, "Вам нужно подождать 20 секунд перед тем, как отправить следующее сообщение!");
                   return;
                }else
                {
                   var a =  await  botClient.SendTextMessageAsync(chatId, "⌛");
                   _image.prompt = messageText;
                   await     _service.SendMessageAsync(_image);
                   await      botClient.DeleteMessageAsync(chatId,a.MessageId );
                   await botClient.SendPhotoAsync(chatId,_service.GetResult());
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
             if ((DateTime.Now - lastMessageTime).TotalSeconds <= 20) 
             {
                return false;
             }
          }
            
          userLastMessage[userId] = DateTime.Now; 
            
          return true;
       }
}