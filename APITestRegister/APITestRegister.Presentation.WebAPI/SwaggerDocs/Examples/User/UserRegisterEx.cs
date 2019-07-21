using APITestRegister.Presentation.WebAPI.Models.VM;
using Swashbuckle.AspNetCore.Examples;

namespace APITestRegister.Presentation.WebAPI.SwaggerDocs.Examples.User
{
    public class UserRegisterEx : IExamplesProvider
    {
        public object GetExamples()
        {
            return new UserRegisterVM
            {
                name = "User Test",
                email = "user@test.com",
                password = "1234"
            };
        }
    }
}
