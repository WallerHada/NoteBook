
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer4Templates;
public static class Config
{
    /// <summary>
    /// 定义API范围
    /// </summary>
    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
                new ApiScope("api1", "My API")
        };

    /// <summary>
    /// 定义客户端
    /// </summary>
    public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            // 可以将 ClientId 和 ClientSecret 视为应用程序本身的登录名和密码。
            // 它向身份服务器标识您的应用程序，以便它知道哪个应用程序正在尝试连接到它。
                new Client
                {
                    ClientId = "client",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    // scopes that client has access to
                    AllowedScopes = { "api1" }
                }
        };
}
