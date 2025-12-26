using System.Net;
using GW2EIDPSReport.DPSReportJsons;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;

[assembly: CLSCompliant(false)]
namespace GW2EIDPSReport;

/// <summary>
/// https://dps.report/api
/// </summary>
public static class DPSReportController
{

    public delegate void TraceHandler(string trace);

    private static bool IsUserTokenValid(string userToken)
    {
        return userToken != null && userToken.Length > 0;
    }

    private static readonly JsonSerializerOptions DeserializerSettings = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        IncludeFields = true,
    };

    private static readonly HttpClient HTTPClient = new HttpClient()
    {
        Timeout = Timeout.InfiniteTimeSpan
    };

    private class DPSReportUserTokenResponse
    {
        public string UserToken { get; set; }
    }
    public class GetUploadsParameters
    {
        public int Page { get; set; } = 1;
        public int PerPage { get; set; } = 25;
        public uint Since { get; set; } = 0;
        public uint SinceEncounter { get; set; } = 0;
        public uint UntilEncounter { get; set; } = 0;
        public uint Unique { get; set; } = 0;
        public GetUploadsParameters()
        {

        }
    }
    ///////////////// URL Utilities

    private const string MainEntryPoint = "https://dps.report";
    private const string SecondaryEntryPoint = "http://a.dps.report";
    private const string TertiaryEntryPoint = "https://b.dps.report";

    private const string UploadContentURL = "/uploadContent";
    private const string GetUploadsURL = "/getUploads";
    private const string GetUserTokenURL = "/getUserToken";
    private const string GetUploadMetadataURL = "/getUploadMetadata";
    private const string GetJsonURL = "/getJson";

    // https://stackoverflow.com/questions/273313/randomize-a-listt
    private static readonly Random rng = new();

    private static List<T> Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
        return list;
    }
    //

    private static string GetURL(string baseURL, string userToken)
    {
        string url = baseURL;
        if (IsUserTokenValid(userToken))
        {
            url += "&userToken=" + userToken;
        }
        return url;
    }

    private static List<string> GetUploadContentURLs(string userToken, bool anonymous = false, bool detailedWvW = false)
    {
        string url = UploadContentURL + "?json=1";
        if (anonymous)
        {
            url += "&anonymous=true";
        }
        if (detailedWvW)
        {
            url += "&detailedwvw=true";
        }
        url += "&generator=ei";
        List<string> urls =
        [
            GetURL(MainEntryPoint + url, userToken),
            GetURL(SecondaryEntryPoint + url, userToken),
            GetURL(TertiaryEntryPoint + url, userToken)
        ];
        return Shuffle(urls);
    }
    private static List<string> GetGetUploadsURLs(GetUploadsParameters parameters, string userToken)
    {
        string url = GetUploadsURL + "?page=" + parameters.Page + "&perPage=" + parameters.PerPage;
        if (parameters.Since > 0)
        {
            url += "&since=" + parameters.Since;
        }
        if (parameters.SinceEncounter > 0)
        {
            url += "&sinceEncounter=" + parameters.SinceEncounter;
        }
        if (parameters.UntilEncounter > 0)
        {
            url += "&untilEncounter=" + parameters.UntilEncounter;
        }
        if (parameters.Unique > 0)
        {
            url += "&unique=1";
        }
        List<string> urls =
        [
            GetURL(MainEntryPoint + url, userToken),
            GetURL(SecondaryEntryPoint + url, userToken),
            GetURL(TertiaryEntryPoint + url, userToken)
        ];
        return Shuffle(urls);
    }
    private static List<string> GetUserTokenURLs()
    {
        List<string> urls =
        [
            MainEntryPoint + GetUserTokenURL,
            SecondaryEntryPoint + GetUserTokenURL,
            TertiaryEntryPoint + GetUserTokenURL,
        ];
        return Shuffle(urls);
    }
    private static List<string> GetUploadMetadataURLs(string? id, string? permalink)
    {
        string url = GetUploadMetadataURL;
        if (id != null)
        {
            url += "?id=" + id;
        }
        if (permalink != null)
        {
            url += "?permalink=" + permalink;
        }
        List<string> urls =
        [
            MainEntryPoint + url,
            SecondaryEntryPoint + url,
            TertiaryEntryPoint + url
        ];
        return Shuffle(urls);
    }
    private static List<string> GetJsonURLs(string? id, string? permalink)
    {
        string url = GetJsonURL;
        if (id != null)
        {
            url += "?id=" + id;
        }
        if (permalink != null)
        {
            url += "?permalink=" + permalink;
        }
        List<string> urls =
        [
            MainEntryPoint + url,
            SecondaryEntryPoint + url,
            TertiaryEntryPoint + url
        ];
        return Shuffle(urls);
    }
    ///////////////// APIs
    public static DPSReportUploadObject? UploadUsingEI(FileInfo fi, TraceHandler traceHandler, string userToken, bool anonymous = false, bool detailedWvW = false)
    {
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

        DPSReportUploadObject? response = GetDPSReportResponse<DPSReportUploadObject>("UploadUsingEI", GetUploadContentURLs(userToken, anonymous, detailedWvW), traceHandler, contentCreator);
        if (response != null && response.Error != null)
        {
            traceHandler("UploadUsingEI generated an error - " + response.Error);
        }
        return response;
    }

    public static DPSReportGetUploadsObject? GetUploads(TraceHandler traceHandler, string userToken, GetUploadsParameters parameters)
    {
        return GetDPSReportResponse<DPSReportGetUploadsObject>("GetUploads", GetGetUploadsURLs(parameters, userToken), traceHandler);
    }
    public static string GenerateUserToken(TraceHandler traceHandler)
    {
        DPSReportUserTokenResponse? responseItem = GetDPSReportResponse<DPSReportUserTokenResponse>("GenerateUserToken", GetUserTokenURLs(), traceHandler);
        return responseItem != null ? responseItem.UserToken : "";
    }
    public static DPSReportUploadObject? GetUploadMetaDataWithID(string id, TraceHandler traceHandler)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new InvalidDataException("Missing ID for GetUploadMetaData end point");
        }
        return GetDPSReportResponse<DPSReportUploadObject>("GetUploadMetaDataWithID", GetUploadMetadataURLs(id, null), traceHandler);
    }
    public static DPSReportUploadObject? GetUploadMetaDataWithPermalink(string permalink, TraceHandler traceHandler)
    {
        if (string.IsNullOrEmpty(permalink))
        {
            throw new InvalidDataException("Missing Permalink for GetUploadMetaData end point");
        }
        return GetDPSReportResponse<DPSReportUploadObject>("GetUploadMetaDataWithPermalink", GetUploadMetadataURLs(null, permalink), traceHandler);
    }

    public static T? GetJsonWithID<T>(string id, TraceHandler traceHandler)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new InvalidDataException("Missing ID for GetJson end point");
        }
        return GetDPSReportResponse<T>("GetJsonWithID", GetJsonURLs(id, null), traceHandler);
    }
    public static T? GetJsonWithPermalink<T>(string permalink, TraceHandler traceHandler)
    {
        if (string.IsNullOrEmpty(permalink))
        {
            throw new InvalidDataException("Missing Permalink for GetJson end point");
        }
        return GetDPSReportResponse<T>("GetJsonWithPermalink", GetJsonURLs(null, permalink), traceHandler);
    }
    ///////////////// Response Utilities
    private static T? GetDPSReportResponse<T>(string requestName, List<string> URIs, TraceHandler traceHandler, Func<HttpContent>? content = null)
    {
        const int tentatives = 2;
        for (int i = 0; i < tentatives; i++)
        {
            foreach (string URI in URIs)
            {
                traceHandler(requestName + " tentative");
                var webService = new Uri(URI);
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, webService);
                requestMessage.Headers.ExpectContinue = false;

                if (content != null)
                {
                    requestMessage.Content = content();
                }
                try
                {
                    Task<HttpResponseMessage> httpRequest = HTTPClient.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead);
                    HttpResponseMessage httpResponse = httpRequest.Result;
                    HttpStatusCode statusCode = httpResponse.StatusCode;
                    HttpContent responseContent = httpResponse.Content;

                    if (statusCode != HttpStatusCode.OK)
                    {
                        throw new HttpRequestException(statusCode.ToString());
                    }

                    if (responseContent != null)
                    {
                        var stringContents = responseContent.ReadAsStringAsync().Result;
                        T item = JsonSerializer.Deserialize<T>(stringContents, DeserializerSettings)!;
                        traceHandler(requestName + " tentative successful");
                        return item;
                    }
                }
                catch (AggregateException agg)
                {
                    traceHandler(requestName + " tentative failed");
                    traceHandler("Main reason: " + agg.Message);             
                    foreach (Exception e in agg.InnerExceptions)
                    {
                        traceHandler("Sub reason: " + e.Message);
                    }
                }
                catch (Exception e)
                {
                    traceHandler(requestName + " tentative failed");
                    traceHandler("Reason: " + e.Message);
                }
                finally
                {
                    requestMessage.Dispose();
                }
            }
        }
        return default;
    }

}
