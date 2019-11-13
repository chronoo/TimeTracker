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
        public static Connection connection { get; set; }
        public static void Pause(int work, double delta) {
            ChangeWorkState(work, State.Pause, delta);
            ChangeWorkCurrentState(work, State.Pause);
        }

        public static void Play(int work) {
            ChangeWorkCurrentState(work, State.Play);
        }

        public static void Stop(int work, double delta) {
            ChangeWorkState(work, State.Stop, delta);
            ChangeWorkCurrentState(work, State.Stop);
        }

        public static void ChangeWorkCurrentState(int work, State state) {
            if (connection.area != "") {
                var currentTime = WorkUtility.GetWorkTime(work);
                var localState = GetLocalState(state);
                ChangeWorkField(work, connection.area, localState);
            }
        }

        public static void ChangeWorkState(int work, State state, double delta) {
            var currentTime = WorkUtility.GetWorkTime(work);
            var value = currentTime + delta;
            ChangeWorkField(work, "/fields/Microsoft.VSTS.Scheduling.CompletedWork", value);
        }

        // TODO: сделать заполнение из списка свойств
        private static void ChangeWorkField(int work, string path, object value) {
            var currentTime = WorkUtility.GetWorkTime(work);

            var Uri_getId = string.Format(@"https://dev.azure.com/{0}/{1}/_apis/wit/workitems/{2}?api-version=5.1", connection.organization, connection.project, work);
            var postParameters = JArray.FromObject(new[]{new {
                op = "add",
                 path,
                 value
            }}).ToString();

            using (WebClient wc = new WebClient()) {
                wc.Headers[HttpRequestHeader.ContentType] = "application/json-patch+json";
                wc.Headers[HttpRequestHeader.Authorization] = $"Basic {Utils.Base64Encode(":" + connection.token)}";
                wc.UploadString(Uri_getId, "PATCH", postParameters);
            }
        }

        private static string GetLocalState(State state) {
            Dictionary<State, string> states = new Dictionary<State, string> {
                { State.Play, "Active" },
                { State.Pause, "Pause" },
                { State.Stop, "Stop" }
            };

            var localState = states[state];

            return localState;
        }

        public static string GetArea() {
            var arrea = "";
            var HtmlResult = "";

            try {
                var Uri_getTitles = string.Format(@"https://dev.azure.com/{0}/{1}/_apis/wit/fields/{2}?api-version=5.1", connection.organization, connection.project, "Текущий статус");
                using (WebClient wc = new WebClient()) {
                    wc.Encoding = Encoding.UTF8;
                    wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                    wc.Headers[HttpRequestHeader.Authorization] = $"Basic {Utils.Base64Encode(":" + connection.token)}";
                    HtmlResult = wc.DownloadString(Uri_getTitles);
                }

                var jObject = JObject.Parse(HtmlResult);
                arrea = "/fields/" + jObject["referenceName"].ToString();
            } catch (WebException) {
            }
            return arrea;
        }
    }
    public enum State {
        Play,
        Pause,
        Stop
    }
}
