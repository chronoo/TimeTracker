using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TimeTracker.Projects
{
    public class ProjectUtility
    {
        public static Connection connection { get; set; }
        public static List<Project> GetProjectTitle()
        {
            var HtmlResult = "";
            var Uri_getId = string.Format(@"https://dev.azure.com/{0}/_apis/projects?api-version=5.1", connection.organization);
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                wc.Headers[HttpRequestHeader.Authorization] = $"Basic {Utils.Base64Encode(":" + connection.token)}";
                HtmlResult = wc.DownloadString(Uri_getId);
            }

            var jObject = JObject.Parse(HtmlResult);
            var projectsTitle = jObject["value"].ToObject<List<Project>>();

            return projectsTitle;
        }
    }
}
