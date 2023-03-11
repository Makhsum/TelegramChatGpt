using ChatGPT.Models;
using ChatGPT.Services.abstractions;
using Newtonsoft.Json;

namespace ChatGPT.Services;

public class SettingsServices
{
    private string currentEnvironmentOcVersion = Environment.OSVersion.ToString();
    private string questionMessagepath = Directory.GetCurrentDirectory() + "/QuestionMessageSettings.json";
    private string appSettingspath = Directory.GetCurrentDirectory() + "/appsettings.json";
    public QuestionMessage questionMessage { get; set; }
    public Dictionary<string,string>? ChatServiceSettings { get; set; }

    public SettingsServices()
    {
        ReadProperties();
    }
    private void ReadProperties()
    {
       
        if (currentEnvironmentOcVersion.Contains("Windows"))
        {
            questionMessagepath = questionMessagepath.Replace("/","\\");
            appSettingspath = appSettingspath.Replace("/","\\");
        }


        if (File.Exists(questionMessagepath) &&File.Exists(appSettingspath))
        {
            try
            {
                
                var deserializedModel = File.ReadAllText(questionMessagepath);
                var deserializedDictionaty = File.ReadAllText(appSettingspath);
                SetProperties(deserializedModel, deserializedDictionaty);
            }
            catch (Exception e)
            {
                Console.WriteLine("Неверная конфигурация в файлах 'QuestionMessageSettings.json' и 'appsettings.json' или может они не существуют!");
                throw new InvalidOperationException(e.Message);
            }
           
        }
        else
        {
            throw new FileNotFoundException("Отсутствует фалы конфигурации!");
        }
        
    }

    private void SetProperties(string deserializedModel, string deserializedDictionaty)
    {
        questionMessage = JsonConvert.DeserializeObject<QuestionMessage>(deserializedModel)?? new QuestionMessage();
        ChatServiceSettings = JsonConvert.DeserializeObject<Dictionary<string, string>>(deserializedDictionaty);
    }
    
}