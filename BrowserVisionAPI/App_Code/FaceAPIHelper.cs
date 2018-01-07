using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;


/// <summary>
/// Summary description for CustomVisionHelper
/// </summary>

namespace FaceApi
{

    public class FaceApiHelper
    {

        const string FaceApiUrl = "https://westeurope.api.cognitive.microsoft.com/face/v1.0/detect";

        private static readonly HttpClient client = new HttpClient();
        private string ApiKey;


        public FaceApiHelper(string apiKey = "")
        {
            ApiKey = apiKey;

            //
            // TODO: Add constructor logic here
            //
        }

        
        public string FaceDetect(byte[] bytes, bool returnFaceId = true, bool returnFaceLandmarks = true, string returnFaceAttributes = "age,gender,headPose,smile,facialHair,glasses,emotion,hair,makeup,occlusion,accessories")
        {
            
            var client = new RestClient(FaceApiUrl + "?returnFaceId=" + returnFaceId.ToString() + "&returnFaceLandmarks=" + returnFaceLandmarks.ToString() + "&returnFaceAttributes=" + returnFaceAttributes.ToString());
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/octet-stream");
            request.AddHeader("Ocp-Apim-Subscription-Key", ApiKey);
            request.AddParameter("application/octet-stream", bytes, ParameterType.RequestBody);

            var response_rest = client.Execute(request);
                        
            return response_rest.Content;

        }
            
        public async Task<string> FaceDetectAsync(byte[] bytes, bool returnFaceId = true, bool returnFaceLandmarks = true, string returnFaceAttributes = "age,gender,headPose,smile,facialHair,glasses,emotion,hair,makeup,occlusion,accessories")
        {
            var client = new RestClient(FaceApiUrl + "?returnFaceId=" + returnFaceId.ToString() + "&returnFaceLandmarks=" + returnFaceLandmarks.ToString() + "&returnFaceAttributes=" + returnFaceAttributes.ToString());
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/octet-stream");
            request.AddHeader("Ocp-Apim-Subscription-Key", ApiKey);
            request.AddParameter("application/octet-stream", bytes, ParameterType.RequestBody);

            var response_rest = await client.ExecutePostTaskAsync(request);

            return response_rest.Content;

        }


    }

}

    