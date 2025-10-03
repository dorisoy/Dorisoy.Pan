﻿using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using FluentAvalonia.Core;

namespace FluentAvalonia.UI.Controls;

[PseudoClasses(SharedPseudoclasses.s_pcAllowClick, s_pcEmpty)]
[TemplatePart(s_tpExpander, typeof(Expander))]
[TemplatePart(s_tpContentHost, typeof(SettingsExpanderItem))]
public partial class SettingsExpander
{
    /// <summary>
    /// Defines the <see cref="Description"/> property
    /// </summary>
    public static readonly StyledProperty<string> DescriptionProperty = 
        AvaloniaProperty.Register<SettingsExpander, string>(nameof(Description));

    /// <summary>
    /// Defines the <see cref="IconSource"/> property
    /// </summary>
    public static readonly StyledProperty<IconSource> IconSourceProperty = 
        AvaloniaProperty.Register<SettingsExpander, IconSource>(nameof(IconSource));

    /// <summary>
    /// Defines the <see cref="Footer"/> property
    /// </summary>
    public static readonly StyledProperty<object> FooterProperty = 
        AvaloniaProperty.Register<SettingsExpander, object>(nameof(Footer));

    /// <summary>
    /// Defines the <see cref="FooterTemplate"/> property
    /// </summary>
    public static readonly StyledProperty<IDataTemplate> FooterTemplateProperty = 
        AvaloniaProperty.Register<SettingsExpander, IDataTemplate>(nameof(FooterTemplate));

    /// <summary>
    /// Defines the <see cref="IsExpanded"/> property
    /// </summary>
    public static readonly StyledProperty<bool> IsExpandedProperty =
        Expander.IsExpandedProperty.AddOwner<SettingsExpander>();

    /// <summary>
    /// Defines the <see cref="ActionIconSource"/> property
    /// </summary>
    public static readonly StyledProperty<IconSource> ActionIconSourceProperty = 
        AvaloniaProperty.Register<SettingsExpander, IconSource>(nameof(ActionIconSource));

    /// <summary>
    /// Defines the <see cref="IsClickEnabled"/> property
    /// </summary>
    public static readonly StyledProperty<bool> IsClickEnabledProperty = 
        AvaloniaProperty.Register<SettingsExpander, bool>(nameof(IsClickEnabled));

    /// <summary>
    /// Defines the <see cref="Command"/> property
    /// </summary>
    public static readonly StyledProperty<ICommand> CommandProperty = 
        Button.CommandProperty.AddOwner<SettingsExpander>();

    /// <summary>
    /// Defines the <see cref="CommandParameter"/> property
    /// </summary>
    public static readonly StyledProperty<object> CommandParameterProperty = 
        Button.CommandParameterProperty.AddOwner<SettingsExpander>();

    // NOTE: Don't use Button.Click event here - when SettingsExpanderItem is in the top-level SettingsExpander
    // there is a ToggleButton that is used to raise this event. If we use Button.Click here, and someone is 
    // listening to Button.Click event with handledEventsToo = true, they'll get 2 click events as a result
    /// <summary>
    /// Defines the <see cref="Click"/> event
    /// </summary>
    public static readonly RoutedEvent<RoutedEventArgs> ClickEvent =
        RoutedEvent.Register<SettingsExpander, RoutedEventArgs>(nameof(Click), RoutingStrategies.Tunnel | RoutingStrategies.Bubble);

    /// <summary>
    /// Gets or sets the description text
    /// </summary>
    public string Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    /// <summary>
    /// Gets or sets the IconSource for the SettingsExpander
    /// </summary>
    public IconSource IconSource
    {
        get => GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the Footer content for the SettingsExpander
    /// </summary>
    public object Footer
    {
        get => GetValue(FooterProperty);
        set => SetValue(FooterProperty, value);
    }

    /// <summary>
    /// Gets or sets the Footer template for the SettingsExpander
    /// </summary>
    public IDataTemplate FooterTemplate
    {
        get => GetValue(FooterTemplateProperty);
        set => SetValue(FooterTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the SettingsExpander is currently expanded
    /// </summary>
    public bool IsExpanded
    {
        get => GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    /// <summary>
    /// Gets or sets the Action IconSource when <see cref="IsClickEnabled"/> is true
    /// </summary>
    public IconSource ActionIconSource
    {
        get => GetValue(ActionIconSourceProperty);
        set => SetValue(ActionIconSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the item is clickable which can be used for navigation within an app
    /// </summary>
    /// <remarks>
    /// This property can only be set if no items are added to the SettingsExpander. Attempting to mark
    /// a settings expander clickable and adding child items will throw an exception
    /// </remarks>
    public bool IsClickEnabled
    {
        get => GetValue(IsClickEnabledProperty);
        set => SetValue(IsClickEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets the Command that is invoked upon clicking the item
    /// </summary>
    public ICommand Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the command parameter
    /// </summary>
    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    protected override bool IsEnabledCore => base.IsEnabledCore && _commandCanExecute;

    /// <summary>
    /// Event raised when the SettingsExpander is clicked and IsClickEnabled = true
    /// </summary>
    public event EventHandler<RoutedEventArgs> Click
    {
        add => AddHandler(ClickEvent, value);
        remove => RemoveHandler(ClickEvent, value);
    }

    private const string s_tpExpander = "Expander";
    private const string s_tpContentHost = "ContentHost";

    private const string s_pcEmpty = ":empty";
    private const string s_pcIconPlaceholder = ":iconPlaceholder";
}
