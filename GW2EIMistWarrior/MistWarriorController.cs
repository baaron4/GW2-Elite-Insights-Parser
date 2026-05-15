using System.Net;

[assembly: CLSCompliant(false)]
namespace GW2EIMistWarrior;

/// <summary>
/// https://api.mistwarrior.com/api/v1/logs
/// </summary>
public static class MistWarriorController
{
    public delegate void TraceHandler(string trace);

#if DEBUG_LOCAL
    private const string UploadUrl = "http://localhost:3000/api/v1/logs/upload/ei";
#else
    private const string UploadUrl = "https://api.mistwarrior.com/api/v1/logs/upload/ei"; 
#endif

    private static readonly HttpClient HTTPClient = new()
    {
        Timeout = Timeout.InfiniteTimeSpan
    };

    /// <summary>
    /// Uploads an EVTC archive to Mist Warrior. Sends <c>Authorization: Bearer {userToken}</c>.
    /// </summary>
    public static bool Upload(FileInfo fi, TraceHandler traceHandler, string userToken)
    {
        if (string.IsNullOrWhiteSpace(userToken))
        {
            traceHandler("Upload token is missing");
            return false;
        }

        if (!fi.Exists)
        {
            traceHandler("File does not exist");
            return false;
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

    private static bool SendUploadRequest(Func<HttpContent> contentFactory, TraceHandler traceHandler, string userToken)
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

                if (statusCode is HttpStatusCode.OK or HttpStatusCode.Created)
                {
                    return true;
                }
                
                string stringContents = httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                traceHandler("Upload tentative failed");
                traceHandler("HTTP " + (int)statusCode + " " + statusCode + ": " + stringContents);
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

        return false;
    }
}
