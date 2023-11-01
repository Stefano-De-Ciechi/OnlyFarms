namespace OnlyFarms.WebApp.Models;

public class Token
{
    public required string Value { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
}