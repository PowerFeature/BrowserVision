using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using System.Configuration;

namespace BrowserVisionAPI.Controllers
{
    public class PredictionController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> Prediction()
        {
            var response = new HttpResponseMessage();
            //response.Content.Headers.Add("Content-Type", "application/json");
            try
            {
                var res = await Request.Content.ReadAsStringAsync();
                var req_str = res.Replace("data:image/jpeg;base64,", "");
                Byte[] bytes = System.Convert.FromBase64String(req_str);
                if (bytes.Count() < 1000)
                {
                    throw new HttpRequestException("Image is " + bytes.Count().ToString() + " bytes - must be larger than 1000 bytes");
                }
                var CVhelper = new CustomVision.CustomVisionHelper(ConfigurationManager.AppSettings["Training-Key"], ConfigurationManager.AppSettings["Prediction-Key"]);
                var projectID = await CVhelper.SetActiveProjectAsync(ConfigurationManager.AppSettings["CustomVisionProjectName"]);
                var result = await CVhelper.PredictImageAsync(bytes);
                response.Content = new StringContent(result);

                return response;
            }
            catch (Exception ex)
            {
                response.StatusCode = (HttpStatusCode)429;
                response.Content = new StringContent(ex.Message);
                return response;
            }


        }
    }
}
