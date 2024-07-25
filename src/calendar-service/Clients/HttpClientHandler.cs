using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Calendar.Service.Clients
{
    public interface IHttpClientHandler
    {
        Task<HttpResponseMessage> GetAsync(string url);
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage);
    }

    public class HttpClientHandler : IHttpClientHandler
    {
        private readonly HttpClient _client = new HttpClient();

        public HttpClientHandler(string url)
        {
            _client.BaseAddress = new Uri(url);
        }

        public Task<HttpResponseMessage> GetAsync(string url)
        {
            return _client.GetAsync(url);
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage)
        {
            return _client.SendAsync(requestMessage);
        }
    }
}
