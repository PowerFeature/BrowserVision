using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

/// <summary>
/// Summary description for CustomVisionHelper
/// </summary>

namespace CustomVision
{

    public class CustomVisionHelper
    {
        // https://{endpoint}/customvision/v3.1/Prediction/{projectId}/classify/iterations/{publishedName}/image[?application]

        const string PredictionUrl = "https://southcentralus.api.cognitive.microsoft.com/customvision/v3.1/Prediction/";
        const string TrainingUrl = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.2/Training";

        private static readonly HttpClient client = new HttpClient();
        private string PredictionKey;
        private string TrainingKey;
        private string IterationName;

        private List<CustomVisionTag> projectTags;
        private CustomVisionProject ActiveProject;
        

        public CustomVisionHelper(string trainingKey ="", string predictionKey ="", string iterationName = "")
        {
            PredictionKey = predictionKey;
            TrainingKey = trainingKey;
            IterationName = iterationName;

            //
            // TODO: Add constructor logic here
            //
        }
        public CustomVisionHelper(string trainingKey, string predictionKey, string iterationName, string activeProjectID)
        {
            PredictionKey = predictionKey;
            TrainingKey = trainingKey;
            ActiveProject = new CustomVisionProject
            {
                Id = activeProjectID
            };

            //
            // TODO: Add constructor logic here
            //
        }

      public async Task<List<CustomVisionProject>> GetProjectsAsync()
        {
            List<CustomVisionProject> projects = new List<CustomVisionProject>();

            var client = new HttpClient();
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(TrainingUrl + "/projects"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Training-key", TrainingKey);
            var response_rest = await client.SendAsync(request);

            JArray a = JArray.Parse(await response_rest.Content.ReadAsStringAsync());

            foreach (JObject o in a.Children<JObject>())
            {

                projects.Add(new CustomVisionProject() { Id = o["Id"].ToString(), Name = o["Name"].ToString(), CurrentIterationId = o["CurrentIterationId"].ToString() });

            }
            return projects;
        }


        public async Task<List<CustomVisionProject>> GetProjectsAsync(string trainingKey)
        {
            List<CustomVisionProject> projects = new List<CustomVisionProject>();

            var client = new HttpClient();
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(TrainingUrl + "/projects"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Training-key", TrainingKey);
            var response_rest = await client.SendAsync(request);

            JArray a = JArray.Parse(await response_rest.Content.ReadAsStringAsync());

            foreach (JObject o in a.Children<JObject>())
            {

                projects.Add(new CustomVisionProject() { Id = o["Id"].ToString(), Name = o["Name"].ToString(), CurrentIterationId = o["CurrentIterationId"].ToString() });

            }
            return projects;
        }



        public string GetProjectId(string ProjectName) {
            return ActiveProject.Name;
        }
        public async void SetActiveProject(string ProjectName) {
            var projects = await GetProjectsAsync();
            for (int i = 0; i < projects.Count; i++)
            {
                if (projects[i].Name.Equals(ProjectName))
                {
                    ActiveProject = projects[i];
                    projectTags = await GetTagsAsync(ActiveProject.Id);
                    break;
                }
            }
        }
        public async Task<string> SetActiveProjectAsync(string ProjectName)
        {
            var projects = await GetProjectsAsync();
            for (int i = 0; i < projects.Count; i++)
            {
                if (projects[i].Name.Equals(ProjectName))
                {
                    ActiveProject = projects[i];
                    projectTags = await GetTagsAsync(ActiveProject.Id);
                    break;
                }
            }
            return ActiveProject.Id;
        }





        public async Task<List<CustomVisionTag>> GetTagsAsync(string projectId)
        {
            List<CustomVisionTag> tags = new List<CustomVisionTag>();

            var client = new HttpClient();
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(TrainingUrl + "/projects/" + projectId + "/tags"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Training-key", TrainingKey);
            var response_rest = await client.SendAsync(request);
            var content = await response_rest.Content.ReadAsStringAsync();

            JObject a = JObject.Parse(content);

            for (int i = 0; i < a["Tags"].Children().Count(); i++)
            {
                tags.Add(new CustomVisionTag() { Id = a["Tags"][i]["Id"].ToString(), Name = a["Tags"][i]["Name"].ToString(), Description = a["Tags"][i]["Description"].ToString() });
            }

            return tags;
        }
        public async Task<string> GetTagIdAsync(string tagName)
        {
            //var projects = GetTags(ActiveProject.Id);
            
            for (int i = 0; i < projectTags.Count; i++)
            {
                if (projectTags[i].Name.Equals(tagName))
                {
                    return projectTags[i].Id;
                }
                else if (i == projectTags.Count-1)
                {
                    // Tag not Found create tag

                    return await CreateTagAsync(tagName);
                }

            }
            return "";


        }

        private async Task<string> CreateTagAsync(string tagName, string description = "")
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(TrainingUrl + "/projects/" + ActiveProject.Id + "/tags?name=" + tagName + "&description=" + description),
                Method = HttpMethod.Post,
            };
            request.Headers.Add("Training-key", TrainingKey);

            var response_rest = await client.SendAsync(request);
            var content = await response_rest.Content.ReadAsStringAsync();

            JObject a = JObject.Parse(content);

            return a["Id"].ToString();
        }


        public async Task<string> PredictImageAsync(byte[] bytes)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Prediction-key", PredictionKey);
            

            // Request parameters
            //queryString["application"] = "{string}";
            var uri = "https://southcentralus.api.cognitive.microsoft.com/customvision/v3.1/Prediction/" + ActiveProject.Id + "/classify/iterations/" + IterationName + "/image?" + queryString;

            HttpResponseMessage response_rest;

            

            using (var bytecontent = new ByteArrayContent(bytes))
            {
                bytecontent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response_rest = await client.PostAsync(uri, bytecontent);
            }

            var content = await response_rest.Content.ReadAsStringAsync();


            return content;

        }






