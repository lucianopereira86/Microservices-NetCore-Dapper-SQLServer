using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using APITestGateway.Presentation.WebAPI.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Microsoft.Extensions.Primitives;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using System.Collections.Generic;

namespace APITestGateway.Presentation.WebAPI.Controllers
{
    public class BaseController : Controller
    {
        protected Routes rotas;

        public BaseController(IOptions<Routes> r)
        {
            rotas = r.Value;
        }

        internal async Task<IActionResult> GetAPIRoute(dynamic vm = null)
        {
            try
            {
                string url = UriHelper.GetDisplayUrl(Request).Replace("_", "/");
                string rota = GetCtrl(url);
                string ctrl = rota.Contains("/") ? rota.Substring(0, rota.IndexOf("/")) : rota;
                foreach (var api in rotas.APIs)
                {
                    var ctrls = api.Controllers.Split(",");
                    foreach (string c in ctrls)
                    {
                        if (c.ToUpper().Equals(ctrl.ToUpper()))
                        {
                            string novaURL = api.URL + rota;
                            var result = await HttpRequest(novaURL, vm);
                            return Ok(result);
                        }
                    }
                }
                return BadRequest("Invalid route");
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(BusinessException))
                {
                    var bex = (BusinessException)ex;
                    return BadRequest(bex.ErrorList);
                }
                throw ex;
            }
        }

        private static string GetCtrl(string url)
        {
            if (url == null)
                return null;
            string rota = "";
            rota = url.Substring(url.ToUpper().IndexOf("GATEWAY/") + 8);
            return rota;
        }

        private async Task<dynamic> HttpRequest(string url, dynamic obj = null)
        {
            Uri uri = new Uri(url);
            var request = HttpWebRequest.Create(uri) as HttpWebRequest;
            request.ContentType = "application/json";
            request.Method = "POST";
            request.Timeout = 30000;
            request.PreAuthenticate = true;
            getToken(request);

            byte[] byteArray = obj == null ? new byte[0] : Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(obj));
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            return await GetResult(request);
        }

        private void getToken(HttpWebRequest request)
        {

            var bearer = GetBearer;
            if (bearer.Count > 0)
            {
                request.Headers.Add("Authorization", bearer[0]);
                request.Accept = request.ContentType;
            }
        }

        private async Task<dynamic> GetResult(HttpWebRequest request)
        {
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var isOk = response.StatusCode == HttpStatusCode.OK;
                using (var reader = new StreamReader(response.GetResponseStream(), Encoding.ASCII))
                {
                    string retorno = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<dynamic>(retorno);
                }
            }
            catch (WebException wex)
            {
                CheckWebException(wex);
                return null;
            }
        }

        internal StringValues GetBearer
        {
            get
            {
                StringValues bearer;
                HttpContext.Request.Headers.TryGetValue("Authorization", out bearer);
                return bearer;
            }
        }

        private void CheckWebException(WebException wex)
        {
            if (wex.Response != null)
            {
                using (var errorResponse = (HttpWebResponse)wex.Response)
                {
                    using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                    {
                        if (errorResponse.StatusCode == HttpStatusCode.BadRequest)
                        {
                            string retorno = reader.ReadToEnd();
                            var ErrorList = JsonConvert.DeserializeObject<List<dynamic>>(retorno);
                            throw new BusinessException { ErrorList = ErrorList };
                        }
                        else
                            throw wex;
                    }
                }
            }
        }
    }
}