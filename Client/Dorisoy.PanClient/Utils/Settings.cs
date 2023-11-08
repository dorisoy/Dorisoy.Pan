namespace Dorisoy.PanClient.Utils;

public class Settings
{
    // DIRECT VIDEO TEST LINKS:
    // http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4
    // http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ElephantsDream.mp4

    // VIDEO TEST LINKS TO BE HANDLED BY YT-DLP (actually only free videos. To be added: registered/login/cookies)
    // https://www.youtube.com/watch?v=niG3YMU6jFk
    // https://www.youtube.com/watch?v=YFsC83mw5kg
    // https://vimeo.com/359318136 16K?
    // https://vimeo.com/evosia/ephemeral 8K?
    // https://www.facebook.com/1424858414264105/videos/1566308450213846


    // STRANGE BEHAVIOUR TEST LINK
    // https://vimeo.com/martinlisius/prairiewind16k
    // This is declared as 16K stream but the max real resolution available is 7680x3212 (near 8K, 4320p)
    // It seems YAMP (or LibVLC) is not able to handle, with the actual settings (SAR issue?).
    // Strangely is solved in yt-dlp taking formats less than 7680 in width
    // so it will play up to 4K which is perfectly handled and gives a decent result.


    /// <summary>
    /// Set this to TRUE while debugging LibVLC. Set to FALSE in production.
    /// </summary>
    public const bool ISDEBUGGING = false;

    /// <summary>
    /// Seek/Rewind (msecs)
    /// </summary>
    public const int SEEK_OFFSET = 5000;

    /// <summary>
    /// Position of FFMPEG (86/64) executables        
    /// </summary>
    public static string FfmpegPath = Path.Combine(Utilities.ApplicationFolder(), Utilities.IsWin64() ? @"bins\iffx64" : @"bins\iffx86");

    /// <summary>
    /// Position of yt-dlp (86/64) executable
    /// </summary>
    public static string BackendYtDlpPath = Path.Combine(Utilities.ApplicationFolder(), Utilities.IsWin64() ? @"bins\iffx64\yt-dlp.exe" : @"bins\iffx86\yt-dlp.exe");

    /// <summary>
    /// Network Caching for LibVlc (msecs)
    /// </summary>
    public const int LibVlcNetworkCaching = 5000;

    /// <summary>
    /// Local files caching for LibVlc (msecs)
    /// </summary>
    public const int LibVlcFileCaching = 1500;

    /// <summary>
    /// Muxing caching for LibVlc (msecs)
    /// </summary>
    public const int LibVlcMuxCaching = 1500;

    /// <summary>
    /// Network Caching for Media (msecs)
    /// </summary>
    public const int MediaNetworkCaching = 3000;

    /// <summary>
    /// Additional options to create LibVlc instance
    /// </summary>

    /*
    public static string[] LibVlc_AdditionalOptions = {                                      
          "--avcodec-hw=any"
        //, "--avcodec-skiploopfilter=4"
        //, "--clock-jitter=0"
        //, "--clock-synchro=0"
        //, "--codec=all"
        , $"--file-caching={LibVlcFileCaching}"
        , $"--network-caching={LibVlcNetworkCaching}"
        //, $"--sout-mux-caching={LibVlcMuxCaching}"
    };
    */

    public static string[] LibVlc_AdditionalOptions = { "--directx-use-sysmem", "--network-caching=4000" };

    /// <summary>
    /// Additional options to create Media instance
    /// </summary>

    public static string[] Media_AdditionalOptions = {
        $":network-caching={MediaNetworkCaching}"
    };


    //public static string[] Media_AdditionalOptions = { };

    /// <summary>
    /// Image to be used when it's not possible to get a media thumbnail
    /// </summary>
    public const string WeTubeImageNotAvailable = @"assets\covernotfound.png";


}


