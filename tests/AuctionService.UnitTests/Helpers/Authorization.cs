using System.Security.Claims;

namespace AuctionService.UnitTests.Helpers;
public class Authorization
{
  public static ClaimsPrincipal GetClaims()
  {
    List<Claim> claims = [
      new Claim(ClaimTypes.Name, "test"),
    ];
    ClaimsIdentity identity = new(claims);
    ClaimsPrincipal principal = new(identity);
    return principal;
  }
}
