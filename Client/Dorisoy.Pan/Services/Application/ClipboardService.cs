﻿namespace Dorisoy.PanClient.Services;

public static class ClipboardService
{
    public static TopLevel Owner { get; set; }
    public static Task SetTextAsync(string text) => Owner.Clipboard.SetTextAsync(text);
}
