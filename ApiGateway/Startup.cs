using System;
using System.Linq;
using System.Net.Http;
using KubeClient;
using KubeClient.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApiGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(new KubeConfigs
            {
                Host = Configuration["Kubernetes:Host"],
                Server = Configuration["Kubernetes:Server"],
                ClientCertificateData = Configuration["Kubernetes:ClientCertificateData"],
                ClientCertificateKeyData = Configuration["Kubernetes:ClientCertificateKeyData"]
            });
            services.AddTransient<IKubeClient, KubeClient.KubeClient>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IKubeClient kubeClient)
        {
            app.UseDeveloperExceptionPage();
            app.Run(async (context) =>
            {
                var host = Configuration["Kubernetes:Host"];
                var service = await kubeClient.GetServiceAsync("friendlyhello");
                var port = service.Spec.Ports[0].NodePort;
                if (service.Status != null && service.Status.LoadBalancer != null && service.Status.LoadBalancer.Ingress.Any())
                {
                    host = service.Status.LoadBalancer.Ingress.First().Ip;
                }
                var client = new HttpClient
                {
                    BaseAddress = new Uri($"http://{host}:{port}")
                };
                var res = await client.GetAsync("/");
                var content = await res.Content.ReadAsStringAsync();
                context.Response.Headers.Add("content-type", "application/json");
                await context.Response.WriteAsync(content);
            });
        }
    }
}
