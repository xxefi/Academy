using Academy.Server.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace Academy.Server.Services.Classes;

public class BlackListService : IBlackListService
{
    private readonly HashSet<string> _blacklist = [];
    private readonly ITokenService _tokenService;
    private readonly static CancellationTokenSource _cancellationtokenSource = new();

    public BlackListService(ITokenService tokenService)
    {
        StartPeriodicClearing();
        _tokenService = tokenService;
    }

    private async void StartPeriodicClearing()
    {
        while (!_cancellationtokenSource.Token.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMinutes(10), _cancellationtokenSource.Token);
            ClearBlackList();
        }
    }

    private void ClearBlackList()
    {
        lock (_blacklist)
        {
            foreach (var token in _blacklist)
            {
                var principal = _tokenService.GetPrincipalFromToken(token);
                var expiryDateUnix = long.Parse(principal.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Exp).Value);

                if (DateTime.UtcNow > new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expiryDateUnix))
                {
                    _blacklist.Remove(token);
                    Console.WriteLine($"{token} removed");
                }
                Console.WriteLine(token);
            }
        }
    }

    public void AddTokenToBlackList(string token)
    {
        lock (_blacklist)
        {
            _blacklist.Add(token);
        }
    }

    public bool IsTokenBlackListed(string token)
    {
        lock (_blacklist)
        {
            return _blacklist.Contains(token);
        }
    }
}
