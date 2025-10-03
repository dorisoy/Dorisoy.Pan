using H264Sharp;
using NetworkLibrary;
using NetworkLibrary.Components;


namespace Dorisoy.PanClient.Services.Video.H264;


internal class H264Transcoder : IDisposable
{
    public Action<Action<PooledMemoryStream>, int, bool> EncodedFrameAvailable2;
    public Action<byte[], int, bool> EncodedFrameAvailable;
    public Action<ImageReference> DecodedFrameAvailable;
    public Action KeyFrameRequested;
    public Action<byte[], int, int> MarkingFeedback;
    public Action<byte[], int, int> LtrRecoveryRequest;
    public ConcurrentBag<RgbImage> imagePool = new ConcurrentBag<RgbImage>();
    public ConcurrentBag<ImageReference> imrefPool = new ConcurrentBag<ImageReference>();
    private bool DecodeNoDelay = true;
    public bool InvokeAction => EncodedFrameAvailable2 != null;
    public double Duration => jitterBufffer.Duration;
    public ushort encoderWidth { get; private set; }
    public ushort encoderHeight { get; private set; }
    public bool IsSetUp = false;

    private H264Encoder encoder;
    private H264Decoder decoder;
    private JitterBufffer jitterBufffer = new JitterBufffer();
    private object incrementLocker = new object();
    private object changeLock = new object();
    private byte[] cache = new byte[64000];
    private int keyReq = 3;
    private int fps;
    private int bps;
    private int frameCnt = 0;
    private int keyFrameInterval = -1;
    private int bytesSent = 0;
    private ushort seqNo = 0;
    private ushort decoderWidth;
    private ushort decoderHeight;
    int consecutiveError = 0;
    Converter converter = new Converter();
    //public H264Transcoder(ConcurrentBag<RgbImage> matPool,int desiredFps,int desiredBps)
    //{
    //    this.imagePool = matPool;
    //    fps = desiredFps;
    //    bps = desiredBps;
    //    jitterBufffer.FrameAvailable += (f) => Decode(f.Data, f.Offset, f.Count,f.w,f.h);
    //}
    public H264Transcoder(int desiredFps, int desiredBps)
    {
        fps = desiredFps;
        bps = desiredBps;
        jitterBufffer.FrameAvailable += (f) => Decode2(f.Data, f.Offset, f.Count, f.w, f.h);
    }

    public unsafe void SetupTranscoder(int encoderWidth, int encoderHeight, ConfigType configType = ConfigType.CameraBasic)
    {
        if (encoder != null)
        {
            encoder.Dispose();
        }
        encoder = H264TranscoderProvider.CreateEncoder(encoderWidth, encoderHeight, fps: fps, bps: bps, configType);


        if (decoder == null)
            decoder = H264TranscoderProvider.CreateDecoder();
        this.encoderWidth = (ushort)encoderWidth;
        this.encoderHeight = (ushort)encoderHeight;
        IsSetUp = true;

    }


    public void Downscale(ImageData src, RgbImage dst, int multiplier)
    {
        converter.Downscale(src, dst, multiplier);
    }


    private void OnEncoded(EncodedData[] frames)
    {
        int length = 0;
        bool isKeyFrame = false;
        foreach (var f in frames)
        {
            length += f.Length;
            if (f.FrameType == FrameType.IDR || f.FrameType == FrameType.I)
            {
                isKeyFrame = true;
            }
        }
        if (length == 0)
            return;
        bytesSent += length;
        if (!InvokeAction)
            PublishFrame(frames, length, isKeyFrame);
        else
            PublishFrameWithAction(frames, length, isKeyFrame);

    }

    private void PublishFrame(EncodedData[] frames, int length, bool isKeyFrame)
    {

        if (cache.Length < length * 2)
            cache = new byte[length * 2];

        int offset = 0;
        WriteMetadata(cache, ref offset);
        foreach (var frame in frames)
        {
            frame.CopyTo(cache, offset);
            offset += frame.Length;
        }

        EncodedFrameAvailable?.Invoke(cache, offset, isKeyFrame);

    }

