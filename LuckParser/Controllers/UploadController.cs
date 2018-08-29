using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LuckParser.Controllers
{
    public class UploadController
    {
        public string  UploadDPSReportsEI(FileInfo fi)
        {
            string fileName = fi.Name;
            byte[] fileContents = File.ReadAllBytes(fi.FullName);
            Uri webService = new Uri(@"https://dps.report/uploadContent?generator=ei");
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, webService);
            requestMessage.Headers.ExpectContinue = false;

            MultipartFormDataContent multiPartContent = new MultipartFormDataContent("----MyGreatBoundary");
            ByteArrayContent byteArrayContent = new ByteArrayContent(fileContents);
            byteArrayContent.Headers.Add("Content-Type", "application/octet-stream");
            multiPartContent.Add(byteArrayContent, "file", fileName);
            //multiPartContent.Add(new StringContent("generator=ei"), "gen", "ei");
            requestMessage.Content = multiPartContent;

            HttpClient httpClient = new HttpClient();
            try
            {
                Task<HttpResponseMessage> httpRequest = httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
                HttpResponseMessage httpResponse = httpRequest.Result;
                HttpStatusCode statusCode = httpResponse.StatusCode;
                HttpContent responseContent = httpResponse.Content;

                if (responseContent != null)
                {
                    Task<String> stringContentsTask = responseContent.ReadAsStringAsync();
                    String stringContents = stringContentsTask.Result;
                    int first = stringContents.IndexOf('{');
                    int length = stringContents.LastIndexOf('}') - first +1;
                    String JSONFormat = stringContents.Substring(first,length);
                    DPSReportsResponseItem item = JsonConvert.DeserializeObject<DPSReportsResponseItem>(JSONFormat);
                    String logLink = item.permalink;
                    return logLink;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
               // Console.WriteLine(ex.Message);
            }
            return "";
        }
        public string UploadDPSReportsRH(FileInfo fi)
        {
            string fileName = fi.Name;
            byte[] fileContents = File.ReadAllBytes(fi.FullName);
            Uri webService = new Uri(@"https://dps.report/uploadContent?generator=rh");
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, webService);
            requestMessage.Headers.ExpectContinue = false;

            MultipartFormDataContent multiPartContent = new MultipartFormDataContent("----MyGreatBoundary");
            ByteArrayContent byteArrayContent = new ByteArrayContent(fileContents);
            byteArrayContent.Headers.Add("Content-Type", "application/octet-stream");
            multiPartContent.Add(byteArrayContent, "file", fileName);
            //multiPartContent.Add(new StringContent("generator=ei"), "gen", "ei");
            requestMessage.Content = multiPartContent;

            HttpClient httpClient = new HttpClient();
            try
            {
                Task<HttpResponseMessage> httpRequest = httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
                HttpResponseMessage httpResponse = httpRequest.Result;
                HttpStatusCode statusCode = httpResponse.StatusCode;
                HttpContent responseContent = httpResponse.Content;

                if (responseContent != null)
                {
                    Task<String> stringContentsTask = responseContent.ReadAsStringAsync();
                    String stringContents = stringContentsTask.Result;
                    int first = stringContents.IndexOf('{');
                    int length = stringContents.LastIndexOf('}') - first + 1;
                    String JSONFormat = stringContents.Substring(first, length);
                    DPSReportsResponseItem item = JsonConvert.DeserializeObject<DPSReportsResponseItem>(JSONFormat);
                    String logLink = item.permalink;
                    return logLink;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
                // Console.WriteLine(ex.Message);
            }
            return "";
        }
        public string UploadRaidar(FileInfo fi)
        {
            string fileName = fi.Name;
            byte[] fileContents = File.ReadAllBytes(fi.FullName);
            Uri webService = new Uri(@"https://www.gw2raidar.com/api/v2/encounters/new");
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, webService);
            requestMessage.Headers.ExpectContinue = false;

            MultipartFormDataContent multiPartContent = new MultipartFormDataContent("----MyGreatBoundary");
            ByteArrayContent byteArrayContent = new ByteArrayContent(fileContents);
            byteArrayContent.Headers.Add("Content-Type", "application/octet-stream");
            multiPartContent.Add(byteArrayContent, "file", fileName);
            requestMessage.Content = multiPartContent;

            HttpClient httpClient = new HttpClient();
            try
            {
                Task<HttpResponseMessage> httpRequest = httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
                HttpResponseMessage httpResponse = httpRequest.Result;
                HttpStatusCode statusCode = httpResponse.StatusCode;
                HttpContent responseContent = httpResponse.Content;

                if (responseContent != null)
                {
                    Task<String> stringContentsTask = responseContent.ReadAsStringAsync();
                    String stringContents = stringContentsTask.Result;
                    int first = stringContents.IndexOf('{');
                    int length = stringContents.LastIndexOf('}') - first + 1;
                    String JSONFormat = stringContents.Substring(first, length);
                    DPSReportsResponseItem item = JsonConvert.DeserializeObject<DPSReportsResponseItem>(JSONFormat);
                    String logLink = item.permalink;
                    return logLink;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
                // Console.WriteLine(ex.Message);
            }
            return "";
        }
        public class DPSReportsResponseItem
        {
            public string permalink;
        }
        //public async Task UploadDPSReports(FileInfo fi)
        //{

           
        //    //var tasks = new List<Task> { };
        //    //tasks.Add(Task.Run(() => Upload("https://dps.report/uploadContent", filePath)));
        //    //// ...
        //    //await Task.WhenAll(tasks);
        //}
          
    }
    
}
