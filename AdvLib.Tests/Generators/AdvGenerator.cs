using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adv;

namespace AdvLib.Tests.Generators
{
    public enum AdvSourceDataFormat
    {
        Format8BitByte,
        Format12BitPackedByte,
        Format16BitLittleEndianByte,
        Format16BitUShort,
        Format24BitColour
    }

    public delegate uint GetCurrentImageExposureCallback(int frameId);

    public delegate DateTime GetCurrentImageTimeStampCallback(int frameId);

    public delegate float GetCurrentImageGammaCallback(int frameId);

    public delegate float GetCurrentImageGainCallback(int frameId);

    public delegate string[] GetCurrentExampleMassagesCallback(int frameId);

    public delegate uint GetCurrentExampleCustomGainCallback(int frameId);

    public class CustomClockConfig
    {
        public Func<long> ClockTicksCallback;
        public long ClockFrequency;
        public int TicksTimingAccuracy;
    }

    public enum CompressionType
    {
        Uncompressed,
        QuickLZ,
        Lagarith16
    }

    public class AdvGenerationConfig
    {
        public bool SaveLocationData;
        public byte DynaBits;
        public int? NormalPixelValue;
        public AdvSourceDataFormat SourceFormat;
        public BayerPattern? BayerPattern;
        public CompressionType Compression;
        public int NumberOfFrames;

        public long UtcTimestampAccuracyInNanoseconds;

        public GetCurrentImageExposureCallback ExposureCallback;
        public GetCurrentImageTimeStampCallback TimeStampCallback;
        public GetCurrentImageGammaCallback GammaCallback;
        public GetCurrentImageGainCallback GainCallback;
        public GetCurrentExampleMassagesCallback MassagesCallback;
        public GetCurrentExampleCustomGainCallback CustomGainCallback;

        public CustomClockConfig MainStreamCustomClock;
        public CustomClockConfig CalibrationStreamCustomClock;

        public Dictionary<string, string> MainStreamMetadata = new Dictionary<string, string>();
        public Dictionary<string, string> CalibrationStreamMetadata = new Dictionary<string, string>();
        public Dictionary<string, string> UserMetadata = new Dictionary<string, string>();
    }

    public class AdvGenerator
    {
        private ImageGenerator imageGenerator = new ImageGenerator();

