using APITestRegister.Presentation.WebAPI.Models.VM;
using APITestRegister.Domain.Domain.Models;

namespace APITestRegister.Presentation.WebAPI.Mappers
{
    public class ViewModelToDomainMappingProfile : AutoMapper.Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<UserLoginVM, User>();
            CreateMap<UserRegisterVM, User>();
        }
    }
}
