namespace KomalliAPI.Clientes.Entities
{
    public interface ITokenRevocationService
    {
        void RevokeToken(string token);
        bool IsTokenRevoked(string token);
    }
}
