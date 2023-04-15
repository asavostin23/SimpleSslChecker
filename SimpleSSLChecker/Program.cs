using SimpleSSLChecker;
using System.Text;

List<string> uris = new();
using (StreamReader reader = new("..\\..\\...\\..\\..\\input.txt"))
{
    string? line;
    while ((line = await reader.ReadLineAsync()) != null)
        uris.Add(line);
}
FixUris(uris);

StreamWriter writer = new StreamWriter("..\\..\\...\\..\\..\\output.txt");
await writer.WriteLineAsync(DateTime.Now.ToString());

int threads = 4;
Task[] tasks = new Task[threads];
for (int i = 0; i < threads; i++)
{
    tasks[i] = ProcessSites(uris.GetRange(i * uris.Count / threads, uris.Count / threads), writer);
}
await Task.WhenAll(tasks);
writer.Close();
static void FixUris(IList<string> uris)
{
    for (int i = 0; i < uris.Count; i++)
    {
        if (!uris[i].StartsWith("https://"))
            if (uris[i].StartsWith("http://"))
                uris[i].Replace("http://", "https://");
            else
                uris[i] = "https://" + uris[i];
    }
}
static async Task ProcessSites(IEnumerable<string> uris, StreamWriter writer)
{
    DateTime dateTimeBefore = DateTime.Now;
    int counter = 1;
    foreach (string uri in uris)
    {
        CertificateData? certData = await CertificateData.GetCertData(uri);

        StringBuilder result = new StringBuilder();
        DateTime expirationDateTime;

        if (certData != null && DateTime.TryParse(certData.ExpirationDateString, out expirationDateTime))
        {
            if (expirationDateTime.Subtract(DateTime.Now).TotalDays > 0)
                result.Append($"{counter++} - {uri} - VALID - {((int)expirationDateTime.Subtract(DateTime.Now).TotalDays)} days left");
            else
                result.Append($"{counter++} - {uri} - EXPIRED  - {((int)expirationDateTime.Subtract(DateTime.Now).TotalDays)} days left");
        }
        else
            result.Append($"{counter++} - {uri} - FAIL");

        await writer.WriteLineAsync(result.ToString());
    }
    DateTime dateTimeAfter = DateTime.Now;

    await writer.WriteLineAsync($"Time estimated: {(uint)dateTimeAfter.Subtract(dateTimeBefore).TotalSeconds} seconds and {counter - 1} sites were checked");
}
