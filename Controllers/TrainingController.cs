using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BrowserVision2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public TrainingController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        [HttpPost]
        public async Task<HttpResponseMessage> Training(string tags)
        {
            var response = new HttpResponseMessage();
            var inputTags = "";
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
                    var CVhelper = new CustomVision.CustomVisionHelper(Configuration["Training-Key"], Configuration["Prediction-Key"]);

                    if (CVhelper.ValidateTags(tags))
                    {
                        var projectID = await CVhelper.SetActiveProjectAsync(Configuration["CustomVisionProjectName"]);
                        string[] intags = tags.Split(Convert.ToChar(";"));
                        var result = await CVhelper.CreateImageAsync(bytes, intags);
                        response.Content = new StringContent(result);
                    }
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
