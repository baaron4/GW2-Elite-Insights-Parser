using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
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
        public delegate void TraceHandler(string trace);

        private static readonly DefaultContractResolver DefaultJsonContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };
        private static readonly UTF8Encoding NoBOMEncodingUTF8 = new UTF8Encoding(false);
        private static readonly HttpClient HTTPClient = new HttpClient();
       

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

        private static bool IsDPSReportLinkValid(string dpsReportLink, TraceHandler traceHandler)
        {
            if (!dpsReportLink.Contains("https") && !dpsReportLink.Contains(".report"))
            {
                traceHandler("Wingman: Invalid dps.report link");
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
        private static bool CheckConnection(TraceHandler traceHandler)
        {
            return _GetWingmanResponse("CheckConnection", TestConnectionURL, traceHandler, HttpMethod.Get) == "True";
        }

        private static bool IsEIVersionValid(Version parserVersion, TraceHandler traceHandler)
        {
            string returnedVersion = _GetWingmanResponse("EIVersionURL", EIVersionURL, traceHandler, HttpMethod.Get);
            if (returnedVersion == null)
            {
                return false;
            }
            returnedVersion = returnedVersion.Replace("v", "");
            var expectedVersion = new Version(returnedVersion);
            return parserVersion.CompareTo(expectedVersion) >= 0;
        }

        public static bool CanBeUsed(Version parserVersion, TraceHandler traceHandler)
        {
            return CheckConnection(traceHandler) && IsEIVersionValid(parserVersion, traceHandler);
        }
        //

        public static WingmanCheckLogQueuedOrDBObject GetCheckLogQueuedOrDB(string dpsReportLink, TraceHandler traceHandler, Version parserVersion)
        {
            if (!IsDPSReportLinkValid(dpsReportLink, traceHandler))
            {
                return null;
            }
            try
            {
                return JsonConvert.DeserializeObject<WingmanCheckLogQueuedOrDBObject>(GetWingmanResponse("CheckLogQueuedOrDB", GetCheckLogQueuedOrDBURL(dpsReportLink), traceHandler, parserVersion, HttpMethod.Post), new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = DefaultJsonContractResolver,
                    StringEscapeHandling = StringEscapeHandling.EscapeHtml
                });
            }
            catch (Exception e)
            {
                traceHandler("Wingman: CheckLogQueuedOrDB failed - " + e.Message);
                return null;
            }
        }

        public static WingmanCheckLogQueuedObject GetCheckLogQueued(string dpsReportLink, TraceHandler traceHandler, Version parserVersion)
        {
            if (!IsDPSReportLinkValid(dpsReportLink, traceHandler))
            {
                return null;
            }
            try
            {
                return JsonConvert.DeserializeObject<WingmanCheckLogQueuedObject>(GetWingmanResponse("CheckLogQueued", GetCheckLogQueuedOrDBURL(dpsReportLink), traceHandler, parserVersion, HttpMethod.Post), new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = DefaultJsonContractResolver,
                    StringEscapeHandling = StringEscapeHandling.EscapeHtml
                });
            }
            catch (Exception e)
            {
                traceHandler("Wingman: CheckLogQueued failed - " + e.Message);
                return null;
            }
        }

        public static WingmanImportLogQueuedObject ImportLogQueued(string dpsReportLink, TraceHandler traceHandler, Version parserVersion)
        {
            if (!IsDPSReportLinkValid(dpsReportLink, traceHandler))
            {
                return null;
            }
            try
            {
                return JsonConvert.DeserializeObject<WingmanImportLogQueuedObject>(GetWingmanResponse("ImportLogQueued", GetCheckLogQueuedOrDBURL(dpsReportLink), traceHandler, parserVersion, HttpMethod.Post), new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = DefaultJsonContractResolver,
                    StringEscapeHandling = StringEscapeHandling.EscapeHtml
                });
            }
            catch (Exception e)
            {
                traceHandler("Wingman: ImportLogQueued failed - " + e.Message);
                return null;
            }
        }

        public static bool UploadToWingmanUsingImportLogQueued(string dpsReportLink, TraceHandler traceHandler, Version parserVersion)
        {
            // Check if the URL is already present on Wingman
            WingmanCheckLogQueuedOrDBObject wingmanCheck = GetCheckLogQueuedOrDB(dpsReportLink, traceHandler, parserVersion);
            if (wingmanCheck != null)
            {
                if (wingmanCheck.InDB || wingmanCheck.InQueue)
                {
                    traceHandler("Wingman: Upload failed - Log already present in Wingman DB");
                    return false;
                }
                else
                {
                    WingmanImportLogQueuedObject wingmanUpload = ImportLogQueued(dpsReportLink, traceHandler, parserVersion);
                    if (wingmanUpload != null)
                    {
                        if (wingmanUpload.Success != 1)
                        {
                            traceHandler("Wingman: Upload failed - " + wingmanUpload.Note);
                            return true;
                        }
                        else
                        {
                            traceHandler("Wingman: Upload successful - " + wingmanUpload.Note);
                            return true;
                        }
                    }
                    return false;
                }
            }
            return false;
        }

        public static bool CheckUploadPossible(FileInfo fi, string account, TraceHandler traceHandler, Version parserVersion)
        {
            string creationTime = new DateTimeOffset(fi.CreationTime).ToUnixTimeSeconds().ToString();
            var data = new Dictionary<string, string> { { "account", account }, { "filesize", fi.Length.ToString() }, { "timestamp", creationTime }, { "file", fi.Name } };
            Func<HttpContent> contentCreator = () =>
            {
                return new FormUrlEncodedContent(data);
            };
            return GetWingmanResponse("CheckUploadPossible", CheckUploadURL, traceHandler, parserVersion, HttpMethod.Post, contentCreator) == "True";
        }
        public static bool UploadProcessed(FileInfo fi, string account, byte[] jsonFile, byte[] htmlFile, TraceHandler traceHandler, Version parserVersion)
        {
            //var data = new Dictionary<string, string> { { "account", account }, { "file", File.ReadAllText(fi.FullName) }, { "jsonfile", jsonString }, { "htmlfile", htmlString } };
            byte[] fileBytes = File.ReadAllBytes(fi.FullName);
            string name = fi.Name;
            string jsonName = Path.GetFileNameWithoutExtension(fi.Name) + ".json";
            string htmlName = Path.GetFileNameWithoutExtension(fi.Name) + ".html";
            Func<HttpContent> contentCreator = () =>
            {
                var multiPartContent = new MultipartFormDataContent();
                var fileContent = new ByteArrayContent(fileBytes);
                fileContent.Headers.Add("Content-Type", "application/octet-stream");
                multiPartContent.Add(fileContent, "file", name);
                var jsonContent = new StringContent(Encoding.UTF8.GetString(jsonFile));
                multiPartContent.Add(jsonContent, "jsonfile", jsonName);
                var htmlContent = new StringContent(Encoding.UTF8.GetString(htmlFile));
                multiPartContent.Add(htmlContent, "htmlfile", htmlName);
                var data = new Dictionary<string, string> { { "account", account } };
                var dataContent = new FormUrlEncodedContent(data);
                multiPartContent.Add(dataContent);
                return multiPartContent;
            };

            string response = GetWingmanResponse("UploadProcessed", UploadProcessedURL, traceHandler, parserVersion, HttpMethod.Post, contentCreator);
            return response != null && response != "False";
        }

        //
        private static string _GetWingmanResponse(string requestName, string url, TraceHandler traceHandler, HttpMethod method, Func<HttpContent> content = null)
        {
            const int tentatives = 5;
            for (int i = 0; i < tentatives; i++)
            {
                traceHandler("Wingman: " + requestName + " tentative");
                var webService = new Uri(@url);
                var requestMessage = new HttpRequestMessage(method, webService);
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
                        traceHandler("Wingman: " + requestName + " successful");
                        return stringContents;
                    }
                }
                catch (Exception e)
                {
                    traceHandler("Wingman: " + requestName + " failed " + e.Message);
                }
                finally
                {
                    requestMessage.Dispose();
                }
            }
            return null;
        }


        private static string GetWingmanResponse(string requestName, string url, TraceHandler traceHandler, Version parserVersion, HttpMethod method, Func<HttpContent> content = null)
        {
            if (!CanBeUsed(parserVersion, traceHandler))
            {
                return default;
            }
            return _GetWingmanResponse(requestName, url, traceHandler, method, content);
        }

    }

}
