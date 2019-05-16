using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KubeClient.Objects
{
    public class ServiceStatusLoadBalancer
    {
        [JsonProperty("ingress")]
        public ServiceStatusLoadBanlancerIngress[] Ingress { get; set; }
    }
}