using Newtonsoft.Json;

public class ProfileModel
{
    [JsonProperty("userId")]
    public string UserId { get; set; }

    [JsonProperty("userName")]
    public string UserName { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("fullName")]
    public string FullName { get; set; }

    [JsonProperty("birthday")]
    public DateTime? Birthday { get; set; }

    [JsonProperty("gender")]
    public string Gender { get; set; }

    [JsonProperty("isPremium")]
    public bool IsPremium { get; set; }

    [JsonProperty("premiumExpiredTime")]
    public DateTime? PremiumExpiredTime { get; set; } 

    [JsonProperty("isVerified")]
    public bool IsVerified { get; set; }
}
