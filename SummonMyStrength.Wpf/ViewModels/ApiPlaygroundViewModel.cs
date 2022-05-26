using SEA.Mvvm.ViewModelSupport;

namespace SummonMyStrength.ViewModels
{
    public class ApiPlaygroundViewModel : ViewModelWithModel
    {
        public string Url
        {
            get => Get<string>();
            set => Set(value);
        }

        public string Output
        {
            get => Get<string>();
            set => Set(value);
        }

        public CommandBase ExecuteGet
        {
            get => Get<CommandBase>();
            set => Set(value);
        }
    }
}
