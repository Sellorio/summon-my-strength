using System.Collections.Generic;

namespace SummonMyStrength.Api.Data
{
    public class SummonerSpell
    {
        public int Id { get; }
        public string Name { get; }
        public string Description { get; }
        public int Cooldown { get; }
        public byte[] Icon { get; }
        public IReadOnlyList<string> Modes { get; }

        public SummonerSpell(int id, string name, string description, int cooldown, byte[] icon, IReadOnlyList<string> modes)
        {
            Id = id;
            Name = name;
            Description = description;
            Cooldown = cooldown;
            Icon = icon;
            Modes = modes;
        }
    }
}
