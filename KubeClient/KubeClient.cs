using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using KubeClient.Configurations;
using KubeClient.Extensions;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace KubeClient
{
    public class KubeClient : IKubeClient
    {
        HttpClient _httpClient;

        public KubeClient()
        {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ClientCertificates.Add(GeneratePfx(
                "LS0tLS1CRUdJTiBDRVJUSUZJQ0FURS0tLS0tCk1JSUM4akNDQWRxZ0F3SUJBZ0lJT1o3c3dIa1R5eEF3RFFZSktvWklodmNOQVFFTEJRQXdGVEVUTUJFR0ExVUUKQXhNS2EzVmlaWEp1WlhSbGN6QWVGdzB4T1RBMU1EVXhOalV6TkRKYUZ3MHlNREExTURReE5qVXpORFJhTURReApGekFWQmdOVkJBb1REbk41YzNSbGJUcHRZWE4wWlhKek1Sa3dGd1lEVlFRREV4QnJkV0psY201bGRHVnpMV0ZrCmJXbHVNSUlCSWpBTkJna3Foa2lHOXcwQkFRRUZBQU9DQVE4QU1JSUJDZ0tDQVFFQXZUc21HeGRLYml3SEh6bUEKejk2UHhlM0IvRHlsRS9nMVRwSXF3bHVxSnpOM2F5VDVIYzYvblY5SlBhRkloakYzR0h1RnJPeDBrSTFRWGwwQwpWNWFrcFV6SldYNHV0ZzhUUHU4dllXelg3NEs5TGZOVkZ0ZGJSaktuN0ludkllKzh1TTM5bnVZYllPcktTVFJhClNiQkJLTDhFRFQ2TEt6eHFNZzZobG5nUEpXcEI4elZkbzNtdlFLKzl3NGE4NGcybHZlcys3SVBjK2JWOVJWMU0KbzJ3MWRubStRTGlBYXViYXpkc0FMSTI0NlZ4b09LamJ0VFFCUW9sd2JZOSswMjJnRjBsUGpZa1BQM0pwV1RRcgorQkJCNHJTUjNZMGJPUGh2c1ZPMjlLMWxuTVUySzBHY1l3TUV6RzlTTjVqWEFtWXRROFpnbGlQNXRxanAxTXVPCmJjUktHd0lEQVFBQm95Y3dKVEFPQmdOVkhROEJBZjhFQkFNQ0JhQXdFd1lEVlIwbEJBd3dDZ1lJS3dZQkJRVUgKQXdJd0RRWUpLb1pJaHZjTkFRRUxCUUFEZ2dFQkFIVXluVzJ3UjZVak9kVDdwOEFZU0plTnhKUFlUOVkrOUZtMgpSUjNoWnZpZFZ0cWwyUWQyMnNNK3hyY2lrVy95eTZ2Q0d5OWk1UXFRaDBVd0dWcTd0cFhwRXp3SzRkb0IzM24zCm05NzdwS1g4a0RKSEFIU3lva1BGRHpsb3JpSDE3WGZROC9KOHpVRThYcktobjVGc3Zja0dJc2w1ZjZTV1RVb0MKRXd5QjFmMXo0Z09Ld3RtVEM0SjZ6UDlsTnNGZ21QM3FYWFZzUmpZUThXZWlvL1RMZkxnTzJpV1BRN2wrV1paSAoxbVQzVEZCUkNqeVBsVVdRZm4vVmZDYVcwK2E4bkc2dFFidUhkSjFKTGk1NHRmMjN2MkFpbHMzbHJTc1FaeTBnCmhQdXp1Z0NiSTRxcTkzQWFOckd4VWhFRXc2SXhwckhyaXNsdXVieThuUkJvaEZhUDBzWT0KLS0tLS1FTkQgQ0VSVElGSUNBVEUtLS0tLQo=",
                "LS0tLS1CRUdJTiBSU0EgUFJJVkFURSBLRVktLS0tLQpNSUlFcEFJQkFBS0NBUUVBdlRzbUd4ZEtiaXdISHptQXo5NlB4ZTNCL0R5bEUvZzFUcElxd2x1cUp6TjNheVQ1CkhjNi9uVjlKUGFGSWhqRjNHSHVGck94MGtJMVFYbDBDVjVha3BVekpXWDR1dGc4VFB1OHZZV3pYNzRLOUxmTlYKRnRkYlJqS243SW52SWUrOHVNMzludVliWU9yS1NUUmFTYkJCS0w4RURUNkxLenhxTWc2aGxuZ1BKV3BCOHpWZApvM212UUsrOXc0YTg0ZzJsdmVzKzdJUGMrYlY5UlYxTW8ydzFkbm0rUUxpQWF1YmF6ZHNBTEkyNDZWeG9PS2piCnRUUUJRb2x3Ylk5KzAyMmdGMGxQallrUFAzSnBXVFFyK0JCQjRyU1IzWTBiT1BodnNWTzI5SzFsbk1VMkswR2MKWXdNRXpHOVNONWpYQW1ZdFE4WmdsaVA1dHFqcDFNdU9iY1JLR3dJREFRQUJBb0lCQVFDeW9hV0ZLV1Zmdnp3eAo0NnlQamYrV3pxeXltZVVUaHRsN2hFdk5FWTc4Vm45Q3E3Sm15d2JqWGxIYlZlY0tscU5nZnlwZlpROWNiYW9TCmlpWnQzSTBzWmVJbEV4S3hWbVhLb1N2UEFscU5oSk5sVHpGaDBJWWZMQzZOQW1DMVhvUit5Q1hFM0YrNEM2eE4Ka3Bnd3U2dW05VHF6N2ZpZUpmZnhyUklvNHNrWmZTYjE2TGMveDZrOW9PUEU1YVBSVmg0eUlXdzNaWHJtKzcrNApUOFhkditzaWNhQmVlM3YzVGlvS0FxZnhzK1cwUitQeHdVMVVvWkNsc1Z3WmF3MXJ3T3ZycmdQZGI0MFYzK0ZoCldpSVRVNnk0c3UxdXg2SG0zalcyd2tSb3R6Qk9iMmdqNm1oZ05Nc0NnL1YzZzFJZENlSW9jdVJobkVZc2tVK3kKZ29QQTVZa0JBb0dCQU9qSkpWb1ZFRS8ydmtoVFdnMm0zQVBYODhkYng4dDhzNnZpZjFSb254M0Rlb1QyRXptNwpLZ0RaYkdRTUFObUpaSXBmN09PSW8wOHF3UjYyN1M1aDdjQXFGWFFzeGVCSTFpRHErdDFKR25jakswdUUzRUU1CmdlR2pnWGgwRU1sZk56UmtqYmFoZHM3ZHpwTGp4SnJ1dEZlbkVOQ0NMd0xXMGFrM3VSYzNiZEZiQW9HQkFOQWEKRlVHTXBTTU5GdTlMVHNEdDhDbk9FVHFJL1ZicDBRMlg3ei9FM0lnbXZjUVVMcU4wNVNoeGRsYjlCYlF5MFBvYQpLS1VNK3dEVlY5ZHpGdGdzaEo1QklSYkcyVVVTSGFGLzRPT3owOUdxVTVBMTVQSWpCQ0VTZjB3NFNCZVIrcGZqCllHcXdxdzd5Ymt3WVpXdldBWGdORjZFZjFJWUZJRWxjcXZVUERnWkJBb0dBSDRtNDdNWnp5bEdrb0FuaTVueVgKekZHQ3JwekpxZEVBU3FaS0YvYUFzRGllcmNyaytlcFltM3JaaTRod3lRZ3JQZkFYR1JDWEI0L0VVRlA2T0ZGKwpNNDNBRUZoTWlzRnplVXc4cHFSVDJKWkVORndRdHltQ1pqNlB1UEVJb1Mrc1BBWHZpVEhBOWUvcEg4K0tBRytjCnNVOXFJTVd5ZHRFQjRNb3k0bWxaRUljQ2dZQWNGRmpuNWwxbDlGOHkzTHQ4cHQ1d0ZhRVhmK0tITnJlQm1RMnMKVWhqYnVqYUMzYUpKWmYyQnpvV0VTVkhhdGJ5aThxS25XckFIdGNITGpYajRwRkVrdFc4TWpycEVhcVJhNVZGbwpDQnk5dnJqVnpaL2QyRUczWUxLU2kwbVcvSmlPcEJ5ZmFNVThHQlF2Nmw3TkJTeGZrWXl0cmdqVUtKN3BwenBlCkpMRGt3UUtCZ1FEZ1pqbmpVcW9VODJwSlRaWnpnSlB3cG1hcithMFI3K0g4VTFuYmtISWJmYjdONzEvZDd6WkUKWDYrSjgrMWdscEFETFI0cEZubGFNM2p1L2d3MW14RVRCbnlnbE10THUxQmRtYlJ0ejVFbEdBRkc3SVZxOU1BMgpLaERYSDVhRDFjUFpDbHEwby9lUks2VGl6TDVJYng3aWIzUHN6L1pVK3VNQWNDQXdKd3Q0U0E9PQotLS0tLUVORCBSU0EgUFJJVkFURSBLRVktLS0tLQo="));
            httpClientHandler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            _httpClient = new HttpClient(httpClientHandler);
            _httpClient.BaseAddress = new Uri("https://192.168.1.181:6443");
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
