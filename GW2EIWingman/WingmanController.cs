using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

[assembly: System.CLSCompliant(false)]
namespace GW2EIWingman
{
    public static class WingmanController
    {
        ///////////////// URL Utilities

        private static string TestConnectionURL { get; } = "https://gw2wingman.nevermindcreations.de/testConnection";

        private const string BaseAPIURL = "https://gw2wingman.nevermindcreations.de/api/";
        private static string EIVersionURL { get; } = BaseAPIURL + "EIversion";
        private static string ImportLogQueuedURL { get; } = BaseAPIURL + "importLogQueued";
        private static string CheckLogQueuedURL { get; } = BaseAPIURL + "checkLogQueued";
        private static string CheckLogQueuedOrDBURL { get; } = BaseAPIURL + "checkLogQueuedOrDB";
        private static string CheckUploadURL { get; } = BaseAPIURL + "checkUpload";
        private static string UploadProcessedURL { get; } = BaseAPIURL + "uploadProcessed";
    }

}
