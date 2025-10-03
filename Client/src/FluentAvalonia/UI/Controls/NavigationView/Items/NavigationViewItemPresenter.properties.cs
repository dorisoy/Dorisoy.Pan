﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.VisualTree;
using FluentAvalonia.Core;

namespace FluentAvalonia.UI.Controls.Primitives;

[PseudoClasses(s_pcExpanded)]
[PseudoClasses(s_pcClosedCompactTop, s_pcNotClosedCompactTop)]
[PseudoClasses(SharedPseudoclasses.s_pcLeftNav, SharedPseudoclasses.s_pcTopNav, SharedPseudoclasses.s_pcTopOverflow)]
[PseudoClasses(SharedPseudoclasses.s_pcChevronOpen, SharedPseudoclasses.s_pcChevronClosed, SharedPseudoclasses.s_pcChevronHidden)]
[PseudoClasses(SharedPseudoclasses.s_pcIconLeft, SharedPseudoclasses.s_pcIconOnly, SharedPseudoclasses.s_pcContentOnly)]
[PseudoClasses(SharedPseudoclasses.s_pcPressed)]
public partial class NavigationViewItemPresenter
{
    /// <summary>
    /// Defines the <see cref="Icon"/> property
    /// </summary>
    public static readonly StyledProperty<IconSource> IconSourceProperty =
        SettingsExpander.IconSourceProperty.AddOwner<NavigationViewItemPresenter>();

    /// <summary>
    /// Defines the <see cref="TemplateSettings"/> property
    /// </summary>
    public static readonly StyledProperty<NavigationViewItemPresenterTemplateSettings> TemplateSettingsProperty =
        AvaloniaProperty.Register<NavigationViewItemPresenter, NavigationViewItemPresenterTemplateSettings>(nameof(TemplateSettings));

    /// <summary>
    /// Defines the <see cref="InfoBadge"/> property
    /// </summary>
    public static readonly StyledProperty<InfoBadge> InfoBadgeProperty =
        NavigationViewItem.InfoBadgeProperty.AddOwner<NavigationViewItemPresenter>();

    /// <summary>
    /// Gets or sets the icon in a NavigationView item.
    /// </summary>
    public IconSource IconSource
    {
        get => GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    /// <summary>
    /// Gets the template settings used in the NavigationViewItemPresenter
    /// </summary>
    public NavigationViewItemPresenterTemplateSettings TemplateSettings
    {
        get => GetValue(TemplateSettingsProperty);
        internal set => SetValue(TemplateSettingsProperty, value);
    }

    /// <summary>
    /// Gets or sets the InfoBadge used in the NavigationViewItemPresenter
    /// </summary>
    public InfoBadge InfoBadge
    {
        get => GetValue(InfoBadgeProperty);
        set => SetValue(InfoBadgeProperty, value);
    }

    internal NavigationViewItem GetNVI
    {
        get
        {
            return this.FindAncestorOfType<NavigationViewItem>();
        }
    }

    internal Control SelectionIndicator => _selectionIndicator;

    private const string s_tpSelectionIndicator = "SelectionIndicator";
    private const string s_tpPresenterContentRootGrid = "PresenterContentRootGrid";
    private const string s_tpInfoBadgePresenter = "InfoBadgePresenter";
    private const string s_tpExpandCollapseChevron = "ExpandCollapseChevron";

    private const string s_pcClosedCompactTop = ":closedcompacttop";
    private const string s_pcNotClosedCompactTop = ":notclosedcompacttop";
    private const string s_pcExpanded = ":expanded";
}
