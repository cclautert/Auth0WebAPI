using Auth0.AspNetCore.Authentication;
using Auth0WebAPI.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using RestSharp;
using Microsoft.Extensions.Options;

namespace Auth0WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly Auth0Configuration _auth0Config;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IOptions<Auth0Configuration> auth0Config)
        {
            _logger = logger;
            _auth0Config = auth0Config.Value;
        }

        [HttpGet("login")]
        public async Task<string> Login()
        {
            var client = new RestClient(_auth0Config.Domain);
            var request = new RestRequest("oauth/token", Method.Post);

            // Body as object → RestSharp serializes to JSON
            request.AddJsonBody(new
            {
                client_id = _auth0Config.ClientId,
                client_secret = _auth0Config.ClientSecret,
                audience = _auth0Config.Audience,
                grant_type = _auth0Config.grant_type
            });

            var response = await client.ExecuteAsync<Auth0TokenResponse>(request);

            if (response.IsSuccessful)
            {
                Console.WriteLine($"Token: {response.Data.access_token}");
                return response.Data.access_token;
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode} - {response.Content}");
            }

            return null;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        [Authorize] // This endpoint is autenticação required
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
