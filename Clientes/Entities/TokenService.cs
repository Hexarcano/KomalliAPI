using NuGet.Common;

namespace KomalliAPI.Clientes.Entities
{
    public class TokenService : ITokenService
    {
        //Simlumación de revocado de tokens.
        //No lo guarda en disco, solo en memoria
        private static List<string> revokedTokens = new List<string>();

        public void RevokeToken(string token)
        {
            revokedTokens.Add(token);
        }

        public bool IsTokenRevoked(string token)
        {
            return revokedTokens.Contains(token);
        }
    }
}
