using AutoMapper;
using Avalonia;
using Avalonia.Reactive;

namespace Dorisoy.PanClient.ViewModels;

[View(typeof(CommentsView))]
public class CommentsViewModel : ViewModelBase, IActivatableViewModel
{
    private readonly IDocumentCommentService _documentCommentService;
    private readonly IDocumentService _documentService;
    private readonly IMapper _mapper;
    private readonly ContentDialog dialog;
    private CancellationTokenSource Cts = new CancellationTokenSource();

    [Reactive] public DocumentModel Document { get; set; }

    private ReadOnlyObservableCollection<DocumentCommentModel> _items;
    public ReadOnlyObservableCollection<DocumentCommentModel> Items => _items;

    public CommentsViewModel(ContentDialog dialog, DocumentModel document) : base()
    {
        _documentCommentService = Locator.Current.GetService<IDocumentCommentService>();
        _documentService = Locator.Current.GetService<IDocumentService>();
        _mapper = Locator.Current.GetService<IMapper>();

        if (dialog is null)
            throw new ArgumentNullException(nameof(dialog));

        Document = document;


        this.dialog = dialog;
        dialog.Closed += DialogOnClosed;
        dialog.CloseButtonClick += Dialog_CloseButtonClick;


        this.WhenActivated((CompositeDisposable disposables) =>
        {
            _documentCommentService
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _items)
            .Subscribe(s =>
            {
                System.Diagnostics.Debug.Print($"{s.Count}");
            })
            .DisposeWith(disposables);

            RxApp.MainThreadScheduler.Schedule(LoadData)
            .DisposeWith(disposables);
        });
    }

    private async void LoadData()
    {
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            await _documentCommentService.GetAllDocumentComment(Document.Id);
        }, DispatcherPriority.Background);
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
    }

    protected override void DisposeManaged()
    {
    }

    protected override void DisposeUnmanaged()
    {

    }
}