        public void GenerateaAdv_V2(AdvGenerationConfig config, string fileName)
        {
            var recorder = new AdvRecorder();

            // First set the values of the standard file metadata
            recorder.FileMetaData.RecorderSoftwareName = "AdvLibRecorder";
            recorder.FileMetaData.RecorderSoftwareVersion = "x.y.z";
            recorder.FileMetaData.RecorderHardwareName = "a.b.c";

            recorder.FileMetaData.CameraModel = "Flea3 FL3-FW-03S3M";
            recorder.FileMetaData.CameraSensorInfo = "Sony ICX414AL (1/2\" 648x488 CCD)";

            if (config.SaveLocationData)
            {
                recorder.LocationData.SetLocation(
                    150 + 38 / 60.0 + 27.7 / 3600.0,
                    -1 * (33 + 39 / 60.0 + 49.3 / 3600.0),
                    284.4);
            }

            recorder.ImageConfig.SetImageParameters(640, 480, config.DynaBits, config.NormalPixelValue);

            // By default no status section values will be recorded. The user must enable the ones they need recorded and 
            // can also define additional status parameters to be recorded with each video frame
            recorder.StatusSectionConfig.RecordGain = true;
            recorder.StatusSectionConfig.RecordGamma = true;
            int customTagIdCustomGain = recorder.StatusSectionConfig.AddDefineTag("EXAMPLE-GAIN", AdvTagType.UInt32);
            int customTagIdMessages = recorder.StatusSectionConfig.AddDefineTag("EXAMPLE-MESSAGES", AdvTagType.List16OfAnsiString255);

            if (config.MainStreamCustomClock != null)
                recorder.DefineCustomClock(AdvRecorder.AdvStream.MainStream, config.MainStreamCustomClock.ClockFrequency, config.MainStreamCustomClock.TicksTimingAccuracy, config.MainStreamCustomClock.ClockTicksCallback);

            if (config.CalibrationStreamCustomClock != null)
                recorder.DefineCustomClock(AdvRecorder.AdvStream.CalibrationStream, config.CalibrationStreamCustomClock.ClockFrequency, config.CalibrationStreamCustomClock.TicksTimingAccuracy, config.CalibrationStreamCustomClock.ClockTicksCallback);

            if (config.BayerPattern != null)
                recorder.ImageConfig.SetBayerPattern(config.BayerPattern.Value);

            foreach (string key in config.MainStreamMetadata.Keys)
                recorder.FileMetaData.AddMainStreamTag(key, config.MainStreamMetadata[key]);

            foreach (string key in config.CalibrationStreamMetadata.Keys)
                recorder.FileMetaData.AddCalibrationStreamTag(key, config.CalibrationStreamMetadata[key]);

            foreach (string key in config.UserMetadata.Keys)
                recorder.FileMetaData.AddUserTag(key, config.UserMetadata[key]);

            recorder.StartRecordingNewFile(fileName, config.UtcTimestampAccuracyInNanoseconds);

            AdvRecorder.AdvStatusEntry status = new AdvRecorder.AdvStatusEntry();
            status.AdditionalStatusTags = new object[2];

            for (int i = 0; i < config.NumberOfFrames; i++)
            {
                // NOTE: Get the test data
                uint exposure = config.ExposureCallback != null ? config.ExposureCallback(i) : 0;
                DateTime startTimestamp = config.TimeStampCallback != null ? config.TimeStampCallback(i) : DateTime.Now;
                var utcStart = AdvTimeStamp.FromDateTime(startTimestamp);
                var utcEnd = utcStart.AddNanoseconds(exposure);

                status.Gain = config.GainCallback != null ? config.GainCallback(i) : 0;
                status.Gamma = config.GammaCallback != null ? config.GammaCallback(i) : 0;
                status.AdditionalStatusTags[customTagIdMessages] = config.MassagesCallback != null ? config.MassagesCallback(i) : null;
                status.AdditionalStatusTags[customTagIdCustomGain] = config.CustomGainCallback != null ? config.CustomGainCallback(i) : 0;

                
                if (config.SourceFormat == AdvSourceDataFormat.Format16BitUShort)
                {
                    ushort[] imagePixels = imageGenerator.GetCurrentImageBytesInt16(i, config.DynaBits);

                    recorder.AddVideoFrame(
                        imagePixels,

                        // NOTE: Use with caution! Using compression is slower and may not work at high frame rates 
                        // i.e. it may take longer to compress the data than for the next image to arrive on the buffer
                        config.Compression != CompressionType.Uncompressed,

                        config.Compression == CompressionType.Lagarith16 ? PreferredCompression.Lagarith16 : PreferredCompression.QuickLZ,

                        utcStart,
                        utcEnd,
                        status,
                        AdvImageData.PixelDepth16Bit);
                }
                else if (config.SourceFormat == AdvSourceDataFormat.Format16BitLittleEndianByte)
                {
                    byte[] imageBytes = imageGenerator.GetCurrentImageBytes(i, config.DynaBits);

                    recorder.AddVideoFrame(
                        imageBytes,

                        // NOTE: Use with caution! Using compression is slower and may not work at high frame rates 
                        // i.e. it may take longer to compress the data than for the next image to arrive on the buffer
                        config.Compression != CompressionType.Uncompressed,

                        config.Compression == CompressionType.Lagarith16 ? PreferredCompression.Lagarith16 : PreferredCompression.QuickLZ,

                        utcStart,
                        utcEnd,
                        status,

                        AdvImageData.PixelDepth16Bit);
                }
                else if (config.SourceFormat == AdvSourceDataFormat.Format12BitPackedByte)
                {
                    byte[] imageBytes = imageGenerator.GetCurrentImageBytes(i, config.DynaBits);

                    recorder.AddVideoFrame(
                        imageBytes,

                        // NOTE: Use with caution! Using compression is slower and may not work at high frame rates 
                        // i.e. it may take longer to compress the data than for the next image to arrive on the buffer
                        config.Compression != CompressionType.Uncompressed,

                        config.Compression == CompressionType.Lagarith16 ? PreferredCompression.Lagarith16 : PreferredCompression.QuickLZ,

                        utcStart,
                        utcEnd,
                        status,

                        AdvImageData.PixelData12Bit);
                }
                else if (config.SourceFormat == AdvSourceDataFormat.Format8BitByte)
                {
                    byte[] imageBytes = imageGenerator.GetCurrentImageBytes(i, config.DynaBits);

                    recorder.AddVideoFrame(
                        imageBytes,

                        // NOTE: Use with caution! Using compression is slower and may not work at high frame rates 
                        // i.e. it may take longer to compress the data than for the next image to arrive on the buffer
                        config.Compression != CompressionType.Uncompressed,

                        config.Compression == CompressionType.Lagarith16 ? PreferredCompression.Lagarith16 : PreferredCompression.QuickLZ,

                        utcStart,
                        utcEnd,
                        status,

                        AdvImageData.PixelDepth8Bit);
                }
                else if (config.SourceFormat == AdvSourceDataFormat.Format24BitColour)
                {
                    throw new NotImplementedException();
                }
            }

            recorder.FinishRecording();
        }

        public void GenerateSpecExampleFile(string fileName)
        {
            Adv.AdvLib.NewFile(fileName);

            Adv.AdvLib.DefineExternalClockForMainStream(76900, 77);
            Adv.AdvLib.DefineExternalClockForCalibrationStream(76900, 77);

            Adv.AdvLib.AddMainStreamTag("Name1", "Христо");
            Adv.AdvLib.AddMainStreamTag("Name2", "Frédéric");
            Adv.AdvLib.AddCalibrationStreamTag("Name1", "好的茶");

            Adv.AdvLib.DefineImageSection(640, 480, 16);
            Adv.AdvLib.DefineStatusSection(5000000 /* 5ms */);
            Adv.AdvLib.DefineImageLayout(0, "FULL-IMAGE-RAW", "UNCOMPRESSED", 16);

            Adv.AdvLib.AddUserTag("Example", "Value");

            Adv.AdvLib.BeginFrame(0, 0, 0, 0, 0, 0);
            Adv.AdvLib.EndFile();
            
        }
    }
}
