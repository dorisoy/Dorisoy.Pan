//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using NetworkLibrary;
//using Dorisoy.PanClient.Services.Latency;
using Dorisoy.PanClient.Services.ScreenShare;
using Dorisoy.PanClient.Services.Video;

namespace Dorisoy.PanClient.Services;

public class ServiceHub
{
    private static ServiceHub instance;
    public static ServiceHub Instance
    {
        get
        {
            if (instance == null)
                instance = new ServiceHub();
            return instance;
        }
    }

    //public AudioHandler AudioHandler { get; }
    public VideoHandler2 VideoHandler { get; }

    //public MessageHandler MessageHandler { get; }

    public FileShare FileShare { get; }

    //public LatencyPublisher LatencyPublisher { get; }

    public ScreenShareHandlerH264 ScreenShareHandler { get; }

    public Action<VCStatistics> VideoStatisticsAvailable;
    public Action<int, int> CamSizeFeedbackAvailable;
    public event Action<string, string> LogAvailable;
    private VCStatistics stats;
    private VCStatistics statsPrev;

    private ServiceHub()
    {
        //AudioHandler = new AudioHandler();

        VideoHandler = new VideoHandler2();
        FileShare = new FileShare();

        //MessageHandler = new MessageHandler();
        //LatencyPublisher = new LatencyPublisher(MessageHandler);
        ScreenShareHandler = new ScreenShareHandlerH264();

        //AudioHandler.StartSpeakers();
        //MessageHandler.OnMessageAvailable += HandleMessage;
        //CallStateManager.Instance.StaticPropertyChanged += CallStateChanged;
        //AudioHandler.OnStatisticsAvailable += OnAudioStatsAvailable;

        VideoHandler.CamSizeFeedbackAvailable = (w, h) => CamSizeFeedbackAvailable?.Invoke(w, h);

        PublishStatistics();
    }

    private void PublishStatistics()
    {
        Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(1000);// dont change time
                var vs = VideoHandler.GetStatistics();
                var scs = ScreenShareHandler.GetStatistics();

                stats.OutgoingFrameRate = vs.OutgoingFrameRate + scs.OutgoingFrameRate;
                stats.IncomingFrameRate = vs.IncomingFrameRate + scs.IncomingFrameRate;
                stats.TransferRate = scs.TransferRate + vs.TransferRate;
                stats.AverageLatency = vs.AverageLatency;
                stats.ReceiveRate = vs.ReceiveRate + scs.ReceiveRate;
                stats.CurrentMaxBitRate = vs.CurrentMaxBitRate;

                if (statsPrev != stats)
                {
                    statsPrev = stats;
                    VideoStatisticsAvailable?.Invoke(stats);
                }
            }

        });
    }

    private void HandleMessage(MessageEnvelope message)
    {
        //if (message.Header == MessageHeaders.MicClosed)
        //{
        //    AudioHandler.FlushBuffers();
        //}
        //else if (message.Header == MessageHeaders.RemoteClosedCam)
        {
            VideoHandler.FlushBuffers();
        }
    }

    //private void OnAudioStatsAvailable(AudioStatistics stats)
    //{
    //    // you can mode it as prop
    //    VideoHandler.AudioBufferLatency = AudioHandler.BufferedDurationAvg;
    //}

    //private void CallStateChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    //{
    //    var currState = CallStateManager.GetState();
    //    if (currState == CallStateManager.CallState.OnCall ||
    //        currState == CallStateManager.CallState.Available)
    //    {
    //        AudioHandler.ResetStatistics();
    //        AudioHandler.FlushBuffers();
    //        VideoHandler.FlushBuffers();
    //    }
    //}

    public void ResetBuffers()
    {
        //AudioHandler.ResetStatistics();
        //AudioHandler.FlushBuffers();
        VideoHandler.FlushBuffers();
    }

    public void Log(string logType, string log)
    {
        LogAvailable?.Invoke(logType, log);
    }
}
