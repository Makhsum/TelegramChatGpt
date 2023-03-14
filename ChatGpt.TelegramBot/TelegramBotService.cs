using ChatGpt.Application.Services;
using ChatGpt.Domain.Models;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ChatGpt.TelegramBot;

public class TelegramBotService
{
    // private string databasePath = Directory.GetCurrentDirectory()+"/UsersDatabase.json";
    private readonly TelegramBotClient botClient;
    private readonly SettingsServices _settings;
    private long myId;
    // public List<Client> Users =default;
    // private Dictionary<string, DateTime> userLastMessage = new Dictionary<string, DateTime>();
    private readonly ChatService _service;
    private readonly ImgService _imgService;
    private readonly AdminWorkService _adminservice;


    public TelegramBotService(SettingsServices settings,ImgService imgService,AdminWorkService adminservice,ChatService service)
    {
        
        _settings = settings;
        _service = service;
        _imgService = imgService;
        _adminservice = adminservice;
        botClient = new TelegramBotClient(settings.ChatServiceSettings["TelegramBotToken"]);
        
    }

    
    public async Task StartWorkingAsync()
    {
        
        using CancellationTokenSource cts = new();
         

        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };
        bool idParse = long.TryParse(_settings.ChatServiceSettings["ChatIdForTelegramBotAdmin"], out myId);
        if (idParse)
        {
            botClient.StartReceiving(HandleUpdateAsync, HandlePollingErrorAsync, receiverOptions, cts.Token);
        }
        else
        {
            Console.WriteLine("Неверное значение ключа ChatIdForTelegramBotAdmin");
            return;
        }
        

        var me = await botClient.GetMeAsync();

        Console.WriteLine($"Начало работы @{me.Username} бота!");
        
        Console.ReadLine();
        cts.Cancel();

        
    }
    async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {   
       //  Users = await GetFromDataBaseAsync();
       if (update.Message is not { } message)
           return;
       if (message.Text is not { } messageText)
           return;
       var chatId = message.Chat.Id;
       //  if (Users!=null&&Users.Count > 0&&chatId!=null)
       // {
       //      var selectedUser = Users.FirstOrDefault(x => x.ChatId == chatId);    
       //       
       //      if (selectedUser == null)
       //      {
       //          Users.Add(new Client() {ChatId = update.Message.Chat.Id,FirstName = update.Message.Chat.FirstName,LastName = update.Message.Chat.LastName,UserName = update.Message.Chat.Username });
       //        await  SaveToDatabaseAsync();
       //      }
       // } 
       //  if(Users.Count == 0)
       // {
       //      Users.Add(new Client() { ChatId = update.Message.Chat.Id, FirstName = update.Message.Chat.FirstName, LastName = update.Message.Chat.LastName, UserName = update.Message.Chat.Username });
       //      await  SaveToDatabaseAsync();   
       // }
        if (update.Message.Chat.Id == myId)
        {
            await _adminservice.AdminWorkAsync(botClient,update);
            return;
        }
        
      
        
         Console.WriteLine($"Received a '{messageText}' message in chat {chatId}. Username is '{update.Message.Chat.FirstName}' message id = {message.MessageId}");
         try
         {
             if (messageText != "/start")
             {
                 if (messageText.Contains("/img"))
                 {
                     await  _imgService.SendMessage(botClient, update);
                 }
                 await  _service.SendMessage(botClient, update);
                 return;
             }
             else
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

    Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }

 /*   async Task SaveToDatabaseAsync()
    {
        var json = JsonConvert.SerializeObject(Users);
        await System.IO.File.WriteAllTextAsync(databasePath,json);
    }
    async Task<List<Client>> GetFromDataBaseAsync()
    {
        if (System.IO.File.Exists(databasePath))
        {
            var deserializeObject = await  System.IO.File.ReadAllTextAsync(databasePath);
            return JsonConvert.DeserializeObject<List<Client>>(deserializeObject);
        }
       
            Console.WriteLine("Бд не существует!");
            return new List<Client>();
    }

    private static bool send;
    async Task AdminWorkAsync(ITelegramBotClient botClient, Update update) 
    {
       
        ReplyKeyboardMarkup keyboardMarkup = new(new[]
        {
            new KeyboardButton[]{"/Количество пользователей"},
            new KeyboardButton[]{"/Отправить сообщение"}
        });
        if (send)
        {
            long cl = default;
            try
            {
                foreach (var user in Users)
                {

                    cl = user.ChatId;
                        await botClient.SendTextMessageAsync(user.ChatId,update.Message.Text);
                        send = false;
                }
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException ex)when( ex.Message.Contains("bot was blocked by the user"))
            {
                Console.WriteLine("bot was blocked by the user");
                var selecteduser = Users.FirstOrDefault(x => x.ChatId == cl);
                Users.Remove(selecteduser);
               await SaveToDatabaseAsync();
            }
        }else{
        switch (update.Message.Text)
        {
            case "/Количество пользователей":
                await botClient.SendTextMessageAsync(myId, $"Количество пользователей {Users.Count}",replyMarkup:keyboardMarkup);
                break;
            case "/Отправить сообщение":
                send = true;
                break;
                
                default:
                    await botClient.SendTextMessageAsync(myId, "Извините но пока я не могу распознать эту команду");
                    break;
                    
        }
    }
        
    }

    
    private bool IsTimeOut(string userId)
    {
        DateTime lastMessageTime;
        
        if (userLastMessage.TryGetValue(userId, out lastMessageTime))
        {
            if ((DateTime.Now - lastMessageTime).TotalSeconds <= 30) 
            {
                return false;
            }
        }
        
        userLastMessage[userId] = DateTime.Now; 
        
        return true;
    }*/
}