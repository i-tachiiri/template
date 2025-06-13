namespace Api;

public class AzureAdB2COptions
{
    public string Tenant { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string SignUpSignInPolicyId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}
