using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GW2EIWingman.WingmanUploadJsons;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

[assembly: System.CLSCompliant(false)]
namespace GW2EIWingman
{
    /// <summary>
    /// https://gw2wingman.nevermindcreations.de/api
    /// </summary>
    public static class WingmanController
    {

        private static readonly DefaultContractResolver DefaultJsonContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };
        private static readonly UTF8Encoding NoBOMEncodingUTF8 = new UTF8Encoding(false);

        ///////////////// URL Utilities
        private const string BaseURL = "https://gw2wingman.nevermindcreations.de/";
        private static string TestConnectionURL { get; } = BaseURL + "testConnection";

        private const string BaseAPIURL = BaseURL + "api/";
        private static string EIVersionURL { get; } = BaseAPIURL + "EIversion";
        private static string ImportLogQueuedURL { get; } = BaseAPIURL + "importLogQueued";
        private static string CheckLogQueuedURL { get; } = BaseAPIURL + "checkLogQueued";
        private static string CheckLogQueuedOrDBURL { get; } = BaseAPIURL + "checkLogQueuedOrDB";
        private static string CheckUploadURL { get; } = BaseURL + "checkUpload";
        private static string UploadProcessedURL { get; } = BaseURL + "uploadProcessed";

        private static bool IsDPSReportLinkValid(string dpsReportLink, List<string> traces)
        {
            if (!dpsReportLink.Contains("https") && !dpsReportLink.Contains(".report"))
            {
                traces.Add("Wingman: Invalid dps.report link");
                return false;
            }
            return true;
        }

        private static string GetImportLogQueuedURL(string dpsReportLink)
        {
            return ImportLogQueuedURL + "?link=" + dpsReportLink;
        }

        private static string GetCheckLogQueuedURL(string dpsReportLink)
        {
            return CheckLogQueuedURL + "?link=" + dpsReportLink;
        }

        private static string GetCheckLogQueuedOrDBURL(string dpsReportLink)
        {
            return CheckLogQueuedOrDBURL + "?link=" + dpsReportLink;
        }

        // Connection checking
        private static bool CheckConnection(List<string> traces)
        {
            return _GetWingmanStringResponse("CheckConnection", TestConnectionURL, traces, HttpMethod.Get) == "True";
        }

        private static bool IsEIVersionValid(Version parserVersion, List<string> traces)
        {
            string returnedVersion = _GetWingmanStringResponse("EIVersionURL", EIVersionURL, traces, HttpMethod.Get);
            if (returnedVersion == null)
            {
                return false;
            }
            returnedVersion = returnedVersion.Replace("v", "");
            var expectedVersion = new Version(returnedVersion);
            return parserVersion.CompareTo(expectedVersion) >= 0;
        }

        public static bool CanBeUsed(Version parserVersion, List<string> traces)
        {
            return CheckConnection(traces) && IsEIVersionValid(parserVersion, traces);
        }
        //

        public static WingmanCheckLogQueuedOrDBObject GetCheckLogQueuedOrDB(string dpsReportLink, List<string> traces, Version parserVersion)
        {
            if (!IsDPSReportLinkValid(dpsReportLink, traces))
            {
                return null;
            }
            return GetWingmanResponse<WingmanCheckLogQueuedOrDBObject>("CheckLogQueuedOrDB", GetCheckLogQueuedOrDBURL(dpsReportLink), traces, parserVersion, HttpMethod.Post);
        }

        public static WingmanCheckLogQueuedObject GetCheckLogQueued(string dpsReportLink, List<string> traces, Version parserVersion)
        {
            if (!IsDPSReportLinkValid(dpsReportLink, traces))
            {
                return null;
            }
            return GetWingmanResponse<WingmanCheckLogQueuedObject>("CheckLogQueued", GetCheckLogQueuedURL(dpsReportLink), traces, parserVersion, HttpMethod.Post);
        }

        public static WingmanImportLogQueuedObject ImportLogQueued(string dpsReportLink, List<string> traces, Version parserVersion)
        {
            if (!IsDPSReportLinkValid(dpsReportLink, traces))
            {
                return null;
            }
            return GetWingmanResponse<WingmanImportLogQueuedObject>("ImportLogQueued", GetImportLogQueuedURL(dpsReportLink), traces, parserVersion, HttpMethod.Post);
        }

        public static bool UploadToWingmanUsingImportLogQueued(string dpsReportLink, List<string> traces, Version parserVersion)
        {
            // Check if the URL is already present on Wingman
            WingmanCheckLogQueuedOrDBObject wingmanCheck = GetCheckLogQueuedOrDB(dpsReportLink, traces, parserVersion);
            if (wingmanCheck != null)
            {
                if (wingmanCheck.InDB || wingmanCheck.InQueue)
                {
                    traces.Add("Wingman: Upload failed - Log already present in Wingman DB");
                    return false;
                }
                else
                {
                    WingmanImportLogQueuedObject wingmanUpload = ImportLogQueued(dpsReportLink, traces, parserVersion);
                    if (wingmanUpload != null)
                    {
                        if (wingmanUpload.Success != 1)
                        {
                            traces.Add("Wingman: Upload failed - " + wingmanUpload.Note);
                            return true;
                        }
                        else
                        {
                            traces.Add("Wingman: Upload successful - " + wingmanUpload.Note);
                            return true;
                        }
                    }
                    return false;
                }
            }
            return false;
        }

