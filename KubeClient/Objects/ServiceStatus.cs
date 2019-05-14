using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KubeClient.Objects
{
    public class ServiceStatus
    {
        [JsonProperty("loadBalancer")]
        public ServiceStatusLoadBalancer LoadBalancer { get; set; }
    }
}
