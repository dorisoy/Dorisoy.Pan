using Dorisoy.Pan.ViewModels;

namespace Dorisoy.Pan.Pages.Chat;

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

                //�������������촰��
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
    ///  �ڵ�ǰ���λ�ò���һ���ַ�
    /// </summary>
    /// <param name="input"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void ViewModel_InsertTextAtCaret(string input)
    {
        InsertTextAtCaret(input);
    }

    /// <summary>
    /// �ڵ�ǰ���λ�ò���һ���ַ�
    /// </summary>
    /// <param name="insertText"></param>
    public void InsertTextAtCaret(string insertText)
    {
        // ��ȡ���ĵ�ǰλ��
        var caretIndex = txtLimitedInput.CaretIndex;

        // ��ȡ�ı������е��ı�
        var text = txtLimitedInput.Text ?? string.Empty;

        // ���ı��ָ�������֣������м����ϣ�����ӵ��ı�
        var newText = text.Substring(0, caretIndex) + insertText + text.Substring(caretIndex);

        // �����ı����������
        txtLimitedInput.Text = newText;

        // ���¹���λ�ã�ʹ��λ�ڲ�����ı�֮��
        txtLimitedInput.CaretIndex = caretIndex + insertText.Length;
    }

    /// <summary>
    /// ������򿪣�Windows��
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
    /// �س�������Ϣ
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
