﻿using Avalonia;

namespace FluentAvalonia.UI.Controls.Primitives;

/// <summary>
/// Provides settings used in the template of a <see cref="NavigationViewItemPresenter"/>
/// </summary>
public class NavigationViewItemPresenterTemplateSettings : AvaloniaObject
{
    internal NavigationViewItemPresenterTemplateSettings() { }

    /// <summary>
    /// Defines the <see cref="IconWidth"/> property
    /// </summary>
    public static readonly StyledProperty<double> IconWidthProperty =
        AvaloniaProperty.Register<NavigationViewItemPresenterTemplateSettings, double>(nameof(IconWidth));

    /// <summary>
    /// Defines the <see cref="SmallerIconWidth"/> property
    /// </summary>
    public static readonly StyledProperty<double> SmallerIconWidthProperty =
        AvaloniaProperty.Register<NavigationViewItemPresenterTemplateSettings, double>(nameof(SmallerIconWidth));

    /// <summary>
    /// Defines the <see cref="Icon"/> property
    /// </summary>
    public static readonly StyledProperty<FAIconElement> IconProperty =
        MenuFlyoutItemTemplateSettings.IconProperty.AddOwner<NavigationViewItemPresenterTemplateSettings>();

    /// <summary>
    /// TODO: Get docs from MS - relatively new setting
    /// </summary>
    public double IconWidth
    {
        get => GetValue(IconWidthProperty);
        internal set => SetValue(IconWidthProperty, value);
    }

    /// <summary>
    /// TODO: Get docs from MS - relatively new setting
    /// </summary>
    public double SmallerIconWidth
    {
        get => GetValue(SmallerIconWidthProperty);
        internal set => SetValue(SmallerIconWidthProperty, value);
    }

    /// <summary>
    /// Gets the <see cref="FAIconElement"/> used in the NavigationViewItem
    /// </summary>
    public FAIconElement Icon
    {
        get => GetValue(IconProperty);
        internal set => SetValue(IconProperty, value);
    }
}
