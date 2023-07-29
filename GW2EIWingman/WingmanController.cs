using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
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

        ///////////////// URL Utilities

        private static string TestConnectionURL { get; } = "https://gw2wingman.nevermindcreations.de/testConnection";

        private const string BaseAPIURL = "https://gw2wingman.nevermindcreations.de/api/";
        private static string EIVersionURL { get; } = BaseAPIURL + "EIversion";
        private static string ImportLogQueuedURL { get; } = BaseAPIURL + "importLogQueued";
        private static string CheckLogQueuedURL { get; } = BaseAPIURL + "checkLogQueued";
        private static string CheckLogQueuedOrDBURL { get; } = BaseAPIURL + "checkLogQueuedOrDB";
        private static string CheckUploadURL { get; } = BaseAPIURL + "checkUpload";
        private static string UploadProcessedURL { get; } = BaseAPIURL + "uploadProcessed";

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
        private static bool CheckConnection()
        {
            return false;
        }

        private static bool IsEIVersionValid(Version parserVersion)
        {
            return false;
        }

        public static bool CanBeUsed(Version parserVersion)
        {
            return CheckConnection() && IsEIVersionValid(parserVersion);
        }
        //

        public static CheckLogQueuedOrDBObject GetCheckLogQueuedOrDB(string dpsReportLink, List<string> traces, Version parserVersion)
        {
            if (!IsDPSReportLinkValid(dpsReportLink, traces))
            {
                return null;
            }
            return GetWingmanResponse<CheckLogQueuedOrDBObject>("CheckLogQueuedOrDB", GetCheckLogQueuedOrDBURL(dpsReportLink), traces, parserVersion);
        }

        public static CheckLogQueuedObject GetCheckLogQueued(string dpsReportLink, List<string> traces, Version parserVersion)
        {
            if (!IsDPSReportLinkValid(dpsReportLink, traces))
            {
                return null;
            }
            return GetWingmanResponse<CheckLogQueuedObject>("CheckLogQueued", GetCheckLogQueuedURL(dpsReportLink), traces, parserVersion);
        }

        public static ImportLogQueuedObject ImportLogQueued(string dpsReportLink, List<string> traces, Version parserVersion)
        {
            if (!IsDPSReportLinkValid(dpsReportLink, traces))
            {
                return null;
            }
            return GetWingmanResponse<ImportLogQueuedObject>("ImportLogQueued", GetImportLogQueuedURL(dpsReportLink), traces, parserVersion);
        }

        public static bool UploadToWingmanUsingImportLogQueued(string dpsReportLink, List<string> traces, Version parserVersion)
        {
            // Check if the URL is already present on Wingman
            CheckLogQueuedOrDBObject wingmanCheck = GetCheckLogQueuedOrDB(dpsReportLink, traces, parserVersion);
            if (wingmanCheck != null)
            {
                if (wingmanCheck.InDB || wingmanCheck.InQueue)
                {
                    traces.Add("Wingman: Upload failed - Log already present in Wingman DB");
                    return false;
                }
                else
                {
                    ImportLogQueuedObject wingmanUpload = ImportLogQueued(dpsReportLink, traces, parserVersion);
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

        private static T GetWingmanResponse<T>(string requestName, string url, List<string> traces, Version parserVersion)
        {
            if (!CanBeUsed(parserVersion))
            {
                traces.Add("Wingman: Not available");
                return default;
            }
            const int tentatives = 5;
            for (int i = 0; i < tentatives; i++)
            {
                traces.Add("Wingman: " + requestName + " tentative");
                var webService = new Uri(@url);
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, webService);
                requestMessage.Headers.ExpectContinue = false;

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

    }

}
