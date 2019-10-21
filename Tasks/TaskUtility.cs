using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TimeTracker.Tasks {
    public class TaskUtility {
        public static void pause(int work, string organization, string project, string token, double delta) {
            changeWorkState(work, organization, project, token, State.Pause, delta);
        }

        public static void stop(int work, string organization, string project, string token, double delta) {
            changeWorkState(work, organization, project, token, State.Stop, delta);
        }

        private static void changeWorkState(int work, string organization, string project, string token, State state, double delta) {
            var currentTime = WorkUtility.getWorkTime(work, organization, project, token);
            var value = currentTime + delta;

            var Uri_getId = string.Format(@"https://dev.azure.com/{0}/{1}/_apis/wit/workitems/{2}?api-version=5.1", organization, project, work);
            var postParameters = JArray.FromObject(new[]{new {
                op = "add",
                path = "/fields/Microsoft.VSTS.Scheduling.CompletedWork",
                value = value
            }}).ToString();

            using (WebClient wc = new WebClient()) {
                wc.Headers[HttpRequestHeader.ContentType] = "application/json-patch+json";
                wc.Headers[HttpRequestHeader.Authorization] = $"Basic {Utils.Base64Encode(":" + token)}";
                wc.UploadString(Uri_getId, "PATCH", postParameters);
            }
        }
    }
    enum State {
        Pause,
        Stop
    }
}
