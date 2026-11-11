
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using ProgresDemo.Models;

namespace ProgresDemo.Services;

public class RateService : IRateService
{
    private readonly HttpClient _httpClient;

    private readonly string[] _currencies =
    {
        "RUB", "EUR", "USD", "CHF", "CNY", "GBP"
    };

    public RateService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public IEnumerable<Rate> GetRates(DateTime date)
    {
        string request = $"?ondate={date:yyyy-MM-dd}&periodicity=0";

        HttpResponseMessage response = _httpClient
            .GetAsync(request)
            .GetAwaiter()
            .GetResult();

        response.EnsureSuccessStatusCode();

        string json = response.Content
            .ReadAsStringAsync()
            .GetAwaiter()
            .GetResult();

        List<Rate> rates = JsonSerializer.Deserialize<List<Rate>>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<Rate>();

        return rates
            .Where(rate => _currencies.Contains(rate.Cur_Abbreviation))
            .ToList();
    }
}
