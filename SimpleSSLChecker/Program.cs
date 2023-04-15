using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

CertificateData certData = await GetCertData("https://docs.microsoft.com/");

Console.WriteLine($"URI: {certData.URI}");
Console.WriteLine($"Effective date: {certData.EffectiveDateString}");
Console.WriteLine($"Exp date: {certData.ExpirationDateString}");
Console.WriteLine($"Issuer: {certData.Issuer}");
Console.WriteLine($"Subject: {certData.Subject}");
Console.WriteLine();

async static Task<CertificateData> GetCertData(string uri)
{
    CertificateData certData = null;

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
        Console.WriteLine($"Message: {e.Message} ");
    }
    finally
    {
        handler.Dispose();
        client.Dispose();
    }
    return certData;

    bool ServerCertificateCustomValidation(HttpRequestMessage requestMessage, X509Certificate2? certificate, X509Chain? chain, SslPolicyErrors sslErrors)
    {
        certData = new(uri, certificate);
        return sslErrors == SslPolicyErrors.None;
    }
}
public class CertificateData
{
    public string URI { get; }
    public string EffectiveDateString { get; }
    public string ExpirationDateString { get; }

    public string Issuer { get; }

    public string Subject { get; }
    public CertificateData(string uri, string effectiveDateString, string expirationDateString, string issuer, string subject)
    {
        URI = uri;
        EffectiveDateString = effectiveDateString;
        ExpirationDateString = expirationDateString;
        Issuer = issuer;
        Subject = subject;
    }
    public CertificateData(string uri, X509Certificate2 cert)
    {
        URI = uri;
        EffectiveDateString = cert.GetEffectiveDateString();
        ExpirationDateString = cert.GetExpirationDateString();
        Issuer = cert.Issuer;
        Subject = cert.Subject;
    }
}