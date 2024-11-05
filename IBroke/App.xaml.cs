using IBroke.Repository;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Windows;

namespace IBroke;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private ServiceProvider _serviceProvider;

    public App()
    {
        var services = new ServiceCollection();
        services.AddDbContext<EntryContext>();
        services.AddSingleton<EntryRepository>();
        services.AddSingleton<MainWindow>();
        services.AddTransient<MainViewModel>();

        _serviceProvider = services.BuildServiceProvider();

        Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-DE");
        Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator = ".";
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var mainWindow = _serviceProvider.GetService<MainWindow>();
        if (mainWindow != null)
            mainWindow.Show();
    }
}
