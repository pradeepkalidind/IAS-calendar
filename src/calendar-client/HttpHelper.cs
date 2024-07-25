using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PWC.IAS.Calendar.Client
{
    public class HttpHelper
    {
        private readonly HttpClient httpClient;
        private readonly string userName;
        private readonly string password;

        public HttpHelper(HttpClient httpClient, string userName, string password)
        {
            this.httpClient = httpClient;
            this.userName = userName;
            this.password = password;
        }

        public async Task<HttpResponseMessage> SendRequest(string resourceUrl, HttpMethod method)
        {
            var request = CreateRequestWithCredential(resourceUrl, method);
            return await SendRequest(request);
        }

        public async Task<HttpResponseMessage> SendRequest(HttpRequestMessage request)
        {
            return await httpClient.SendAsync(request);
        }

        public HttpRequestMessage CreateRequestWithCredential(string resourceUrl, HttpMethod method)
        {
            return CreateRequestWithCredentialAndBody(resourceUrl, method, "");
        }

        public HttpRequestMessage CreateRequestWithCredentialAndBody(string resourceUrl, HttpMethod method, string body)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{httpClient.BaseAddress.OriginalString}/{resourceUrl}"),
                Method = method
            };
            if (method == HttpMethod.Post)
            {
                request.Content = new StringContent(body);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            }

            var credential = $"{userName}:{password}";
            var credentialEncoded = Convert.ToBase64String(Encoding.ASCII.GetBytes(credential));
            request.Headers.Add("Authorization", "Basic " + credentialEncoded);

            return request;
        }
    }
}
