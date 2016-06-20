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
		        }
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

        //[Test]
        //[TestCase(@"TestFiles\UNCOMPRESSED\TestFile.Win32.GNU.adv")]
        //[TestCase(@"TestFiles\UNCOMPRESSED\TestFile.Win64.GNU.adv")]
        //[TestCase(@"TestFiles\UNCOMPRESSED\TestFile.Win32.MSVC.adv")]
        //[TestCase(@"TestFiles\UNCOMPRESSED\TestFile.Win64.MSVC.adv")]
        //[TestCase(@"TestFiles\UNCOMPRESSED\TestFile.Ubuntu32.GNU.adv")]
        //[TestCase(@"TestFiles\UNCOMPRESSED\TestFile.Ubuntu64.GNU.adv")]
        //public void ReadLinuxFile1(string fileName)
        //{
        //    fileName = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\" + fileName);
        //    var hasher = new Hasher();
        //    string h1 = hasher.CalcMd5(fileName);
        //    Console.WriteLine(h1);

        //    using (var file = new AdvFile2(fileName))
        //    {
        //        Console.WriteLine("MainSteamInfo.FrameCount: " + file.MainSteamInfo.FrameCount);
        //        Console.WriteLine("CalibrationSteamInfo.FrameCount: " + file.CalibrationSteamInfo.FrameCount);                
        //    }
        //}

        //[Test]
        //public void CompareFiles()
        //{
        //    var hasher = new Hasher();
        //    string h1 = hasher.CalcMd5(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\" + @"TestFiles\UNCOMPRESSED\TestFile.Win32.GNU.adv"));
        //    string h2 = hasher.CalcMd5(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\" + @"TestFiles\UNCOMPRESSED\TestFile.Win32.MSVC.adv"));
        //    string h3 = hasher.CalcMd5(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\" + @"TestFiles\UNCOMPRESSED\TestFile.Win64.GNU.adv"));
        //    string h4 = hasher.CalcMd5(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\" + @"TestFiles\UNCOMPRESSED\TestFile.Win64.MSVC.adv"));
        //    string h5 = hasher.CalcMd5(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\" + @"TestFiles\UNCOMPRESSED\TestFile.Ubuntu32.GNU.adv"));
        //    string h6 = hasher.CalcMd5(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\" + @"TestFiles\UNCOMPRESSED\TestFile.Ubuntu64.GNU.adv"));
        //    Console.WriteLine(h1);
        //    Console.WriteLine(h2);
        //    Console.WriteLine(h3);
        //    Console.WriteLine(h4);
        //    Assert.AreEqual(h1, h2);
        //    Assert.AreEqual(h1, h3);
        //    Assert.AreEqual(h1, h4);
        //    Assert.AreEqual(h1, h5);
        //    Assert.AreEqual(h1, h6);
        //}

    }
}
