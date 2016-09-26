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

        [Test]
        public void SampleAdvFileRecording2()
        {
            fileName = @"C:\hello-world.adv";

            const uint MILLI_TO_NANO = 1000000;
            const int WIDTH = 800;
            const int HEIGHT = 600;

            var recorder = new AdvRecorder();
            recorder.ImageConfig.SetImageParameters(WIDTH, HEIGHT, 16, 0);

            recorder.FileMetaData.RecorderSoftwareName = "MyVideoRecorder";
            recorder.FileMetaData.RecorderSoftwareVersion = "x.y.z";
            recorder.FileMetaData.CameraModel = "TestCamera";
            recorder.FileMetaData.CameraSensorInfo = "TestSensor";

            recorder.StartRecordingNewFile(fileName, 1 * MILLI_TO_NANO /* 1ms */);

            for (int i = 0; i < 10; i++)
            {
                ushort[] pixels = new ushort[WIDTH * HEIGHT];

                recorder.AddVideoFrame(
                    pixels, false, null,
                    AdvTimeStamp.FromDateTime(DateTime.Now),
                    AdvTimeStamp.FromDateTime(DateTime.Now.AddSeconds(0.5 * i)),
                    null, AdvImageData.PixelDepth16Bit);
            }

            recorder.FinishRecording();
        }


        private long GetCustomClockTickCount()
        {
            return DateTime.Now.Ticks;
        }

        [Test]
        public void SampleAdvFileRecording3()
        {
            fileName = @"C:\hello-world.adv";

            const uint MILLI_TO_NANO = 1000000;
            const int WIDTH = 800;
            const int HEIGHT = 600;

            var recorder = new AdvRecorder();
            recorder.ImageConfig.SetImageParameters(WIDTH, HEIGHT, 16, 0);

            recorder.FileMetaData.RecorderSoftwareName = "MyVideoRecorder";
            recorder.FileMetaData.RecorderSoftwareVersion = "x.y.z";
            recorder.FileMetaData.CameraModel = "TestCamera";
            recorder.FileMetaData.CameraSensorInfo = "TestSensor";

            recorder.DefineCustomClock(
                AdvRecorder.AdvStream.MainStream, 
                98304000 /* 98.304MHz */, 
                1, /* 1 tick */ 
                GetCustomClockTickCount);

            recorder.StartRecordingNewFile(fileName, 1 * MILLI_TO_NANO /* 1ms */);

            for (int i = 0; i < 10; i++)
            {
                ushort[] pixels = new ushort[WIDTH * HEIGHT];

                recorder.AddVideoFrame(
                    pixels, false, null,
                    AdvTimeStamp.FromDateTime(DateTime.Now),
                    AdvTimeStamp.FromDateTime(DateTime.Now.AddSeconds(0.5 * i)),
                    null, AdvImageData.PixelDepth16Bit);
            }

            recorder.FinishRecording();
        }

        [Test]
        public void SampleAdvFileRecording4()
        {
            fileName = @"C:\hello-world.adv";

            const uint MILLI_TO_NANO = 1000000;
            const int WIDTH = 800;
            const int HEIGHT = 600;

            var recorder = new AdvRecorder();
            recorder.ImageConfig.SetImageParameters(WIDTH, HEIGHT, 16, 0);

            recorder.FileMetaData.RecorderSoftwareName = "MyVideoRecorder";
            recorder.FileMetaData.RecorderSoftwareVersion = "x.y.z";
            recorder.FileMetaData.CameraModel = "TestCamera";
            recorder.FileMetaData.CameraSensorInfo = "TestSensor";

            recorder.StatusSectionConfig.RecordGain = true;
            recorder.StatusSectionConfig.RecordGamma = true;

            recorder.StartRecordingNewFile(fileName, 1 * MILLI_TO_NANO /* 1ms */);

            for (int i = 0; i < 10; i++)
            {
                ushort[] pixels = new ushort[WIDTH * HEIGHT];
                
                var statusEntry = new AdvRecorder.AdvStatusEntry()
                {
                    Gain = 38.3f,
                    Gamma = 1
                };

                recorder.AddVideoFrame(
                    pixels, false, null,
                    AdvTimeStamp.FromDateTime(DateTime.Now),
                    AdvTimeStamp.FromDateTime(DateTime.Now.AddSeconds(0.5 * i)),
                    statusEntry, AdvImageData.PixelDepth16Bit);
            }

            recorder.FinishRecording();
        }

        [Test]
        public void SampleAdvFileRecording5()
        {
            fileName = @"C:\hello-world-5.adv";

            const uint MILLI_TO_NANO = 1000000;
            const int WIDTH = 800;
            const int HEIGHT = 600;

            var rec = new AdvRecorder();
            rec.ImageConfig.SetImageParameters(WIDTH, HEIGHT, 16, 0);

            rec.FileMetaData.RecorderSoftwareName = "MyVideoRecorder";
            rec.FileMetaData.RecorderSoftwareVersion = "x.y.z";
            rec.FileMetaData.CameraModel = "TestCamera";
            rec.FileMetaData.CameraSensorInfo = "TestSensor";

            rec.FileMetaData.GainResponseMode = ResponseMode.Linear;            
            rec.FileMetaData.Telescope = "14\" LX-200 ACF (Tangra Observatory)";
            rec.FileMetaData.Observer = "Hristo Pavlov";
            rec.FileMetaData.ObjectName = "Chariklo";
            rec.FileMetaData.Comment = "Full moon only 20 deg away from the target.";

            rec.FileMetaData.AddUserTag("Timing Hardware", "IOTA-VTI v3");

            rec.StatusSectionConfig.AddDefineTag("ErrorFlag", Adv2TagType.Int8);
            rec.StatusSectionConfig.AddDefineTag("Temperature", Adv2TagType.Real);

            rec.StartRecordingNewFile(fileName, 1 * MILLI_TO_NANO /* 1ms */);

            // TODO: Get the real actual timestamps and exposure 
            DateTime startTime = DateTime.UtcNow;
            double exposureSeconds = 0.5;

            for (int i = 0; i < 10; i++)
            {
                ushort[] pixels = new ushort[WIDTH * HEIGHT];

                var statusEntry = new AdvRecorder.AdvStatusEntry()
                {
                    // Set actual values. Order and type of values is important.
                    AdditionalStatusTags = new object[]{ (byte)1, 15.3f }
                };

                rec.AddVideoFrame(
                    pixels, false, null,
                    AdvTimeStamp.FromDateTime(startTime.AddSeconds(exposureSeconds * i)),
                    AdvTimeStamp.FromDateTime(startTime.AddSeconds(exposureSeconds * (i + 1))),
                    statusEntry, AdvImageData.PixelDepth16Bit);
            }

            rec.FinishRecording();
        }
    }
}
