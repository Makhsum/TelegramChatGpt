namespace ChatGPT.Models;

public class QuestionMessage
{
    public string model { get; set; }
    public string prompt { get; set; }
    public double temperature { get; set; }
    public int max_tokens { get; set; }
    public double top_p { get; set; }
    public double frequency_penalty { get; set; }
    public double presence_penalty { get; set; }
    
    
    
}