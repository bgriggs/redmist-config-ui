using Avalonia.Controls;
using Avalonia.Controls.Templates;
using BigMission.RedMist.Config.UI.Shared.CanBus;
using BigMission.RedMist.Config.UI.Shared.Channels;
using BigMission.RedMist.Config.UI.Shared.General;
using BigMission.RedMist.Config.UI.Shared.Logging;
using BigMission.RedMist.Config.UI.ViewModels;
using BigMission.RedMist.Config.UI.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace BigMission.RedMist.Config.UI;

// https://github.com/stevemonaco/AvaloniaViewModelFirstDemos
// NuGet source: https://pkgs.dev.azure.com/dotnet/CommunityToolkit/_packaging/CommunityToolkit-Labs/nuget/v3/index.json
/// <summary>
/// Associates a view with a view model.
/// </summary>
public class ViewLocator : IDataTemplate
{
    private readonly Dictionary<Type, Func<Control?>> _locator = [];
    private readonly IServiceProvider serviceProvider;

    public ViewLocator(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
        RegisterViewFactory<MainViewModel, MainView>();
        RegisterViewFactory<ChannelMappingEditViewModel, ChannelMappingEditDialog>();
        RegisterViewFactory<GeneralViewModel, GeneralView>();
        RegisterViewFactory<CanMessageDialogViewModel, CanMessageDialog>();
        RegisterViewFactory<CanChannelSelectionDialogViewModel, CanChannelSelectionDialog>();
        RegisterViewFactory<ChannelSelectionControlViewModel, ChannelSelectionControl>();
        RegisterViewFactory<ChannelSelectionDialogViewModel, ChannelSelectionDialog>();
        RegisterViewFactory<ChannelMappingDetailsViewModel, ChannelDetailsDialog>();
        RegisterViewFactory<LoggingViewModel, LoggingView>();
    }

    public Control Build(object? data)
    {
        if (data is null)
            return new TextBlock { Text = $"No VM provided" };

        _locator.TryGetValue(data.GetType(), out var factory);
        return factory?.Invoke() ?? new TextBlock { Text = $"VM or View Not Registered: {data.GetType()}" };
    }

    public bool Match(object? data)
    {
        return data is ObservableObject;
    }

    public void RegisterViewFactory<TViewModel>(Func<Control> factory) where TViewModel : class => _locator.Add(typeof(TViewModel), factory);

    public void RegisterViewFactory<TViewModel, TView>()
        where TViewModel : ObservableObject
        where TView : Control
        //=> _locator.Add(typeof(TViewModel), Ioc.Default.GetService<TView>);
    {
        _locator.Add(typeof(TViewModel), serviceProvider.GetService<TView>);
    }
}