    // This black magic writes directly to socket buffer
    private void PublishFrameWithAction(EncodedData[] frames, int length, bool isKeyFrame)
    {
        EncodedFrameAvailable2?.Invoke(Stream =>
        {
            Stream.Reserve(length + 50);
            var cache = Stream.GetBuffer();
            int offset = Stream.Position32;

            WriteMetadata(cache, ref offset);
            foreach (var frame in frames)
            {
                frame.CopyTo(cache, offset);
                offset += frame.Length;
            }
            Stream.Position32 = offset;
        }, length, isKeyFrame);


    }

    private void WriteMetadata(byte[] cache, ref int offset)
    {
        lock (incrementLocker)// its uint16 no interlocked support
            seqNo++;
        PrimitiveEncoder.WriteFixedUint16(cache, offset, seqNo);
        PrimitiveEncoder.WriteFixedUint16(cache, offset + 2, encoderWidth);
        PrimitiveEncoder.WriteFixedUint16(cache, offset + 4, encoderHeight);
        offset += 6;

    }
    public unsafe void Encode(byte* Yuv420p)
    {
        if (keyFrameInterval != -1 && frameCnt++ > keyFrameInterval)
        {
            lock (changeLock)
                encoder.ForceIntraFrame();
            frameCnt = 0;
        }

        lock (changeLock)
        {
            if (Interlocked.CompareExchange(ref isDisposed, 0, 0) == 1)
                return;

            if (encoder.Encode(Yuv420p, out EncodedData[] frames))
            {
                OnEncoded(frames);
            }
        }
    }

    public void Encode(ImageData image)
    {
        if (keyFrameInterval != -1 && frameCnt++ > keyFrameInterval)
        {
            lock (changeLock)
                encoder.ForceIntraFrame();
            frameCnt = 0;
        }

        lock (changeLock)
        {
            if (Interlocked.CompareExchange(ref isDisposed, 0, 0) == 1)
                return;

            //EncodedData
            if (encoder.Encode(image, out var frames))
            {
                OnEncoded(frames);
            }
        }
    }

    /// <summary>
    /// 强制在下一次编码时使用即时解码器刷新帧内帧
    /// </summary>
    internal void ForceIntraFrame()
    {
        lock (changeLock)
        {
            if (Interlocked.CompareExchange(ref isDisposed, 0, 0) == 1)
                return;

            //强制在下一次编码时使用即时解码器刷新帧内帧。这是在丢失帧的情况下使用的刷新解码器。请注意Inta框架很大
            encoder.ForceIntraFrame();

        }
    }

    public void SetKeyFrameInterval(int everyNFrame)
    {
        keyFrameInterval = everyNFrame;
    }

    public void SetTargetBps(int bps)
    {
        lock (changeLock)
        {
            if (Interlocked.CompareExchange(ref isDisposed, 0, 0) == 1)
                return;
            encoder?.SetMaxBitrate(bps);

        }
    }

    public void SetTargetFps(float fps)
    {
        lock (changeLock)
        {
            if (Interlocked.CompareExchange(ref isDisposed, 0, 0) == 1)
                return;
            encoder?.SetTargetFps(fps);

        }
    }

    // Decode
    // Goes to jitter, jitter publishes, then decode is called.
    internal unsafe void HandleIncomingFrame(DateTime timeStamp, byte[] payload, int payloadOffset, int payloadCount)
    {
        lock (changeLock)
        {
            if (IsDisposed())
                return;
            ushort sqn = BitConverter.ToUInt16(payload, payloadOffset);
            ushort w = BitConverter.ToUInt16(payload, payloadOffset + 2);
            ushort h = BitConverter.ToUInt16(payload, payloadOffset + 4);
            if (encoderWidth == 0)
            {
                decoderWidth = w;
                decoderHeight = h;

            }
            if (decoderWidth != w || decoderHeight != h)
            {
                lock (changeLock)
                {
                    decoderWidth = w;
                    decoderHeight = h;
                    decoder.Dispose();
                    decoder = H264TranscoderProvider.CreateDecoder();
                    jitterBufffer.Reset();
                }

            }
            payloadOffset += 6;
            payloadCount -= 6;

            jitterBufffer.HandleFrame(timeStamp, sqn, w, h, payload, payloadOffset, payloadCount);
        }

    }

