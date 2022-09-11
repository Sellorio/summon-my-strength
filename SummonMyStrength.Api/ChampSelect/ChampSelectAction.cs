using System.Text.Json.Serialization;

namespace SummonMyStrength.Api.ChampSelect
{
    public class ChampSelectAction
    {
        public long? Id { get; set; }
        public long? ActorCellId { get; set; }
        public int? ChampionId { get; set; }
        public bool? IsAllyAction { get; set; }
        public bool? IsInProgress { get; set; }
        public bool? Completed { get; set; }
        public int? PickTurn { get; set; }

        [JsonIgnore]
        public ActionType Type { get; set; }

        [JsonPropertyName("type")]
        public string TypeString
        {
            get
            {
                return Type.ToString().ToLower();
            }
            set
            {
                Type = value switch
                {
                    "pick" => ActionType.Pick,
                    "ban" => ActionType.Ban,
                    "ten_bans_reveal" => ActionType.TenBansReveal,
                    _ => ActionType.Other
                };
            }
        }
    }
}
