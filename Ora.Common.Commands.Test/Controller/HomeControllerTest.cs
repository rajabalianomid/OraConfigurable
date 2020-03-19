using FluentAssertions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Moq;
using Ora.Services.Configurable;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ora.Common.Commands.Test.Controller
{
    public class HomeControllerTest
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public HomeControllerTest()
        {
            _server = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task home_controller_test_should_return_string_content()
        {
            var response = await _client.GetAsync("/");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            content.Should().BeEquivalentTo("Ora.Services.Configurable Is Active");
        }
    }
}
