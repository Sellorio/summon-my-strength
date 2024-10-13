using System.Diagnostics;
using System.Management;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;
using System.Runtime.Versioning;

namespace SummonMyStrength.Api.Connectors;

internal partial class LeagueConnectionSettingsProvider : ILeagueConnectionSettingsProvider, IDisposable
{
    [GeneratedRegex("\"--app-port=(\\d+?)\"")]
    private static partial Regex PortRegex();

    [GeneratedRegex("\"--remoting-auth-token=(.+?)\"")]
    private static partial Regex AuthTokenRegex();

    private Process _leagueOfLegendsClientProcess;
    private LeagueConnectionSettings _settings;

    [SupportedOSPlatform("windows")]
    public Task<bool> TryReadAsync(Action<LeagueConnectionSettings> settingsOut)
    {
        if (_settings != null)
        {
            settingsOut.Invoke(_settings);
            return Task.FromResult(true);
        }

        if (_leagueOfLegendsClientProcess == null)
        {
            _leagueOfLegendsClientProcess = Process.GetProcessesByName("LeagueClientUx").FirstOrDefault();

            if (_leagueOfLegendsClientProcess == null)
            {
                return Task.FromResult(false);
            }

            _leagueOfLegendsClientProcess.Exited += OnLeagueOfLegendsClientProcessExited;
        }

        using var managementObjectSearcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + _leagueOfLegendsClientProcess.Id);
        using var moc = managementObjectSearcher.Get();

        var commandLine = (string)moc.OfType<ManagementObject>().First()["CommandLine"];

        try
        {
            var password = AuthTokenRegex().Match(commandLine).Groups[1].Value;
            var port = PortRegex().Match(commandLine).Groups[1].Value;

            _settings = new LeagueConnectionSettings(int.Parse(port), password);
            settingsOut.Invoke(_settings);

            return Task.FromResult(true);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Error while trying to get the status for LeagueClientUx: {e}\n\n(CommandLine = {commandLine})");
        }
    }

    public void Dispose()
    {
        if (_leagueOfLegendsClientProcess != null)
        {
            _leagueOfLegendsClientProcess.Exited -= OnLeagueOfLegendsClientProcessExited;
        }
    }

    private void OnLeagueOfLegendsClientProcessExited(object sender, EventArgs e)
    {
        _leagueOfLegendsClientProcess.Exited -= OnLeagueOfLegendsClientProcessExited;
        _leagueOfLegendsClientProcess = null;
        _settings = null;
    }
}
