using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using KubeClient.Objects;
using KubeClient.Extensions;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using KubeClient.Configurations;

namespace KubeClient
{
    public class KubeClient : IKubeClient
    {
        HttpClient _httpClient;

        public KubeClient(KubeConfigs configs)
        {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ClientCertificates.Add(GeneratePfx(configs.ClientCertificateData, configs.ClientCertificateKeyData));
            httpClientHandler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            _httpClient = new HttpClient(httpClientHandler);
            _httpClient.BaseAddress = new Uri("https://192.168.99.100:6443");
        }

        public async Task<Service> GetServiceAsync(string serviceName)
        {
            var response = await _httpClient.GetAsync($"/api/v1/namespaces/default/services/{serviceName}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return json.ToObject<Service>();
            }
            else
                throw new Exception();
        }

        private X509Certificate2 GeneratePfx(string clientCertificateData, string clientCertificateKeyData)
        {
            byte[] keyData = Convert.FromBase64String(clientCertificateKeyData);
            byte[] certData = Convert.FromBase64String(clientCertificateData);

            var cert = new X509CertificateParser().ReadCertificate(new MemoryStream(certData));

            object obj;
            using (var reader = new StreamReader(new MemoryStream(keyData)))
            {
                obj = new PemReader(reader).ReadObject();
                var key = obj as AsymmetricCipherKeyPair;
                if (key != null)
                {
                    var cipherKey = key;
                    obj = cipherKey.Private;
                }
            }

            var keyParams = (AsymmetricKeyParameter)obj;

            var store = new Pkcs12StoreBuilder().Build();
            store.SetKeyEntry("K8SKEY", new AsymmetricKeyEntry(keyParams), new[] { new X509CertificateEntry(cert) });

            using (var pkcs = new MemoryStream())
            {
                store.Save(pkcs, new char[0], new SecureRandom());

                return new X509Certificate2(pkcs.ToArray());
            }
        }
    }
}
