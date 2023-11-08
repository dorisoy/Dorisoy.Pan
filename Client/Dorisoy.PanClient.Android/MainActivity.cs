using Android.App;
using Android.Content.PM;
using Avalonia.Android;

namespace Dorisoy.PanClient.Android;

[Activity(Label = "Dorisoy.PanClient.Android", Theme = "@style/MyTheme.NoActionBar", 
    Icon = "@drawable/icon", 
    LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
public class MainActivity : AvaloniaMainActivity
{
}
