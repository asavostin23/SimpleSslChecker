using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace SimpleSSLChecker
{
    public class CertificateData
    {
        public async static Task<CertificateData?> GetCertData(string uri)
        {
            CertificateData? certData = null;

            HttpClientHandler handler = new();
            handler.ServerCertificateCustomValidationCallback = ServerCertificateCustomValidation;
            HttpClient client = new HttpClient(handler);

            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Exception {uri} -  {e.Message}");
                
                return null;
            }
            finally
            {
                handler.Dispose();
                client.Dispose();
            }
            return certData;

            bool ServerCertificateCustomValidation(HttpRequestMessage requestMessage, X509Certificate2? certificate, X509Chain? chain, SslPolicyErrors sslErrors)
            {
                if (certificate is not null)
                    certData = new CertificateData(uri, certificate);
                return sslErrors == SslPolicyErrors.None;
            }
        }

        public string URI { get; } = "unset";
        public string EffectiveDateString { get; }
        public string ExpirationDateString { get; }

        public string Issuer { get; }

        public string Subject { get; }

        public CertificateData(X509Certificate2 cert)
        {
            EffectiveDateString = cert.GetEffectiveDateString();
            ExpirationDateString = cert.GetExpirationDateString();
            Issuer = cert.Issuer;
            Subject = cert.Subject;
        }
        public CertificateData(string uri, X509Certificate2 cert) : this(cert)
        {
            URI = uri;
        }
    }
}
