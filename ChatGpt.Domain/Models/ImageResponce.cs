namespace ChatGpt.Domain.Models;

public class ImageResponce
{
    public long created { get; set; }
    public ImageData[] data { get; set; }
}