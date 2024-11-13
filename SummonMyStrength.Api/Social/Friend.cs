namespace SummonMyStrength.Api.Social;

public class Friend
{
    public long SummonerId { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
    public string Pid { get; set; }
    public string Puuid { get; set; }
    public string GameName { get; set; }
    public string GameTag { get; set; }
    public int Icon { get; set; }
    public string Availability { get; set; }
    public string PlatformId { get; set; }
    public string Patchline { get; set; }
    public string Product { get; set; }
    public string ProductName { get; set; }
    public string Summary { get; set; }
    public long Time { get; set; }
    public string StatusMessage { get; set; }
    public string Note { get; set; }
    public string LastSeenOnlineTimestamp { get; set; }
    public bool IsP2PConversationMuted { get; set; }
    public long GroupId { get; set; }
    public long DisplayGroupId { get; set; }
    public string GroupName { get; set; }
    public string DisplayGroupName { get; set; }
}
