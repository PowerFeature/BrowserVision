using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Configuration;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;

namespace BrowserVisionAPI.Controllers
{
    public class TrainingController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> Training()
        {
            var response = new HttpResponseMessage();
            var inputTags = "";
            try
            {
                var Querystring = Request.GetQueryNameValuePairs();
                for (int i = 0; i < Querystring.Count(); i++)
                {
                    if (Querystring.ElementAt(i).Key.Equals("tags"))
                    {
                        inputTags = Querystring.ElementAt(i).Value;
                    }
                }

                
                var res = await Request.Content.ReadAsStringAsync();
                var req_str = res.Replace("data:image/jpeg;base64,", "");
                Byte[] bytes = System.Convert.FromBase64String(req_str);


                if (bytes.Count() < 1000)
                {
                    throw new HttpRequestException("Image is " + bytes.Count().ToString() + " bytes - must be larger than 1000 bytes");
                }
                var CVhelper = new CustomVision.CustomVisionHelper(ConfigurationManager.AppSettings["Training-Key"], ConfigurationManager.AppSettings["Prediction-Key"]);

                if (CVhelper.ValidateTags(inputTags))
                {
                    var projectID = await CVhelper.SetActiveProjectAsync(ConfigurationManager.AppSettings["CustomVisionProjectName"]);
                    string[] tags = inputTags.Split(Convert.ToChar(";"));
                    var result = await CVhelper.CreateImageAsync(bytes, tags);
                    response.Content = new StringContent(result);
                }
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
