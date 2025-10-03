using Dorisoy.PanClient.ViewModels;

namespace Dorisoy.PanClient.Pages;

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
    ///// 正在导航到的页面
    ///// </summary>
    ///// <param name="sender"></param>
    ///// <param name="e"></param>
    //private void OnNavigatedTo(object sender, NavigationEventArgs e)
    //{
    //    System.Diagnostics.Debug.Print($"到达:------>-------WebCamBannerView");
    //    var vm = ViewModel;
    //    vm?.StartCamera();
    //}

    ///// <summary>
    ///// 指示要离开的页面
    ///// </summary>
    ///// <param name="sender"></param>
    ///// <param name="e"></param>
    //private void OnNavigatingFrom(object sender, NavigatingCancelEventArgs e)
    //{
    //    System.Diagnostics.Debug.Print($"离开:------>-------WebCamBannerView");
    //    var vm = ViewModel;
    //    vm?.StopCamera();
    //}

}
