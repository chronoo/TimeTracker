using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TimeTracker
{
    public class Work
    {
        public Work(object fields, int id)
        {
            this.id = id;
            this.fields = JObject.Parse(fields.ToString()).ToObject<Fields>();
            title = this.fields.title;
            time = this.fields.time;
        }
        public int id { get; set; }
        public Fields fields;
        public string title { get; set; }
        public double time { get; set; }
    }

    public class Fields
    {
        [JsonProperty(PropertyName = "System.Title")]
        public string title;

        [JsonProperty(PropertyName = "Microsoft.VSTS.Scheduling.CompletedWork")]
        public double time;
    }
}