    private void Decode(byte[] payload, int payloadOffset, int payloadCount, int w, int h)
    {
        lock (changeLock)
        {
            if (IsDisposed())
                return;
            if (decoder == null)
                decoder = H264TranscoderProvider.CreateDecoder();

            try
            {

                if (!imagePool.TryTake(out var rgbImage) || rgbImage == null)
                {
                    rgbImage = new RgbImage(w, h);
                }
                if (rgbImage.Width != w || rgbImage.Height != h)
                {
                    rgbImage.Dispose();
                    rgbImage = new RgbImage(w, h);
                }

                bool succ = decoder.Decode(payload, payloadOffset, payloadCount, noDelay: DecodeNoDelay, out DecodingState statusCode, ref rgbImage);
                CheckMarkingFeedback(statusCode);
                if (succ)
                {
                    ManageError(statusCode);

                    imrefPool.TryTake(out var imref);
                    if (imref == null)
                        imref = ImageReference.FromRgbImage(rgbImage, ReturnImage);
                    else
                        imref.Update(rgbImage);

                    DecodedFrameAvailable?.Invoke(imref);

                    keyReq = 0;
                }
                if (statusCode != DecodingState.dsErrorFree)
                {
                    consecutiveError++;
                    if (consecutiveError > 10)
                    {
                        consecutiveError = 0;
                        //KeyFrameRequested?.Invoke();

                    }
                }
                else
                {
                    consecutiveError = 0;
                }

                if (statusCode.HasFlag(DecodingState.dsNoParamSets))
                {
                    if (--keyReq < 0)
                    {
#if Debug
                                Console.WriteLine("KeyFrameRequested");
#endif
                        //jitterBufffer.Discard();
                        KeyFrameRequested?.Invoke();
                        keyReq = 3;
                    }

                }
            }
            catch (Exception e)
            {
                decoder.Dispose();
                decoder = null;
#if Debug
                        Console.WriteLine("DecoderBroken: " + e.Message);
#endif
                return;
            }


        }
    }


    private unsafe void Decode2(byte[] payload, int payloadOffset, int payloadCount, int w, int h)
    {
        lock (changeLock)
        {
            if (IsDisposed())
                return;

            if (decoder == null)
                decoder = H264TranscoderProvider.CreateDecoder();
            try
            {

                if (!imagePool.TryTake(out var rgbImage) || rgbImage == null)
                {
                    rgbImage = new RgbImage(w, h);
                }
                if (rgbImage.Width != w || rgbImage.Height != h)
                {
                    rgbImage.Dispose();
                    rgbImage = new RgbImage(w, h);
                }
                var succ = decoder.Decode(payload, payloadOffset, payloadCount, noDelay: DecodeNoDelay, out DecodingState statusCode, ref rgbImage);
                CheckMarkingFeedback(statusCode);
                if (succ)
                {
                    imrefPool.TryTake(out var imref);
                    if (imref == null)
                        imref = ImageReference.FromRgbImage(rgbImage, ReturnImage);
                    else
                        imref.Update(rgbImage);

                    DecodedFrameAvailable?.Invoke(imref);
                    keyReq = 0;
                    ManageError(statusCode);

                }

                if (statusCode != DecodingState.dsErrorFree)
                {
                    consecutiveError++;
                    if (consecutiveError > 10)
                    {
                        consecutiveError = 0;
                        //KeyFrameRequested?.Invoke();

                    }
                }
                else
                {
                    consecutiveError = 0;
                }
                if (statusCode.HasFlag(DecodingState.dsNoParamSets))
                //if (statusCode.HasFlag(DecodingState.dsNoParamSets))
                {

                    if (--keyReq < 0)
                    {
#if Debug
                                Console.WriteLine("KeyFrameRequested");
#endif
                        jitterBufffer.Discard();
                        KeyFrameRequested?.Invoke();
                        keyReq = 5;
                    }

                }
            }
            catch (Exception e)
            {
                decoder.Dispose();
                decoder = null;
#if Debug
                        Console.WriteLine("DecoderBroken: " + e.Message);
#endif
                return;
            }


        }
    }

