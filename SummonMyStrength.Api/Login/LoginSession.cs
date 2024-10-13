using System.Text.Json.Serialization;

namespace SummonMyStrength.Api.Login;

public class LoginSession
{
    public long AccountId { get; set; }
    public bool Connected { get; set; }
    public LoginError Error { get; set; }
    //gasToken has no schema
    public string IdToken { get; set; }
    public bool IsInLoginQueue { get; set; }
    public bool IsNewPlayer { get; set; }
    public string Puuid { get; set; }

    [JsonPropertyName("state")]
    public LoginStateRaw RawLoginState { get; set; }

    public LoginState State
    {
        get => (LoginState)(int)RawLoginState;
        set => RawLoginState = (LoginStateRaw)(int)value;
    }

    public long SummonerId { get; set; }
    public string UserAuthToken { get; set; }
    public string Username { get; set; }
}
