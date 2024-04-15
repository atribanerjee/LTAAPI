using LTAAPI.Models;

namespace LTAAPI.Interfaces
{
    public interface IJWTRepository
    {
        String GenerateJWTToken (UsersModel model);
    }
}
