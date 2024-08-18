namespace Academy.Server.Data.Models.Dtos;

public class AccessInfoDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
}
