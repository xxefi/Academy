namespace Academy.Domain.DTOS.Read.Auth;

public class AccessInfoDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiryTime { get; set; }
}