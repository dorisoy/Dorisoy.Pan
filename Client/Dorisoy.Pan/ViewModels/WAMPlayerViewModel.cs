namespace Dorisoy.Pan.ViewModels;

public class WAMPlayerViewModel : ViewModelBase
{
    private readonly ISettingsProvider<AppSettings> _settingsProvider;
    private YampView playerView;

    private string _mediaFile;
    private bool _isLoading;

    string Output = string.Empty;
    StringBuilder outputString = new();
    double probedDuration = 0.0;
    double probedWidth = 0.0;
    double probedHeight = 0.0;
    string probedAspectRatio = string.Empty;

    string tempMediaFile { get; set; }

    public WAMPlayerViewModel()
    {
        _settingsProvider = Locator.Current.GetService<ISettingsProvider<AppSettings>>();

        IsLoading = false;
        _mediaFile = _settingsProvider.Settings.GetRSTPServer();

        CallOpenPlayer();
    }


    ///// <summary>
    ///// Updates yt-dlp
    ///// </summary>
    //private void UpdateDl()
    //{
    //    var dlProcess = new Process();
    //    dlProcess.StartInfo.CreateNoWindow = true;
    //    dlProcess.StartInfo.UseShellExecute = false;
    //    dlProcess.StartInfo.FileName = Settings.BackendYtDlpPath;
    //    dlProcess.StartInfo.ArgumentList.Clear();

    //    try
    //    {
    //        dlProcess.StartInfo.ArgumentList.Add($"-U");
    //        dlProcess.Start();
    //    }
    //    catch (Exception ex)
    //    {

    //    }
    //}

    public string MediaFile
    {
        get => _mediaFile;
        set => this.RaiseAndSetIfChanged(ref _mediaFile, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    private void ProbeProcess_Exited(object sender, EventArgs e)
    {
        var p = sender as Process;
        probedWidth = 0.0;
        probedHeight = 0.0;
        probedDuration = 0.0;
        probedAspectRatio = string.Empty;

        if (null != p)
        {
            p.CancelErrorRead();
            p.CancelOutputRead();

            var obj = JsonConvert.DeserializeObject<MovieData>(Output);

            if (null != obj)
            {
                if (obj.Format != null)
                    probedDuration = double.Parse(obj.Format.Duration, System.Globalization.CultureInfo.InvariantCulture);

                if (obj.Streams != null)
                {
                    foreach (var stream in obj.Streams)
                    {
                        if (stream.CodecType.Contains("video"))
                        {
                            string tmpAR;

                            var tmpW = double.Parse(stream.Width, System.Globalization.CultureInfo.InvariantCulture);
                            var tmpH = double.Parse(stream.Height, System.Globalization.CultureInfo.InvariantCulture);

                            try
                            {
                                tmpAR = stream.AspectRatio.Replace("\"", "");
                            }
                            catch { tmpAR = string.Empty; }

                            if (probedWidth < tmpW)
                            {
                                probedWidth = tmpW;
                                probedHeight = tmpH;
                                if (string.IsNullOrWhiteSpace(tmpAR))
                                    probedAspectRatio = $"{probedWidth}:{probedHeight}";
                                else
                                {
                                    probedAspectRatio = tmpAR;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void EphemeralProcess_Exited(object sender, EventArgs e)
    {
        var p = sender as Process;
        if (null != p)
        {
            p.CancelErrorRead();
            p.CancelOutputRead();
        }
        if (!string.IsNullOrWhiteSpace(Output) && Output.StartsWith("http"))
            tempMediaFile = Output.Replace("\n", "").Replace("\r", "");
        else
        {
            // Errors happened
            tempMediaFile = string.Empty;
        }
    }

    private void ProcessOutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
    {
        if (!string.IsNullOrEmpty(outLine.Data))
        {
            outputString.Append(outLine.Data);
            outputString.Append(Environment.NewLine);
            Output = outputString.ToString();
        }
    }

    private void EphemeralOutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
    {
        if (!string.IsNullOrEmpty(outLine.Data))
        {
            outputString.Append(outLine.Data);
            outputString.Append(Environment.NewLine);
            Output = outputString.ToString();
        }
    }

    /// <summary>
    /// ������Ƶ
    /// </summary>
    public void CallOpenPlayer()
    {
        //var thread = new Thread(() => OpenPlayer());
        //thread.Start();
        OpenPlayer();
    }

    public async void OpenPlayer()
    {
        await Dispatcher.UIThread.InvokeAsync(() => { IsLoading = true; });

        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        var assemblyPath = System.IO.Path.GetDirectoryName(assembly.Location);


        tempMediaFile = _mediaFile;


        if (!string.IsNullOrEmpty(assemblyPath))
        {

            outputString.Clear();
            Output = string.Empty;

            if (!string.IsNullOrWhiteSpace(tempMediaFile))
            {
                var pProcess = new Process();
                pProcess.StartInfo.CreateNoWindow = true;
                pProcess.StartInfo.UseShellExecute = false;
                pProcess.StartInfo.RedirectStandardError = true;
                pProcess.StartInfo.RedirectStandardOutput = true;
                pProcess.StartInfo.StandardErrorEncoding = Encoding.UTF8;
                pProcess.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                pProcess.EnableRaisingEvents = true;

                pProcess.ErrorDataReceived += ProcessOutputHandler;
                pProcess.OutputDataReceived += ProcessOutputHandler;
                pProcess.Exited += ProbeProcess_Exited;

                pProcess.StartInfo.FileName = System.IO.Path.Combine(Settings.FfmpegPath, "ffprobe.exe");
                pProcess.StartInfo.ArgumentList.Clear();
                pProcess.StartInfo.Arguments = $"-loglevel quiet -print_format json -show_format -show_streams \"{tempMediaFile}\"";

                try
                {
                    pProcess.Start();
                    pProcess.BeginErrorReadLine();
                    pProcess.BeginOutputReadLine();
                    pProcess.WaitForExit();
                }
                catch { }
            }

        }

        if (!string.IsNullOrWhiteSpace(tempMediaFile))
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                IsLoading = false;

                playerView = YampView.GetInstance();

                //��Ƶ���ŵ�ַ
                playerView.videoUrl = tempMediaFile;
                playerView.coverUrl = string.Empty;
                var dur = TimeSpan.FromSeconds(probedDuration);
                playerView.videoDuration = dur.ToString(@"hh\:mm\:ss\:fff");
                playerView.videoTitle = MediaFile.StartsWith("http") ? MediaFile : System.IO.Path.GetFileNameWithoutExtension(MediaFile);
                playerView.videoWidth = Convert.ToInt32(probedWidth);
                playerView.videoHeight = Convert.ToInt32(probedHeight);
                playerView.videoAspectRatio = probedAspectRatio;
                //������Ƶ
                playerView.VideoPlayerViewControl_Play();

            });
        }
        else
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                IsLoading = false;
                MediaFile = "ERROR! �޷����Ŵ���Ƶ������֧���Ŷӱ���ʹ�õ�����.";
            });
        }
    }
}

public class MovieData
{
    [JsonProperty("streams")]
    public MovieStream[] Streams { get; set; }
    public MovieFormat Format { get; set; }
}

public class MovieStream
{
    [JsonProperty("codec_type")]
    public string CodecType { get; set; }

    [JsonProperty("width")]
    public string Width { get; set; }

    [JsonProperty("height")]
    public string Height { get; set; }

    [JsonProperty("display_aspect_ratio")]
    public string AspectRatio { get; set; }
}

public class MovieFormat
{
    [JsonProperty("duration")]
    public string Duration { get; set; }
}
