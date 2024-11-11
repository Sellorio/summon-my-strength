//using SummonMyStrength.Api;
//using SummonMyStrength.Api.ChampSelect;
//using SummonMyStrength.Api.General;

//namespace SummonMyStrength.Maui.Services;

//internal class ChampSelectSessionAccessor : IChampSelectSessionAccessor
//{
//    private readonly LeagueClient _leagueClient;

//    public ChampSelectSession Session { get; private set; }

//    public event Func<ChampSelectSession, ChampSelectSession, Task> SessionChanged;

//    public ChampSelectSessionAccessor(LeagueClient leagueClient)
//    {
//        _leagueClient = leagueClient;
//        _leagueClient.Gameflow.GameflowPhaseChanged += GameflowPhaseChanged;
//        _leagueClient.ChampSelect.SessionChanged += ChampSelectSessionChanged;
//    }

//    private async Task GameflowPhaseChanged(GameflowPhase newPhase)
//    {
//        if (newPhase != GameflowPhase.ChampSelect && Session != null)
//        {
//            var oldSession = Session;
//            Session = null;
//            await SessionChanged.InvokeAsync(oldSession, null);
//        }
//    }

//    private async Task ChampSelectSessionChanged(ChampSelectSession session)
//    {
//        if (Session != session)
//        {
//            var oldSession = Session;
//            Session = session;
//            await SessionChanged.InvokeAsync(oldSession, session);
//        }
//    }
//}