        public static bool CheckUploadPossible(FileInfo fi, string account, List<string> traces, Version parserVersion)
        {
            string fileName = fi.Name;
            string length = fi.Length.ToString();
            string creationTime = new DateTimeOffset(fi.CreationTime).ToUnixTimeSeconds().ToString();
            using (var multiPartContent = new MultipartFormDataContent("----MyGreatBoundary"))
            {
                using (var fileContent = new StringContent(fileName, NoBOMEncodingUTF8, "text/plain"))
                {
                    multiPartContent.Add(fileContent, "file", fileName);
                    using (var creationContent = new StringContent(creationTime, NoBOMEncodingUTF8, "text/plain"))
                    {
                        multiPartContent.Add(creationContent, "timestamp");
                        using (var sizeContent = new StringContent(length, NoBOMEncodingUTF8, "text/plain"))
                        {
                            multiPartContent.Add(sizeContent, "filesize");
                            using (var accountContent = new StringContent(account, NoBOMEncodingUTF8, "text/plain"))
                            {
                                multiPartContent.Add(accountContent, "account");
                                return GetWingmanStringResponse("CheckUploadPossible", CheckUploadURL, traces, parserVersion, HttpMethod.Post, multiPartContent) == "True";
                            }
                        }
                    }
                }
            }
        }
        public static bool UploadProcessed(FileInfo fi, string account, string jsonString, string htmlString, List<string> traces, Version parserVersion)
        {
            string fileName = fi.Name;
            byte[] fileContents = File.ReadAllBytes(fi.FullName);
            using (var multiPartContent = new MultipartFormDataContent("----MyGreatBoundary"))
            {
                using (var byteArrayContent = new ByteArrayContent(fileContents))
                {
                    byteArrayContent.Headers.Add("Content-Type", "application/octet-stream");
                    multiPartContent.Add(byteArrayContent, "file", fileName);
                    using (var jsonContent = new StringContent(jsonString, NoBOMEncodingUTF8, "application/json"))
                    {
                        multiPartContent.Add(jsonContent, "jsonfile");
                        using (var htmlContent = new StringContent(htmlString, NoBOMEncodingUTF8, "text/html"))
                        {
                            multiPartContent.Add(htmlContent, "htmlfile");
                            using (var accountContent = new StringContent(account, NoBOMEncodingUTF8, "text/plain"))
                            {
                                multiPartContent.Add(accountContent, "account");
                                return GetWingmanResponse<GW2EIJSON.JsonLog>("UploadProcessed", UploadProcessedURL, traces, parserVersion, HttpMethod.Post, multiPartContent) != null;
                            }
                        }
                    }
                }
            }
        }

        //
        private static string _GetWingmanStringResponse(string requestName, string url, List<string> traces, HttpMethod method, HttpContent content = null)
        {
            const int tentatives = 5;
            for (int i = 0; i < tentatives; i++)
            {
                traces.Add("Wingman: " + requestName + " tentative");
                var webService = new Uri(@url);
                var requestMessage = new HttpRequestMessage(method, webService);
                requestMessage.Headers.ExpectContinue = false;

                if (content != null)
                {
                    requestMessage.Content = content;
                }

                var httpClient = new HttpClient();
                try
                {
                    Task<HttpResponseMessage> httpRequest = httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
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
                        traces.Add("Wingman: " + requestName + " successful");
                        return stringContents;
                    }
                }
                catch (Exception e)
                {
                    traces.Add("Wingman: " + requestName + " failed " + e.Message);
                }
                finally
                {
                    httpClient.Dispose();
                    requestMessage.Dispose();
                }
            }
            return default;
        }


        private static string GetWingmanStringResponse(string requestName, string url, List<string> traces, Version parserVersion, HttpMethod method, HttpContent content = null)
        {
            if (!CanBeUsed(parserVersion, traces))
            {
                return default;
            }
            return _GetWingmanStringResponse(requestName, url, traces, method, content);
        }
        private static T _GetWingmanResponse<T>(string requestName, string url, List<string> traces, HttpMethod method, HttpContent content = null)
        {
            const int tentatives = 5;
            for (int i = 0; i < tentatives; i++)
            {
                traces.Add("Wingman: " + requestName + " tentative");
                var webService = new Uri(@url);
                var requestMessage = new HttpRequestMessage(method, webService);
                requestMessage.Headers.ExpectContinue = false;

                if (content != null)
                {
                    requestMessage.Content = content;
                }

                var httpClient = new HttpClient();
                try
                {
                    Task<HttpResponseMessage> httpRequest = httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
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
                        traces.Add("Wingman: " + requestName + " successful");
                        return item;
                    }
                }
                catch (Exception e)
                {
                    traces.Add("Wingman: " + requestName + " failed " + e.Message);
                }
                finally
                {
                    httpClient.Dispose();
                    requestMessage.Dispose();
                }
            }
            return default;
        }

        private static T GetWingmanResponse<T>(string requestName, string url, List<string> traces, Version parserVersion, HttpMethod method, HttpContent content = null)
        {
            if (!CanBeUsed(parserVersion, traces))
            {
                return default;
            }
            return _GetWingmanResponse<T>(requestName, url, traces, method, content);
        } 

    }

}
