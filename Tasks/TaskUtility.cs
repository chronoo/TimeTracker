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
            changeWorkCurrentState(work, organization, project, token, State.Pause);
        }

        public static void play(int work, string organization, string project, string token) {
            changeWorkCurrentState(work, organization, project, token, State.Play);
        }

        public static void stop(int work, string organization, string project, string token, double delta) {
            changeWorkState(work, organization, project, token, State.Stop, delta);
            changeWorkCurrentState(work, organization, project, token, State.Stop);
        }

        public static void changeWorkCurrentState(int work, string organization, string project, string token, State state) {
            var currentTime = WorkUtility.getWorkTime(work, organization, project, token);
            var localState = getLocalState(state);
            changeWorkField(work, organization, project, token, "/fields/Custom.9c5f55a3-cb5b-4dd3-b28e-57d194609601", localState);
        }

        public static void changeWorkState(int work, string organization, string project, string token, State state, double delta) {
            var currentTime = WorkUtility.getWorkTime(work, organization, project, token);
            var value = currentTime + delta;
            changeWorkField(work, organization, project, token, "/fields/Microsoft.VSTS.Scheduling.CompletedWork", value);
        }

        // TODO: сделать заполнение из списка свойств
        private static void changeWorkField(int work, string organization, string project, string token, string path, object value) {
            var currentTime = WorkUtility.getWorkTime(work, organization, project, token);

            var Uri_getId = string.Format(@"https://dev.azure.com/{0}/{1}/_apis/wit/workitems/{2}?api-version=5.1", organization, project, work);
            var postParameters = JArray.FromObject(new[]{new {
                op = "add",
                 path,
                 value
            }}).ToString();

            using (WebClient wc = new WebClient()) {
                wc.Headers[HttpRequestHeader.ContentType] = "application/json-patch+json";
                wc.Headers[HttpRequestHeader.Authorization] = $"Basic {Utils.Base64Encode(":" + token)}";
                wc.UploadString(Uri_getId, "PATCH", postParameters);
            }
        }

        private static string getLocalState(State state) {
            Dictionary<State, string> states = new Dictionary<State, string> {
                { State.Play, "Active" },
                { State.Pause, "Pause" },
                { State.Stop, "Stop" }
            };

            var localState = states[state];

            return localState;
        }
    }
    public enum State {
        Play,
        Pause,
        Stop
    }
}
