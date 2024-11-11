using System.Text.Json;

namespace SummonMyStrength.Maui.Data;
internal class UserSettingsService : IUserSettingsService
{
    private static readonly string _settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Summon My Strength", "data.json");
    private SemaphoreSlim _semaphore = new(1);
    private UserSettings _settings;

    public UserSettings GetSettings()
    {
        if (_settings == null)
        {
            _semaphore.Wait();

            try
            {
                var text = File.ReadAllText(_settingsPath);

                _settings ??=
                    File.Exists(_settingsPath)
                        ? JsonSerializer.Deserialize<UserSettings>(text)
                        : new UserSettings();
            }
            catch
            {

            }

            _semaphore.Release();
        }

        return _settings;
    }

    public async Task<UserSettings> GetSettingsAsync()
    {
        if (_settings == null)
        {
            await _semaphore.WaitAsync();

            try
            {
                var text = await File.ReadAllTextAsync(_settingsPath);

                _settings ??=
                    File.Exists(_settingsPath)
                        ? JsonSerializer.Deserialize<UserSettings>(text)
                        : new UserSettings();
            }
            catch
            {

            }

            _semaphore.Release();
        }

        return _settings;
    }

    public async Task SaveSettingsAsync()
    {
        await GetSettingsAsync();
        Directory.CreateDirectory(Path.GetDirectoryName(_settingsPath));
        await File.WriteAllTextAsync(_settingsPath, JsonSerializer.Serialize(_settings));
    }
}
