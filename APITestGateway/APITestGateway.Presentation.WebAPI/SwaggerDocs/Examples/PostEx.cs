using APITestGateway.Presentation.WebAPI.Models.VM;
using Swashbuckle.AspNetCore.Examples;

namespace APITestGateway.Presentation.WebAPI.SwaggerDocs.Examples.Visitante
{
    public class PostEx : IExamplesProvider
    {
        public object GetExamples()
        {
            return new PostVM
            {
                route = "QRCode/NumeroSorteio",
                obj = new
                {
                    id = 0,
                    nome = "MARIA"
                }
            };
        }
    }
}
