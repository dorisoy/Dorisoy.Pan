using System;

namespace LocalizationManager.Avalonia.Extensions;
internal static class ExceptionExtensions
{
    public static void Throw(this Exception exception) => throw exception;

}
