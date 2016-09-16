using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Adv;
using NUnit.Framework;


namespace Samples
{
    [TestFixture]
    public class SampleFilesGeneration
    {
        private string fileName;

        [SetUp]
        public void Setup()
        {
            fileName = Path.GetTempFileName();
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        [Test]
        public void SampleAdvFileRecording()
        {
            fileName = @"C:\hello-world.adv";

            const uint MILLI_TO_NANO = 1000000;
            const byte IMAGE_LAYOUT_ID = 1;
            const int WIDTH = 800;
            const int HEIGHT = 600;

            AdvError.ShowMessageBoxErrorMessage = true;

            AdvError.Check(Adv.AdvLib.NewFile(fileName));
            AdvError.Check(Adv.AdvLib.AddOrUpdateFileTag("FSTF-TYPE", "ADV"));
            AdvError.Check(Adv.AdvLib.AddOrUpdateFileTag("ADV-VERSION", "2"));
            AdvError.Check(Adv.AdvLib.AddOrUpdateFileTag("OBJNAME", "Sample Generated Object"));
            AdvError.Check(Adv.AdvLib.DefineImageSection(WIDTH, HEIGHT, 16));
            AdvError.Check(Adv.AdvLib.DefineImageLayout(IMAGE_LAYOUT_ID, "FULL-IMAGE-RAW", "UNCOMPRESSED", 16));
            AdvError.Check(Adv.AdvLib.DefineStatusSection(1 * MILLI_TO_NANO /* 1ms */));

            // TODO: Get the real actual timestamps and exposure 
            DateTime startTime = DateTime.UtcNow;
            uint exposureMilliseconds = 500;

            for (int i = 0; i < 10; i++)
            {
                // TODO: Get the real actual timestamps and exposure 
                DateTime exposureStartTime = startTime.AddMilliseconds(exposureMilliseconds * i);

                AdvError.Check(Adv.AdvLib.BeginFrame(0, AdvTimeStamp.FromDateTime(exposureStartTime).NanosecondsAfterAdvZeroEpoch, MILLI_TO_NANO * exposureMilliseconds));

                ushort[] pixels = new ushort[WIDTH * HEIGHT];
                AdvError.Check(Adv.AdvLib.FrameAddImage(IMAGE_LAYOUT_ID, pixels, 16));
                AdvError.Check(Adv.AdvLib.EndFrame());
            }

            AdvError.Check(Adv.AdvLib.EndFile());
        }
    }
}
