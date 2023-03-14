using System.Text;
using ChatGpt.Domain.Models;
using Newtonsoft.Json;

namespace ChatGpt.Application.Services;

public class QuestionService
{
    private string result;
    private readonly HttpClient _client;
    private  StringContent _content;
    private HttpResponseMessage _responseMessage;
    private readonly SettingsServices _settings;

    private readonly string _requesUrl;
    // private readonly string _token;
    
    public QuestionService(HttpClient client,HttpResponseMessage responseMessage,SettingsServices settings)
    {
        _client = client;
        _responseMessage = responseMessage;
        _settings = settings;
        _requesUrl = settings.ChatServiceSettings["requestUrl"];
        // _client.DefaultRequestHeaders.Add("authorization",$"Bearer {settings.ChatServiceSettings["OpenAiToken"]}");
        // _client.DefaultRequestHeaders.Add("authorization","Bearer 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");
    }
    

    public async Task SendMessageAsync(QuestionMessage msg)
    {
        
        result = await ResponseAsync(_responseMessage, msg);
    }
    private async Task<string> ResponseAsync(HttpResponseMessage responseMessage,QuestionMessage msg )
    {
        try
        {
            // var message = msg;
            string requsetUrl = "";
            _content = new StringContent(JsonConvert.SerializeObject(msg),Encoding.UTF8,"application/json");
            responseMessage = await _client.PostAsync(_requesUrl,_content);
            string responseString = await responseMessage.Content.ReadAsStringAsync();
            dynamic  dyData = JsonConvert.DeserializeObject<dynamic>(responseString);
            return result = dyData.choices[0].text;
        }
        catch (Exception e)
        {
            result = "Извините не удалось найти ответ на ваш вопрос! ";
        }

        return result;
    }

    public string GetResult() => result;

}