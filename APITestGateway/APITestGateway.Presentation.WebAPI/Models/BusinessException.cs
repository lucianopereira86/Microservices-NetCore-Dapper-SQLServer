using System;
using System.Collections.Generic;

namespace APITestGateway.Presentation.WebAPI.Models
{
    public class BusinessException: Exception
    {
        public IList<dynamic> ErrorList { get; set; }

    }
}
