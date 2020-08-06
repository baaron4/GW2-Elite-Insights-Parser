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

namespace GW2EIDPSReport
{
    public static class DPSReportAPI
    {
        private static readonly DefaultContractResolver DefaultJsonContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };
        private static string BaseUploadContentURL { get; } = "https://dps.report/uploadContent?json=1";

        private static string GetUploadContentURL(DPSReportSettings settings)
        {
            string url = BaseUploadContentURL;
            if (settings.UserToken != null && settings.UserToken.Length > 0)
            {
                url += "&userToken=" + settings.UserToken;
            }
            return url;
        }

        public static DPSReportUploadObject UploadDPSReportsEI(FileInfo fi, DPSReportSettings settings, List<string> traces)
        {         
            return UploadToDPSR(fi, GetUploadContentURL(settings) + "&generator=ei", traces);
        }
        public static DPSReportUploadObject UploadDPSReportsRH(FileInfo fi, DPSReportSettings settings, List<string> traces)
        {
            return UploadToDPSR(fi, GetUploadContentURL(settings) + "&generator=rh", traces);

        }
        /*private static string UploadRaidar(FileInfo fi)
        {
            //string fileName = fi.Name;
            //byte[] fileContents = File.ReadAllBytes(fi.FullName);
            //Uri webService = new Uri(@"https://www.gw2raidar.com/api/v2/encounters/new");
            //HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, webService);
            //requestMessage.Headers.ExpectContinue = false;

            //MultipartFormDataContent multiPartContent = new MultipartFormDataContent("----MyGreatBoundary");
            //ByteArrayContent byteArrayContent = new ByteArrayContent(fileContents);
            //byteArrayContent.Headers.Add("Content-Type", "application/octet-stream");
            //multiPartContent.Add(byteArrayContent, "file", fileName);
            //requestMessage.Content = multiPartContent;

            //HttpClient httpClient = new HttpClient();
            //try
            //{
            //    Task<HttpResponseMessage> httpRequest = httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
            //    HttpResponseMessage httpResponse = httpRequest.Result;
            //    HttpStatusCode statusCode = httpResponse.StatusCode;
            //    HttpContent responseContent = httpResponse.Content;

            //    if (responseContent != null)
            //    {
            //        Task<String> stringContentsTask = responseContent.ReadAsStringAsync();
            //        String stringContents = stringContentsTask.Result;
            //        int first = stringContents.IndexOf('{');
            //        int length = stringContents.LastIndexOf('}') - first + 1;
            //        String JSONFormat = stringContents.Substring(first, length);
            //        DPSReportsResponseItem item = JsonConvert.DeserializeObject<DPSReportsResponseItem>(JSONFormat);
            //        String logLink = item.permalink;
            //        return logLink;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    return ex.Message;
            //    // Console.WriteLine(ex.Message);
            //}
            return "";
        }*/
        private static DPSReportUploadObject UploadToDPSR(FileInfo fi, string URI, List<string> traces)
        {
            string fileName = fi.Name;
            byte[] fileContents = File.ReadAllBytes(fi.FullName);
            const int tentatives = 5;
            for (int i = 0; i < tentatives; i++)
            {
                traces.Add("Upload tentative");
                var webService = new Uri(@URI);
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, webService);
                requestMessage.Headers.ExpectContinue = false;

                var multiPartContent = new MultipartFormDataContent("----MyGreatBoundary");
                var byteArrayContent = new ByteArrayContent(fileContents);
                byteArrayContent.Headers.Add("Content-Type", "application/octet-stream");
                multiPartContent.Add(byteArrayContent, "file", fileName);
                //multiPartContent.Add(new StringContent("generator=ei"), "gen", "ei");
                requestMessage.Content = multiPartContent;

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
                        DPSReportUploadObject item = JsonConvert.DeserializeObject<DPSReportUploadObject>(stringContents, new JsonSerializerSettings
                        {
                            ContractResolver = DefaultJsonContractResolver
                        });
                        if (item.Error != null)
                        {
                            throw new InvalidOperationException(item.Error);
                        }
                        traces.Add("Upload tentative successful");
                        return item;
                    }
                }
                catch (Exception e)
                {
                    traces.Add("Upload tentative failed: " + e.Message);
                }
                finally
                {
                    byteArrayContent.Dispose();
                    httpClient.Dispose();
                    requestMessage.Dispose();
                }
            }        
            return null;
        }

    }

}
