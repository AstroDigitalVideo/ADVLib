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
