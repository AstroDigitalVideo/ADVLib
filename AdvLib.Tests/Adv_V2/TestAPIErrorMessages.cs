using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Adv;
using NUnit.Framework;

namespace AdvLib.Tests.Adv_V2
{
    [TestFixture]
    public class TestAPIErrorMessages
    {
        private string m_FileName;

        [SetUp]
        public void Setup()
        {
            m_FileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            if (File.Exists(m_FileName)) File.Delete(m_FileName);
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                if (File.Exists(m_FileName))
                    File.Delete(m_FileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Trace.WriteLine(ex);
            }
        }

        [Test]
        public void TestStatusTagEntryAlreadyAddedCode()
        {
            Adv.AdvLib.NewFile(m_FileName);

            Adv.AdvLib.DefineImageSection(640, 480, 16);
            Adv.AdvLib.DefineStatusSection(5000000 /* 5ms */);
            Adv.AdvLib.DefineImageLayout(0, "FULL-IMAGE-RAW", "UNCOMPRESSED", 16);

            uint idx1 = Adv.AdvLib.DefineStatusSectionTag("Int8", Adv2TagType.Int8);
            uint idx2 = Adv.AdvLib.DefineStatusSectionTag("Int16", Adv2TagType.Int16);
            uint idx3 = Adv.AdvLib.DefineStatusSectionTag("Int32", Adv2TagType.Int32);
            uint idx4 = Adv.AdvLib.DefineStatusSectionTag("Long64", Adv2TagType.Long64);
            uint idx5 = Adv.AdvLib.DefineStatusSectionTag("Real", Adv2TagType.Real);
            uint idx6 = Adv.AdvLib.DefineStatusSectionTag("UTF8String", Adv2TagType.UTF8String);

            Adv.AdvLib.BeginFrame(0, 0, 0, 0, 0, 0);

            int rv = Adv.AdvLib.FrameAddStatusTagUInt8(idx1, 42);
            Assert.AreEqual(AdvErrorCodes.S_OK, rv);

            rv = Adv.AdvLib.FrameAddStatusTagUInt8(idx1, 42);
            Assert.AreEqual(AdvErrorCodes.E_ADV_STATUS_ENTRY_ALREADY_ADDED, rv);

            rv = Adv.AdvLib.FrameAddStatusTag16(idx2, 12891);
            Assert.AreEqual(AdvErrorCodes.S_OK, rv);

            rv = Adv.AdvLib.FrameAddStatusTag16(idx2, 12891);
            Assert.AreEqual(AdvErrorCodes.E_ADV_STATUS_ENTRY_ALREADY_ADDED, rv);

            rv = Adv.AdvLib.FrameAddStatusTag32(idx3, -12312);
            Assert.AreEqual(AdvErrorCodes.S_OK, rv);

            rv = Adv.AdvLib.FrameAddStatusTag32(idx3, 12);
            Assert.AreEqual(AdvErrorCodes.E_ADV_STATUS_ENTRY_ALREADY_ADDED, rv);

            rv = Adv.AdvLib.FrameAddStatusTag64(idx4, -12312);
            Assert.AreEqual(AdvErrorCodes.S_OK, rv);

            rv = Adv.AdvLib.FrameAddStatusTag64(idx4, 12);
            Assert.AreEqual(AdvErrorCodes.E_ADV_STATUS_ENTRY_ALREADY_ADDED, rv);

            rv = Adv.AdvLib.FrameAddStatusTagReal(idx5, 12.12f);
            Assert.AreEqual(AdvErrorCodes.S_OK, rv);

            rv = Adv.AdvLib.FrameAddStatusTagReal(idx5, 13.13f);
            Assert.AreEqual(AdvErrorCodes.E_ADV_STATUS_ENTRY_ALREADY_ADDED, rv);

            rv = Adv.AdvLib.FrameAddStatusTagUTF8String(idx6, "Val1");
            Assert.AreEqual(AdvErrorCodes.S_OK, rv);

            rv = Adv.AdvLib.FrameAddStatusTagUTF8String(idx6, "Val2");
            Assert.AreEqual(AdvErrorCodes.E_ADV_STATUS_ENTRY_ALREADY_ADDED, rv);

            Adv.AdvLib.EndFile();
        }
    }
}
