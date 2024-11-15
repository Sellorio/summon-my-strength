if exist .\bin\Release\net8.0-windows10.0.19041.0 (rmdir .\bin\Release\net8.0-windows10.0.19041.0 /s /q)
if exist .\publish (rmdir .\publish /s /q)

dotnet publish -f net8.0-windows10.0.19041.0 -c Release --self-contained -p:WindowsPackageType=None -p:PublishTrimmed

move .\bin\Release\net8.0-windows10.0.19041.0\win10-x64\publish .\publish
rename .\publish\SummonMyStrength.Maui.exe "Summon My Strength.exe"
