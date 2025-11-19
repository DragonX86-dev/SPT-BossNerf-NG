using System.Text.Json.Serialization;

namespace BossNerf.NG;

public record BossConfig
{
    [JsonPropertyName("boss_name")]
    public required string BossName { get; init; }
    
    [JsonPropertyName("boss_adjustment")]
    public required int BossAdjustment { get; init; }
    
    [JsonPropertyName("follower_adjustment")]
    public required int FollowerAdjustment { get; init; }
    
    [JsonPropertyName("followers")]
    public required string[] Followers { get; init; }
}