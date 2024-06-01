using KomalliAPI.Clientes.Entities;

namespace KomalliAPI.Clientes.Utils
{
    public static class Autorizador
    {
        public static bool TieneToken(string token)
        {
            return token != null; //true si tiene contenido
        }

        public static bool EsTokenValido(ITokenService service, string token)
        {
            return !service.IsTokenRevoked(token); //true si no esta revocado
        }

        public static void RevocarToken(ITokenService service, string token)
        {
            service.RevokeToken(token);
        }
    }
}
