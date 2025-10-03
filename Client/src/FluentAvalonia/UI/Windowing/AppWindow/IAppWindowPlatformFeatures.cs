﻿using Avalonia.Media;

namespace FluentAvalonia.UI.Windowing;

/// <summary>
/// Represents constants that define the TaskBarProgressBar's state
/// </summary>
public enum TaskBarProgressBarState
{
    /// <summary>
    /// No TaskBarProgressBar is displayed
    /// </summary>
    None = 0x00000000,

    /// <summary>
    /// A green indicator is displayed in the taskbar button
    /// </summary>
    Normal = 0x00000002,

    /// <summary>
    /// A yellow progress indicator is displayed in the taskbar button.
    /// </summary>
    Paused = 0x00000008,

    /// <summary>
    /// A red progress indicator is displayed in the taskbar button.
    /// </summary>
    Error = 0x00000004,

    /// <summary>
    /// A pulsing green indicator is displayed in the taskbar button.
    /// </summary>
    Indeterminate = 0x00000001
}

/// <summary>
/// Provides a set of function that enable platform-specific functionality through AppWindow
/// These function are only available on Windows at the current time
/// </summary>
public interface IAppWindowPlatformFeatures
{
    /// <summary>
    /// Windows11 only, sets the border color of the current window to the specified color
    /// </summary>
    void SetWindowBorderColor(Color color);

    /// <summary>
    /// Activate the taskbar progressbar indicator with the given state
    /// </summary>
    void SetTaskBarProgressBarState(TaskBarProgressBarState state);

    /// <summary>
    /// Activate the taskbar progressbar indicator with the given values
    /// </summary>
    void SetTaskBarProgressBarValue(ulong currentValue, ulong totalValue);
}
