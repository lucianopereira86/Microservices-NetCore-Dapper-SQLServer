using System.Collections.Generic;

namespace APITestGateway.Presentation.WebAPI.Models
{
    public class Routes
    {
        public List<API> APIs { get; set; }
    }

    public class API
    {
        public string URL { get; set; }
        public string Controllers { get; set; }
    }
}
