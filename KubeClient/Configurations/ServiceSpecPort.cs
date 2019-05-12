using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KubeClient.Configurations
{
    public class ServiceSpecPort
    {
        [JsonProperty("nodePort")]
        public string NodePort { get; set; }
    }
}
