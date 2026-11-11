
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProgresDemo;

public partial class MainPage : ContentPage
{
    private CancellationTokenSource? cts;

    private const double A = 0.0;
    private const double B = 1.0;
    private const double H = 0.00000001;

    private const int N = 100000000;
    private const int BatchSize = 1000000;

    public MainPage()
    {
        InitializeComponent();
    }

    // Запуск вычисления
    private async void StartButton_Clicked(
        object sender, EventArgs e)
    {
        cts = new CancellationTokenSource();

        CancellationToken token = cts.Token;

        StartButton.IsEnabled = false;
        CancelButton.IsEnabled = true;

        CalculationProgress.Progress = 0;
        PercentLabel.Text = "0%";
        MessageLabel.Text = "Вычисление";

        IProgress<double> progress =
             new Progress<double>(value =>
             {
                 MainThread.BeginInvokeOnMainThread(() =>
                 {
                     CalculationProgress.Progress = value;
                     PercentLabel.Text = $"{value * 100:F0}%";
                  });
              });

        try
        {
            double result = await Task.Run(
                async () =>
                {
                    double sum = 0.0;

                    for (int start = 0; start < N;
                         start += BatchSize)
                    {
                        token.ThrowIfCancellationRequested();

                        int end = Math.Min(
                            start + BatchSize, N);

                        // Вычисление части интеграла
                        for (int i = start; i < end; i++)
                        {
                            double x = A + (i + 0.5) * H;
                            sum += Math.Sin(x);
                        }

                        // Обновление прогресса
                        double percent = (double)end / N;
                        progress.Report(percent);

                        // Небольшая задержка для наглядности
                        await Task.Delay(100, token);
                    }

                    token.ThrowIfCancellationRequested();

                    return sum * H;
                },
                token);

            MessageLabel.Text =
                $"Результат: {result:F10}";

            CalculationProgress.Progress = 1;
            PercentLabel.Text = "100%";
        }
        catch (OperationCanceledException)
        {
            MessageLabel.Text = "Задание отменено";
        }
        finally
        {
            StartButton.IsEnabled = true;
            CancelButton.IsEnabled = false;

            cts?.Dispose();
            cts = null;
        }
    }

    // Отмена вычисления
    private void CancelButton_Clicked(
        object sender, EventArgs e)
    {
        cts?.Cancel();
    }
}

