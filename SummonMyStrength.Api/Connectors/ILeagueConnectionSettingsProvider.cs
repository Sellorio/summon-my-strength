using System;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.Connectors;

public interface ILeagueConnectionSettingsProvider
{
    /// <summary>
    /// Attempts to read the settings into the <see cref="Settings"/> property.
    /// </summary>
    /// <returns></returns>
    [SupportedOSPlatform("windows")]
    Task<bool> TryReadAsync(Action<LeagueConnectionSettings> settingsOut);
}
