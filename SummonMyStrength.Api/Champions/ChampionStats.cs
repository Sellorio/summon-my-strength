using System.Text.Json.Serialization;

namespace SummonMyStrength.Api.Champions
{
    public class ChampionStats
    {
        [JsonPropertyName("hp")]
        public float Health { get; set; }

        [JsonPropertyName("hpperlevel")]
        public float HealthPerLevel { get; set; }

        [JsonPropertyName("mp")]
        public float Mana { get; set; }

        [JsonPropertyName("mpperlevel")]
        public float ManaPerLevel { get; set; }

        [JsonPropertyName("movespeed")]
        public float MoveSpeed { get; set; }

        [JsonPropertyName("armor")]
        public float Armor { get; set; }

        [JsonPropertyName("armorperlevel")]
        public float ArmorPerLevel { get; set; }

        [JsonPropertyName("spellblock")]
        public float MagicResist { get; set; }

        [JsonPropertyName("spellblockperlevel")]
        public float MagicResistPerLevel { get; set; }

        public float AttackRange { get; set; }

        [JsonPropertyName("hpregen")]
        public float HealthRegen { get; set; }

        [JsonPropertyName("hpregenperlevel")]
        public float HealthRegenPerLevel { get; set; }

        [JsonPropertyName("mpregen")]
        public float ManaRegen { get; set; }

        [JsonPropertyName("mpregenperlevel")]
        public float ManaRegenPerLevel { get; set; }

        [JsonPropertyName("crit")]
        public float CritChance { get; set; }

        [JsonPropertyName("critperlevel")]
        public float CritChancePerLevel { get; set; }

        public float AttackDamage { get; set; }

        public float AttackDamagePerLevel { get; set; }

        public float AttackSpeed { get; set; }

        public float AttackSpeedPerLevel { get; set; }
    }
}
