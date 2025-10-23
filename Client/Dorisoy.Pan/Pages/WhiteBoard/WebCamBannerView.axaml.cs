using Dorisoy.Pan.ViewModels;

namespace Dorisoy.Pan.Pages;

public partial class WebCamBannerView : ReactiveUserControl<WebCamBannerViewModel>
{
    public WebCamBannerView()
    {
        this.InitializeComponent();

        this.WhenActivated(disposables =>
        {
        });
    }

    ///// <summary>
    ///// ���ڵ�������ҳ��
    ///// </summary>
    ///// <param name="sender"></param>
    ///// <param name="e"></param>
    //private void OnNavigatedTo(object sender, NavigationEventArgs e)
    //{
    //    System.Diagnostics.Debug.Print($"����:------>-------WebCamBannerView");
    //    var vm = ViewModel;
    //    vm?.StartCamera();
    //}

    ///// <summary>
    ///// ָʾҪ�뿪��ҳ��
    ///// </summary>
    ///// <param name="sender"></param>
    ///// <param name="e"></param>
    //private void OnNavigatingFrom(object sender, NavigatingCancelEventArgs e)
    //{
    //    System.Diagnostics.Debug.Print($"�뿪:------>-------WebCamBannerView");
    //    var vm = ViewModel;
    //    vm?.StopCamera();
    //}

}
