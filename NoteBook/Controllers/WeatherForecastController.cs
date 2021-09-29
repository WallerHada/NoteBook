using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NoteBook.HttpNote;
using NoteBook.ZZService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NoteBook.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private readonly string id4ConfigUrl = "https://localhost:5001/.well-known/openid-configuration";
        private readonly string getToken = "https://localhost:5001/connect/token";
        private readonly string identity = "https://localhost:6001/identity";

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly BaseClients _baseClients;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, BaseClients baseClients)
        {
            _logger = logger;
            _baseClients = baseClients;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [AllowAnonymous]
        [Route("id4ConfigUrl")]
        [HttpGet]
        public async Task<string> Id4ConfigUrl()
        {
            return await _baseClients.OnGet(id4ConfigUrl);
        }

        [AllowAnonymous]
        [Route("getToken")]
        [HttpGet]
        public async Task<string> GetToken()
        {
            var client = new HttpClient();

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = getToken,
                ClientId = "client",
                ClientSecret = "secret",

                Scope = "api1"
            });

            if (tokenResponse.IsError)
            {
                return tokenResponse.Error;
            }
            else
            {
                return tokenResponse.Json.ToString();
            }
        }

        [AllowAnonymous]
        [Route("getApi")]
        [HttpGet]
        public async Task<string> GetApi(string accessToken)
        {
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(accessToken);

            var response = await apiClient.GetAsync(identity);
            if (!response.IsSuccessStatusCode)
            {
                return response.StatusCode.ToString();
            }
            else
            {
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
