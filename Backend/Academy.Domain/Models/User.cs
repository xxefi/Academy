namespace Academy.Domain.Models;

public class User
{
    public const int MAX_USERNAME_LENGTH = 50;
    public const int MAX_PASSWORD_LENGTH = 100;
    public const int MAX_EMAIL_LENGTH = 100;

    public const string USER_USERNAME_EMPTY_KEY = "UserUsernameEmpty";
    public const string USER_USERNAME_LENGTH_KEY = "UserUsernameLength";
    public const string USER_PASSWORD_EMPTY_KEY = "UserPasswordEmpty";
    public const string USER_PASSWORD_LENGTH_KEY = "UserPasswordLength";
    public const string USER_EMAIL_EMPTY_KEY = "UserEmailEmpty";
    public const string USER_EMAIL_LENGTH_KEY = "UserEmailLength";

    public User(
        Guid id, 
        string username, 
        string password, 
        string email, 
        Role? role, 
        Guid roleId, 
        string? refreshToken, 
        DateTime? refreshTokenExpiryTime)
    {
        Id = id;
        Username = username;
        Password = password;
        Email = email;
        Role = role;
        RoleId = roleId;
        RefreshToken = refreshToken;
        RefreshTokenExpiryTime = refreshTokenExpiryTime;
    }

    public Guid Id { get; }
    public string Username { get; }
    public string Password { get; }
    public string Email { get; }
    public Role? Role { get; }
    public Guid RoleId { get; }
    public string? RefreshToken { get; }
    public DateTime? RefreshTokenExpiryTime { get; }

    public static (User? User, string Errors) Create(
        Guid id, 
        string username, 
        string password, 
        string email, 
        Role? role, 
        Guid roleId,
        string? refreshToken, 
        DateTime? refreshTokenExpiryTime)
    {
        var errors = new List<string>
        {
            string.IsNullOrWhiteSpace(username) ? USER_USERNAME_EMPTY_KEY : string.Empty,
            username.Length > MAX_USERNAME_LENGTH ? $"{USER_USERNAME_LENGTH_KEY}:{MAX_USERNAME_LENGTH}" : string.Empty,
            string.IsNullOrWhiteSpace(password) ? USER_PASSWORD_EMPTY_KEY : string.Empty,
            password.Length > MAX_PASSWORD_LENGTH ? $"{USER_PASSWORD_LENGTH_KEY}:{MAX_PASSWORD_LENGTH}" : string.Empty,
            string.IsNullOrWhiteSpace(email) ? USER_EMAIL_EMPTY_KEY : string.Empty,
            email.Length > MAX_EMAIL_LENGTH ? $"{USER_EMAIL_LENGTH_KEY}:{MAX_EMAIL_LENGTH}" : string.Empty
        }.FirstOrDefault(e => !string.IsNullOrEmpty(e));

        return !string.IsNullOrEmpty(errors)
            ? (null, string.Join("\n", errors))
            : (new User(id, username, password, email, role, roleId, refreshToken, refreshTokenExpiryTime), string.Empty);
    }
}