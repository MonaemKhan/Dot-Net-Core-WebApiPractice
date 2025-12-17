using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace APICOOKIESTESTING.TEST
{
    public class CookieValidationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public CookieValidationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetData_WithValidCookie_ReturnsOk()
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("Cookie", "AuthCookie=ValidToken123");

            var response = await client.GetAsync("/api/data");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Cookie Validated", content);
        }

        [Fact]
        public async Task GetData_MissingCookie_ReturnsUnauthorized()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/data");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetData_InvalidCookie_ReturnsForbidden()
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("Cookie", "AuthCookie=WrongValue");

            var response = await client.GetAsync("/api/data");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
