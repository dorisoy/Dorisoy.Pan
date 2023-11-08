using AutoMapper;
using AvaloniaEdit.Utils;

namespace Dorisoy.PanClient.ViewModels;

[View(typeof(AddFilesView))]
public class AddFilesViewModel : ReactiveObject, IActivatableViewModel
{
    private readonly IPhysicalFolderService _physicalFolderService;
    private readonly IRemoteVirtualFolderService _virtualFolderService;
    private readonly IPhysicalFolderUserService _physicalFolderUserService;
    private readonly IVirtualFolderUserUserService _virtualFolderUserUserService;
    private readonly IDocumentService _documentService;
    private readonly IMapper _mapper;
    private readonly IFolderService _folderService;


    private readonly ContentDialog dialog;
    private CancellationTokenSource Cts = new CancellationTokenSource();
    private bool AutoClose;

    public virtual ViewModelActivator Activator { get; } = new ViewModelActivator();

    [Reactive] public UploadingModel Progresser { get; set; } = new UploadingModel();


    /// <summary>
    /// 当前文件夹
    /// </summary>
    [Reactive] public VirtualFolderModel RootFolder { get; set; }

    /// <summary>
    /// 待上传的文件
    /// </summary>
    public List<UploadFile> FilePaths { get; set; }

    /// <summary>
    /// 根目录
    /// </summary>
    public string RootPath { get; set; }

    /// <summary>
    /// 项目信息
    /// </summary>
    public PatientModel Patient { get; set; }

    private Action CompletedHandle;


    public AddFilesViewModel(PatientModel patient,
        ContentDialog dialog,
        VirtualFolderModel rootFolder,
        List<UploadFile> filePaths,
        string root = "",
        bool autoClose = false,
        Action complete = null) : base()
    {
        _physicalFolderService = Locator.Current.GetService<IPhysicalFolderService>();
        _virtualFolderService = Locator.Current.GetService<IRemoteVirtualFolderService>();
        _physicalFolderUserService = Locator.Current.GetService<IPhysicalFolderUserService>();
        _virtualFolderUserUserService = Locator.Current.GetService<IVirtualFolderUserUserService>();
        _documentService = Locator.Current.GetService<IDocumentService>();
        _mapper = Locator.Current.GetService<IMapper>();
        _folderService = Locator.Current.GetService<IFolderService>();
        CompletedHandle = complete;


        if (dialog is null)
            throw new ArgumentNullException(nameof(dialog));

        FilePaths = filePaths;
        RootFolder = rootFolder;
        RootPath = root;
        AutoClose = autoClose;
        Patient = patient;

        this.dialog = dialog;
        dialog.Closed += DialogOnClosed;
        dialog.Opened += Dialog_Opened;
        dialog.CloseButtonClick += Dialog_CloseButtonClick;
    }


