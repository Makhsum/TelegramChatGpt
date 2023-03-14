namespace ChatGpt.Domain.Models;

public class User
{
    public long ChatId { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}