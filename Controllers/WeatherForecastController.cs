using System.Text.Json;
using chatGptApi.ChatGpt;
using chatGptApi.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace chatGptApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly HttpClient _httpCLient;
    private readonly ChatGptOptions _chatGptOptions;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, HttpClient httpClient, IOptions<ChatGptOptions> chatGptOptions)
    {
        _logger = logger;
        _httpCLient = httpClient;
        _chatGptOptions = chatGptOptions.Value;
    }
    [HttpGet]
    public async Task<IActionResult> Get(string text)
    {
        _httpCLient.DefaultRequestHeaders.Authorization = 
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _chatGptOptions.ApiKey);

        var model = new ParametersModel(text);

        var requestBody = JsonSerializer.Serialize(model);

        var content = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");

        var response = await _httpCLient.PostAsync("https://api.openai.com/v1/completions", content);

        var result = await response.Content.ReadFromJsonAsync<ResponseModel>();

        return Ok(result.Choices[0].Text.Replace("\n", string.Empty).Replace("\t", string.Empty));
    }

}