    private DispatcherTimer CutDownTimer = null;
    private void CutDown(int time, Action action)
    {
        double value = 0;
        CutDownTimer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, (s, e) =>
        {
            ++value;
            dialog.CloseButtonText = $"{value}秒后关闭";
            if (value == time)
            {
                action?.Invoke();
                CutDownTimer.Stop();
            }
        });
        CutDownTimer.Start();
    }

    private void Dialog_Opened(ContentDialog sender, EventArgs args)
    {
        UploadFiles();
    }

    /// <summary>
    /// 上传文件
    /// </summary>
    private async void UploadFiles()
    {
        try
        {
            Progresser.ValueFormat = 0.ToString("p2");

            if (FilePaths == null || !FilePaths.Any())
                return;

            var step = 0;
            var done = 0;
            var failed = 0;
            var pending = FilePaths.Count;

            //待上传的文件
            foreach (var fp in FilePaths)
            {
                if (!Cts.IsCancellationRequested)
                {
                    step++;
                    //await Task.Delay(100);
                    var result = await UploadDocumentHandle(fp);
                    if (result.Item1)
                        done++;
                    else
                        failed++;

                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        double percent = Convert.ToDouble(step) / Convert.ToDouble(pending);

                        if (!result.Item1)
                            Progresser.FileName = $"[{step}/{pending}] : {fp}";
                        else
                            Progresser.FileName = result.Item2;

                        Progresser.Value = percent * 100;
                        Progresser.ValueFormat = percent.ToString("p2");

                    }, DispatcherPriority.Background);
                }
            }

            Progresser.FileName = $"共：{pending} ,失败 {failed} , 成功 {done} 个文件.";

            //自动关闭窗口
            if (AutoClose)
            {
                CutDown(2, () => { dialog.Hide(ContentDialogResult.Primary); });
            }
        }
        catch (Exception ex)
        {
            MessageBox(ex.Message);
        }
    }

    /// <summary>
    /// 远程上传文件
    /// </summary>
    private async Task<Tuple<bool, string>> UploadDocumentHandle(UploadFile uploadFile)
    {
        try
        {
            //获取当前用户的存储目录
            var user = Globals.CurrentUser;
            var fullFileName = uploadFile.Path;

            var fileToSave = new FileInfo(fullFileName);
            if (!string.IsNullOrEmpty(RootPath))
                fullFileName = fullFileName.Replace(RootPath, "");
            else
                fullFileName = fileToSave.Name;

            var physicalFolderId = RootFolder.PhysicalFolderId;

            //使用缩略目录
            if (uploadFile.IsThumbnail)
            {
                physicalFolderId = uploadFile.PhysicalThumbnailFolderId;
                if (physicalFolderId == Guid.Empty)
                    return new Tuple<bool, string>(false, "缩略目录未指定");
            }

            var file = new FileInfo(uploadFile.Path);
            var result = await _folderService.UploadDocuments(file,
                fullFileName,
                physicalFolderId,
                user.Id,
                Patient?.Id ?? Guid.Empty);
            if (result?.Success ?? false)
            {
                CompletedHandle?.Invoke();
                return new Tuple<bool, string>(true, "上传成功！");
            }
            else
                return new Tuple<bool, string>(false, "服务器错误！");
        }
        catch (Exception ex)
        {
            return new Tuple<bool, string>(false, ex.Message);
        }
    }

    /*
    /// <summary>
    /// 本地上传文件
    /// </summary>
    private async Task<Tuple<bool, string>> UploadDocumentHandle2(UploadFile uploadFile)
    {
        try
        {
            //获取当前用户的存储目录
            var user = Globals.CurrentUser;

            //"C:\\Users\\Administrator\\Desktop\\test2\\3 - 副本.png"
            var fullFileName = uploadFile.Path;

            var fileToSave = new FileInfo(fullFileName);
            if (!string.IsNullOrEmpty(RootPath))
                fullFileName = fullFileName.Replace(RootPath, "");
            else
                fullFileName = fileToSave.Name;

            var physicalFolderId = RootFolder.PhysicalFolderId;

            //使用缩略目录
            if (uploadFile.IsThumbnail)
            {
                physicalFolderId = uploadFile.PhysicalThumbnailFolderId;
                if (physicalFolderId == Guid.Empty)
                    return new Tuple<bool, string>(false, "缩略目录未指定");
            }

            //(从当前用户物理文件夹)获取文档目录父路径  fullFileName = "\\test2\\test\\3.png"
            var parentId = await GetDocumentParentIdAndPath(physicalFolderId, fullFileName);
            if (Guid.Empty == parentId)
            {
                //目录路径不存在
                return new Tuple<bool, string>(false, "文档目录父路径不存在");
            }

            var fileName = fullFileName.Split("/").LastOrDefault();
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = fullFileName.Split("\\").LastOrDefault();
            }

            var _pathHelper = new PathHelper();
            //获取扩展名
            var extension = Path.GetExtension(fileName);
            if (_pathHelper.ExecutableFileTypes.Any(c => c == extension))
            {
                //不允许上载可执行文件
                return new Tuple<bool, string>(false, "不允许上载可执行文件");
            }

            //用户文档根目录
            var path = $"{Guid.NewGuid()}{extension}";
            var documentPath = Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath, user.Id.ToString());
            var thumbnailPath = Path.Combine(_pathHelper.AppRootPath, _pathHelper.DocumentPath, user.Id.ToString());

            //不存在则创建
            if (!Directory.Exists(documentPath))
                Directory.CreateDirectory(documentPath);

            //根据文件名获取用户文档
            var document = await _documentService.GetDocument(parentId, fileName, user.Id);
            //如果文档存在，则更新
            if (document != null)
            {
                #region

                //版本路径
                var versionPath = Path.Combine(documentPath, document.Id.ToString());
                //不存在则创建
                if (!Directory.Exists(versionPath))
                    Directory.CreateDirectory(versionPath);

                //版本文件名
                var versionFileName = $"{Guid.NewGuid()}{extension}";
                //源文件
                var sourceFile = Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath, document.Path);
                //目标文件
                var detinationFile = Path.Combine(versionPath, versionFileName);

                //D:\\MAUI\\Sinol\\Dorisoy.PanClient\\Dorisoy.PanClient\\bin\\Debug\\net7.0\\Documents\\115ce6fb-eaeb-49c9-9842-583ace34aa91\\6d3fc311-f8d6-4d62-a1ee-14a4aee6ac8b.xlsx
                //D:\\MAUI\\Sinol\\Dorisoy.PanClient\\Dorisoy.PanClient\\bin\\Debug\\net7.0\\Documents\\115ce6fb-eaeb-49c9-9842-583ace34aa91\\6d3fc311-f8d6-4d62-a1ee-14a4aee6ac8b.xlsx

                //这里更新替换已经存在的文件，并记录版本
                var checkf = new FileInfo(sourceFile);
                if (checkf.Exists)
                {
                    //移动文件
                    File.Move(sourceFile, detinationFile);

                    //读取文件加密
                    var file = new FileInfo(uploadFile.Path);
                    if (file.Exists)
                    {
                        //读取文件字节
                        var bytesData = AesOperation.ReadAsBytesAsync(uploadFile.Path);
                        using (var stream = new FileStream(Path.Combine(documentPath, path), FileMode.Create))
                        {
                            //加密文件流
                            var ceskey = _pathHelper.EncryptionKey;
                            var byteArray = AesOperation.EncryptStream(bytesData, ceskey);
                            stream.Write(byteArray, 0, byteArray.Length);
                        }
                    }


                    document.Path = Path.Combine(user.Id.ToString(), path);
                    document.Size = fileToSave.Length;
                    document.ModifiedBy = user.Id;
                    document.ModifiedDate = DateTime.UtcNow;
                    //项目
                    document.PatienterId = Patient?.Id ?? Guid.Empty;

                    //TODO...这里应该记录版本历史

                    //更新文档
                    var result = await _documentService.UpdateAsync(document);
                    if (result.Succeeded)
                    {
                        return new Tuple<bool, string>(true, "上传成功");
                    }
                    else
                    {
                        // 反向移动
                        File.Move(detinationFile, sourceFile);
                    }
                }
                #endregion
            }
            //不存在则创建文档
            else
            {
                #region

                var documentId = Guid.NewGuid();
                document = new DocumentModel
                {
                    Id = documentId,
                    Extension = extension,
                    Path = Path.Combine(user.Id.ToString(), path),
                    Size = fileToSave.Length,
                    Name = fileName,
                    //创建缩略图
                    ThumbnailPath = Path.Combine(user.Id.ToString(), path),
                    ThumbnailIcon = Symbol.OpenFile,
                    PhysicalFolderId = parentId,
                    //项目
                    PatienterId = Patient?.Id ?? Guid.Empty,
                    CreatedBy = user.Id,
                    CreatedDate = DateTime.Now
                };

                if (uploadFile.IsThumbnail)
                {
                    document.ThumbnailPath = Path.Combine(user.Id.ToString(), path);
                    document.IsAttachment = true;
                }

                if (extension.Contains(".avi"))
                    document.ThumbnailPath = Path.Combine(user.Id.ToString(), path.Replace(".avi", ".bmp"));

                var file = new FileInfo(uploadFile.Path);
                if (file.Exists)
                {
                    //读取文件字节
                    var bytesData = AesOperation.ReadAsBytesAsync(uploadFile.Path);
                    var storePath = Path.Combine(documentPath, path);
                    using (var stream = new FileStream(storePath, FileMode.Create))
                    {
                        //加密文件流
                        var ceskey = _pathHelper.EncryptionKey;
                        var byteArray = AesOperation.EncryptStream(bytesData, ceskey);
                        stream.Write(byteArray, 0, byteArray.Length);
                    }
                }

                //创建缩略图

                //添加文档
                await _documentService.AddAsync(document, false);

                #endregion
            }

            return new Tuple<bool, string>(true, "上传成功");
        }
        catch (Exception ex)
        {
            return new Tuple<bool, string>(false, ex.Message);
        }
    }
    */

    /*
    /// <summary>
    /// (从当前用户物理文件夹)获取文档目录父路径
    /// </summary>
    /// <param name="rootId"></param>
    /// <param name="fullFileName"></param>
    /// <returns></returns>
    private async Task<Guid> GetDocumentParentIdAndPath(Guid rootId, string fullFileName)
    {
        var user = Globals.CurrentUser;
        var folderPaths = fullFileName.Split("/").SkipLast(1).ToList();
        if (folderPaths.Count() == 0)
            folderPaths = fullFileName.Split("\\").SkipLast(1).ToList();

        folderPaths = folderPaths.Where(s => !string.IsNullOrEmpty(s)).ToList();
        var parentId = rootId;
        if (folderPaths.Any())
        {
            foreach (var folderName in folderPaths)
            {
                var pfs = await _physicalFolderService.GetPhysicalFolders();
                var folder = pfs.FirstOrDefault(c => c.PhysicalFolderUsers.Any(c => c.UserId == user.Id)
                        && c.ParentId == parentId
                        && c.Name == folderName);

                if (folder == null)
                    return Guid.Empty;
                else
                    parentId = folder.Id;
            }
        }
        return parentId;
    }

    */

    /// <summary>
    /// 取消
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private async void Dialog_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        //timer.Stop();
        //MessageBox("操作成功！");
        //dialog.Hide(ContentDialogResult.Primary);
        //args.Cancel = true;

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            try
            {
                CompletedHandle?.Invoke();
            }
            catch { }
        });

        Cts?.Cancel();
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
        dialog.Opened -= Dialog_Opened;
        dialog.CloseButtonClick -= Dialog_CloseButtonClick;
    }

}
