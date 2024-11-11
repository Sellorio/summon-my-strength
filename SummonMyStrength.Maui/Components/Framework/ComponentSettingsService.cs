using Microsoft.AspNetCore.Components;

namespace SummonMyStrength.Maui.Components.Framework;

internal class ComponentSettingsService : IComponentSettingsService
{
    private readonly Dictionary<string, List<(object Owner, RenderFragment RenderFragment)>> _settingsSections = [];

    public event Action SettingsChanged;

    public void AddComponentSettings(string sectionName, object owner, RenderFragment form)
    {
        if (!_settingsSections.TryGetValue(sectionName, out var settings))
        {
            _settingsSections.Add(sectionName, settings = []);
        }

        settings.Add((owner, form));
        SettingsChanged?.Invoke();
    }

    public void RemoveComponentSettings(string sectionName, object owner)
    {
        var settings = _settingsSections[sectionName];

        foreach (var form in settings.ToArray())
        {
            if (form.Owner == owner)
            {
                settings.Remove(form);
            }
        }

        if (settings.Count == 0)
        {
            _settingsSections.Remove(sectionName);
        }

        SettingsChanged?.Invoke();
    }

    public string[] GetSettingsSections()
    {
        return _settingsSections.Keys.ToArray();
    }

    public RenderFragment[] GetSettingsForms(string sectionName)
    {
        _settingsSections.TryGetValue(sectionName, out var settings);
        return settings.Select(x => x.RenderFragment).ToArray();
    }

    public void RefreshEditors()
    {
        SettingsChanged?.Invoke();
    }
}
