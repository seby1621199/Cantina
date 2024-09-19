using DataAccessLayer.Models;

namespace CantinaAPI.Auth
{
    public interface IAuth
    {
        string GenerateJWTToken(User user, IList<string> roles);
    }
}
