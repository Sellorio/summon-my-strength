namespace SummonMyStrength.Api.Connectors;

public class LeagueConnectionSettings
{
    public int PortNumber { get; }
    public string Password { get; }

    public LeagueConnectionSettings(int portNumber, string password)
    {
        PortNumber = portNumber;
        Password = password;
    }
}
