using IBroke.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
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

        _serviceProvider = services.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var mainWindow = _serviceProvider.GetService<MainWindow>();        
        if (mainWindow != null)
            mainWindow.Show();
    }
}
