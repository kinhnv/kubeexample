using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KubeClient.Configurations
{
    public class ServiceSpec
    {
        [JsonProperty("ports")]
        public ServiceSpecPort[] Ports { get; set; }
    }
}
