using SummonMyStrength.ViewModels;

namespace SummonMyStrength.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView
    {
        public MainView()
        {
            InitializeComponent();

            Loaded += MainView_Loaded;
        }

        private void MainView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).Initialise.Execute(null);
        }
    }
}
