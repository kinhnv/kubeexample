using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KubeClient.Configurations
{
    public class Service
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("spec")]
        public ServiceSpec Spec { get; set; }
    }
}
