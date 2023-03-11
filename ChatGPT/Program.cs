using System.Net;
using ChatGPT.Services;
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
    ChatService service = new ChatService(client,new HttpResponseMessage(),settings);
    TelegramBotService botService = new TelegramBotService(settings,settings.questionMessage,service);
    await botService.StartWorkingAsync();
}

Console.WriteLine("Неверное имя ключей в файле настроек appsettings.json");





