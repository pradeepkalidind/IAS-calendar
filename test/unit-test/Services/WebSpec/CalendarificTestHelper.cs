using System;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Calendar.Service.Clients;
using Moq;

namespace Calendar.Tests.Unit.Services.WebSpec
{
    public class CalendarificTestHelper
    {
        public static CalendarificClient MockCalendarificClient(string mockHolidaysResponse)
        {
            var mockedHttpClient = new Mock<IHttpClientHandler>();
            var mockHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(mockHolidaysResponse, Encoding.UTF8, MediaTypeNames.Application.Json)
            };

            mockedHttpClient
                .Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()))
                .Returns(Task.FromResult(mockHttpResponseMessage));

            return new CalendarificClient(mockedHttpClient.Object);
        }
    }
}
