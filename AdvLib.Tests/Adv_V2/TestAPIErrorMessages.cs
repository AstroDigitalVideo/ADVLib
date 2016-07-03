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

        [Test]
        public void TestStatusTagInvalidTagIdAndType()
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

            int rv = Adv.AdvLib.FrameAddStatusTagUInt8(unchecked((uint)-1), 42);
            Assert.AreEqual(AdvErrorCodes.E_ADV_INVALID_STATUS_TAG_ID, rv);

            rv = Adv.AdvLib.FrameAddStatusTagUInt8(idx1 - 1, 42);
            Assert.AreEqual(AdvErrorCodes.E_ADV_INVALID_STATUS_TAG_ID, rv);

            rv = Adv.AdvLib.FrameAddStatusTagUInt8(idx6 + 1, 42);
            Assert.AreEqual(AdvErrorCodes.E_ADV_INVALID_STATUS_TAG_ID, rv);


            rv = Adv.AdvLib.FrameAddStatusTagUInt8(idx2, 42);
            Assert.AreEqual(AdvErrorCodes.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagUInt8(idx3, 42);
            Assert.AreEqual(AdvErrorCodes.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagUInt8(idx4, 42);
            Assert.AreEqual(AdvErrorCodes.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagUInt8(idx5, 42);
            Assert.AreEqual(AdvErrorCodes.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagUInt8(idx6, 42);
            Assert.AreEqual(AdvErrorCodes.E_ADV_INVALID_STATUS_TAG_TYPE, rv);

            rv = Adv.AdvLib.FrameAddStatusTag16(idx1, 42);
            Assert.AreEqual(AdvErrorCodes.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTag16(idx3, 42);
            Assert.AreEqual(AdvErrorCodes.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTag16(idx4, 42);
            Assert.AreEqual(AdvErrorCodes.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTag16(idx5, 42);
            Assert.AreEqual(AdvErrorCodes.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTag16(idx6, 42);
            Assert.AreEqual(AdvErrorCodes.E_ADV_INVALID_STATUS_TAG_TYPE, rv);

            rv = Adv.AdvLib.FrameAddStatusTag32(idx1, 42);
            Assert.AreEqual(AdvErrorCodes.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTag32(idx2, 42);
            Assert.AreEqual(AdvErrorCodes.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTag32(idx4, 42);
            Assert.AreEqual(AdvErrorCodes.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTag32(idx5, 42);
            Assert.AreEqual(AdvErrorCodes.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTag32(idx6, 42);
            Assert.AreEqual(AdvErrorCodes.E_ADV_INVALID_STATUS_TAG_TYPE, rv);

            rv = Adv.AdvLib.FrameAddStatusTag64(idx1, 42);
            Assert.AreEqual(AdvErrorCodes.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTag64(idx2, 42);
            Assert.AreEqual(AdvErrorCodes.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTag64(idx3, 42);
            Assert.AreEqual(AdvErrorCodes.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTag64(idx5, 42);
            Assert.AreEqual(AdvErrorCodes.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTag64(idx6, 42);
            Assert.AreEqual(AdvErrorCodes.E_ADV_INVALID_STATUS_TAG_TYPE, rv);

            rv = Adv.AdvLib.FrameAddStatusTagReal(idx1, 42);
            Assert.AreEqual(AdvErrorCodes.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagReal(idx2, 42);
            Assert.AreEqual(AdvErrorCodes.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagReal(idx3, 42);
            Assert.AreEqual(AdvErrorCodes.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagReal(idx4, 42);
            Assert.AreEqual(AdvErrorCodes.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagReal(idx6, 42);
            Assert.AreEqual(AdvErrorCodes.E_ADV_INVALID_STATUS_TAG_TYPE, rv);

            rv = Adv.AdvLib.FrameAddStatusTagUTF8String(idx1, "42");
            Assert.AreEqual(AdvErrorCodes.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagUTF8String(idx2, "42");
            Assert.AreEqual(AdvErrorCodes.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagUTF8String(idx3, "42");
            Assert.AreEqual(AdvErrorCodes.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagUTF8String(idx4, "42");
            Assert.AreEqual(AdvErrorCodes.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagUTF8String(idx5, "42");
            Assert.AreEqual(AdvErrorCodes.E_ADV_INVALID_STATUS_TAG_TYPE, rv);

            Adv.AdvLib.EndFile();
        }
    }
}
