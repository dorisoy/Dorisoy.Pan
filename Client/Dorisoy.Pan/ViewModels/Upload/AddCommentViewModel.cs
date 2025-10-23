namespace Dorisoy.Pan.ViewModels;

//[View(typeof(AddCommentView))]
public class AddCommentViewModel : ViewModelBase, IActivatableViewModel, IValidatableViewModel
{
    private readonly IDocumentCommentService _documentCommentService;
    private readonly IDocumentService _documentService;
    private readonly IMapper _mapper;
    private readonly ContentDialog dialog;
    private CancellationTokenSource Cts = new CancellationTokenSource();
    ValidationContext IValidatableViewModel.ValidationContext { get; } = new ValidationContext();

    [Reactive] public string Comments { get; set; }
    [Reactive] public Guid DocumentId { get; set; }

    public AddCommentViewModel(ContentDialog dialog, Guid documentId) : base()
    {
        _documentCommentService = Locator.Current.GetService<IDocumentCommentService>();
        _documentService = Locator.Current.GetService<IDocumentService>();
        _mapper = Locator.Current.GetService<IMapper>();

        if (dialog is null)
            throw new ArgumentNullException(nameof(dialog));

        DocumentId = documentId;

        this.dialog = dialog;
        dialog.Closed += DialogOnClosed;
        dialog.CloseButtonClick += Dialog_CloseButtonClick;
        dialog.PrimaryButtonClick += Dialog_PrimaryButtonClick;
        this.WhenActivated((CompositeDisposable disposables) =>
        {
        });
    }

    private void Dialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (string.IsNullOrEmpty(Comments))
            MessageBox("备注内容不能为空！");

        this.IsValid().Subscribe(async s =>
        {
            if (s)
            {
                var user = Globals.CurrentUser;

                var comment = new DocumentCommentModel()
                {
                    Id = Guid.NewGuid(),
                    DocumentId = DocumentId,
                    Comment = Comments,
                    CreatedDate = DateTime.Now,
                    UserName = user.RaleName,
                };
                var result = await _documentCommentService.AddAsync(comment);
                if (result.Succeeded)
                {
                    MessageBox("备注添加成功！");
                    dialog.Hide(ContentDialogResult.Primary);
                }
                else
                    MessageBox(string.Join("/r", result.Errors.Select(s => s.Message)));
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

    private void DialogOnClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        dialog.Closed -= DialogOnClosed;
        dialog.CloseButtonClick -= Dialog_CloseButtonClick;
        dialog.PrimaryButtonClick -= Dialog_PrimaryButtonClick;
    }
}
