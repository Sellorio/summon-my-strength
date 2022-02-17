using SEA.Mvvm.ModelSupport;
using SummonMyStrength.ViewModels;
using System.Text.Json;
using System.Threading.Tasks;

namespace SummonMyStrength.Models
{
    class ApiPlaygroundModel : ModelBase<ApiPlaygroundViewModel>
    {
        private static readonly JsonSerializerOptions _prettyPrintOptions;

        static ApiPlaygroundModel()
        {
            _prettyPrintOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            _prettyPrintOptions.WriteIndented = true;
        }

        public ApiPlaygroundModel(IInterface @interface, ApiPlaygroundViewModel viewModel)
            : base(@interface, viewModel)
        {
        }

        protected override void BindModel(ModelBinder<ApiPlaygroundViewModel> modelBinder)
        {
            modelBinder.BindAsync(x => x.ExecuteGet, ExecuteGetAsync);
        }

        private async Task ExecuteGetAsync()
        {
            ViewModel.Output = await App.LeagueClient.HttpClient.GetStringAsync(ViewModel.Url);

            if (ViewModel.Output.StartsWith("[") || ViewModel.Output.StartsWith("{"))
            {
                ViewModel.Output =
                    JsonSerializer.Serialize(
                        JsonSerializer.Deserialize<JsonElement>(ViewModel.Output, _prettyPrintOptions),
                        _prettyPrintOptions);
            }
        }
    }
}
