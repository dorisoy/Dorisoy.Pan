using Path = System.IO.Path;

namespace Dorisoy.Pan.Utils;

public class Settings
{
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

    /// <summary>
    /// Image to be used when it's not possible to get a media thumbnail
    /// </summary>
    public const string WeTubeImageNotAvailable = @"assets\covernotfound.png";


}