        public async Task<string> CreateImageAsync(byte[] bytes, string[] tags, bool isTagNames = true)
        {
            var client = new HttpClient();
            StringBuilder tids = new StringBuilder();
            for (int i = 0; i < tags.Length; i++)
            {
                // Fetch Tagnames if they are not parsed as guids. 
                if (isTagNames)
                {
                    tids.Append(await GetTagIdAsync(tags[i]));
                }
                else
                {
                    tids.Append(tags[i]);
                }
                if (i < tags.Length - 1)
                {
                    tids.Append(",");
                }
            }
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["tagIds"] = tids.ToString();
            HttpResponseMessage response;
            using (var bytecontent = new ByteArrayContent(bytes))
            {
                bytecontent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                bytecontent.Headers.Add("Training-key", TrainingKey);
                bytecontent.Headers.Add("projectId", ActiveProject.Id);
                bytecontent.Headers.Add("Training-key", TrainingKey);
                response = await client.PostAsync (new Uri(TrainingUrl + "/projects/" + ActiveProject.Id + "/images/image?" + queryString), bytecontent);
            }

                //System.Text.Encoding.Default.GetString(bytes));
            var content = await response.Content.ReadAsStringAsync();

            return content;

        }

        public async Task<CustomVisionProject> GetCustomVisionProjectAsync(string trainingKey, string projectName)
        {
            var projects = await GetProjectsAsync(trainingKey);

            CustomVisionProject returnProject = new CustomVisionProject();
            for (int i = 0; i < projects.Count; i++)
            {
                if (projects[i].Name.Equals(projectName))
                {
                    returnProject = projects[i];
                    returnProject.tags = await GetTagsAsync(returnProject.Id);
                }
            }

            return returnProject;
        }

        public bool ValidateTags(string tagsQuesystring)
        {



            if (tagsQuesystring.IndexOf(";") > 0 && (tagsQuesystring.IndexOf(";") + 1) < tagsQuesystring.Length)
            {
                return true;

            }
            else
            {
                return false;
            }

        }
    }
    




    [Serializable]
    public class  CustomVisionProject
    {
        public string Id;
        public string Name;
        public string CurrentIterationId;
        public string iterationName;
        public string DomainId;
        public List<CustomVisionTag> tags;
    }
    [Serializable]
    public class CustomVisionTag {
        public string Id;
        public string Name;
        public string Description;
        public int ImageCount;

    }
    [Serializable]
    public class CustomVisionAccount
    {
        public List<CustomVisionProject> projects;
        public List<CustomVisionTag> tags;
    }
  


}