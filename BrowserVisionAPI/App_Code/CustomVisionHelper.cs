using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// Summary description for CustomVisionHelper
/// </summary>

namespace CustomVision
{

    public class CustomVisionHelper
    {
        
        const string PredictionUrl = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction";
        const string TrainingUrl = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Training";

        private static readonly HttpClient client = new HttpClient();
        private string PredictionKey;
        private string TrainingKey;

        private List<CustomVisionTag> projectTags;
        private CustomVisionProject ActiveProject;

        public CustomVisionHelper(string trainingKey ="", string predictionKey ="")
        {
            PredictionKey = predictionKey;
            TrainingKey = trainingKey;

            //
            // TODO: Add constructor logic here
            //
        }
        public CustomVisionHelper(string trainingKey, string predictionKey, string activeProjectID)
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
        public List<CustomVisionProject> GetProjects()
        {
            List<CustomVisionProject> projects = new List<CustomVisionProject>();

            var client = new RestClient(TrainingUrl + "/projects");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Training-key", TrainingKey);

            IRestResponse response_rest = client.Execute(request);
            JArray a = JArray.Parse(response_rest.Content.ToString());

            foreach (JObject o in a.Children<JObject>())
            {

                projects.Add(new CustomVisionProject() { Id = o["Id"].ToString(), Name = o["Name"].ToString(), CurrentIterationId = o["CurrentIterationId"].ToString() });
              
            }
            
            return projects;
        }
        public async Task<List<CustomVisionProject>> GetProjectsAsync()
        {
            List<CustomVisionProject> projects = new List<CustomVisionProject>();

            var client = new RestClient(TrainingUrl + "/projects");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Training-key", TrainingKey);

            var response_rest = await client.ExecuteGetTaskAsync(request);
            JArray a = JArray.Parse(response_rest.Content.ToString());

            foreach (JObject o in a.Children<JObject>())
            {

                projects.Add(new CustomVisionProject() { Id = o["Id"].ToString(), Name = o["Name"].ToString(), CurrentIterationId = o["CurrentIterationId"].ToString() });

            }
            return projects;
        }

        public List<CustomVisionProject> GetProjects(string trainingKey)
        {
            List<CustomVisionProject> projects = new List<CustomVisionProject>();

            var client = new RestClient(TrainingUrl + "/projects");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Training-key", trainingKey);

            IRestResponse response_rest = client.Execute(request);
            JArray a = JArray.Parse(response_rest.Content.ToString());

            foreach (JObject o in a.Children<JObject>())
            {

                projects.Add(new CustomVisionProject() { Id = o["Id"].ToString(), Name = o["Name"].ToString(), CurrentIterationId = o["CurrentIterationId"].ToString() });

            }

            //var resp_str = resp_task.Result;



            return projects;
        }
        public async Task<List<CustomVisionProject>> GetProjectsAsync(string trainingKey)
        {
            List<CustomVisionProject> projects = new List<CustomVisionProject>();

            var client = new RestClient(TrainingUrl + "/projects");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Training-key", trainingKey);

            var response_rest = await client.ExecutePostTaskAsync(request);
            JArray a = JArray.Parse(response_rest.Content.ToString());

            foreach (JObject o in a.Children<JObject>())
            {

                projects.Add(new CustomVisionProject() { Id = o["Id"].ToString(), Name = o["Name"].ToString(), CurrentIterationId = o["CurrentIterationId"].ToString() });

            }
            return projects;
        }



