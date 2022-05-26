using SummonMyStrength.Api;
using SummonMyStrength.Views;
using System.Windows;

namespace SummonMyStrength
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static LeagueClient LeagueClient { get; set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SEA.Mvvm.Wpf.WpfMvvm.Initialise();
            new MainView().Show();
        }
    }
}
