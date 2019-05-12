using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using KubeClient;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Kubernetes;

namespace ApiGateway
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IKubeClient, KubeClient.KubeClient>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IKubeClient kubeClient)
        {
            app.Run(async (context) =>
            {
                var service = await kubeClient.GetServiceAsync("service2");
                var port = service.Spec.Ports[0].NodePort;
                var client = new HttpClient();
                client.BaseAddress = new Uri($"https://172.17.117.246:{port}");
                var res = await client.GetAsync("/api/values");
                var content = await res.Content.ReadAsStringAsync();
                if (res.Headers.GetValues("content-type").FirstOrDefault() == "application/json")
                {
                    context.Response.Headers.Add("content-type", "application/json");
                }
                await context.Response.WriteAsync(content);
            });
        }
    }
}
