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
        [TestCase(AdvSourceDataFormat.Format16BitLittleEndianByte, 16, CompressionType.Uncompressed, "4350FE877C63306F215A15410F360C97")]
        [TestCase(AdvSourceDataFormat.Format16BitLittleEndianByte, 16, CompressionType.Lagarith16, "506F016FDF057731C89507A87637F97B")]
        [TestCase(AdvSourceDataFormat.Format16BitLittleEndianByte, 16, CompressionType.QuickLZ, "7567183065FADA1C1DA775CE781C99D6")]
        [TestCase(AdvSourceDataFormat.Format16BitUShort, 16, CompressionType.Uncompressed, "4350FE877C63306F215A15410F360C97")]
        [TestCase(AdvSourceDataFormat.Format16BitUShort, 16, CompressionType.Lagarith16, "506F016FDF057731C89507A87637F97B")]
        [TestCase(AdvSourceDataFormat.Format16BitUShort, 16, CompressionType.QuickLZ, "7567183065FADA1C1DA775CE781C99D6")]
        [TestCase(AdvSourceDataFormat.Format16BitUShort, 12, CompressionType.Uncompressed, "EDE5B23FA48B44D79DCCE6594C48EEBB")]
        [TestCase(AdvSourceDataFormat.Format16BitUShort, 12, CompressionType.Lagarith16, "82557CF3D0AD2CFDA2AA99C90124F6C7")]
        [TestCase(AdvSourceDataFormat.Format16BitUShort, 12, CompressionType.QuickLZ, "BF1AAEC448E4B12A208956D4D21ECC0A")]
        [TestCase(AdvSourceDataFormat.Format16BitUShort, 8, CompressionType.Uncompressed, "BFBB03A7034501DC027FB6DD05ED8BBB")]
        [TestCase(AdvSourceDataFormat.Format16BitUShort, 8, CompressionType.Lagarith16, "705030FC44EC86E2612FEF0C7F6BF918")]
        [TestCase(AdvSourceDataFormat.Format16BitUShort, 8, CompressionType.QuickLZ, "787B13B5916DB799E2D3FEB40EC898FD")]
        [TestCase(AdvSourceDataFormat.Format8BitByte, 8, CompressionType.Uncompressed, "B5A75F11E1B25B335E568B09197AD042")]
        [TestCase(AdvSourceDataFormat.Format8BitByte, 8, CompressionType.Lagarith16, "D3F4B11619EE4B8CB6A87129EBB5DAFF")]
        [TestCase(AdvSourceDataFormat.Format8BitByte, 8, CompressionType.QuickLZ, "A6641407F7FFC7C05FFEF20B5309B5B3")]
        public void TestFileHashesOfZeroTimestampFiles(AdvSourceDataFormat dataFormat, byte dynaBits, CompressionType compression, string expectedHash)
        {
            var fileGen = new AdvGenerator();
            var cfg = BuildZeroTimestampConfig(dataFormat, dynaBits, compression);

            string fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

            if (File.Exists(fileName)) File.Delete(fileName);

            try
            {
                // Generate
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
