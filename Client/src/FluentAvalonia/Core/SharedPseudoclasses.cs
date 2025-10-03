﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FluentAvalonia.Core;

// These are pseudoclass names that are used in multiple controls. Share them so we don't have
// a bunch of duplicate strings everywhere
internal class SharedPseudoclasses
{
    public const string s_pcOpen = ":open";
    public const string s_pcCompact = ":compact";
    public const string s_pcIcon = ":icon";
    public const string s_pcLabel = ":label";
    public const string s_pcFlyout = ":flyout";
    public const string s_pcHotkey = ":hotkey";
    public const string s_pcOverflow = ":overflow";
    public const string s_pcHidden = ":hidden";
    public const string s_pcHeader = ":header";
    public const string s_pcFooter = ":footer";
    public const string s_pcPressed = ":pressed";
    public const string s_pcChecked = ":checked";

    // SettingsExpander specific
    public const string s_pcAllowClick = ":allowClick";

    // TabViewSpecific
    public const string s_pcNoBorder = ":noborder";
    public const string s_pcBorderLeft = ":borderLeft";
    public const string s_pcBorderRight = ":borderRight";

    // NavigationViewItem / NavigationViewItemPresenter
    public const string s_pcIconLeft = ":iconleft";
    public const string s_pcIconOnly = ":icononly";
    public const string s_pcContentOnly = ":contentonly";
    public const string s_pcLeftNav = ":leftnav";
    public const string s_pcTopNav = ":topnav";
    public const string s_pcTopOverflow = ":topoverflow";
    public const string s_pcChevronOpen = ":chevronopen";
    public const string s_pcChevronClosed = ":chevronclosed";
    public const string s_pcChevronHidden = ":chevronhidden";

    public const string s_cAccent = "accent";
}
