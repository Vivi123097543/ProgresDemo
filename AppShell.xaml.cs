using Microsoft.Extensions.DependencyInjection;
using ProgresDemo.Pages;

namespace ProgresDemo;

public partial class AppShell : Shell
{
    public AppShell(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        // Создаём страницу через DI, чтобы передать IRateService
        CurrencyConverterShellContent.ContentTemplate =
            new DataTemplate(() =>
                serviceProvider.GetRequiredService<CurrencyConverterPage>());
    }
}
