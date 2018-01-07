using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Configuration;
using System.Threading.Tasks;

namespace BrowserVisionAPI.Controllers
{
    public class FaceController : ApiController
    {

        [HttpPost]
        public async Task<HttpResponseMessage> Face()
        {
            var response = new HttpResponseMessage();

            var res = await Request.Content.ReadAsStringAsync();
            var req_str = res.Replace("data:image/jpeg;base64,", "");
            Byte[] bytes = System.Convert.FromBase64String(req_str);

            var FaceHelper = new FaceApi.FaceApiHelper(ConfigurationManager.AppSettings["FaceApiKey"]);
            var result = FaceHelper.FaceDetect(bytes);


            if (result.Equals("[]")) // Somethimes Face API returns this on heavy load
            {
                response.StatusCode = (HttpStatusCode)429;
                response.Content = new StringContent("Too many requests");
                return response;
            }
            else
            {
                response.Content = new StringContent(result);
                return response;
            }

        }
    }
            
           
    
}
