using APITestRegister.Domain.Domain.Models;

namespace APITestRegister.Domain.Domain.Interfaces.Repositories
{
    public interface IUserRepository
    {
        bool Exists(string email);
        User Register(User vm);
        User Login(User vm);
    }
}
