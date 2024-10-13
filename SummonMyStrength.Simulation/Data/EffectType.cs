namespace SummonMyStrength.Simulation.Data;

public enum EffectType
{
    // Stats - supports Duration
    AttackDamage,
    AttackDamagePercent,
    AbilityPower,
    AbilityPowerPercent,
    AttackSpeed,
    AbilityHaste,
    AbilityHastePercent,
    MovementSpeed,
    MovementSpeedPercent,
    Slow,
    SlowPercent,
    CritChance,
    CritDamage,
    MaxHealth,
    MaxHealthPercent,
    Armor,
    ArmorPercent,
    ArmorPenetration,
    ArmorPenetrationPercent,
    MagicResistance,
    MagicResistancePercent,
    MagicResistancePenetration,
    MagicResistancePenetrationPercent,

    // Ramping/Decaying Stats - requires Duration
    MovementSpeedPercentRamping,
    MovementSpeedPercentDecaying,
    SlowPercentRamping,
    SlowPercentDecaying,

    // Deal Damage (pre-mitigation) - supports Duration and Frequency
    PhysicalDamage,
    CurrentHealthPhysicalDamage,
    MaxHealthPhysicalDamage,
    MagicDamage,
    CurrentHealthMagicDamage,
    MaxHealthMagicDamage,
    TrueDamage,
    CurrentHealthTrueDamage,
    MaxHealthTrueDamage
}
