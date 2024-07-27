using System;
using System.Collections.Generic;
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

        private static readonly string MainEntryPoint = "https://dps.report";
        private static readonly string SecondaryEntryPoint = "https://a.dps.report";
        private static readonly string TertiaryEntryPoint = "https://b.dps.report";

        private static readonly string UploadContentURL = "/uploadContent";
        private static readonly string GetUploadsURL = "/getUploads";
        private static readonly string GetUserTokenURL = "/getUserToken";
        private static readonly string GetUploadMetadataURL = "/getUploadMetadata";
        private static readonly string GetJsonURL = "/getJson";

        // https://stackoverflow.com/questions/273313/randomize-a-listt
        private static readonly Random rng = new Random();

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
            var urls = new List<string>()
            {
                GetURL(MainEntryPoint + url, userToken),
                GetURL(SecondaryEntryPoint + url, userToken),
                GetURL(TertiaryEntryPoint + url, userToken)
            };
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
            var urls = new List<string>()
            {
                GetURL(MainEntryPoint + url, userToken),
                GetURL(SecondaryEntryPoint + url, userToken),
                GetURL(TertiaryEntryPoint + url, userToken)
            };
            return Shuffle(urls);
        }
        private static List<string> GetUserTokenURLs()
        {
            string url = GetUserTokenURL;
            var urls = new List<string>()
            {
                MainEntryPoint + url,
                SecondaryEntryPoint + url,
                TertiaryEntryPoint + url
            };
            return Shuffle(urls);
        }
        private static List<string> GetUploadMetadataURLs(string id, string permalink)
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
            var urls = new List<string>()
            {
                MainEntryPoint + url,
                SecondaryEntryPoint + url,
                TertiaryEntryPoint + url
            };
            return Shuffle(urls);
        }
        private static List<string> GetJsonURLs(string id, string permalink)
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
            var urls = new List<string>()
            {
                MainEntryPoint + url,
                SecondaryEntryPoint + url,
                TertiaryEntryPoint + url
            };
            return Shuffle(urls);
        }
        ///////////////// APIs
        public static DPSReportUploadObject UploadUsingEI(FileInfo fi, TraceHandler traceHandler, string userToken, bool anonymous = false, bool detailedWvW = false)
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

            DPSReportUploadObject response = GetDPSReportResponse<DPSReportUploadObject>("UploadUsingEI", GetUploadContentURLs(userToken, anonymous, detailedWvW), traceHandler, contentCreator);
            if (response != null && response.Error != null)
            {
                traceHandler("UploadUsingEI generated an error - " + response.Error);
            }
            return response;
        }

        public static DPSReportGetUploadsObject GetUploads(TraceHandler traceHandler, string userToken, GetUploadsParameters parameters)
        {
            return GetDPSReportResponse<DPSReportGetUploadsObject>("GetUploads", GetGetUploadsURLs(parameters, userToken), traceHandler);
        }
        public static string GenerateUserToken(TraceHandler traceHandler)
        {
            DPSReportUserTokenResponse responseItem = GetDPSReportResponse<DPSReportUserTokenResponse>("GenerateUserToken", GetUserTokenURLs(), traceHandler);
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
            return GetDPSReportResponse<DPSReportUploadObject>("GetUploadMetaDataWithID", GetUploadMetadataURLs(id, null), traceHandler);
        }
        public static DPSReportUploadObject GetUploadMetaDataWithPermalink(string permalink, TraceHandler traceHandler)
        {
            if (permalink == null || permalink.Length == 0)
            {
                throw new InvalidDataException("Missing Permalink for GetUploadMetaData end point");
            }
            return GetDPSReportResponse<DPSReportUploadObject>("GetUploadMetaDataWithPermalink", GetUploadMetadataURLs(null, permalink), traceHandler);
        }

        public static T GetJsonWithID<T>(string id, TraceHandler traceHandler)
        {
            if (id == null || id.Length == 0)
            {
                throw new InvalidDataException("Missing ID for GetJson end point");
            }
            return GetDPSReportResponse<T>("GetJsonWithID", GetJsonURLs(id, null), traceHandler);
        }
        public static T GetJsonWithPermalink<T>(string permalink, TraceHandler traceHandler)
        {
            if (permalink == null || permalink.Length == 0)
            {
                throw new InvalidDataException("Missing Permalink for GetJson end point");
            }
            return GetDPSReportResponse<T>("GetJsonWithPermalink", GetJsonURLs(null, permalink), traceHandler);
        }
        ///////////////// Response Utilities
        private static T GetDPSReportResponse<T>(string requestName, List<string> URIs, TraceHandler traceHandler, Func<HttpContent> content = null)
        {
            const int tentatives = 5;
            for (int i = 0; i < tentatives; i++)
            {
                foreach (string URI in URIs)
                {
                    traceHandler(requestName + " tentative " + URI);
                    var webService = new Uri(URI);
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
                            traceHandler(requestName + " tentative successful " + URI);
                            return item;
                        }
                    }
                    catch (AggregateException agg)
                    {
                        traceHandler(requestName + " tentative failed " + URI);
                        traceHandler("Main reasong: " + agg.Message);
                        foreach (Exception e in agg.InnerExceptions)
                        {
                            traceHandler("Sub reason: " + e.Message);
                        }
                    }
                    catch (Exception e)
                    {
                        traceHandler(requestName + " tentative failed " + URI);
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

}
