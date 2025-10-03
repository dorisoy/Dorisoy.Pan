using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using H264Sharp;

namespace Dorisoy.PanClient.Services.Video.H264;

internal class H264TranscoderProvider
{
    static H264TranscoderProvider()
    {
        H264Sharp.Defines.CiscoDllName32bit = "openh264-2.4.0-win32.dll";
        H264Sharp.Defines.CiscoDllName64bit = "openh264-2.4.0-win64.dll";
    }
   
    public static H264Encoder CreateEncoder(int width, int height, 
       int fps = 30, int bps = 3_000_000, ConfigType configNo = ConfigType.CameraBasic)
    {

        H264Encoder encoder = new H264Encoder();
        encoder.Initialize(width, height, bps, fps, configNo );
        encoder.SetOption(ENCODER_OPTION.ENCODER_OPTION_RC_FRAME_SKIP, (byte)1);

        if (configNo == ConfigType.CameraCaptureAdvanced)
        {
            encoder.SetOption(ENCODER_OPTION.ENCODER_OPTION_IDR_INTERVAL, 600);
            encoder.ConverterNumberOfThreads = 1;
        }
        else if(configNo == ConfigType.ScreenCaptureAdvanced)
            encoder.SetOption(ENCODER_OPTION.ENCODER_OPTION_IDR_INTERVAL, 1800);

        //encoder.SetMaxBitrate(bps);
        //encoder.SetTargetFps(fps);
        return encoder;
    }

    public static H264Decoder CreateDecoder()
    {
        H264Decoder decoder = new H264Decoder();
        decoder.Initialize();
        decoder.ConverterNumberOfThreads = 1;
       // decoder.EnableParallelImageConversion = true;
        return decoder;
    }
}
