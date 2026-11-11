using System.Globalization;
using ProgresDemo.Models;
using ProgresDemo.Services;

namespace ProgresDemo.Pages;

public partial class CurrencyConverterPage : ContentPage
{
    private readonly IRateService _rateService;
    private List<Rate> _rates = new();

    private readonly string[] _currencyCodes =
    {
        "RUB", "EUR", "USD", "CHF", "CNY", "GBP"
    };

    public CurrencyConverterPage(IRateService rateService)
    {
        InitializeComponent();

        _rateService = rateService;
        RateDatePicker.Date = DateTime.Today;

        CurrencyPicker.SelectedIndex = 0;
        DirectionPicker.SelectedIndex = 0;
    }

    // Получение курсов валют
    private async void LoadRatesButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            LoadRatesButton.IsEnabled = false;
            StatusLabel.Text = "Загрузка курсов...";

            DateTime date = RateDatePicker.Date;

            _rates = await Task.Run(() =>
                _rateService.GetRates(date).ToList());

            string[] currencyNames =
            {
                "Российский рубль",
                "Евро",
                "Доллар США",
                "Швейцарский франк",
                "Китайский юань",
                "Фунт стерлингов"
            };

            var result = new System.Text.StringBuilder();

            result.AppendLine($"Дата курса: {date:dd.MM.yyyy}");
            result.AppendLine();

            foreach (string code in _currencyCodes)
            {
                Rate? rate = _rates.FirstOrDefault(
                    r => r.Cur_Abbreviation == code);

                if (rate != null)
                {
                    int index = Array.IndexOf(_currencyCodes, code);

                    result.AppendLine(
                        $"{currencyNames[index]} ({code})");

                    result.AppendLine(
                        $"{rate.Cur_Scale} ед. = " +
                        $"{rate.Cur_OfficialRate:F4} BYN");

                    result.AppendLine();
                }
            }

            RatesLabel.Text = result.ToString();

            StatusLabel.Text = _rates.Count == 6
                ? "Курсы успешно загружены."
                : $"Получено курсов: {_rates.Count} из 6.";

        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Ошибка",
                "Не удалось загрузить курсы валют.\n" + ex.Message,
                "OK");

            StatusLabel.Text = "Ошибка загрузки курсов.";
        }
        finally
        {
            LoadRatesButton.IsEnabled = true;
        }
    }

    // Изменение выбранной валюты
    private void CurrencyPicker_SelectedIndexChanged(
        object sender, EventArgs e)
    {
        ResultLabel.Text = "Здесь появится результат";
    }

    // Выполнение конвертации
    private async void ConvertButton_Clicked(object sender, EventArgs e)
    {
        if (_rates.Count == 0)
        {
            await DisplayAlert(
                "Внимание",
                "Сначала загрузите курсы валют.",
                "OK");
            return;
        }

        if (CurrencyPicker.SelectedIndex < 0 ||
            DirectionPicker.SelectedIndex < 0)
        {
            await DisplayAlert(
                "Внимание",
                "Выберите валюту и направление обмена.",
                "OK");
            return;
        }

        string input = AmountEntry.Text?.Trim() ?? "";

        if (!decimal.TryParse(
            input.Replace(',', '.'),
            NumberStyles.Number,
            CultureInfo.InvariantCulture,
            out decimal amount) || amount <= 0)
        {
            await DisplayAlert(
                "Ошибка",
                "Введите корректную положительную сумму.",
                "OK");
            return;
        }

        string code = _currencyCodes[CurrencyPicker.SelectedIndex];

        Rate? rate = _rates.FirstOrDefault(
            r => r.Cur_Abbreviation == code);

        if (rate == null || rate.Cur_Scale <= 0)
        {
            await DisplayAlert(
                "Ошибка",
                "Курс выбранной валюты не найден.",
                "OK");
            return;
        }

        // Цена одной единицы иностранной валюты в BYN
        decimal ratePerUnit = rate.Cur_OfficialRate / rate.Cur_Scale;

        decimal converted;

        if (DirectionPicker.SelectedIndex == 0)
        {
            // BYN -> иностранная валюта
            converted = amount / ratePerUnit;

            ResultLabel.Text =
                $"{amount:F2} BYN = {converted:F4} {code}";
        }
        else
        {
            // Иностранная валюта -> BYN
            converted = amount * ratePerUnit;

            ResultLabel.Text =
                $"{amount:F4} {code} = {converted:F4} BYN";
        }
    }
}
