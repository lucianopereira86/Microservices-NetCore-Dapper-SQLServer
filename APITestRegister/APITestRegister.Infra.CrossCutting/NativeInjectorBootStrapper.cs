using APITestRegister.Domain.Domain.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using APITestRegister.Infra.Data.MYSQL.Repositories;

namespace APITestRegister.Infra.CrossCutting
{
    public class NativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            #region Repositories 
            services.AddScoped<IUserRepository, UserRepository>();
            #endregion
        }
    }
}