        public string GetProjectId(string ProjectName) {
            return ActiveProject.Name;
        }
        public void SetActiveProject(string ProjectName) {
            var projects = GetProjects();
            for (int i = 0; i < projects.Count; i++)
            {
                if (projects[i].Name.Equals(ProjectName))
                {
                    ActiveProject = projects[i];
                    projectTags = GetTags(ActiveProject.Id);
                    break;
                }
            }

            //throw new KeyNotFoundException("Project not found");
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




        public List<CustomVisionTag> GetTags(string projectId) {
            List<CustomVisionTag> tags = new List<CustomVisionTag>();
            var client = new RestClient(TrainingUrl + "/projects/" + projectId + "/tags" );
            var request = new RestRequest(Method.GET);
            request.AddHeader("Training-key", TrainingKey);
            IRestResponse response_rest = client.Execute(request);
            JObject a = JObject.Parse(response_rest.Content);
            for (int i = 0; i < a["Tags"].Children().Count() ; i++)
            {
                tags.Add(new CustomVisionTag() { Id = a["Tags"][i]["Id"].ToString(), Name = a["Tags"][i]["Name"].ToString(), Description = a["Tags"][i]["Description"].ToString()});
            }
            
            return tags;
        }
        public async Task<List<CustomVisionTag>> GetTagsAsync(string projectId)
        {
            List<CustomVisionTag> tags = new List<CustomVisionTag>();
            var client = new RestClient(TrainingUrl + "/projects/" + projectId + "/tags");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Training-key", TrainingKey);
            var response_rest = await client.ExecuteGetTaskAsync(request);

            JObject a = JObject.Parse(response_rest.Content);
            for (int i = 0; i < a["Tags"].Children().Count(); i++)
            {
                tags.Add(new CustomVisionTag() { Id = a["Tags"][i]["Id"].ToString(), Name = a["Tags"][i]["Name"].ToString(), Description = a["Tags"][i]["Description"].ToString() });
            }

            return tags;
        }
        public string GetTagId(string tagName)
        {
            //var projects = GetTags(ActiveProject.Id);
            
            for (int i = 0; i < projectTags.Count; i++)
            {
                if (projectTags[i].Name.Equals(tagName))
                {
                    return projectTags[i].Id;
                }
            }
            // Tag not Found create tag
            return CreateTag(tagName);


        }
        private string CreateTag(string tagName, string description = "") {
            var client = new RestClient(TrainingUrl + "/projects/" + ActiveProject.Id + "/tags?name=" + tagName + "&description=" + description);
            var request = new RestRequest(Method.POST);
            //request.AddParameter("name", tagName);
            //request.AddParameter("description", description);
            request.AddHeader("Training-key", TrainingKey);
            IRestResponse response_rest = client.Execute(request);
            JObject a = JObject.Parse(response_rest.Content);

            return a["Id"].ToString();
        }
        private async Task<string> CreateTagAsync(string tagName, string description = "")
        {
            var client = new RestClient(TrainingUrl + "/projects/" + ActiveProject.Id + "/tags?name=" + tagName + "&description=" + description);
            var request = new RestRequest(Method.POST);
            //request.AddParameter("name", tagName);
            //request.AddParameter("description", description);
            request.AddHeader("Training-key", TrainingKey);
            var response_rest = await client.ExecutePostTaskAsync(request);
            JObject a = JObject.Parse(response_rest.Content);

            return a["Id"].ToString();
        }


        public string PredictImage(byte[] bytes)
        {
            var client = new RestClient(PredictionUrl + "/" + ActiveProject.Id + "/image");
            var request = new RestRequest(Method.POST);
            //request.AddHeader("cache-control", "no-cache");
            request.AddHeader("projectId", ActiveProject.Id);
            request.AddHeader("content-type", "application/octet-stream");
            request.AddHeader("Prediction-key", PredictionKey);
            request.AddParameter("application/octet-stream", bytes, ParameterType.RequestBody);

            //request.AddBody();


            IRestResponse response_rest = client.Execute(request);

            return response_rest.Content;

        }
        public async Task<string> PredictImageAsync(byte[] bytes)
        {
            var client = new RestClient(PredictionUrl + "/" + ActiveProject.Id + "/image");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("projectId", ActiveProject.Id);
            request.AddHeader("content-type", "application/octet-stream");
            request.AddHeader("Prediction-key", PredictionKey);
            request.AddParameter("application/octet-stream", bytes, ParameterType.RequestBody);

            var response_rest = await client.ExecutePostTaskAsync(request);

            return response_rest.Content;

        }




        public string CreateImage(byte[] bytes, string[] tags, bool isTagNames =true) {
            //var client = new RestClient("https://requestb.in/1ldf3c41");

            var client = new RestClient(TrainingUrl + "/projects/" + ActiveProject.Id + "/images/image");
            var request = new RestRequest(Method.POST);
            //request.AddHeader("cache-control", "no-cache");
            request.AddHeader("projectId", ActiveProject.Id);
            request.AddHeader("content-type", "application/octet-stream");
            request.AddHeader("Training-key", TrainingKey);
            for (int i = 0; i < tags.Count(); i++)
            {
                if (isTagNames)
                {
                    request.AddParameter("tagIds", GetTagId(tags[i]), ParameterType.QueryString);
                }
                else
                {
                    request.AddParameter("tagIds", tags[i], ParameterType.QueryString);
                }
            }
            request.AddParameter("application/octet-stream", bytes, ParameterType.RequestBody);
            IRestResponse response_rest = client.Execute(request);

            return response_rest.Content;

        }
        public async Task<string> CreateImageAsync(byte[] bytes, string[] tags, bool isTagNames = true)
        {
            var client = new RestClient(TrainingUrl + "/projects/" + ActiveProject.Id + "/images/image");
            var request = new RestRequest(Method.POST);
            request.AddHeader("projectId", ActiveProject.Id);
            request.AddHeader("content-type", "application/octet-stream");
            request.AddHeader("Training-key", TrainingKey);
            for (int i = 0; i < tags.Count(); i++)
            {
                if (isTagNames)
                {
                    request.AddParameter("tagIds", GetTagId(tags[i]), ParameterType.QueryString);
                }
                else
                {
                    request.AddParameter("tagIds", tags[i], ParameterType.QueryString);
                }
            }
            request.AddParameter("application/octet-stream", bytes, ParameterType.RequestBody);
            var response_rest = await client.ExecutePostTaskAsync(request);

            return response_rest.Content;

        }
        public CustomVisionProject GetCustomVisionProject(string trainingKey, string projectName)
        {
            var projects = GetProjects(trainingKey);
            CustomVisionProject returnProject = new CustomVisionProject();
            for (int i = 0; i < projects.Count; i++)
            {
                if (projects[i].Name.Equals(projectName))
                {
                    returnProject = projects[i];
                    returnProject.tags = GetTags(returnProject.Id);
                }
            }

            return returnProject;
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
                    returnProject.tags = GetTags(returnProject.Id);
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