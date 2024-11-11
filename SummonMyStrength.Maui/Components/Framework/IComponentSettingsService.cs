using Microsoft.AspNetCore.Components;

namespace SummonMyStrength.Maui.Components.Framework;

public interface IComponentSettingsService
{
    event Action SettingsChanged;

    void AddComponentSettings(string sectionName, object owner, RenderFragment form);
    RenderFragment[] GetSettingsForms(string sectionName);
    string[] GetSettingsSections();
    void RemoveComponentSettings(string sectionName, object owner);
    void RefreshEditors();
}
