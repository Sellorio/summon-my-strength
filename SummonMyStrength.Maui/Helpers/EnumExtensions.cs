namespace SummonMyStrength.Maui.Helpers
{
    public static class EnumExtensions
    {
        public static string ToDisplay(this Enum @enum)
        {
            return string.Join(string.Empty, @enum.ToString().Select(x => (char.IsUpper(x) ? " " : string.Empty) + x)).Trim();
        }
    }
}
