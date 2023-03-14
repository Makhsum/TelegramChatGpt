using ChatGpt.Application.Services;
using ChatGpt.Domain.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ChatGpt.TelegramBot;

public class AdminWorkService
{
    private readonly Application.Services.ImageService _service;
    private readonly QuestionService _questionService;
    private readonly QuestionMessage _questionMessage;
    private readonly Image _image;
    ReplyKeyboardMarkup keyboardMarkup = new(new[]
    {
        new KeyboardButton[]{"/Опубликовать текст с изображением"},
        new KeyboardButton[]{"/Отправить сообщение"}
    });

    public AdminWorkService(Application.Services.ImageService service,QuestionService questionService,QuestionMessage questionMessage,Image image)
    {
        _service = service;
        _questionService = questionService;
        _questionMessage = questionMessage;
        _image = image;
    }
    
    private static bool sendQuestion;
    private static bool sendimage;
    private static string questionreq;
    private static string imagereq;
   public async Task AdminWorkAsync(ITelegramBotClient botClient, Update update) 
    {
       
       
        if (sendQuestion)
        {
            try
            {
                questionreq = update.Message.Text;
                sendQuestion = false;
                sendimage = true;
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, $"Напишите запрос изображения ");
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException ex)when( ex.Message.Contains("bot was blocked by the user"))
            {
                Console.WriteLine("bot was blocked by the user");
             
            }
        }else if (sendimage)
        {
            imagereq = update.Message.Text;
            sendimage = false;
            if (questionreq!=null&&imagereq!=null)
            {
                await SendMessageAndText(botClient, update, questionreq, imagereq);
            }
        }
        
        else{
            switch (update.Message.Text)
            {
                case "/Опубликовать текст с изображением":
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, $"Напишите запрос текста ",replyMarkup: keyboardMarkup);
                    sendQuestion = true;

                    break;
                case "/Отправить сообщение":
                    break;
                
                default:
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Извините но пока я не могу распознать эту команду");
                    break;
                    
            }
        }
        
    }

    async Task SendMessageAndText(ITelegramBotClient botClient, Update update,string messagerequest, string imagerequest)
    {
        var a =  await  botClient.SendTextMessageAsync(update.Message.Chat.Id, "⌛");
        _image.prompt = imagerequest;
        _questionMessage.prompt = messagerequest;
        await _service.SendMessageAsync(_image);
        
        await _questionService.SendMessageAsync(_questionMessage);
        await      botClient.DeleteMessageAsync(update.Message.Chat.Id,a.MessageId );
        await botClient.SendPhotoAsync(update.Message.Chat.Id, _service.GetResult(),caption:_questionService.GetResult(),parseMode:ParseMode.Html);
    }
}