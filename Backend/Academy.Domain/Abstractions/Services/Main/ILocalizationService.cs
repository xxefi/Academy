namespace Academy.Domain.Abstractions.Services.Main;

public interface ILocalizationService
{
    string GetMessage(string key, string language = "en");
}