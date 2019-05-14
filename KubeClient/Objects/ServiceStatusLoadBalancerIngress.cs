using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KubeClient.Objects
{    
    public class ServiceStatusLoadBanlancerIngress
    {
        [JsonProperty("ip")]
        public string Ip { get; set; }
    }
}