using System.Net;
using System.Text.Json;
using GW2EIMistWarrior.MistWarriorUploadJsons;

[assembly: CLSCompliant(false)]
namespace GW2EIMistWarrior;

/// <summary>
/// https://api.mistwarrior.com/api/v1/logs
/// </summary>
public static class MistWarriorController
{
    public delegate void TraceHandler(string trace);

#if DEBUG
    private const string UploadUrl = "http://localhost:3000/api/v1/logs/upload/ei";
#else
    private const string UploadUrl = "https://api.mistwarrior.com/api/v1/logs/upload/ei"; 
#endif

    private static readonly JsonSerializerOptions DeserializerSettings = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        IncludeFields = true,
    };

    private static readonly HttpClient HTTPClient = new()
    {
        Timeout = Timeout.InfiniteTimeSpan
    };

    /// <summary>
    /// Uploads an EVTC archive to Mist Warrior. Sends <c>Authorization: Bearer {userToken}</c>.
    /// </summary>
    public static MistWarriorUploadObject? Upload(FileInfo fi, TraceHandler traceHandler, string userToken)
    {
        if (string.IsNullOrWhiteSpace(userToken))
        {
            traceHandler("Upload token is missing");
            return null;
        }

        if (!fi.Exists)
        {
            traceHandler("File does not exist");
            return null;
        }

        string fileName = fi.Name;
        byte[] fileContents = File.ReadAllBytes(fi.FullName);

        HttpContent contentCreator()
        {
            var multiPartContent = new MultipartFormDataContent("----MyGreatBoundary");
            var byteArrayContent = new ByteArrayContent(fileContents);
            byteArrayContent.Headers.Add("Content-Type", "application/octet-stream");
            multiPartContent.Add(byteArrayContent, "file", fileName);
            return multiPartContent;
        }

        return SendUploadRequest(contentCreator, traceHandler, userToken.Trim());
    }

    private static MistWarriorUploadObject? SendUploadRequest(Func<HttpContent> contentFactory, TraceHandler traceHandler, string userToken)
    {
        const int tentatives = 2;
        for (int i = 0; i < tentatives; i++)
        {
            traceHandler("Upload tentative");
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(UploadUrl));
            requestMessage.Headers.ExpectContinue = false;
            requestMessage.Headers.TryAddWithoutValidation("X-API-TOKEN", userToken);
            requestMessage.Content = contentFactory();

            try
            {
                using HttpResponseMessage httpResponse = HTTPClient.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead).GetAwaiter().GetResult();
                HttpStatusCode statusCode = httpResponse.StatusCode;
                string stringContents = httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                if (statusCode != HttpStatusCode.OK && statusCode != HttpStatusCode.Created)
                {
                    traceHandler("Upload tentative failed");
                    traceHandler("HTTP " + (int)statusCode + " " + statusCode + ": " + stringContents);
                    continue;
                }

                try
                {
                    MistWarriorUploadObject? item = JsonSerializer.Deserialize<MistWarriorUploadObject>(stringContents, DeserializerSettings);
                    traceHandler("Upload tentative successful");
                    return item;
                }
                catch (JsonException ex)
                {
                    traceHandler("Upload tentative failed");
                    traceHandler("Invalid JSON response: " + ex.Message);
                }
            }
            catch (AggregateException agg)
            {
                traceHandler("Upload tentative failed");
                traceHandler("Main reason: " + agg.Message);
                foreach (Exception e in agg.InnerExceptions)
                {
                    traceHandler("Sub reason: " + e.Message);
                }
            }
            catch (Exception e)
            {
                traceHandler("Upload tentative failed");
                traceHandler("Reason: " + e.Message);
            }
        }

        return null;
    }
}
