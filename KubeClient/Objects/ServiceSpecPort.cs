using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KubeClient.Objects
{
    public class ServiceSpecPort
    {
        [JsonProperty("nodePort")]
        public string NodePort { get; set; }
    }
}
