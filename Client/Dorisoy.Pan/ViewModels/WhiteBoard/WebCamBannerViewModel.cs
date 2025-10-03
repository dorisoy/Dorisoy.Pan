namespace Dorisoy.PanClient.ViewModels;

//[View(typeof(WebCamBannerView))]
public class WebCamBannerViewModel : ViewModelBase
{
    [Reactive] public Avalonia.Media.Imaging.Bitmap VideoFrame { get; set; }
    [Reactive] public string UserName { get; set; }
    [Reactive] public string IPAddress { get; set; }
    [Reactive] public Guid UserId { get; set; }


    public WebCamBannerViewModel(Guid uid, string name, string ip) : base()
    {
        UserId = uid;
        IPAddress = ip;
        UserName = name;


        this.WhenActivated((CompositeDisposable disposables) =>
        {
            //RxApp.MainThreadScheduler.Schedule(StartCamera).DisposeWith(disposables);
        });
    }

}
