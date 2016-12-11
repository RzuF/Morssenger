using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Windows.Foundation;
using Newtonsoft.Json;
using Windows.Security.Cryptography;
using ServiceStack;
using MarkerMetro.Unity.WinLegacy.Security.Cryptography;

namespace HelloWorldBlinky
{
    public sealed class QueueGate
    {
        private HttpClient _client;

        public int Interval { get; }
        public string MyAccount { get; }
        public string MyQueue { get; }
        public string MyKey { get; }

        public QueueGate(int interval, string myAccount, string myQueue, string myKey)
        {
            Interval = interval;
            MyAccount = "morsecode";
            MyQueue = "morsecode";
            MyKey = "rmn80OV8EdSrKej30NQWoaQqDnQwPBNzbpfU+Z4u5900KH4dlVnpBH6j18H10A+siEoPQGHqecjsUJKceOLMww==";
        }

        //public async Task<bool> CreateQueue(string name)
        //{
        //    string requestMethod = "PUT";

        //    string urlPath = $"{name}";

        //    string storageServiceVersion = "2015-12-11";
        //    string dateInRfc1123Format = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);

        //    string stringToSign = $"{requestMethod}\n\n\n\n\ntext/plain; charset=utf-8\n\n\n\n\n\n\nx-ms-date:{dateInRfc1123Format}\nx-ms-version:{storageServiceVersion}\n/{MyAccount}/{urlPath}";

        //    string signature = string.Empty;

        //    using (HMACSHA256 hmacSha256 = new HMACSHA256(Convert.FromBase64String(MyKey)))
        //    {
        //        byte[] dataToHmac = Encoding.UTF8.GetBytes(stringToSign);
        //        signature = Convert.ToBase64String(hmacSha256.ComputeHash(dataToHmac));
        //    }

        //    string authorizationHeader = $"SharedKey {MyAccount}:{signature}";

        //    _client.DefaultRequestHeaders.Add("Authorization", authorizationHeader);
        //    _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/atom+xml"));
        //    _client.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8"));
        //    _client.DefaultRequestHeaders.Add("x-ms-date", dateInRfc1123Format);
        //    _client.DefaultRequestHeaders.Add("x-ms-version", storageServiceVersion);

        //    var response = await _client.PutAsync($"https://{MyAccount}.queue.core.windows.net/{name}", new StringContent(""));

        //    return response.StatusCode == HttpStatusCode.Created;
        //}

        public IAsyncOperation<bool> SendMessageOperation(string message)
        {
            return SendMessage(message).AsAsyncOperation();
        }

        internal async Task<bool> SendMessage(string message)
        {
            string str = "morscodereverse";
            _client = new HttpClient();

            string requestMethod = "POST";

            string urlPath = $"{str}/messages";

            string storageServiceVersion = "2012-02-12";
            string dateInRfc1123Format = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);

            string messageText = $"<QueueMessage><MessageText>{message}</MessageText></QueueMessage>";
            UTF8Encoding utf8Encoding = new UTF8Encoding();
            byte[] messageContent = utf8Encoding.GetBytes(messageText);
            int messageLength = messageContent.Length;

            string canonicalizedHeaders = $"x-ms-date:{dateInRfc1123Format}\nx-ms-version:{storageServiceVersion}";
            string canonicalizedResource = $"/{MyAccount}/{urlPath}\nmessagettl:{Interval}";
            string stringToSign = $"{requestMethod}\n\n\n{messageLength}\n\ntext/plain; charset=utf-8\n\n\n\n\n\n\n{canonicalizedHeaders}\n{canonicalizedResource}";

            string signature = string.Empty;

            using (HMACSHA256 hmacSha256 = new HMACSHA256(Convert.FromBase64String(MyKey)))
            {
                byte[] dataToHmac = Encoding.UTF8.GetBytes(stringToSign);
                signature = Convert.ToBase64String(hmacSha256.ComputeHash(dataToHmac));
            }

            string authorizationHeader = $"SharedKey {MyAccount}:{signature}";

            _client.DefaultRequestHeaders.Add("Authorization", authorizationHeader);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/atom+xml"));
            _client.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8"));
            _client.DefaultRequestHeaders.Add("x-ms-date", dateInRfc1123Format);
            _client.DefaultRequestHeaders.Add("x-ms-version", storageServiceVersion);

            var response = await _client.PostAsync($"https://{MyAccount}.queue.core.windows.net/{str}/messages?messagettl={Interval}", new StringContent(messageText, Encoding.UTF8));

            return response.StatusCode == HttpStatusCode.Created;
        }

        public IAsyncOperation<string> GetLastMessageOperation()
        {
            return GetLastMessage().AsAsyncOperation();
        }

        internal async Task<string> GetLastMessage()
        {
            _client = new HttpClient();

            string requestMethod = "GET";

            string urlPath = "messages";

            string storageServiceVersion = "2015-12-11";
            string dateInRfc1123Format = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);

            string stringToSign = $"{requestMethod}\n\n\n\n\n\n\n\n\n\n\n\nx-ms-date:{dateInRfc1123Format}\nx-ms-version:{storageServiceVersion}\n/{MyAccount}/{MyQueue}/{urlPath}";

            string signature = string.Empty;

            using (HMACSHA256 hmacSha256 = new HMACSHA256(Convert.FromBase64String(MyKey)))
            {
                Byte[] dataToHmac = Encoding.UTF8.GetBytes(stringToSign);
                signature = Convert.ToBase64String(hmacSha256.ComputeHash(dataToHmac));
            }

            string authorizationHeader = $"SharedKey {MyAccount}:{signature}";

            _client.DefaultRequestHeaders.Add("Authorization", authorizationHeader);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/atom+xml"));
            _client.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8"));
            _client.DefaultRequestHeaders.Add("x-ms-date", dateInRfc1123Format);
            _client.DefaultRequestHeaders.Add("x-ms-version", storageServiceVersion);

            var response = await _client.GetAsync($"https://{MyAccount}.queue.core.windows.net/{MyQueue}/messages");

            var xml = await response.Content.ReadAsStringAsync();

            XDocument xdoc = XDocument.Parse(xml);
            string jsonText = JsonConvert.SerializeXNode(xdoc);

            var results = JsonConvert.DeserializeObject<dynamic>(jsonText);

            return results["QueueMessagesList"] == null ? null : results["QueueMessagesList"]["QueueMessage"]["MessageText"];
        }
    }
}
