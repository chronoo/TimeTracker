using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TimeTracker.Teams
{
    public class TeamUtility
    {
        public static Connection connection { get; set; }
        public static string getProjectTeam()
        {
            var HtmlResult = "";

            var Uri_getTitles = string.Format(@"https://dev.azure.com/{0}/_apis/teams?api-version=5.1-preview.3", connection.organization);
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                wc.Headers[HttpRequestHeader.Authorization] = $"Basic {Utils.Base64Encode(":" + connection.token)}";
                HtmlResult = wc.DownloadString(Uri_getTitles);
            }

            var jObject = JObject.Parse(HtmlResult);
            var worksTitle = jObject["value"].ToObject<List<Team>>();
            var team = worksTitle.Find(x => x.projectName == connection.project).name;

            return team;
        }
    }
}
