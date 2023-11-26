using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GW2EIDPSReport.DPSReportJsons;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

[assembly: System.CLSCompliant(false)]
namespace GW2EIDPSReport
{
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

        private static readonly DefaultContractResolver DefaultJsonContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };

        private static readonly HttpClient HTTPClient = new HttpClient();

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

        private static string BaseUploadContentURL { get; } = "https://dps.report/uploadContent?json=1";
        private static string BaseGetUploadsURL { get; } = "https://dps.report/getUploads?page=";
        private static string BaseGetUserTokenURL { get; } = "https://dps.report/getUserToken";
        private static string BaseGetUploadMetadataURL { get; } = "https://dps.report/getUploadMetadata?";
        private static string BaseGetJsonURL { get; } = "https://dps.report/getJson?";

        private static string GetURL(string baseURL, string userToken)
        {
            string url = baseURL;
            if (IsUserTokenValid(userToken))
            {
                url += "&userToken=" + userToken;
            }
            return url;
        }

        private static string GetUploadContentURL(string baseURL, string userToken, bool anonymous = false, bool detailedWvW = false)
        {
            string url = GetURL(baseURL, userToken);
            if (anonymous)
            {
                url += "&anonymous=true";
            }
            if (detailedWvW)
            {
                url += "&detailedwvw=true";
            }
            return url;
        }
        private static string GetGetUploadsURL(GetUploadsParameters parameters, string userToken)
        {
            string url = BaseGetUploadsURL + parameters.Page + "&perPage=" + parameters.PerPage;
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
            return GetURL(url, userToken);
        }
        ///////////////// APIs
        public static DPSReportUploadObject UploadUsingEI(FileInfo fi, TraceHandler traceHandler, string userToken, bool anonymous = false, bool detailedWvW = false)
        {
            string fileName = fi.Name;
            byte[] fileContents = File.ReadAllBytes(fi.FullName);
            Func<HttpContent> contentCreator = () =>
            {
                var multiPartContent = new MultipartFormDataContent("----MyGreatBoundary");
                var byteArrayContent = new ByteArrayContent(fileContents);
                byteArrayContent.Headers.Add("Content-Type", "application/octet-stream");
                multiPartContent.Add(byteArrayContent, "file", fileName);
                return multiPartContent;
            };

            DPSReportUploadObject response = GetDPSReportResponse<DPSReportUploadObject>("UploadUsingEI", GetUploadContentURL(BaseUploadContentURL, userToken, anonymous, detailedWvW) + "&generator=ei", traceHandler, contentCreator);
            if (response != null && response.Error != null)
            {
                traceHandler("DPSReport: UploadUsingEI failed - " + response.Error);
                return null;
            }
            return response;
        }

        public static DPSReportGetUploadsObject GetUploads(TraceHandler traceHandler, string userToken, GetUploadsParameters parameters)
        {
            return GetDPSReportResponse<DPSReportGetUploadsObject>("GetUploads", GetGetUploadsURL(parameters, userToken), traceHandler);
        }
        public static string GenerateUserToken(TraceHandler traceHandler)
        {
            DPSReportUserTokenResponse responseItem = GetDPSReportResponse<DPSReportUserTokenResponse>("GenerateUserToken", BaseGetUserTokenURL, traceHandler);
            if (responseItem != null)
            {
                return responseItem.UserToken;
            }
            return "";
        }
        public static DPSReportUploadObject GetUploadMetaDataWithID(string id, TraceHandler traceHandler)
        {
            if (id == null || id.Length == 0)
            {
                throw new InvalidDataException("Missing ID for GetUploadMetaData end point");
            }
            return GetDPSReportResponse<DPSReportUploadObject>("GetUploadMetaDataWithID", BaseGetUploadMetadataURL + "id=" + id, traceHandler);
        }
        public static DPSReportUploadObject GetUploadMetaDataWithPermalink(string permalink, TraceHandler traceHandler)
        {
            if (permalink == null || permalink.Length == 0)
            {
                throw new InvalidDataException("Missing Permalink for GetUploadMetaData end point");
            }
            return GetDPSReportResponse<DPSReportUploadObject>("GetUploadMetaDataWithPermalink", BaseGetUploadMetadataURL + "permalink=" + permalink, traceHandler);
        }

        public static T GetJsonWithID<T>(string id, TraceHandler traceHandler)
        {
            if (id == null || id.Length == 0)
            {
                throw new InvalidDataException("Missing ID for GetJson end point");
            }
            return GetDPSReportResponse<T>("GetJsonWithID", BaseGetJsonURL + "id=" + id, traceHandler);
        }
        public static T GetJsonWithPermalink<T>(string permalink, TraceHandler traceHandler)
        {
            if (permalink == null || permalink.Length == 0)
            {
                throw new InvalidDataException("Missing Permalink for GetJson end point");
            }
            return GetDPSReportResponse<T>("GetJsonWithPermalink", BaseGetJsonURL + "permalink=" + permalink, traceHandler);
        }
        ///////////////// Response Utilities
        private static T GetDPSReportResponse<T>(string requestName, string URI, TraceHandler traceHandler, Func<HttpContent> content = null)
        {
            const int tentatives = 5;
            for (int i = 0; i < tentatives; i++)
            {
                traceHandler("DPSReport: " + requestName + " tentative");
                var webService = new Uri(@URI);
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, webService);
                requestMessage.Headers.ExpectContinue = false;

                if (content != null)
                {
                    requestMessage.Content = content();
                }
                try
                {
                    Task<HttpResponseMessage> httpRequest = HTTPClient.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
                    HttpResponseMessage httpResponse = httpRequest.Result;
                    HttpStatusCode statusCode = httpResponse.StatusCode;
                    HttpContent responseContent = httpResponse.Content;

                    if (statusCode != HttpStatusCode.OK)
                    {
                        throw new HttpRequestException(statusCode.ToString());
                    }

                    if (responseContent != null)
                    {
                        Task<string> stringContentsTask = responseContent.ReadAsStringAsync();
                        string stringContents = stringContentsTask.Result;
                        T item = JsonConvert.DeserializeObject<T>(stringContents, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            ContractResolver = DefaultJsonContractResolver,
                            StringEscapeHandling = StringEscapeHandling.EscapeHtml
                        });
                        traceHandler("DPSReport: " + requestName + " tentative successful");
                        return item;
                    }
                }
                catch (Exception e)
                {
                    traceHandler("DPSReport: " + requestName + " tentative failed - " + e.Message);
                }
                finally
                {
                    requestMessage.Dispose();
                }
            }
            return default;
        }

    }

}
