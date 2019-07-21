using APITestRegister.Presentation.WebAPI.Models.VM;
using Swashbuckle.AspNetCore.Examples;

namespace APITestRegister.Presentation.WebAPI.SwaggerDocs.Examples.User
{
    public class UserLoginEx : IExamplesProvider
    {
        public object GetExamples()
        {
            return new UserLoginVM
            {
                email = "user@test.com",
                password = "1234"
            };
        }
    }
}
