using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Adv;
using AdvLib.Tests.Generators;
using AdvLib.Tests.Util;
using NUnit.Framework;

namespace AdvLib.Tests.Adv_V2
{
    [TestFixture]
    public class TestV2FileGeneration
    {
        [Test]
        [TestCase(AdvSourceDataFormat.Format16BitLittleEndianByte, 16, CompressionType.Uncompressed)]
        [TestCase(AdvSourceDataFormat.Format16BitLittleEndianByte, 16, CompressionType.Lagarith16)]
        [TestCase(AdvSourceDataFormat.Format16BitLittleEndianByte, 16, CompressionType.QuickLZ)]
        [TestCase(AdvSourceDataFormat.Format16BitUShort, 16, CompressionType.Uncompressed)]
        [TestCase(AdvSourceDataFormat.Format16BitUShort, 16, CompressionType.Lagarith16)]
        [TestCase(AdvSourceDataFormat.Format16BitUShort, 16, CompressionType.QuickLZ)]
        [TestCase(AdvSourceDataFormat.Format16BitUShort, 12, CompressionType.Uncompressed)]
        [TestCase(AdvSourceDataFormat.Format16BitUShort, 12, CompressionType.Lagarith16)]
        [TestCase(AdvSourceDataFormat.Format16BitUShort, 12, CompressionType.QuickLZ)]
        [TestCase(AdvSourceDataFormat.Format16BitUShort, 8, CompressionType.Uncompressed)]
        [TestCase(AdvSourceDataFormat.Format16BitUShort, 8, CompressionType.Lagarith16)]
        [TestCase(AdvSourceDataFormat.Format16BitUShort, 8, CompressionType.QuickLZ)]
        [TestCase(AdvSourceDataFormat.Format8BitByte, 8, CompressionType.Uncompressed)]
        [TestCase(AdvSourceDataFormat.Format8BitByte, 8, CompressionType.Lagarith16)]
        [TestCase(AdvSourceDataFormat.Format8BitByte, 8, CompressionType.QuickLZ)]
        public void TestPixelDeserialization(AdvSourceDataFormat dataFormat, byte dynaBits, CompressionType compression)
        {
            var fileGen = new AdvGenerator();
            var cfg = new AdvGenerationConfig()
            {
                DynaBits = dynaBits,
                SourceFormat = dataFormat,
                NumberOfFrames = 1,
                Compression = compression,
                NormalPixelValue = null
            };

            string fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            if (File.Exists(fileName)) File.Delete(fileName);
            AdvFile2 file = null;
            try
            {
                // Generate
                fileGen.GenerateaAdv_V2(cfg, fileName);

                var hasher = new Hasher();
                string h1 = hasher.CalcMd5(fileName);
                Console.WriteLine("File hash for {0} at {1} bits: {2}", dataFormat, dynaBits, h1);

                // Verify
                using (file = new AdvFile2(fileName))
                {
                    uint[] pixels = file.GetMainFramePixels(0);

                    var imageGenerator = new ImageGenerator();
                    var verified = imageGenerator.VerifyImagePattern1UInt32(pixels, cfg.DynaBits);
                    Assert.IsTrue(verified);
                }
            }
            finally
            {
                try
                {
                    if (file != null) file.Close();
                    if (File.Exists(fileName))
                        File.Delete(fileName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Trace.WriteLine(ex);
                }
            }
        }

        [Test]
        public void TestZeroTimeGeneratedFilesHaveSameHashes()
        {
            var fileGen = new AdvGenerator();
            var cfg = BuildZeroTimestampConfig(AdvSourceDataFormat.Format16BitUShort, 16, CompressionType.Uncompressed);

            string fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            string fileName2 = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            
            if (File.Exists(fileName)) File.Delete(fileName);
            if (File.Exists(fileName2)) File.Delete(fileName2);

            try
            {
                // Generate
                fileGen.GenerateaAdv_V2(cfg, fileName);
                var hasher = new Hasher();
                string h1 = hasher.CalcMd5(fileName);

                fileGen.GenerateaAdv_V2(cfg, fileName2);
                string h2 = hasher.CalcMd5(fileName2);

                // Verify
                Assert.AreEqual(h1, h2);
            }
            finally
            {
                try
                {
                    if (File.Exists(fileName))
                        File.Delete(fileName);

                    if (File.Exists(fileName2))
                        File.Delete(fileName2);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Trace.WriteLine(ex);
                }
            }
        }

        [Test]
        public void TestMetadataTagsAreSavedAndReadCorrectly()
        {
            var fileGen = new AdvGenerator();
            var cfg = BuildZeroTimestampConfig(AdvSourceDataFormat.Format16BitUShort, 16, CompressionType.Uncompressed);

            string fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

            if (File.Exists(fileName)) File.Delete(fileName);

            try
            {
                // Generate
                cfg.MainStreamMetadata.Add("Name1", "Христо");
                cfg.MainStreamMetadata.Add("Name2", "Frédéric");
                cfg.CalibrationStreamMetadata.Add("Name3", "好的茶");
                cfg.UserMetadata.Add("User0", "1234\rabcd\r\n");
                cfg.UserMetadata.Add("UserArb", "1 قيمة");
                cfg.UserMetadata.Add("UserChi", "值1");
                cfg.UserMetadata.Add("UserCyr", "Значение 1");
                cfg.UserMetadata.Add("UserGal", "કિંમત 1");
                cfg.UserMetadata.Add("UserGre", "αξία 1");
                cfg.UserMetadata.Add("UserHeb", "1 ערך");
                cfg.UserMetadata.Add("UserHin", "मान 1");
                cfg.UserMetadata.Add("UserJpn", "値1");
                cfg.UserMetadata.Add("UserKan", "ಮೌಲ್ಯ 1");
                cfg.UserMetadata.Add("UserKaz", "мән 1");
                cfg.UserMetadata.Add("UserKor", "값 1");
                cfg.UserMetadata.Add("UserMal", "മൂല്യം 1");
                cfg.UserMetadata.Add("UserMar", "मूल्य 1");
                cfg.UserMetadata.Add("UserPer", "1 ارزش");
                cfg.UserMetadata.Add("UserTel", "విలువ 1");
                cfg.UserMetadata.Add("UserUrd", "قیمت 1");
                cfg.UserMetadata.Add("UserViet", "giá trị 1");
                
                
                fileGen.GenerateaAdv_V2(cfg, fileName);


                // Verify
                using (var loadedFile = new AdvFile2(fileName))
                {
                    Assert.AreEqual(2, loadedFile.MainSteamInfo.MetadataTags.Count);
                    Assert.IsTrue(loadedFile.MainSteamInfo.MetadataTags.ContainsKey("Name1"));
                    Assert.AreEqual("Христо", loadedFile.MainSteamInfo.MetadataTags["Name1"]);
                    Assert.IsTrue(loadedFile.MainSteamInfo.MetadataTags.ContainsKey("Name2"));
                    Assert.AreEqual("Frédéric", loadedFile.MainSteamInfo.MetadataTags["Name2"]);

                    Assert.AreEqual(1, loadedFile.CalibrationSteamInfo.MetadataTags.Count);
                    Assert.IsTrue(loadedFile.CalibrationSteamInfo.MetadataTags.ContainsKey("Name3"));
                    Assert.AreEqual("好的茶", loadedFile.CalibrationSteamInfo.MetadataTags["Name3"]);

                    Assert.AreEqual(18, loadedFile.UserMetadataTags.Count);
                    Assert.IsTrue(loadedFile.UserMetadataTags.ContainsKey("User0"));
                    Assert.AreEqual("1234\rabcd\r\n", loadedFile.UserMetadataTags["User0"]);
                    Assert.IsTrue(loadedFile.UserMetadataTags.ContainsKey("UserArb"));
                    Assert.AreEqual("1 قيمة", loadedFile.UserMetadataTags["UserArb"]);
                    Assert.IsTrue(loadedFile.UserMetadataTags.ContainsKey("UserChi"));
                    Assert.AreEqual("值1", loadedFile.UserMetadataTags["UserChi"]);
                    Assert.IsTrue(loadedFile.UserMetadataTags.ContainsKey("UserCyr"));
                    Assert.AreEqual("Значение 1", loadedFile.UserMetadataTags["UserCyr"]);
                    Assert.IsTrue(loadedFile.UserMetadataTags.ContainsKey("UserGal"));
                    Assert.AreEqual("કિંમત 1", loadedFile.UserMetadataTags["UserGal"]);
                    Assert.IsTrue(loadedFile.UserMetadataTags.ContainsKey("UserGre"));
                    Assert.AreEqual("αξία 1", loadedFile.UserMetadataTags["UserGre"]);
                    Assert.IsTrue(loadedFile.UserMetadataTags.ContainsKey("UserHeb"));
                    Assert.AreEqual("1 ערך", loadedFile.UserMetadataTags["UserHeb"]);
                    Assert.IsTrue(loadedFile.UserMetadataTags.ContainsKey("UserHin"));
                    Assert.AreEqual("मान 1", loadedFile.UserMetadataTags["UserHin"]);
                    Assert.IsTrue(loadedFile.UserMetadataTags.ContainsKey("UserJpn"));
                    Assert.AreEqual("値1", loadedFile.UserMetadataTags["UserJpn"]);
                    Assert.IsTrue(loadedFile.UserMetadataTags.ContainsKey("UserKan"));
                    Assert.AreEqual("ಮೌಲ್ಯ 1", loadedFile.UserMetadataTags["UserKan"]);
                    Assert.IsTrue(loadedFile.UserMetadataTags.ContainsKey("UserKaz"));
                    Assert.AreEqual("мән 1", loadedFile.UserMetadataTags["UserKaz"]);
                    Assert.IsTrue(loadedFile.UserMetadataTags.ContainsKey("UserKor"));
                    Assert.AreEqual("값 1", loadedFile.UserMetadataTags["UserKor"]);
                    Assert.IsTrue(loadedFile.UserMetadataTags.ContainsKey("UserMal"));
                    Assert.AreEqual("മൂല്യം 1", loadedFile.UserMetadataTags["UserMal"]);
                    Assert.IsTrue(loadedFile.UserMetadataTags.ContainsKey("UserMar"));
                    Assert.AreEqual("मूल्य 1", loadedFile.UserMetadataTags["UserMar"]);
                    Assert.IsTrue(loadedFile.UserMetadataTags.ContainsKey("UserPer"));
                    Assert.AreEqual("1 ارزش", loadedFile.UserMetadataTags["UserPer"]);
                    Assert.IsTrue(loadedFile.UserMetadataTags.ContainsKey("UserTel"));
                    Assert.AreEqual("విలువ 1", loadedFile.UserMetadataTags["UserTel"]);
                    Assert.IsTrue(loadedFile.UserMetadataTags.ContainsKey("UserUrd"));
                    Assert.AreEqual("قیمت 1", loadedFile.UserMetadataTags["UserUrd"]);
                    Assert.IsTrue(loadedFile.UserMetadataTags.ContainsKey("UserViet"));
                    Assert.AreEqual("giá trị 1", loadedFile.UserMetadataTags["UserViet"]);
                }
            }
            finally
            {
                try
                {
                    if (File.Exists(fileName))
                        File.Delete(fileName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Trace.WriteLine(ex);
                }
            }
        }

        [Test]
        public void TestTimestampsAreSavedAndReadCorrectly()
        {
            DateTime frameTimeStamp = new DateTime(2016, 6, 24, 20, 42, 15).AddMilliseconds(1234);
            long tickStamp = frameTimeStamp.Ticks;
            float exposureMS = 16.7f;
            DateTime frameTimeStamp2 = frameTimeStamp.AddMilliseconds(17);
            long tickStamp2 = frameTimeStamp2.Ticks;

            var utcTimeStamps = new DateTime[] { frameTimeStamp, frameTimeStamp2 };
            var tickStamps = new long[] { tickStamp, tickStamp2 };

            AdvTimeStamp ts = AdvTimeStamp.FromDateTime(frameTimeStamp);
            var tdBackFromMS = new DateTime((long) AdvTimeStamp.ADV_EPOCH_ZERO_TICKS).AddMilliseconds(ts.MillisecondsAfterAdvZeroEpoch);
            Assert.AreEqual(frameTimeStamp.Ticks, tdBackFromMS.Ticks);

            Assert.AreEqual(ts.MillisecondsAfterAdvZeroEpoch, ts.NanosecondsAfterAdvZeroEpoch / 1000000);

            var tdBackFromNS = new DateTime((long)AdvTimeStamp.ADV_EPOCH_ZERO_TICKS).AddMilliseconds(ts.NanosecondsAfterAdvZeroEpoch / 1000000.0);
            Assert.AreEqual(frameTimeStamp.Ticks, tdBackFromNS.Ticks);

            var maxTimeStamp = new DateTime((long)AdvTimeStamp.ADV_EPOCH_ZERO_TICKS).AddMilliseconds(ulong.MaxValue / 1000000.0);
            Console.WriteLine(string.Format("Max ADV UTC Timestamp: {0}", maxTimeStamp.ToString("yyyy-MMM-dd HH:mm:ss")));

            Assert.AreEqual(frameTimeStamp.Ticks, new DateTime((long)AdvTimeStamp.ADV_EPOCH_ZERO_TICKS).AddMilliseconds(204496936234000000 / 1000000.0).Ticks);

            var fileGen = new AdvGenerator();

            int tickId = -1;
            var cfg = new AdvGenerationConfig()
            {
                DynaBits = 16,
                SourceFormat = AdvSourceDataFormat.Format16BitUShort,
                NumberOfFrames = 2,
                Compression = CompressionType.Uncompressed,
                NormalPixelValue = null,
                MainStreamCustomClock = new CustomClockConfig()
                {
                    ClockFrequency = 10000000,
                    ClockTicksCallback = () => { tickId++; return tickStamps[tickId]; },
                    TicksTimingAccuracy = 1
                },
                CalibrationStreamCustomClock = new CustomClockConfig()
                {
                    ClockFrequency = 10000000,
                    ClockTicksCallback = () => 0,
                    TicksTimingAccuracy = 1
                },
                TimeStampCallback = new GetCurrentImageTimeStampCallback((frameId) => utcTimeStamps[frameId]),
                ExposureCallback = id => (uint)(exposureMS * 1000000.0)
            };

            string fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

            if (File.Exists(fileName)) File.Delete(fileName);

            try
            {
                // Generate
                fileGen.GenerateaAdv_V2(cfg, fileName);

                // Verify
                using (var loadedFile = new AdvFile2(fileName))
                {
                    AdvFrameInfo frameInfo;
                    loadedFile.GetMainFramePixels(0, out frameInfo);

                    Assert.IsNotNull(frameInfo);
                    Assert.IsTrue(frameInfo.HasUtcTimeStamp);
                    Assert.AreEqual(frameTimeStamp.Ticks, frameInfo.UtcStartExposureTimeStamp.Ticks);
                    Assert.AreEqual(exposureMS, frameInfo.UtcExposureMilliseconds, 0.00001);

                    Assert.AreEqual(0, frameInfo.TickStampStartTicks);
                    Assert.AreEqual(tickStamp, frameInfo.TickStampEndTicks);

                    loadedFile.GetMainFramePixels(1, out frameInfo);

                    Assert.IsNotNull(frameInfo);
                    Assert.IsTrue(frameInfo.HasUtcTimeStamp);
                    Assert.AreEqual(frameTimeStamp2.Ticks, frameInfo.UtcStartExposureTimeStamp.Ticks);
                    Assert.AreEqual(exposureMS, frameInfo.UtcExposureMilliseconds, 0.00001);

                    Assert.AreEqual(tickStamp, frameInfo.TickStampStartTicks);
                    Assert.AreEqual(tickStamp2, frameInfo.TickStampEndTicks);
                }
            }
            finally
            {
                try
                {
                    if (File.Exists(fileName))
                        File.Delete(fileName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Trace.WriteLine(ex);
                }
            }
        }

        [Test]
        public void TestStatusTagsAreSavedAndReadCorrectly()
        {
            string fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

            if (File.Exists(fileName)) File.Delete(fileName);

            try
            {
                // Generate
                var recorder = new AdvRecorder();
                recorder.ImageConfig.SetImageParameters(640, 480, 16, 0);

                recorder.FileMetaData.RecorderSoftwareName = "AdvLibTestRecorder";
                recorder.FileMetaData.RecorderSoftwareVersion = "x.y.z";
                recorder.FileMetaData.RecorderHardwareName = "a.b.c";
                recorder.FileMetaData.CameraModel = "TestCamera";
                recorder.FileMetaData.CameraSensorInfo = "TestSensor";


                recorder.StatusSectionConfig.RecordGain = true;
                recorder.StatusSectionConfig.RecordGamma = true;
                recorder.StatusSectionConfig.RecordShutter = true;
                recorder.StatusSectionConfig.RecordCameraOffset = true;
                recorder.StatusSectionConfig.RecordSystemTime = true;
                recorder.StatusSectionConfig.RecordTrackedSatellites = true;
                recorder.StatusSectionConfig.RecordAlmanacStatus = true;
                recorder.StatusSectionConfig.RecordAlmanacOffset = true;
                recorder.StatusSectionConfig.RecordFixStatus = true;
                recorder.StatusSectionConfig.RecordSystemErrors = true;
                recorder.StatusSectionConfig.RecordVideoCameraFrameId = true;
                recorder.StatusSectionConfig.RecordHardwareTimerFrameId = true;

                recorder.StatusSectionConfig.AddDefineTag("CustomInt8", Adv2TagType.Int8);
                recorder.StatusSectionConfig.AddDefineTag("CustomInt16", Adv2TagType.Int16);
                recorder.StatusSectionConfig.AddDefineTag("CustomInt32", Adv2TagType.Int32);
                recorder.StatusSectionConfig.AddDefineTag("CustomLong64", Adv2TagType.Long64);
                recorder.StatusSectionConfig.AddDefineTag("CustomReal", Adv2TagType.Real);
                recorder.StatusSectionConfig.AddDefineTag("CustomString", Adv2TagType.UTF8String);

                recorder.StartRecordingNewFile(fileName, 0);

                var systemTimeStamp = DateTime.Now.AddMilliseconds(123);

                var status = new AdvRecorder.AdvStatusEntry()
                {
                    AlmanacStatus = AlmanacStatus.Good,
                    AlmanacOffset = 14,
                    TrackedSatellites = 8,
                    CameraOffset = 8.23f,
                    FixStatus = FixStatus.PFix,
                    Gain = 32.82f,
                    Gamma = 0.35f,
                    Shutter = 2.502f,
                    SystemTime = AdvTimeStamp.FromDateTime(systemTimeStamp),
                    VideoCameraFrameId = 19289232,
                    HardwareTimerFrameId = 9102
                };

                status.AdditionalStatusTags = new object[]
                {
                    (byte)12, (short)-123, (int)192847, -1 * (long)(0x6E9104B012CD110F), 91.291823f, "Значение 1"  
                };

                var imageGenerator = new ImageGenerator();
                ushort[] imagePixels = imageGenerator.GetCurrentImageBytesInt16(0, 16);

                recorder.AddVideoFrame(
                    imagePixels, false, null,
                    AdvTimeStamp.FromDateTime(DateTime.Now), 
                    AdvTimeStamp.FromDateTime(DateTime.Now.AddSeconds(2.56)),
                    status, AdvImageData.PixelDepth16Bit);
                
                recorder.FinishRecording();

                // Verify
                using (var loadedFile = new AdvFile2(fileName))
                {
                    AdvFrameInfo frameInfo;
                    loadedFile.GetMainFramePixels(0, out frameInfo);

                    Assert.AreEqual(status.Gain, frameInfo.Gain, 0.000001);
                    Assert.AreEqual(status.Gamma, frameInfo.Gamma, 0.000001);
                    Assert.AreEqual(status.Shutter, frameInfo.Shutter, 0.000001);
                    Assert.AreEqual(status.CameraOffset, frameInfo.Offset, 0.000001);
                    Assert.AreEqual(status.FixStatus, (FixStatus)frameInfo.GPSFixStatus);
                    Assert.AreEqual(status.AlmanacStatus, (AlmanacStatus)frameInfo.GPSAlmanacStatus);
                    Assert.AreEqual(status.TrackedSatellites, frameInfo.GPSTrackedSattelites);
                    Assert.AreEqual(status.AlmanacOffset, frameInfo.GPSAlmanacOffset);
                    Assert.AreEqual(status.VideoCameraFrameId, frameInfo.VideoCameraFrameId);
                    Assert.AreEqual(status.HardwareTimerFrameId, frameInfo.HardwareTimerFrameId);
                    Assert.AreEqual(systemTimeStamp.Ticks, frameInfo.SystemTimestamp.Ticks);

                    Assert.AreEqual(status.AdditionalStatusTags[0], frameInfo.Status["CustomInt8"]);
                    Assert.AreEqual(status.AdditionalStatusTags[1], frameInfo.Status["CustomInt16"]);
                    Assert.AreEqual(status.AdditionalStatusTags[2], frameInfo.Status["CustomInt32"]);
                    Assert.AreEqual(status.AdditionalStatusTags[3], frameInfo.Status["CustomLong64"]);
                    Assert.AreEqual(status.AdditionalStatusTags[4], frameInfo.Status["CustomReal"]);
                    Assert.AreEqual(status.AdditionalStatusTags[5], frameInfo.Status["CustomString"]);
                }
            }
            finally
            {
                try
                {
                    if (File.Exists(fileName))
                        File.Delete(fileName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Trace.WriteLine(ex);
                }
            }
        }

        private AdvGenerationConfig BuildZeroTimestampConfig(AdvSourceDataFormat dataFormat, byte dynaBits, CompressionType compression)
        {
            return new AdvGenerationConfig()
            {
                DynaBits = dynaBits,
                SourceFormat = dataFormat,
                NumberOfFrames = 1,
                Compression = compression,
                NormalPixelValue = null,
                MainStreamCustomClock = new CustomClockConfig()
                {
                    ClockFrequency = 1,
                    ClockTicksCallback = () => 0,
                    TicksTimingAccuracy = 1
                },
                CalibrationStreamCustomClock = new CustomClockConfig()
                {
                    ClockFrequency = 1,
                    ClockTicksCallback = () => 0,
                    TicksTimingAccuracy = 1
                },
                TimeStampCallback = new GetCurrentImageTimeStampCallback((frameId) => DateTime.MinValue)
            };
        }


		[Test]
		[TestCase(AdvSourceDataFormat.Format16BitLittleEndianByte, 16, CompressionType.Uncompressed, "0D84C84AA463630603707CDA4523F4C6")]
		[TestCase(AdvSourceDataFormat.Format16BitLittleEndianByte, 16, CompressionType.QuickLZ, "0C4EC5D518D85CEBB2FF5C8A133F131E")]
		[TestCase(AdvSourceDataFormat.Format16BitUShort, 16, CompressionType.Uncompressed, "0D84C84AA463630603707CDA4523F4C6")]
		[TestCase(AdvSourceDataFormat.Format16BitUShort, 16, CompressionType.QuickLZ, "0C4EC5D518D85CEBB2FF5C8A133F131E")]
		[TestCase(AdvSourceDataFormat.Format16BitUShort, 12, CompressionType.Uncompressed, "881D22CD0C836D8AE04ECFBFC3BC904A")]
		[TestCase(AdvSourceDataFormat.Format16BitUShort, 12, CompressionType.QuickLZ, "BA49B36DD5A49F22933FD8403D36FFB5")]
		[TestCase(AdvSourceDataFormat.Format16BitUShort, 8, CompressionType.Uncompressed, "564D184F2FE38268C0B044695F7A57FE")]
		[TestCase(AdvSourceDataFormat.Format16BitUShort, 8, CompressionType.QuickLZ, "9442BFC815E41B9AC3D790BB323636E9")]
		[TestCase(AdvSourceDataFormat.Format8BitByte, 8, CompressionType.Uncompressed, "864750F8D86DB745BAF791B0BE4B7478")]
		[TestCase(AdvSourceDataFormat.Format8BitByte, 8, CompressionType.QuickLZ, "F329344DAFC35C37E9500C44C901301D")]
#if !__linux__
		[TestCase(AdvSourceDataFormat.Format16BitLittleEndianByte, 16, CompressionType.Lagarith16, "2B137AE29578EF9803FD941DDA5BCF90")]
		[TestCase(AdvSourceDataFormat.Format16BitUShort, 16, CompressionType.Lagarith16, "2B137AE29578EF9803FD941DDA5BCF90")]
		[TestCase(AdvSourceDataFormat.Format16BitUShort, 12, CompressionType.Lagarith16, "1D4D1A051EAC7C3544CD9230A208EC37")]
		[TestCase(AdvSourceDataFormat.Format16BitUShort, 8, CompressionType.Lagarith16, "9654F23B16D341B617E33E3A8C42BA6A")]
		[TestCase(AdvSourceDataFormat.Format8BitByte, 8, CompressionType.Lagarith16, "5DBDE91E67E87D64DA46885C028CCC1D")]
#endif
        public void TestFileHashesOfZeroTimestampFiles(AdvSourceDataFormat dataFormat, byte dynaBits, CompressionType compression, string expectedHash)
        {
            var fileGen = new AdvGenerator();
            var cfg = BuildZeroTimestampConfig(dataFormat, dynaBits, compression);

            string fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

            if (File.Exists(fileName)) File.Delete(fileName);

            try
            {
                // Generate
                cfg.SaveCustomStatusSectionTags = true;
                fileGen.GenerateaAdv_V2(cfg, fileName);
                var hasher = new Hasher();
                string h1 = hasher.CalcMd5(fileName);

                // Verify
                Assert.AreEqual(expectedHash, h1);
            }
            finally
            {
                try
                {
                    if (File.Exists(fileName))
                        File.Delete(fileName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Trace.WriteLine(ex);
                }
            }            
        }
    }
}
