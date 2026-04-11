using System;
using WixSharp;
using WixSharp.Forms;

namespace SummonMyStrength.Installer
{
    public class Program
    {
        static void Main()
        {
            const string installDir = @"%ProgramFiles64Folder%\Summon My Strength";
            const string applicationExe = "Summon My Strength.exe";

            var project = new ManagedProject("Summon My Strength",
                             new InstallDir(installDir,
                                 new Files(@"..\SummonMyStrength.Maui\publish\*")), // Files is recursive by default, DirFiles is non-recursive
                             new Dir("%ProgramMenu%\\Summon My Strength",
                                new ExeFileShortcut(
                                     "Summon My Strength",
                                     $"[INSTALLDIR]{applicationExe}",
                                     "")
                                {
                                    WorkingDirectory = @"[INSTALLDIR]"
                                }))
            {
                GUID = new Guid("6ff5f687-8f16-404a-829e-fe85b11e1524"),
                ProductId = Guid.NewGuid(),
                Platform = Platform.x64,
                ManagedUI = new ManagedUI(),
                MajorUpgrade = new MajorUpgrade
                {
                    AllowSameVersionUpgrades = true,
                    DowngradeErrorMessage = "A newer version of [ProductName] is already installed.",
                    Schedule = UpgradeSchedule.afterInstallExecute
                }
            };

            project.ManagedUI.InstallDialogs
                .Add(Dialogs.Progress)
                .Add(Dialogs.Exit);

            project.ManagedUI.ModifyDialogs
                .Add(Dialogs.Progress)
                .Add(Dialogs.Exit);

            project.BuildMsi();
        }
    }
}