using SummonMyStrength.Api;
using SummonMyStrength.Api.ChampSelect;
using SummonMyStrength.Maui.Services.ChampSelect;

namespace SummonMyStrength.Maui.Services
{
    internal class ChampSelectSessionAbstractor : IChampSelectSessionAbstractor
    {

        public event Func<ChampSelectPhase, ChampSelectPhase, Task> OnPhaseChanged;
        public event Func<int?, int?, Task> OnSelectedChampionIdChanged;

        public ChampSelectPhase Phase { get; private set; }
        public int? SelectedChampionId { get; private set; }

        public async Task ApplyChangesAsync(ChampSelectSession session)
        {
            if (session == null)
            {
                Phase = ChampSelectPhase.None;
                SelectedChampionId = null;
                return;
            }

            var phase = GetChampSelectPhase(session);

            if (phase != Phase)
            {
                var oldPhase = Phase;
                Phase = phase;
                await OnPhaseChanged.InvokeAsync(oldPhase, phase);
            }

            var selectedChampionId = GetSelectedChampionId(session);

            if (SelectedChampionId != selectedChampionId)
            {
                var oldSelectedChampionId = SelectedChampionId;
                SelectedChampionId = selectedChampionId;
                await OnSelectedChampionIdChanged.InvokeAsync(oldSelectedChampionId, selectedChampionId);
            }
        }

        private ChampSelectPhase GetChampSelectPhase(ChampSelectSession session)
        {
            switch (session.Timer.Phase)
            {
                case "PLANNING":
                    return ChampSelectPhase.PickIntent;
                case "BAN_PICK":
                    if (session.Actions.SelectMany(x => x).Any(x => x.Type == ActionType.Ban && x.IsInProgress == true))
                    {
                        var banAction = session.Actions.SelectMany(x => x).FirstOrDefault(x => x.ActorCellId == session.LocalPlayerCellId && x.Type == ActionType.Ban);

                        if (banAction == null)
                        {
                            return ChampSelectPhase.Banning;
                        }

                        if (banAction.IsInProgress == true)
                        {
                            return banAction.Completed == true ? ChampSelectPhase.BanComplete : ChampSelectPhase.Banning;
                        }

                        return banAction.Completed == true ? ChampSelectPhase.BanComplete : ChampSelectPhase.WaitingToBan;
                    }
                    else if (session.Actions.SelectMany(x => x).Any(x => x.Type == ActionType.Pick && x.IsInProgress == true))
                    {
                        var pickAction = session.Actions.SelectMany(x => x).FirstOrDefault(x => x.ActorCellId == session.LocalPlayerCellId && x.Type == ActionType.Pick);

                        if (pickAction == null)
                        {
                            return ChampSelectPhase.Picking;
                        }

                        if (pickAction.IsInProgress == true)
                        {
                            return pickAction.Completed == true ? ChampSelectPhase.PickCompelete : ChampSelectPhase.Picking;
                        }

                        return pickAction.Completed == true ? ChampSelectPhase.PickCompelete : ChampSelectPhase.WaitingToPick;
                    }
                    else
                    {
                        return ChampSelectPhase.BanComplete;
                    }
                case "FINALIZATION":
                    return ChampSelectPhase.FinalSetup;
                default:
                    return ChampSelectPhase.None;
            }
        }

        private int? GetSelectedChampionId(ChampSelectSession session)
        {
            var championId = session.Player.ChampionId == default ? session.Player.ChampionPickIntent : session.Player.ChampionId;

            if (championId == 0)
            {
                return null;
            }

            var hasBeenBannedOrPickedBySomeoneElse =
                session.Actions
                    .SelectMany(x => x)
                    .Any(x => (x.Type == ActionType.Ban || x.ActorCellId != session.LocalPlayerCellId) && x.ChampionId == championId && x.Completed == true);

            return hasBeenBannedOrPickedBySomeoneElse ? null : championId;
        }
    }
}
