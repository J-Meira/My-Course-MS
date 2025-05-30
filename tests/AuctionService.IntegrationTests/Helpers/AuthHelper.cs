using System.Security.Claims;

namespace AuctionService.IntegrationTests.Helpers;
public class AuthHelper
{
  public static Dictionary<string, object> GetBearerToken(string username)
  {
    return new Dictionary<string, object>
      {
        { ClaimTypes.Name, username },
      };
  }
}
