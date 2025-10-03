using System;
namespace LocalizationManager;
public interface ILocalizationChanged : INotifyPropertyChanged
{
    CultureInfo DefaultCulture { get; }
    CultureInfo CurrentCulture { get; set; }

    //event PropertyChangedEventHandler? StrongPropertyChanged;

    event EventHandler<EventArgs>? LanguageChanged;
}
