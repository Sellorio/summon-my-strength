using System;
using WixSharp;
using WixSharp.Forms;

namespace SummonMyStrength.Installer
{
    public class Program
    {
        static void Main()
        {
            var project = new ManagedProject("Summon My Strength",
                             new InstallDir(@"C:\SummonMyStrength",
                                 new Files(@"..\SummonMyStrength.Maui\publish\*")), // Files is recursive by default, DirFiles is non-recursive
                             new Dir("%ProgramMenu%",
                                new ExeFileShortcut(
                                     "Summon My Strength",
                                     @"C:\SummonMyStrength\Summon My Strength.exe",
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
                    AllowDowngrades = true,
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