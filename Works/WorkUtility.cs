using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace TimeTracker
{
    public class WorkUtility
    {
        public static List<Work> getWorksTitle(string organization, string project, string token, List<ComboData> worksID)
        {
            var HtmlResult = "";
            List<int> ids = new List<int>();
            worksID.ForEach(x => ids.Add(x.Id));
            string idList = string.Join(",", ids.ToArray());
            var worksTitle = new List<Work>();

            try {
                var Uri_getTitles = string.Format(@"https://dev.azure.com/{0}/{1}/_apis/wit/workitems?ids={2}&api-version=5.1", organization, project, idList);
                using (WebClient wc = new WebClient()) {
                    wc.Encoding = Encoding.UTF8;
                    wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                    wc.Headers[HttpRequestHeader.Authorization] = $"Basic {Utils.Base64Encode(":" + token)}";
                    HtmlResult = wc.DownloadString(Uri_getTitles);
                }

                var jObject = JObject.Parse(HtmlResult);
                worksTitle = jObject["value"].ToObject<List<Work>>();
            } catch (WebException) {

            }

            return worksTitle;
        }

        public static double getWorkTime(int workId, string organization, string project, string token) {
            var HtmlResult = "";
            Work work = default;

            try {
                var Uri_getTitles = string.Format(@"https://dev.azure.com/{0}/{1}/_apis/wit/workitems?ids={2}&api-version=5.1", organization, project, workId);
                using (WebClient wc = new WebClient()) {
                    wc.Encoding = Encoding.UTF8;
                    wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                    wc.Headers[HttpRequestHeader.Authorization] = $"Basic {Utils.Base64Encode(":" + token)}";
                    HtmlResult = wc.DownloadString(Uri_getTitles);
                }

                var jObject = JObject.Parse(HtmlResult);
                work = jObject["value"].ToObject<List<Work>>()[0];
            } catch (WebException) {

            }

            return work.time;
        }

        public static List<ComboData> getWorksId(string organization, string project, string team, string token, string eMail)
        {
            var HtmlResult = "";
            var Uri_getId = string.Format(@"https://dev.azure.com/{0}/{1}/{2}/_apis/wit/wiql?api-version=4.1", organization, project, team);
            var postParameters = JObject.FromObject(new { query = string.Format(
                @"Select [System.Id], [System.Title], [System.State] 
                From WorkItems WHERE [System.TeamProject] = @Project 
                AND ([System.WorkItemType] = 'Task' OR [System.WorkItemType] = 'Bug') 
                AND [System.State] <> 'Closed' AND [System.AssignedTo] = '{0}'", eMail) }).ToString();

            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                wc.Headers[HttpRequestHeader.Authorization] = $"Basic {Utils.Base64Encode(":" + token)}";
                HtmlResult = wc.UploadString(Uri_getId, postParameters);
            }

            var jObject = JObject.Parse(HtmlResult);
            var worksID = jObject["workItems"].ToObject<List<ComboData>>();

            return worksID;
        }
    }
}
