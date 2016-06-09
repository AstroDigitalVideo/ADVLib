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
        Format16BitUShort
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

    public class AdvGenerationConfig
    {
        public bool SaveLocationData;
        public byte DynaBits;
        public int? NormalPixelValue;
        public AdvSourceDataFormat SourceFormat;
        public bool UsesCompression;
        public int NumberOfFrames;

        public GetCurrentImageExposureCallback ExposureCallback;
        public GetCurrentImageTimeStampCallback TimeStampCallback;
        public GetCurrentImageGammaCallback GammaCallback;
        public GetCurrentImageGainCallback GainCallback;
        public GetCurrentExampleMassagesCallback MassagesCallback;
        public GetCurrentExampleCustomGainCallback CustomGainCallback;

        public CustomClockConfig MainStreamCustomClock;
        public CustomClockConfig CalibrationStreamCustomClock;
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

            // Then define additional metadata, if required
            recorder.FileMetaData.AddUserTag("TELESCOPE-NAME", "Large Telescope");
            recorder.FileMetaData.AddUserTag("TELESCOPE-FL", "8300");
            recorder.FileMetaData.AddUserTag("TELESCOPE-FD", "6.5");
            recorder.FileMetaData.AddUserTag("CAMERA-DIGITAL-SAMPLIG", "xxx");
            recorder.FileMetaData.AddUserTag("CAMERA-HDR-RESPONSE", "yyy");
            recorder.FileMetaData.AddUserTag("CAMERA-OPTICAL-RESOLUTION", "zzz");

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

            recorder.StartRecordingNewFile(fileName);

            AdvRecorder.AdvStatusEntry status = new AdvRecorder.AdvStatusEntry();
            status.AdditionalStatusTags = new object[2];

            for (int i = 0; i < config.NumberOfFrames; i++)
            {
                // NOTE: Get the test data
                uint exposure = config.ExposureCallback != null ? config.ExposureCallback(i) : 0;
                DateTime startTimestamp = config.TimeStampCallback != null ? config.TimeStampCallback(i) : DateTime.Now;
                DateTime endTimestamp = startTimestamp.AddMilliseconds(exposure / 10.0);
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
                        config.UsesCompression,

                        AdvTimeStamp.FromDateTime(startTimestamp),
                        AdvTimeStamp.FromDateTime(endTimestamp),
                        status);
                }
                else if (config.SourceFormat == AdvSourceDataFormat.Format16BitLittleEndianByte)
                {
                    byte[] imageBytes = imageGenerator.GetCurrentImageBytes(i, config.DynaBits);

                    recorder.AddVideoFrame(
                        imageBytes,

                        // NOTE: Use with caution! Using compression is slower and may not work at high frame rates 
                        // i.e. it may take longer to compress the data than for the next image to arrive on the buffer
                        config.UsesCompression,

                        AdvTimeStamp.FromDateTime(startTimestamp),
                        AdvTimeStamp.FromDateTime(endTimestamp),
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
                        config.UsesCompression,

                        AdvTimeStamp.FromDateTime(startTimestamp),
                        AdvTimeStamp.FromDateTime(endTimestamp),
                        status,

                        AdvImageData.PixelDepth16Bit);
                }
                else if (config.SourceFormat == AdvSourceDataFormat.Format8BitByte)
                {
                    byte[] imageBytes = imageGenerator.GetCurrentImageBytes(i, config.DynaBits);

                    recorder.AddVideoFrame(
                        imageBytes,

                        // NOTE: Use with caution! Using compression is slower and may not work at high frame rates 
                        // i.e. it may take longer to compress the data than for the next image to arrive on the buffer
                        config.UsesCompression,

                        AdvTimeStamp.FromDateTime(startTimestamp),
                        AdvTimeStamp.FromDateTime(endTimestamp),
                        status,

                        AdvImageData.PixelDepth8Bit);
                }
            }

            recorder.FinishRecording();
        }
    }
}
