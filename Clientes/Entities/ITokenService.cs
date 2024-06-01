namespace KomalliAPI.Clientes.Entities
{
    public interface ITokenService
    {
        void RevokeToken(string token);
        bool IsTokenRevoked(string token);
    }
}
