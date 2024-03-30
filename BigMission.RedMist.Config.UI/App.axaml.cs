﻿using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using BigMission.Avalonia.LogViewer.Extensions;
using BigMission.RedMist.Config.UI.Shared.CanBus;
using BigMission.RedMist.Config.UI.Shared.Channels;
using BigMission.RedMist.Config.UI.Shared.General;
using BigMission.RedMist.Config.UI.ViewModels;
using BigMission.RedMist.Config.UI.Views;
using CommunityToolkit.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MsLogger.Core;
using System.Threading;

namespace BigMission.RedMist.Config.UI;

public partial class App : Application
{
    private IHost? _host;
    private CancellationTokenSource? _cancellationTokenSource;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        HostApplicationBuilder builder = Host.CreateApplicationBuilder();
        builder.AddLogViewer().Logging.AddDefaultDataStoreLogger();

        var services = builder.Services;
        //services.AddHostedService<UdpService>();
        //services.AddHostedService<DataCloudForwarder>();
        //ConfigureServices(services);
        ConfigureViewModels(services);
        ConfigureViews(services);

        _host = builder.Build();
        _cancellationTokenSource = new();

        // Dependency injection: https://github.com/stevemonaco/AvaloniaViewModelFirstDemos
        // NuGet source: https://pkgs.dev.azure.com/dotnet/CommunityToolkit/_packaging/CommunityToolkit-Labs/nuget/v3/index.json
        var locator = new ViewLocator(_host.Services);
        DataTemplates.Add(locator);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
            var vm = _host.Services.GetService<MainViewModel>();
            var view = new MainView { DataContext = vm };
            desktop.MainWindow.Content = view;
            desktop.ShutdownRequested += OnShutdownRequested;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            //singleViewPlatform.MainView = new MainView
            //{
            //    DataContext = new MainViewModel()
            //};
        }

        // Startup background services
        _ = _host.StartAsync(_cancellationTokenSource.Token);

        base.OnFrameworkInitializationCompleted();
    }

    private void OnShutdownRequested(object? sender, ShutdownRequestedEventArgs e)
        => _ = _host!.StopAsync(_cancellationTokenSource!.Token);

    //[Singleton(typeof(WindowsSettingsProvider), typeof(ISettingsProvider))]
    //[Singleton(typeof(ChannelMappingProvider), typeof(IChannelMappingProvider))]
    //[Singleton(typeof(CloudComm), typeof(ICloudComm))]
    //internal static partial void ConfigureServices(IServiceCollection services);

    [Singleton(typeof(MainViewModel))]
    [Singleton(typeof(CanBusViewModel))]
    [Singleton(typeof(ChannelsViewModel))]
    [Singleton(typeof(GeneralViewModel))]
    //[Singleton(typeof(QuarterViewModelFactory), typeof(IQuarterViewModelFactory))]
    internal static partial void ConfigureViewModels(IServiceCollection services);

    [Singleton(typeof(MainView))]
    [Singleton(typeof(CanBusView))]
    [Singleton(typeof(ChannelsView))]
    [Singleton(typeof(GeneralView))]
    internal static partial void ConfigureViews(IServiceCollection services);
}
