namespace SummonMyStrength.Api
{
    internal class LeagueClientConnectionSettings
    {
        public int PortNumber { get; }
        public string Password { get; }

        public LeagueClientConnectionSettings(int portNumber, string password)
        {
            PortNumber = portNumber;
            Password = password;
        }
    }
}
