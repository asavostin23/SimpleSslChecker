using SimpleSSLChecker;

string[] uris = new string[]
{
    "https://docs.microsoft.com/",
    "byfly.by",
    "https://microsoft.com/",
    "metanit.com",
    "google.com",
    "advansys.com",
    "vk.com",
    "facebook.com",
    "instagram.com",
    "drive.google.com"
};

FixUris(uris);

DateTime dateTimeBefore = DateTime.Now;
int counter = 1;
foreach (string uri in uris)
{
    CertificateData? certData = await CertificateData.GetCertData(uri);
    Console.WriteLine($"Site: {counter++}");
    if (certData != null)
    {
        Console.WriteLine($"URI: {certData.URI}");
        //Console.WriteLine($"Effective date: {certData.EffectiveDateString}");
        Console.WriteLine($"Exp date: {certData.ExpirationDateString}");
        //Console.WriteLine($"Issuer: {certData.Issuer}");
        //Console.WriteLine($"Subject: {certData.Subject}");
        Console.WriteLine();
    }
    else
    {
        Console.WriteLine($"URI: {uri} - FAIL\n");
    }
}
DateTime dateTimeAfter = DateTime.Now;

Console.WriteLine($"\nTime estimated: {(uint)dateTimeAfter.Subtract(dateTimeBefore).TotalSeconds} seconds and {counter - 1} sites were checked");

static void FixUris(string[] uris)
{
    for (int i = 0; i < uris.Length; i++)
    {
        if (!uris[i].StartsWith("https://"))
            if (uris[i].StartsWith("http://"))
                uris[i].Replace("http://", "https://");
            else
                uris[i] = "https://" + uris[i];
    }
}