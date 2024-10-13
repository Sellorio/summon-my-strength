using System.Collections.Generic;

namespace SummonMyStrength.Api.PostGame;

public class PostGamePlayerStats
{
    public int ChampionId { get; set; }
    public string ChampionName { get; set; }
    public long SummonerId { get; set; }
    public string SummonerName { get; set; }
    public int[] Items { get; set; }

    public Dictionary<string, int> Stats { get; set; }
    public int Kills => Stats.TryGetValue("CHAMPIONS_KILLED", out int kills) ? kills : 0;
    public int Assists => Stats.TryGetValue("ASSISTS", out int assists) ? assists : 0;
    public int Deaths => Stats.TryGetValue("NUM_DEATHS", out int deaths) ? deaths : 0;
    public int Level => Stats.TryGetValue("LEVEL", out int level) ? level : 0;
    public int Gold => Stats.TryGetValue("GOLD_EARNED", out int gold) ? gold : 0;
    public int CritterScore => Stats.TryGetValue("MINIONS_KILLED", out int cs) ? cs : 0;
    public int VisionScore => Stats.TryGetValue("VISION_SCORE", out int vision) ? vision : 0;

    public int CrowdControlScore => Stats.TryGetValue("TIME_CCING_OTHERS", out int ccScore) ? ccScore : 0;
    public int CrowdControlReceived => Stats.TryGetValue("TOTAL_TIME_CROWD_CONTROL_DEALT", out int ccedScore) ? ccedScore : 0;

    public int TotalDamage => Stats.TryGetValue("TOTAL_DAMAGE_DEALT_TO_CHAMPIONS", out int totalDamage) ? totalDamage : 0;
    public int MagicDamage => Stats.TryGetValue("MAGIC_DMAAGE_DEALT_TO_CHAMPIONS", out int magicDamage) ? magicDamage : 0;
    public int PhysicalDamage => Stats.TryGetValue("PHYSICAL_DAMAGE_DEALT_TO_CHAMPIONS", out int physicalDamage) ? physicalDamage : 0;
    public int TrueDamage => Stats.TryGetValue("TRUE_DAMAGE_DEALT_TO_CHAMPIONS", out int trueDamage) ? trueDamage : 0;

    public int DamageTaken => Stats.TryGetValue("TOTAL_DAMAGE_TAKEN", out int damageTaken) ? damageTaken : 0;
    public int MagicDamageTaken => Stats.TryGetValue("MAGIC_DAMAGE_TAKEN", out int magicDamageTaken) ? magicDamageTaken : 0;
    public int PhysicalDamageTaken => Stats.TryGetValue("PHYSICAL_DAMAGE_TAKEN", out int physicalDamageTaken) ? physicalDamageTaken : 0;
    public int TrueDamageTaken => Stats.TryGetValue("TRUE_DAMAGE_TAKEN", out int trueDamageTaken) ? trueDamageTaken : 0;
    public int SelfMitigatedDamage => Stats.TryGetValue("TOTAL_DAMAGE_SELF_MITIGATED", out int selfMitigatedDamage) ? selfMitigatedDamage : 0;

    public int Healing => Stats.TryGetValue("TOTAL_HEAL", out int healing) ? healing : 0;
    public int AllyHealing => Stats.TryGetValue("TOTAL_HEAL_ON_TEAMMATES", out int allyHealing) ? allyHealing : 0;
    public int AllyShielding => Stats.TryGetValue("TOTAL_DAMAGE_SHIELDED_ON_TEAMMATES", out int allyShielding) ? allyShielding : 0;
}

