
namespace SummonMyStrength.Maui.Data;

public interface IUserSettingsService
{
    UserSettings GetSettings();
    Task<UserSettings> GetSettingsAsync();
    Task SaveSettingsAsync();
}
