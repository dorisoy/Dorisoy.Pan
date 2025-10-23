﻿using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;
using FluentAvalonia.UI.Controls.Primitives;

namespace Dorisoy.Pan.Services;

public class DialogHelper
{
    /// <summary>
    /// 显示无法打开的链接对话框
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    public static async Task ShowUnableToOpenLinkDialog(Uri uri)
    {
        var copyLinkButton = new TaskDialogCommand
        {
            Text = "Copy Link",
            IconSource = new SymbolIconSource { Symbol = Symbol.Link },
            Description = uri.ToString(),
            ClosesOnInvoked = false
        };

        var td = new TaskDialog
        {
            Content = "It looks like your platform doesn't support Process.Start " +
            "and we are unable to open a link.",
            SubHeader = "Oh No!",
            Commands =
            {
                copyLinkButton
            },
            Buttons =
            {
                TaskDialogButton.OKButton
            },
            IconSource = new SymbolIconSource { Symbol = Symbol.ImportantFilled }
        };

        copyLinkButton.Click += async (s, __) =>
        {
            await ClipboardService.SetTextAsync(uri.ToString());

            var flyout = new Flyout
            {
                Content = "Copied!"
            };

            var comHost = td.FindDescendantOfType<TaskDialogCommandHost>();

            FlyoutBase.SetAttachedFlyout(comHost, flyout);
            FlyoutBase.ShowAttachedFlyout(comHost);

            DispatcherTimer.RunOnce(() => flyout.Hide(), TimeSpan.FromSeconds(1));
        };

        var app = Application.Current.ApplicationLifetime;
        if (app is IClassicDesktopStyleApplicationLifetime desktop)
        {
            td.XamlRoot = desktop.MainWindow;
        }
        else if (app is ISingleViewApplicationLifetime single)
        {
            td.XamlRoot = TopLevel.GetTopLevel(single.MainView);
        }

        await td.ShowAsync(true);
    }
}
