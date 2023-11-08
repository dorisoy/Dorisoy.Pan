using AutoMapper;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using Dorisoy.PanClient.Commands;

namespace Dorisoy.PanClient.ViewModels;

[View(typeof(AddFolderView))]
public class AddFolderViewModel : ReactiveObject, IActivatableViewModel, IValidatableViewModel
{
    private readonly IPhysicalFolderService _physicalFolderService;
    private readonly IVirtualFolderService _virtualFolderService;
    private readonly IPhysicalFolderUserService _physicalFolderUserService;
    private readonly IVirtualFolderUserUserService _virtualFolderUserUserService;
    private readonly IDocumentService _documentService;
    private readonly IMapper _mapper;
    private readonly ContentDialog dialog;

    private readonly IFolderService _folderService;
    private readonly IRemoteVirtualFolderService _remoteVirtualFolderService;

    private CancellationTokenSource Cts = new CancellationTokenSource();
    public virtual ViewModelActivator Activator { get; } = new ViewModelActivator();
    ValidationContext IValidatableViewModel.ValidationContext { get; } = new ValidationContext();

    [Reactive] public string FolderName { get; set; }
    [Reactive] public VirtualFolderModel RootFolder { get; set; }

    public AddFolderViewModel(ContentDialog dialog, VirtualFolderModel rootFolder) : base()
    {
        _physicalFolderService = Locator.Current.GetService<IPhysicalFolderService>();
        _virtualFolderService = Locator.Current.GetService<IVirtualFolderService>();
        _physicalFolderUserService = Locator.Current.GetService<IPhysicalFolderUserService>();
        _virtualFolderUserUserService = Locator.Current.GetService<IVirtualFolderUserUserService>();
        _documentService = Locator.Current.GetService<IDocumentService>();
        _mapper = Locator.Current.GetService<IMapper>();

        _folderService = Locator.Current.GetService<IFolderService>();
        _remoteVirtualFolderService = Locator.Current.GetService<IRemoteVirtualFolderService>();

        if (dialog is null)
            throw new ArgumentNullException(nameof(dialog));

        this.dialog = dialog;
        dialog.Closed += DialogOnClosed;
        dialog.CloseButtonClick += Dialog_CloseButtonClick;
        dialog.PrimaryButtonClick += Dialog_PrimaryButtonClick;
        this.WhenActivated((CompositeDisposable disposables) =>
        {
        });
        RootFolder = rootFolder;
    }

    private void Dialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (string.IsNullOrEmpty(FolderName))
        {
            MessageBox("文件夹名称不能为空！");
            return;
        }

        this.IsValid().Subscribe(async s =>
        {
            if (s)
            {
                var user = Globals.CurrentUser;

                //本地添加文件夹
                //var result = await _physicalFolderService.AddFolder(FolderName,RootFolder.Id, RootFolder.PhysicalFolderId, user.Id);

                //远程添加文件夹
                var cmd = new AddFolderCommand
                {
                    Name = FolderName,
                    PhysicalFolderId = RootFolder.PhysicalFolderId,
                    VirtualParentId = RootFolder.Id
                };
                var result = await _remoteVirtualFolderService.CreateFolder(cmd);
                if (result?.Success ?? false)
                {
                    MessageBox("文件夹添加成功！");
                    dialog.Hide(ContentDialogResult.Primary);
                }
                else
                    MessageBox(string.Join("/r", result.Errors));
            }
        });

        args.Cancel = true;
    }


    /// <summary>
    /// 取消
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void Dialog_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        Cts.Cancel();
    }

    private void MessageBox(string msg)
    {
        var resultHint = new ContentDialog()
        {
            Content = msg,
            Title = "提示",
            PrimaryButtonText = "确认"
        };
        _ = resultHint.ShowAsync();
    }

    private void DialogOnClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        dialog.Closed -= DialogOnClosed;
        dialog.CloseButtonClick -= Dialog_CloseButtonClick;
        dialog.PrimaryButtonClick -= Dialog_PrimaryButtonClick;
    }

}
