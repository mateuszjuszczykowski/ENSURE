using IdentityServer4.Models;

namespace IDENTITY_service;

public class Config
{
    public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            new Client()
            {
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientId = "client",
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedScopes = { "secretApi" },
            }
        };
    
    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new ApiScope("secretApi")
        };
    
    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>{};
    
    public static IEnumerable<ApiResource> ApiResources =>
        new List<ApiResource>{};
}