using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GW2EIGW2API;
using Newtonsoft.Json;

namespace GW2EIDPSReport
{
    public static class UploadController
    {
        private static string UploadDPSReportsEI(FileInfo fi, List<string> traces)
        {
            return UploadToDPSR(fi, "https://dps.report/uploadContent?json=1&generator=ei", traces);
        }
        private static string UploadDPSReportsRH(FileInfo fi, List<string> traces)
        {
            return UploadToDPSR(fi, "https://dps.report/uploadContent?json=1&generator=rh", traces);

        }
        private static string UploadRaidar(/*FileInfo fi*/)
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
        }
        internal class DPSReportsResponseItem
        {
            public string Permalink { get; set; }
            public string Error { get; set; }
        }
        private static string UploadToDPSR(FileInfo fi, string URI, List<string> traces)
        {
            string fileName = fi.Name;
            byte[] fileContents = File.ReadAllBytes(fi.FullName);
            const int tentatives = 5;
            string res = "Upload process failed";
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
                        DPSReportsResponseItem item = JsonConvert.DeserializeObject<DPSReportsResponseItem>(stringContents, new JsonSerializerSettings
                        {
                            ContractResolver = APIHelper.DefaultJsonContractResolver
                        });
                        if (item.Error != null)
                        {
                            throw new InvalidOperationException(item.Error);
                        }
                        traces.Add("Upload tentative successful");
                        return item.Permalink;
                    }
                }
                catch (Exception e)
                {
                    res = e.Message;
                    traces.Add("Upload tentative failed: " + res);
                }
                finally
                {
                    byteArrayContent.Dispose();
                    httpClient.Dispose();
                    requestMessage.Dispose();
                }
            }        
            return res;
        }

        public static string[] UploadOperation(List<string> traces, FileInfo fInfo, UploadSettings settings)
        {
            //Upload Process
            string[] uploadresult = new string[3] { "", "", "" };
            if (settings.UploadToDPSReportsUsingEI)
            {
                traces.Add("Uploading to DPSReports using EI");
                uploadresult[0] = UploadDPSReportsEI(fInfo, traces);
                traces.Add("DPSReports using EI: " + uploadresult[0]);
            }
            if (settings.UploadToDPSReportsUsingRH)
            {
                traces.Add("Uploading to DPSReports using RH");
                uploadresult[1] = UploadDPSReportsRH(fInfo, traces);
                traces.Add("DPSReports using RH: " + uploadresult[1]);
            }
            if (settings.UploadToRaidar)
            {
                traces.Add("Uploading to Raidar");
                uploadresult[2] = UploadRaidar(/*fInfo*/);
                traces.Add("Raidar: " + uploadresult[2]);
            }
            return uploadresult;
        }

    }

}
