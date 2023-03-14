using System.Text;
using ChatGpt.Domain.Models;
using Newtonsoft.Json;

namespace ChatGpt.Application.Services;

public class ImageService
{   
    private string result;
    private readonly HttpClient _client;
    private  StringContent _content;
    private HttpResponseMessage _responseMessage;
    private readonly SettingsServices _settings;

    private readonly string _requesUrl;
    // private readonly string _token;
    
    public ImageService(HttpClient client,HttpResponseMessage responseMessage,SettingsServices settings)
    {
        _client = client;
        _responseMessage = responseMessage;
        _settings = settings;
        _requesUrl = "https://api.openai.com/v1/images/generations";
        // _client.DefaultRequestHeaders.Add("authorization",$"Bearer {settings.ChatServiceSettings["OpenAiToken"]}");
    }
    

    public async Task SendMessageAsync(Image msg)
    {
        
        result = await ResponseAsync(_responseMessage, msg);
    }
    private async Task<string> ResponseAsync(HttpResponseMessage responseMessage,Image msg )
    {
        try
        {
            // var message = msg;
            _content = new StringContent(JsonConvert.SerializeObject(msg),Encoding.UTF8,"application/json");
            responseMessage = await _client.PostAsync(_requesUrl,_content);
            string responseString = await responseMessage.Content.ReadAsStringAsync();
            ImageResponce resultResponce = JsonConvert.DeserializeObject<ImageResponce>(responseString);
            return result = resultResponce.data[0].url;
        }
        catch (Exception e)
        {
            result = "Извините не удалось найти картинку на ваш запрос! ";
        }

        return result;
    }

    public string GetResult() => result;
    
}