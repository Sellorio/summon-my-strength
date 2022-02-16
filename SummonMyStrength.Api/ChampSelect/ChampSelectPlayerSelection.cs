using System;

namespace SummonMyStrength.Api.ChampSelect
{
    public class ChampSelectPlayerSelection
    {
        public string AssignedPosition { get; set; }
        public long CellId { get; set; }
        public int ChampionId { get; set; }
        public int ChampionPickIntent { get; set; }
        public string EntitledFeatureType { get; set; }
        public int SelectedSkinId { get; set; }
        public int Spell1Id { get; set; }
        public int Spell2Id { get; set; }
        public long SummonerId { get; set; }
        public int Team { get; set; }
        public int WardSkinId { get; set; }

        public ChampSelectAssignedPosition? Position
        {
            get => AssignedPosition?.ToLower() switch
            {
                null => null,
                "" => null,
                "utility" => ChampSelectAssignedPosition.Support,
                "top" => ChampSelectAssignedPosition.Top,
                "middle" => ChampSelectAssignedPosition.Middle,
                "bottom" => ChampSelectAssignedPosition.Bottom,
                "jungle" => ChampSelectAssignedPosition.Jungle,
                _ => throw new NotSupportedException()
            };
            set => AssignedPosition = value switch
            {
                null => "",
                ChampSelectAssignedPosition.Support => "utility",
                ChampSelectAssignedPosition.Top => "top",
                ChampSelectAssignedPosition.Middle => "middle",
                ChampSelectAssignedPosition.Bottom => "bottom",
                ChampSelectAssignedPosition.Jungle => "jungle",
                _ => throw new NotSupportedException()
            };
        }
    }
}
