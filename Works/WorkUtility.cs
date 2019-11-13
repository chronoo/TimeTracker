using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace TimeTracker
{
    public class WorkUtility
    {
        public static Connection connection { get; set; }
        public static List<Work> getWorksTitle(List<ComboData> worksID)
        {
            var HtmlResult = "";
            List<int> ids = new List<int>();
            worksID.ForEach(x => ids.Add(x.Id));
            string idList = string.Join(",", ids.ToArray());
            var worksTitle = new List<Work>();

            try {
                var Uri_getTitles = string.Format(@"https://dev.azure.com/{0}/{1}/_apis/wit/workitems?ids={2}&api-version=5.1", connection.organization, connection.project, idList);
                using (WebClient wc = new WebClient()) {
                    wc.Encoding = Encoding.UTF8;
                    wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                    wc.Headers[HttpRequestHeader.Authorization] = $"Basic {Utils.Base64Encode(":" + connection.token)}";
                    HtmlResult = wc.DownloadString(Uri_getTitles);
                }

                var jObject = JObject.Parse(HtmlResult);
                worksTitle = jObject["value"].ToObject<List<Work>>();
            } catch (WebException) {

            }

            return worksTitle;
        }

        public static double getWorkTime(int workId) {
            var HtmlResult = "";
            Work work = default;

            try {
                var Uri_getTitles = string.Format(@"https://dev.azure.com/{0}/{1}/_apis/wit/workitems?ids={2}&api-version=5.1", connection.organization, connection.project, workId);
                using (WebClient wc = new WebClient()) {
                    wc.Encoding = Encoding.UTF8;
                    wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                    wc.Headers[HttpRequestHeader.Authorization] = $"Basic {Utils.Base64Encode(":" + connection.token)}";
                    HtmlResult = wc.DownloadString(Uri_getTitles);
                }

                var jObject = JObject.Parse(HtmlResult);
                work = jObject["value"].ToObject<List<Work>>()[0];
            } catch (WebException) {

            }

            return work.time;
        }

        public static List<ComboData> getWorksId()
        {
            var HtmlResult = "";
            var Uri_getId = string.Format(@"https://dev.azure.com/{0}/{1}/{2}/_apis/wit/wiql?api-version=4.1", connection.organization, connection.project, connection.team);
            var postParameters = JObject.FromObject(new { query = string.Format(
                @"Select [System.Id], [System.Title], [System.State] 
                From WorkItems WHERE [System.TeamProject] = @Project 
                AND ([System.WorkItemType] = 'Task' OR [System.WorkItemType] = 'Bug') 
                AND [System.State] <> 'Closed' AND [System.AssignedTo] = '{0}'", connection.eMail) }).ToString();

            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                wc.Headers[HttpRequestHeader.Authorization] = $"Basic {Utils.Base64Encode(":" + connection.token)}";
                HtmlResult = wc.UploadString(Uri_getId, postParameters);
            }

            var jObject = JObject.Parse(HtmlResult);
            var worksID = jObject["workItems"].ToObject<List<ComboData>>();

            return worksID;
        }
    }
}
