using APITestRegister.Domain.Domain.Models;
using APITestRegister.Presentation.WebAPI.Models.Return;

namespace APITestRegister.Presentation.WebAPI.Mappers
{
    public class DomainToViewModelMappingProfile : AutoMapper.Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<User, UserReturnVM>();
        }
    }
}
