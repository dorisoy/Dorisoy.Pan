using Dorisoy.PanClient.ViewModels;

namespace Dorisoy.PanClient.Pages.Chat;

public partial class Talk : ReactiveUserControl<MainViewViewModel>
{
    //private ScrollViewer _scrollViewer;
    //private TextBox _txtLimitedInput;
    //private ComboBox  _comboBox;

    public Talk()
    {
        this.InitializeComponent();

        //_scrollViewer = this.FindControl<ScrollViewer>("chatScrollViewer");
        //_txtLimitedInput = this.FindControl<TextBox>("txtLimitedInput");
        //_comboBox = this.FindControl<ComboBox>("CameraDevicesComboBox");

        this.WhenActivated(disposable =>
        {
            Locator.CurrentMutable.RegisterLazySingleton(() => TopLevel.GetTopLevel(this).StorageProvider);
            if (ViewModel != null)
            {
                ViewModel.InsertTextAtCaret += ViewModel_InsertTextAtCaret;

                this.OneWayBind(ViewModel, vm => vm.ChatData, v => v.ItemsControl.ItemsSource)
                .DisposeWith(disposable);

                this.OneWayBind(ViewModel, vm => vm.CaretIndex, v => v.txtLimitedInput.CaretIndex)
                .DisposeWith(disposable);

                //this.OneWayBind(ViewModel, vm => vm.CameraDevices, v => v.CameraDevicesComboBox.ItemsSource)
                //.DisposeWith(disposable);

                //滚动到结束聊天窗口
                ViewModel.SrollToEndChatWindow += () =>
                {
                    chatScrollViewer.ScrollToEnd();
                };
            }
        });
    }


    //private void MyImageButton_Click(object sender, RoutedEventArgs args)
    //{
    //    ShowMenu(true);
    //}

    //private void ShowMenu(bool isTransient)
    //{
    //    var flyout = Resources["CommandBarFlyoutEmoji"] as CommandBarFlyout;
    //    flyout.ShowMode = isTransient ? FlyoutShowMode.Transient : FlyoutShowMode.Standard;
    //    flyout.ShowAt(this.FindControl<CommandBarButton>("EmojiButton"));
    //}


    /// <summary>
    ///  在当前光标位置插入一个字符
    /// </summary>
    /// <param name="input"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void ViewModel_InsertTextAtCaret(string input)
    {
        InsertTextAtCaret(input);
    }

    /// <summary>
    /// 在当前光标位置插入一个字符
    /// </summary>
    /// <param name="insertText"></param>
    public void InsertTextAtCaret(string insertText)
    {
        // 获取光标的当前位置
        var caretIndex = txtLimitedInput.CaretIndex;

        // 获取文本框现有的文本
        var text = txtLimitedInput.Text ?? string.Empty;

        // 将文本分割成两部分，并在中间插入希望添加的文本
        var newText = text.Substring(0, caretIndex) + insertText + text.Substring(caretIndex);

        // 设置文本框的新内容
        txtLimitedInput.Text = newText;

        // 更新光标的位置，使其位于插入的文本之后
        txtLimitedInput.CaretIndex = caretIndex + insertText.Length;
    }

    /// <summary>
    /// 浏览器打开（Windows）
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void HyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        var input = sender as HyperlinkButton;
        string uriStr = input.Content as string;
        if (uriStr.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || uriStr.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            var sInfo = new ProcessStartInfo(uriStr)
            {
                UseShellExecute = true,
            };

            try
            {
                Process.Start(sInfo);
            }
            catch { }
        }
        else
        {
            string argument = "/select, \"" + uriStr + "\"";
            try
            {
                Process.Start("explorer.exe", argument);
            }
            catch { }
        }
    }

    /// <summary>
    /// 回车发送消息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>

    private void txtLimitedInput_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            ((MainViewViewModel)this.DataContext).MessageSendCommand.Execute().Subscribe();
        }
    }
}