// Stats:
//"ASSISTS": 11,
//"BARRACKS_KILLED": 0,
//"CHAMPIONS_KILLED": 4,
//"GAME_ENDED_IN_EARLY_SURRENDER": 0,
//"GAME_ENDED_IN_SURRENDER": 1,
//"GOLD_EARNED": 8743,
//"LARGEST_CRITICAL_STRIKE": 1567,
//"LARGEST_KILLING_SPREE": 2,
//"LARGEST_MULTI_KILL": 2,
//"LEVEL": 11,
//"MAGIC_DAMAGE_DEALT_PLAYER": 0,
//"MAGIC_DAMAGE_DEALT_TO_CHAMPIONS": 0,
//"MAGIC_DAMAGE_TAKEN": 9063,
//"MINIONS_KILLED": 9,
//"NEUTRAL_MINIONS_KILLED": 0,
//"NEUTRAL_MINIONS_KILLED_ENEMY_JUNGLE": 0,
//"NEUTRAL_MINIONS_KILLED_YOUR_JUNGLE": 0,
//"NUM_DEATHS": 5,
//"PERK0": 8369,
//"PERK0_VAR1": 419,
//"PERK0_VAR2": 376,
//"PERK0_VAR3": 0,
//"PERK1": 8321,
//"PERK1_VAR1": 372,
//"PERK1_VAR2": 0,
//"PERK1_VAR3": 0,
//"PERK2": 8345,
//"PERK2_VAR1": 3,
//"PERK2_VAR2": 0,
//"PERK2_VAR3": 192,
//"PERK3": 8316,
//"PERK3_VAR1": 8,
//"PERK3_VAR2": 10,
//"PERK3_VAR3": 0,
//"PERK4": 8009,
//"PERK4_VAR1": 1564,
//"PERK4_VAR2": 0,
//"PERK4_VAR3": 0,
//"PERK5": 9104,
//"PERK5_VAR1": 15,
//"PERK5_VAR2": 20,
//"PERK5_VAR3": 0,
//"PERK_PRIMARY_STYLE": 8300,
//"PERK_SUB_STYLE": 8000,
//"PHYSICAL_DAMAGE_DEALT_PLAYER": 33128,
//"PHYSICAL_DAMAGE_DEALT_TO_CHAMPIONS": 14830,
//"PHYSICAL_DAMAGE_TAKEN": 7508,
//"PLAYER_AUGMENT_1": 0,
//"PLAYER_AUGMENT_2": 0,
//"PLAYER_AUGMENT_3": 0,
//"PLAYER_AUGMENT_4": 0,
//"PLAYER_AUGMENT_5": 0,
//"PLAYER_AUGMENT_6": 0,
//"PLAYER_SUBTEAM": 0,
//"PLAYER_SUBTEAM_PLACEMENT": 0,
//"SIGHT_WARDS_BOUGHT_IN_GAME": 0,
//"SPELL1_CAST": 52,
//"SPELL2_CAST": 34,
//"TEAM_EARLY_SURRENDERED": 0,
//"TEAM_OBJECTIVE": 0,
//"TIME_CCING_OTHERS": 39,
//"TOTAL_DAMAGE_DEALT": 36345,
//"TOTAL_DAMAGE_DEALT_TO_BUILDINGS": 4283,
//"TOTAL_DAMAGE_DEALT_TO_CHAMPIONS": 15244,
//"TOTAL_DAMAGE_DEALT_TO_OBJECTIVES": 6264,
//"TOTAL_DAMAGE_DEALT_TO_TURRETS": 4283,
//"TOTAL_DAMAGE_SELF_MITIGATED": 8476,
//"TOTAL_DAMAGE_SHIELDED_ON_TEAMMATES": 190,
//"TOTAL_DAMAGE_TAKEN": 16797,
//"TOTAL_HEAL": 5712,
//"TOTAL_HEAL_ON_TEAMMATES": 629,
//"TOTAL_TIME_CROWD_CONTROL_DEALT": 175,
//"TOTAL_TIME_SPENT_DEAD": 118,
//"TRUE_DAMAGE_DEALT_PLAYER": 3217,
//"TRUE_DAMAGE_DEALT_TO_CHAMPIONS": 414,
//"TRUE_DAMAGE_TAKEN": 226,
//"TURRETS_KILLED": 1,
//"VISION_SCORE": 16,
//"VISION_WARDS_BOUGHT_IN_GAME": 0,
//"WARD_KILLED": 1,
//"WARD_PLACED": 7,
//"WAS_AFK": 0,
//"WIN": 1
