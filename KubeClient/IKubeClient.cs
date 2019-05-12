using KubeClient.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KubeClient
{
    public interface IKubeClient
    {
        Task<Service> GetServiceAsync(string serviceName);
    }
}
