using System.Collections.Generic;

namespace SummonMyStrength.Api.Perks
{
    public class Style
    {
        public int Id { get; set; }
        public int[] AllowedSubStyles { get; set; }
        public Dictionary<string, string> AssetMap { get; set; }
        public string DefaultPageName { get; set; }
        public int[] DefaultPerks { get; set; }
        public int DefaultSubStyle { get; set; }
        public string IconPath { get; set; }
        public string Name { get; set; }
        public Slot[] Slots { get; set; }
        public PerkSubStyleBonus[] SubStyleBonus { get; set; }
        public string Tooltip { get; set; }
    }
}
