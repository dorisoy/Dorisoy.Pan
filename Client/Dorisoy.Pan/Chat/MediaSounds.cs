namespace Dorisoy.PanClient.Chat;

public class MediaSounds
{
    private readonly VoiceChatModel model;
    private readonly string raing = "avares://Dorisoy.PanClient/Assets/ringtone.mp3";
    private readonly string dial = "avares://Dorisoy.PanClient/Assets/dialtone.mp3";
    private CancellationTokenSource _cts;
    private Task playTask;

    public MediaSounds(VoiceChatModel model)
    {
        this.model = model;
    }

    /// <summary>
    /// 播放声音
    /// </summary>
    public void Play()
    {
        try
        {
            _cts = new CancellationTokenSource();

            if (playTask != null && !playTask.IsCompleted)
                return;

            switch (model.State)
            {
                case ModelStates.OutgoingCall:
                    {
                        playTask = Task.Run(() => Playing(dial, _cts.Token), _cts.Token);
                    }
                    break;
                case ModelStates.IncomingCall:
                    {
                        playTask = Task.Run(() => Playing(raing, _cts.Token), _cts.Token);
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// 停止播放声音
    /// </summary>
    public async void Stop()
    {
        // 如果在停止之前调用“Dispose”
        if (_cts == null || _cts.IsCancellationRequested)
            return;

        if (playTask != null && !playTask.IsCompleted)
        {
            // 取消等待呼叫
            _cts?.Cancel();

            // 等待完成
            await playTask;
        }
    }

    /// <summary>
    /// 播放流
    /// </summary>
    /// <param name="path"></param>
    /// <param name="token"></param>
    private void Playing(string path, CancellationToken token)
    {
        try
        {
            using var waveOut = new WaveOutEvent();
            using var sm = AssetLoader.Open(new Uri(path));
            using var reader = new Mp3FileReader(sm);
            using var pcm = WaveFormatConversionStream.CreatePcmStream(reader);
            using var stream = new BlockAlignReductionStream(pcm);

            waveOut?.Init(stream);
            waveOut?.Play();

            Debug.WriteLine("MediaSounds Play...");

            while (waveOut.PlaybackState == PlaybackState.Playing && !token.IsCancellationRequested)
            {
                Thread.Sleep(100);
            }

            waveOut?.Stop();
        }
        catch { }
    }

}
