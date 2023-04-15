using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using static System.Net.WebRequestMethods;

string[] uris = new string[]
{
    "https://docs.microsoft.com/",
    "https://microsoft.com/",
    "google.com",
    "advansys.com"

};

for (int i = 0; i < uris.Length; i++)
{
    if (!uris[i].StartsWith("https://"))
        if (uris[i].StartsWith("http://"))
            uris[i].Replace("http://", "https://");
        else
            uris[i] = "https://" + uris[i];
}

DateTime dateTimeBefore = DateTime.Now;

int counter = 1;
foreach (string uri in uris)
{
    CertificateData? certData = await GetCertData(uri);
    if (certData != null)
    {
        Console.WriteLine($"Site: {counter++}");
        Console.WriteLine($"URI: {certData.URI}");
        Console.WriteLine($"Effective date: {certData.EffectiveDateString}");
        Console.WriteLine($"Exp date: {certData.ExpirationDateString}");
        Console.WriteLine($"Issuer: {certData.Issuer}");
        Console.WriteLine($"Subject: {certData.Subject}");
        Console.WriteLine();
    }
    else
    {
        Console.WriteLine($"Certificate for URI {uri} was null");
    }
}

DateTime dateTimeAfter = DateTime.Now;
Console.WriteLine($"\nTime estimated: {(uint)dateTimeAfter.Subtract(dateTimeBefore).TotalSeconds} seconds and {counter} sites were checked");
async static Task<CertificateData?> GetCertData(string uri)
{
    CertificateData? certData = null;

    HttpClientHandler handler = new();
    handler.ServerCertificateCustomValidationCallback = ServerCertificateCustomValidation;
    HttpClient client = new HttpClient(handler);

    try
    {
        HttpResponseMessage response = await client.GetAsync(uri);
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();
    }
    catch (HttpRequestException e)
    {
        Console.WriteLine("\nException Caught!");
        Console.WriteLine($"Message: {e.Message}");
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
public class CertificateData
{
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