    internal void ApplyChanges(int fps, int targetBitrate, int frameWidth, int frameHeight, ConfigType configType = ConfigType.CameraBasic)
    {
        lock (changeLock)
        {
            encoderWidth = (ushort)frameWidth;
            encoderHeight = (ushort)frameHeight;
            encoder.Dispose();

            encoder = H264TranscoderProvider.CreateEncoder(frameWidth, frameHeight, fps: fps, bps: targetBitrate, configType);


        }
    }
    public void SetMarkingFeedback(byte[] buff, int offset, int count)
    {
        SLTRMarkingFeedback fb = new SLTRMarkingFeedback();
        fb.uiFeedbackType = PrimitiveEncoder.ReadUInt32(buff, ref offset);
        fb.uiIDRPicId = PrimitiveEncoder.ReadUInt32(buff, ref offset);
        fb.iLTRFrameNum = PrimitiveEncoder.ReadInt32(buff, ref offset);
        fb.iLayerId = PrimitiveEncoder.ReadInt32(buff, ref offset);
        encoder.SetOption(ENCODER_OPTION.ENCODER_LTR_MARKING_FEEDBACK, fb);
    }

    public void SetLTRRecoverRequest(byte[] buff, int offset, int count)
    {
        SLTRRecoverRequest rr = new SLTRRecoverRequest();
        rr.uiFeedbackType = PrimitiveEncoder.ReadUInt32(buff, ref offset);
        rr.uiIDRPicId = PrimitiveEncoder.ReadUInt32(buff, ref offset);
        rr.iLastCorrectFrameNum = PrimitiveEncoder.ReadInt32(buff, ref offset);
        rr.iCurrentFrameNum = PrimitiveEncoder.ReadInt32(buff, ref offset);
        rr.iLayerId = PrimitiveEncoder.ReadInt32(buff, ref offset);
        encoder.SetOption(ENCODER_OPTION.ENCODER_LTR_RECOVERY_REQUEST, rr);

    }
    private void CheckMarkingFeedback(DecodingState ds)
    {
        decoder.GetOption(DECODER_OPTION.DECODER_OPTION_LTR_MARKING_FLAG, out int flag);
        if (flag == 1)
        {
            //mark feedback

            var fb = new SLTRMarkingFeedback();
            fb.uiFeedbackType = (/*ds == DecodingState.dsErrorFree*/true) ?
                  (uint)(KEY_FRAME_REQUEST_TYPE.LTR_MARKING_SUCCESS)
                : (uint)(KEY_FRAME_REQUEST_TYPE.LTR_MARKING_FAILED);

            decoder.GetOption(DECODER_OPTION.DECODER_OPTION_IDR_PIC_ID, out uint pid);
            fb.uiIDRPicId = pid;
            decoder.GetOption(DECODER_OPTION.DECODER_OPTION_LTR_MARKED_FRAME_NUM, out int fnum);
            fb.iLTRFrameNum = fnum;


            int offset = 0;
            var buff = BufferPool.RentBuffer(300);
            PrimitiveEncoder.WriteUInt32(buff, ref offset, fb.uiFeedbackType);
            PrimitiveEncoder.WriteUInt32(buff, ref offset, fb.uiIDRPicId);
            //PrimitiveEncoder.WriteInt32(buff, ref offset, fb.iLTRFrameNum);
            //PrimitiveEncoder.WriteInt32(buff, ref offset, fb.iLayerId);


            MarkingFeedback?.Invoke(buff, 0, offset);
            BufferPool.ReturnBuffer(buff);
            //encoder.SetOption(ENCODER_OPTION.ENCODER_LTR_MARKING_FEEDBACK, fb);


        }

    }
    SLTRRecoverRequest recoverRequest = new SLTRRecoverRequest();
    private void ManageError(DecodingState ds)
    {
        if (ds == DecodingState.dsErrorFree)
        {
            decoder.GetOption(DECODER_OPTION.DECODER_OPTION_IDR_PIC_ID, out int tempInt);
            if (recoverRequest.uiIDRPicId != tempInt)
            {
                recoverRequest.uiIDRPicId = (uint)tempInt;
                recoverRequest.iLastCorrectFrameNum = -1;
            }

            if (decoder.GetOption(DECODER_OPTION.DECODER_OPTION_FRAME_NUM, out tempInt))
                if (tempInt >= 0)
                {
                    recoverRequest.iLastCorrectFrameNum = tempInt;
                }
        }
        else if (!ds.HasFlag(DecodingState.dsNoParamSets))
        {
            var recoverRequest = new SLTRRecoverRequest();
            recoverRequest.uiFeedbackType = (uint)KEY_FRAME_REQUEST_TYPE.LTR_RECOVERY_REQUEST;

            decoder.GetOption(DECODER_OPTION.DECODER_OPTION_FRAME_NUM, out int currFrame);
            recoverRequest.iCurrentFrameNum = currFrame;

            decoder.GetOption(DECODER_OPTION.DECODER_OPTION_IDR_PIC_ID, out uint picId);
            recoverRequest.uiIDRPicId = picId;

            recoverRequest.iLastCorrectFrameNum = this.recoverRequest.iLastCorrectFrameNum;


            int offset = 0;
            var buff = BufferPool.RentBuffer(300);
            PrimitiveEncoder.WriteUInt32(buff, ref offset, recoverRequest.uiFeedbackType);
            PrimitiveEncoder.WriteUInt32(buff, ref offset, recoverRequest.uiIDRPicId);
            //PrimitiveEncoder.WriteInt32(buff, ref offset, recoverRequest.iLastCorrectFrameNum);
            //PrimitiveEncoder.WriteInt32(buff, ref offset, recoverRequest.iCurrentFrameNum);
            //PrimitiveEncoder.WriteInt32(buff, ref offset, recoverRequest.iLayerId);


            LtrRecoveryRequest?.Invoke(buff, 0, offset);
            BufferPool.ReturnBuffer(buff);
            //encoder.SetOption(ENCODER_OPTION.ENCODER_LTR_RECOVERY_REQUEST, recoverRequest);


        }
        else//not possible
        {
            KeyFrameRequested?.Invoke();

        }
    }
    int isDisposed = 0;
    public bool IsDisposed() => Interlocked.CompareExchange(ref isDisposed, 0, 0) == 1;
    public void Dispose()
    {
        lock (changeLock)
        {
            if (Interlocked.CompareExchange(ref isDisposed, 1, 0) == 0)
            {
                encoder?.Dispose();
                decoder?.Dispose();
                jitterBufffer.Reset();
                jitterBufffer.FrameAvailable = null;
            }

        }

    }

    internal void ReturnImage(ImageReference imref)
    {
        var image = (RgbImage)imref.underlyingData;

        if (imagePool.Count < 2)
            imagePool.Add(image);
        else
            image.Dispose();

        imrefPool.Add(imref);

    }

    internal void FlushPool()
    {
        while (imagePool.TryTake(out var img))
            img?.Dispose();

        imrefPool.Clear();
    }
}
