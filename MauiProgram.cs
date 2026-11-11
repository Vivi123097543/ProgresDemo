using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;
using ProgresDemo.Services;
using ProgresDemo.Pages;

namespace ProgresDemo;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>();

        // Регистрация сервиса получения курсов валют
        builder.Services.AddHttpClient<IRateService, RateService>(client =>
        {
            client.BaseAddress = new Uri(
                "https://www.nbrb.by/api/exrates/rates/");
        });
        // Регистрация страницы конвертера
        builder.Services.AddTransient<CurrencyConverterPage>();
#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
