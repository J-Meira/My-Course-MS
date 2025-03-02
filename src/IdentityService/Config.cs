using Duende.IdentityServer.Models;

namespace IdentityService;

public static class Config
{
  public static IEnumerable<IdentityResource> IdentityResources =>
      [
          new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        ];

  public static IEnumerable<ApiScope> ApiScopes =>
      [
          new ApiScope("auctionApp", "Auction app full access"),
        ];

  public static IEnumerable<Client> Clients =>
      [
          new Client
            {
                ClientId = "postman",
                ClientName = "Postman",
                ClientSecrets = { new Secret("NotASecret".Sha256()) },

                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                RedirectUris = { "https://localhost:44300/signin-oidc" },
                AllowedScopes = { "openid", "profile", "auctionApp" }
            }
      ];
}
