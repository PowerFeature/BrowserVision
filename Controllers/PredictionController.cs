using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.IO;
using System.Net;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace BrowserVision2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PredictionController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public PredictionController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        [HttpPost]
        public async Task<HttpResponseMessage> Prediction()
        {
            var response = new HttpResponseMessage();
            //response.Content.Headers.Add("Content-Type", "application/json");
            try
            {
                using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    string res = await reader.ReadToEndAsync();
                    var req_str = res.Replace("data:image/jpeg;base64,", "");
                    Byte[] bytes = System.Convert.FromBase64String(req_str);
                    if (bytes.Count() < 1000)
                    {
                        throw new HttpRequestException("Image is " + bytes.Count().ToString() + " bytes - must be larger than 1000 bytes");
                    }
                    var CVhelper = new CustomVision.CustomVisionHelper(Configuration["Training-Key"], Configuration["Prediction-Key"], Configuration["iterationName"]);
                    var projectID = await CVhelper.SetActiveProjectAsync(Configuration["CustomVisionProjectName"]);
                    var result = await CVhelper.PredictImageAsync(bytes);
                    response.Content = new StringContent(result);
                    return response;

                }


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
