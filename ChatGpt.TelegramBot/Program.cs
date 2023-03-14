using System.Net;
using ChatGpt.Application.Services;
using ChatGpt.Domain.Models;
using ChatGPT.Models;
using ChatGpt.TelegramBot;
using ImageService = ChatGpt.Application.Services.ImageService;

SettingsServices settings = new SettingsServices();

var chatsettings = settings.ChatServiceSettings;
if (chatsettings.ContainsKey("requestUrl")&&chatsettings.ContainsKey("TelegramBotToken")&&chatsettings.ContainsKey("OpenAiToken")&&chatsettings.ContainsKey("useProxy")&&chatsettings.ContainsKey("ProxyAdressAndPort")&&chatsettings.ContainsKey("ChatIdForTelegramBotAdmin"))
{
    var client = new HttpClient();

    if (chatsettings["useProxy"] == "true")
    {
        WebProxy proxy = new WebProxy(chatsettings["ProxyAdressAndPort"],true);
        client = new HttpClient(new HttpClientHandler()
        {
            Proxy = proxy, 
            UseProxy = true
        });
    }
    client.DefaultRequestHeaders.Add("authorization",$"Bearer {settings.ChatServiceSettings["OpenAiToken"]}");
    var responseMessage = new HttpResponseMessage();
    ImageService imageService = new ImageService(client, responseMessage, settings);
    QuestionService service = new QuestionService(client,responseMessage,settings);
    Image img = new Image() {n = 1, prompt = "asd", size = "512x512"};
    AdminWorkService adminWorkService = new AdminWorkService(imageService,service, settings.questionMessage,img);
    ChatService chatService = new ChatService(service,settings.questionMessage);
    ImgService imgService = new ImgService(imageService,img);
    TelegramBotService botService = new TelegramBotService(settings,imgService, adminWorkService,chatService);
    await botService.StartWorkingAsync();
}

Console.WriteLine("Неверное имя ключей в файле настроек appsettings.json");