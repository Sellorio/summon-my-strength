using System.ComponentModel.DataAnnotations;

namespace SummonMyStrength.Api.PowerSystems.Items;

public enum ItemStatType
{
    // Offence

    [Display(Name = "Attack Damage")]
    [WikiLink("https://wiki.leagueoflegends.com/en-us/Attack_damage")]
    AttackDamage,

    [Display(Name = "Ability Power")]
    [WikiLink("https://wiki.leagueoflegends.com/en-us/Ability_power")]
    AbilityPower,

    [Display(Name = "Critical Strike Chance")]
    [WikiLink("https://wiki.leagueoflegends.com/en-us/Critical_strike")]
    CritChance,

    [Display(Name = "Critical Strike Damage")]
    [WikiLink("https://wiki.leagueoflegends.com/en-us/Critical_strike")]
    CritDamage,

    [Display(Name = "Attack Speed")]
    [WikiLink("https://wiki.leagueoflegends.com/en-us/Attack_speed")]
    AttackSpeed,

    // Penetration

    [Display(Name = "Lethality")]
    [WikiLink("https://wiki.leagueoflegends.com/en-us/Armor_penetration#Lethality")]
    Lethality,

    [Display(Name = "Armor Penetration")]
    [WikiLink("https://wiki.leagueoflegends.com/en-us/Armor_penetration")]
    ArmorPenetration,

    [Display(Name = "Magic Penetration")]
    [WikiLink("https://wiki.leagueoflegends.com/en-us/Magic_penetration")]
    MagicPenetration,

    // Defense

    [Display(Name = "Health")]
    [WikiLink("https://wiki.leagueoflegends.com/en-us/Health")]
    Health,

    [Display(Name = "Armor")]
    [WikiLink("https://wiki.leagueoflegends.com/en-us/Armor")]
    Armor,

    [Display(Name = "Magic Resistance")]
    [WikiLink("https://wiki.leagueoflegends.com/en-us/Magic_resistance")]
    MagicResistance,

    [Display(Name = "Tenacity")]
    [WikiLink("https://wiki.leagueoflegends.com/en-us/Tenacity")]
    Tenacity,

    // Sustain

    [Display(Name = "Lifesteal")]
    [WikiLink("https://wiki.leagueoflegends.com/en-us/Life_steal")]
    Lifesteal,

    [Display(Name = "Base Health Regeneration")]
    [WikiLink("https://wiki.leagueoflegends.com/en-us/Health_regeneration")]
    HealthRegeneration,

    [Display(Name = "Mana")]
    [WikiLink("https://wiki.leagueoflegends.com/en-us/Mana")]
    Mana,

    [Display(Name = "Base Mana Regeneration")]
    [WikiLink("https://wiki.leagueoflegends.com/en-us/Mana_regeneration")]
    ManaRegeneration,

    // Utility

    [Display(Name = "Ability Haste")]
    [WikiLink("https://wiki.leagueoflegends.com/en-us/Haste#Ability_haste")]
    AbilityHaste,

    [Display(Name = "Heal & Shield Power")]
    [WikiLink("https://wiki.leagueoflegends.com/en-us/Heal_and_shield_power")]
    HealAndShieldPower,

    [Display(Name = "Movement Speed")]
    [WikiLink("https://wiki.leagueoflegends.com/en-us/Movement_speed")]
    MovementSpeed,

    [Display(Name = "Gold")]
    [WikiLink("https://wiki.leagueoflegends.com/en-us/Gold")]
    GoldGeneration,

    // Other

    [Display(Name = "???")]
    [WikiLink("https://wiki.leagueoflegends.com/")]
    Unknown
}
