using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;

namespace PCSpecs;

public partial class MainWindow : Window
{
    private readonly ObservableCollection<SpecItem> _specs = new();
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(1) };
    private bool _specsLoaded;
    private bool _busy;

    public MainWindow()
    {
        InitializeComponent();
        SpecsList.ItemsSource = _specs;
        _timer.Tick += Timer_Tick;
        Loaded += async (_, _) => await LoadSpecsAsync();
    }

    private async Task LoadSpecsAsync()
    {
        if (_specsLoaded) return;

        LoadingText.Visibility = Visibility.Visible;
        var items = await Task.Run(SystemInfoService.GetAllSpecs);

        _specs.Clear();
        foreach (var item in items) _specs.Add(item);

        LoadingText.Visibility = Visibility.Collapsed;
        _specsLoaded = true;
    }

    private async void SpecsButton_Click(object sender, RoutedEventArgs e)
    {
        _timer.Stop();
        PerfPage.Visibility = Visibility.Collapsed;
        SpecsPage.Visibility = Visibility.Visible;
        await LoadSpecsAsync();
    }

    private void PerfButton_Click(object sender, RoutedEventArgs e)
    {
        SpecsPage.Visibility = Visibility.Collapsed;
        PerfPage.Visibility = Visibility.Visible;
        _timer.Start();
    }

    private async void Timer_Tick(object? sender, EventArgs e)
    {
        if (_busy) return;
        _busy = true;

        try
        {
            var data = await Task.Run(() => (
                Cpu: SystemInfoService.GetCpuUsage(),
                Gpu: SystemInfoService.GetGpuUsage(),
                Ram: SystemInfoService.GetRamUsage(),
                Temps: SystemInfoService.GetTemperatures()));

            CpuBar.Value = data.Cpu;
            CpuText.Text = $"{data.Cpu:0} %";

            GpuBar.Value = data.Gpu;
            GpuText.Text = $"{data.Gpu:0} %";

            RamBar.Value = data.Ram.percent;
            RamText.Text = $"{data.Ram.percent:0} %  ({data.Ram.usedGb:0.0} / {data.Ram.totalGb:0.0} GB)";

            TempText.Text = data.Temps;
        }
        catch { }
        finally
        {
            _busy = false;
        }
    }
}
