using System.Text.Json.Serialization;

namespace SummonMyStrength.Maui.Services.LiveApi;

public class DetailedStats
{
    public decimal AbilityHaste { get; set; }
    public decimal AbilityPower { get; set; }
    public decimal Armor { get; set; }
    public decimal ArmorPenetrationFlat { get; set; }
    public decimal ArmorPenetrationPercent { get; set; }
    public decimal AttackDamage { get; set; }
    public decimal AttackRange { get; set; }
    public decimal AttackSpeed { get; set; }
    public decimal BonusArmorPenetrationPercent { get; set; }
    public decimal BonusMagicPenetrationPercent { get; set; }
    public decimal CritChance { get; set; }
    public decimal CritDamage { get; set; }
    public decimal CurrentHealth { get; set; }
    public decimal HealShieldPower { get; set; }
    public decimal HealthRegenRate { get; set; }
    public decimal LifeSteal { get; set; }
    public decimal MagicLethality { get; set; }
    public decimal MagicPenetrationFlat { get; set; }
    public decimal MagicPenetrationPercent { get; set; }
    public decimal MagicResist { get; set; }
    public decimal MaxHealth { get; set; }
    public decimal MoveSpeed { get; set; }
    public decimal Omnivamp { get; set; }
    public decimal PhysicalLethality { get; set; }
    public decimal PhysicalVamp { get; set; }
    public decimal ResourceMax { get; set; }
    public decimal ResourceRegenRate { get; set; }
    [JsonPropertyName("resourceType")]
    public string ResourceTypeString { get; set; }
    public decimal ResourceValue { get; set; }
    public decimal SpellVamp { get; set; }
    public decimal Tenacity { get; set; }
}
