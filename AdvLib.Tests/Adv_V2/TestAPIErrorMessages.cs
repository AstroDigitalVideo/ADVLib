using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Adv;
using AdvLib.Tests.Generators;
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
            Assert.AreEqual(AdvError.S_OK, rv);

            rv = Adv.AdvLib.FrameAddStatusTagUInt8(idx1, 42);
            Assert.AreEqual(AdvError.E_ADV_STATUS_ENTRY_ALREADY_ADDED, rv);

            rv = Adv.AdvLib.FrameAddStatusTagInt16(idx2, 12891);
            Assert.AreEqual(AdvError.S_OK, rv);

            rv = Adv.AdvLib.FrameAddStatusTagInt16(idx2, 12891);
            Assert.AreEqual(AdvError.E_ADV_STATUS_ENTRY_ALREADY_ADDED, rv);

            rv = Adv.AdvLib.FrameAddStatusTagInt32(idx3, -12312);
            Assert.AreEqual(AdvError.S_OK, rv);

            rv = Adv.AdvLib.FrameAddStatusTagInt32(idx3, 12);
            Assert.AreEqual(AdvError.E_ADV_STATUS_ENTRY_ALREADY_ADDED, rv);

            rv = Adv.AdvLib.FrameAddStatusTagInt64(idx4, -12312);
            Assert.AreEqual(AdvError.S_OK, rv);

            rv = Adv.AdvLib.FrameAddStatusTagInt64(idx4, 12);
            Assert.AreEqual(AdvError.E_ADV_STATUS_ENTRY_ALREADY_ADDED, rv);

            rv = Adv.AdvLib.FrameAddStatusTagFloat(idx5, 12.12f);
            Assert.AreEqual(AdvError.S_OK, rv);

            rv = Adv.AdvLib.FrameAddStatusTagFloat(idx5, 13.13f);
            Assert.AreEqual(AdvError.E_ADV_STATUS_ENTRY_ALREADY_ADDED, rv);

            rv = Adv.AdvLib.FrameAddStatusTagUTF8String(idx6, "Val1");
            Assert.AreEqual(AdvError.S_OK, rv);

            rv = Adv.AdvLib.FrameAddStatusTagUTF8String(idx6, "Val2");
            Assert.AreEqual(AdvError.E_ADV_STATUS_ENTRY_ALREADY_ADDED, rv);

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
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_ID, rv);

            rv = Adv.AdvLib.FrameAddStatusTagUInt8(idx1 - 1, 42);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_ID, rv);

            rv = Adv.AdvLib.FrameAddStatusTagUInt8(idx6 + 1, 42);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_ID, rv);


            rv = Adv.AdvLib.FrameAddStatusTagUInt8(idx2, 42);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagUInt8(idx3, 42);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagUInt8(idx4, 42);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagUInt8(idx5, 42);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagUInt8(idx6, 42);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, rv);

            rv = Adv.AdvLib.FrameAddStatusTagInt16(idx1, 42);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagInt16(idx3, 42);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagInt16(idx4, 42);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagInt16(idx5, 42);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagInt16(idx6, 42);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, rv);

            rv = Adv.AdvLib.FrameAddStatusTagInt32(idx1, 42);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagInt32(idx2, 42);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagInt32(idx4, 42);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagInt32(idx5, 42);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagInt32(idx6, 42);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, rv);

            rv = Adv.AdvLib.FrameAddStatusTagInt64(idx1, 42);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagInt64(idx2, 42);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagInt64(idx3, 42);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagInt64(idx5, 42);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagInt64(idx6, 42);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, rv);

            rv = Adv.AdvLib.FrameAddStatusTagFloat(idx1, 42);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagFloat(idx2, 42);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagFloat(idx3, 42);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagFloat(idx4, 42);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagFloat(idx6, 42);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, rv);

            rv = Adv.AdvLib.FrameAddStatusTagUTF8String(idx1, "42");
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagUTF8String(idx2, "42");
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagUTF8String(idx3, "42");
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagUTF8String(idx4, "42");
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, rv);
            rv = Adv.AdvLib.FrameAddStatusTagUTF8String(idx5, "42");
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, rv);

            Adv.AdvLib.EndFile();
        }

        [Test]
        public void TestStatusTagRetrievalInvalidTagIdAndType()
        {
            Adv.AdvLib.NewFile(m_FileName);

            Adv.AdvLib.DefineImageSection(640, 480, 16);
            Adv.AdvLib.DefineStatusSection(5000000 /* 5ms */);
            Adv.AdvLib.DefineImageLayout(0, "FULL-IMAGE-RAW", "UNCOMPRESSED", 16);

            uint idx1 = Adv.AdvLib.DefineStatusSectionTag("Int8", Adv2TagType.Int8);
            uint idx1a = Adv.AdvLib.DefineStatusSectionTag("Int8-2", Adv2TagType.Int8);
            uint idx2 = Adv.AdvLib.DefineStatusSectionTag("Int16", Adv2TagType.Int16);
            uint idx2a = Adv.AdvLib.DefineStatusSectionTag("Int16-2", Adv2TagType.Int16);
            uint idx3 = Adv.AdvLib.DefineStatusSectionTag("Int32", Adv2TagType.Int32);
            uint idx3a = Adv.AdvLib.DefineStatusSectionTag("Int32-2", Adv2TagType.Int32);
            uint idx4 = Adv.AdvLib.DefineStatusSectionTag("Long64", Adv2TagType.Long64);
            uint idx4a = Adv.AdvLib.DefineStatusSectionTag("Long64-2", Adv2TagType.Long64);
            uint idx5 = Adv.AdvLib.DefineStatusSectionTag("Real", Adv2TagType.Real);
            uint idx5a = Adv.AdvLib.DefineStatusSectionTag("Real-2", Adv2TagType.Real);
            uint idx6 = Adv.AdvLib.DefineStatusSectionTag("UTF8String", Adv2TagType.UTF8String);
            uint idx6a = Adv.AdvLib.DefineStatusSectionTag("UTF8String-2", Adv2TagType.UTF8String);

            Adv.AdvLib.BeginFrame(0, 0, 0, 0, 0, 0);
            Adv.AdvLib.FrameAddStatusTagUInt8(idx1, 42);
            Adv.AdvLib.FrameAddStatusTagInt16(idx2, 42);
            Adv.AdvLib.FrameAddStatusTagInt32(idx3, 42);
            Adv.AdvLib.FrameAddStatusTagInt64(idx4, 42);
            Adv.AdvLib.FrameAddStatusTagFloat(idx5, 42);
            Adv.AdvLib.FrameAddStatusTagUTF8String(idx6, "42");
            var imgGen = new ImageGenerator();
            ushort[] pixels = imgGen.GetImagePattern1BytesInt16(16);
            Adv.AdvLib.FrameAddImage(0, pixels, 16);
            Adv.AdvLib.EndFrame();
            Adv.AdvLib.EndFile();

            AdvFileInfo fileInfo;
            Adv.AdvLib.AdvOpenFile(m_FileName, out fileInfo);

            byte? val8;
            short? val16;
            int? val32;
            long? val64;
            float? valf;
            string vals;
            int errorCode;

            errorCode = Adv.AdvLib.GetStatusTagUInt8(idx1, out val8);
            Assert.AreEqual(AdvError.E_ADV_FRAME_STATUS_NOT_LOADED, errorCode);
            errorCode = Adv.AdvLib.GetStatusTagInt16(idx2, out val16);
            Assert.AreEqual(AdvError.E_ADV_FRAME_STATUS_NOT_LOADED, errorCode);
            errorCode = Adv.AdvLib.GetStatusTagInt32(idx3, out val32);
            Assert.AreEqual(AdvError.E_ADV_FRAME_STATUS_NOT_LOADED, errorCode);
            errorCode = Adv.AdvLib.GetStatusTagInt64(idx4, out val64);
            Assert.AreEqual(AdvError.E_ADV_FRAME_STATUS_NOT_LOADED, errorCode);
            errorCode = Adv.AdvLib.GetStatusTagFloat(idx5, out valf);
            Assert.AreEqual(AdvError.E_ADV_FRAME_STATUS_NOT_LOADED, errorCode);
            errorCode = Adv.AdvLib.GetStatusTagUTF8String(idx6, out vals);
            Assert.AreEqual(AdvError.E_ADV_FRAME_STATUS_NOT_LOADED, errorCode);

            Adv2TagType? tagType;
            string tagName;
            errorCode = Adv.AdvLib.GetStatusTagInfo(idx1 - 1, out tagType, out tagName);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_ID, errorCode);
            errorCode = Adv.AdvLib.GetStatusTagInfo(idx1, out tagType, out tagName);
            Assert.AreEqual(AdvError.S_OK, errorCode);
            Assert.AreEqual(Adv2TagType.Int8, tagType);
            Assert.AreEqual("Int8", tagName);
            errorCode = Adv.AdvLib.GetStatusTagInfo(idx1a, out tagType, out tagName);
            Assert.AreEqual(AdvError.S_OK, errorCode);
            Assert.AreEqual(Adv2TagType.Int8, tagType);
            Assert.AreEqual("Int8-2", tagName);
            errorCode = Adv.AdvLib.GetStatusTagInfo(idx2, out tagType, out tagName);
            Assert.AreEqual(AdvError.S_OK, errorCode);
            Assert.AreEqual(Adv2TagType.Int16, tagType);
            Assert.AreEqual("Int16", tagName);
            errorCode = Adv.AdvLib.GetStatusTagInfo(idx2a, out tagType, out tagName);
            Assert.AreEqual(AdvError.S_OK, errorCode);
            Assert.AreEqual(Adv2TagType.Int16, tagType);
            Assert.AreEqual("Int16-2", tagName);
            errorCode = Adv.AdvLib.GetStatusTagInfo(idx3, out tagType, out tagName);
            Assert.AreEqual(AdvError.S_OK, errorCode);
            Assert.AreEqual(Adv2TagType.Int32, tagType);
            Assert.AreEqual("Int32", tagName);
            errorCode = Adv.AdvLib.GetStatusTagInfo(idx3a, out tagType, out tagName);
            Assert.AreEqual(AdvError.S_OK, errorCode);
            Assert.AreEqual(Adv2TagType.Int32, tagType);
            Assert.AreEqual("Int32-2", tagName);
            errorCode = Adv.AdvLib.GetStatusTagInfo(idx4, out tagType, out tagName);
            Assert.AreEqual(AdvError.S_OK, errorCode);
            Assert.AreEqual(Adv2TagType.Long64, tagType);
            Assert.AreEqual("Long64", tagName);
            errorCode = Adv.AdvLib.GetStatusTagInfo(idx4a, out tagType, out tagName);
            Assert.AreEqual(AdvError.S_OK, errorCode);
            Assert.AreEqual(Adv2TagType.Long64, tagType);
            Assert.AreEqual("Long64-2", tagName);
            errorCode = Adv.AdvLib.GetStatusTagInfo(idx5, out tagType, out tagName);
            Assert.AreEqual(AdvError.S_OK, errorCode);
            Assert.AreEqual(Adv2TagType.Real, tagType);
            Assert.AreEqual("Real", tagName);
            errorCode = Adv.AdvLib.GetStatusTagInfo(idx5a, out tagType, out tagName);
            Assert.AreEqual(AdvError.S_OK, errorCode);
            Assert.AreEqual(Adv2TagType.Real, tagType);
            Assert.AreEqual("Real-2", tagName);
            errorCode = Adv.AdvLib.GetStatusTagInfo(idx6, out tagType, out tagName);
            Assert.AreEqual(AdvError.S_OK, errorCode);
            Assert.AreEqual(Adv2TagType.UTF8String, tagType);
            Assert.AreEqual("UTF8String", tagName);
            errorCode = Adv.AdvLib.GetStatusTagInfo(idx6a, out tagType, out tagName);
            Assert.AreEqual(AdvError.S_OK, errorCode);
            Assert.AreEqual(Adv2TagType.UTF8String, tagType);
            Assert.AreEqual("UTF8String-2", tagName);

            Adv.AdvLib.GetFramePixels(0, 0, 640, 480);

            errorCode = Adv.AdvLib.GetStatusTagUInt8(idx1, out val8);
            Assert.AreEqual(AdvError.S_OK, errorCode);
            errorCode = Adv.AdvLib.GetStatusTagUInt8(idx1 - 1, out val8);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_ID, errorCode);
            errorCode = Adv.AdvLib.GetStatusTagUInt8(idx2, out val8);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, errorCode);
            errorCode = Adv.AdvLib.GetStatusTagUInt8(idx1a, out val8);
            Assert.AreEqual(AdvError.E_ADV_STATUS_TAG_NOT_FOUND_IN_FRAME, errorCode);

            errorCode = Adv.AdvLib.GetStatusTagInt16(idx2, out val16);
            Assert.AreEqual(AdvError.S_OK, errorCode);
            errorCode = Adv.AdvLib.GetStatusTagInt16(idx1 - 1, out val16);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_ID, errorCode);
            errorCode = Adv.AdvLib.GetStatusTagInt16(idx1, out val16);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, errorCode);
            errorCode = Adv.AdvLib.GetStatusTagInt16(idx2a, out val16);
            Assert.AreEqual(AdvError.E_ADV_STATUS_TAG_NOT_FOUND_IN_FRAME, errorCode);

            errorCode = Adv.AdvLib.GetStatusTagInt32(idx3, out val32);
            Assert.AreEqual(AdvError.S_OK, errorCode);
            errorCode = Adv.AdvLib.GetStatusTagInt32(idx1 - 1, out val32);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_ID, errorCode);
            errorCode = Adv.AdvLib.GetStatusTagInt32(idx1, out val32);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, errorCode);
            errorCode = Adv.AdvLib.GetStatusTagInt32(idx3a, out val32);
            Assert.AreEqual(AdvError.E_ADV_STATUS_TAG_NOT_FOUND_IN_FRAME, errorCode);

            errorCode = Adv.AdvLib.GetStatusTagInt64(idx4, out val64);
            Assert.AreEqual(AdvError.S_OK, errorCode);
            errorCode = Adv.AdvLib.GetStatusTagInt64(idx1 - 1, out val64);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_ID, errorCode);
            errorCode = Adv.AdvLib.GetStatusTagInt64(idx1, out val64);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, errorCode);
            errorCode = Adv.AdvLib.GetStatusTagInt64(idx4a, out val64);
            Assert.AreEqual(AdvError.E_ADV_STATUS_TAG_NOT_FOUND_IN_FRAME, errorCode);

            errorCode = Adv.AdvLib.GetStatusTagFloat(idx5, out valf);
            Assert.AreEqual(AdvError.S_OK, errorCode);
            errorCode = Adv.AdvLib.GetStatusTagFloat(idx1 - 1, out valf);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_ID, errorCode);
            errorCode = Adv.AdvLib.GetStatusTagFloat(idx1, out valf);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, errorCode);
            errorCode = Adv.AdvLib.GetStatusTagFloat(idx5a, out valf);
            Assert.AreEqual(AdvError.E_ADV_STATUS_TAG_NOT_FOUND_IN_FRAME, errorCode);

            errorCode = Adv.AdvLib.GetStatusTagUTF8String(idx6, out vals);
            Assert.AreEqual(AdvError.S_OK, errorCode);
            errorCode = Adv.AdvLib.GetStatusTagUTF8String(idx6 + 10, out vals);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_ID, errorCode);
            errorCode = Adv.AdvLib.GetStatusTagUTF8String(idx1, out vals);
            Assert.AreEqual(AdvError.E_ADV_INVALID_STATUS_TAG_TYPE, errorCode);
            errorCode = Adv.AdvLib.GetStatusTagUTF8String(idx6a, out vals);
            Assert.AreEqual(AdvError.E_ADV_STATUS_TAG_NOT_FOUND_IN_FRAME, errorCode);
        }

        [Test]
        public void TestNoFileErrorCode()
        {
            Adv.AdvLib.AdvCloseFile();

            int errorCode = Adv.AdvLib.FrameAddImage(0, new ushort[100], 0);
            Assert.AreEqual(AdvError.E_ADV_NOFILE, errorCode);
            errorCode = Adv.AdvLib.FrameAddImageBytes(0, new byte[100], 0);
            Assert.AreEqual(AdvError.E_ADV_NOFILE, errorCode);

            errorCode = Adv.AdvLib.FrameAddStatusTagUInt8(0, 42);
            Assert.AreEqual(AdvError.E_ADV_NOFILE, errorCode);
            errorCode = Adv.AdvLib.FrameAddStatusTagInt16(0, 42);
            Assert.AreEqual(AdvError.E_ADV_NOFILE, errorCode);
            errorCode = Adv.AdvLib.FrameAddStatusTagInt32(0, 42);
            Assert.AreEqual(AdvError.E_ADV_NOFILE, errorCode);
            errorCode = Adv.AdvLib.FrameAddStatusTagInt64(0, 42);
            Assert.AreEqual(AdvError.E_ADV_NOFILE, errorCode);
            errorCode = Adv.AdvLib.FrameAddStatusTagFloat(0, 42);
            Assert.AreEqual(AdvError.E_ADV_NOFILE, errorCode);
            errorCode = Adv.AdvLib.FrameAddStatusTagUTF8String(0, "42");
            Assert.AreEqual(AdvError.E_ADV_NOFILE, errorCode);

            AdvImageLayoutInfo imageLayoutInfo;
            errorCode = Adv.AdvLib.GetImageLayoutInfo(0, out imageLayoutInfo);
            Assert.AreEqual(AdvError.E_ADV_NOFILE, errorCode);


            string tagName;
            string tagValue;
            errorCode = Adv.AdvLib.GetMainStreamTag(0, out tagName, out tagValue);
            Assert.AreEqual(AdvError.E_ADV_NOFILE, errorCode);
            errorCode = Adv.AdvLib.GetCalibrationStreamTag(0, out tagName, out tagValue);
            Assert.AreEqual(AdvError.E_ADV_NOFILE, errorCode);
            errorCode = Adv.AdvLib.GetUserMetadataTag(0, out tagName, out tagValue);
            Assert.AreEqual(AdvError.E_ADV_NOFILE, errorCode);
            errorCode = Adv.AdvLib.GetImageSectionTag(0, out tagName, out tagValue);
            Assert.AreEqual(AdvError.E_ADV_NOFILE, errorCode);
            errorCode = Adv.AdvLib.GetImageLayoutTag(0, 0, out tagName, out tagValue);
            Assert.AreEqual(AdvError.E_ADV_NOFILE, errorCode);


            AdvFrameInfo frameInfo;
            uint[] pixels;
            errorCode = Adv.AdvLib.GetFramePixels(0, 0, 640, 480, out frameInfo, out pixels);
            Assert.AreEqual(AdvError.E_ADV_NOFILE, errorCode);

            Adv2TagType? tagType;           
            byte? val8;
            short? val16;
            int? val32;
            long? val64;
            float? valf;
            string vals;

            errorCode = Adv.AdvLib.GetStatusTagInfo(0, out tagType, out tagName);
            Assert.AreEqual(AdvError.E_ADV_NOFILE, errorCode);
            errorCode = Adv.AdvLib.GetStatusTagUInt8(0, out val8);
            Assert.AreEqual(AdvError.E_ADV_NOFILE, errorCode);
            errorCode = Adv.AdvLib.GetStatusTagInt16(0, out val16);
            Assert.AreEqual(AdvError.E_ADV_NOFILE, errorCode);
            errorCode = Adv.AdvLib.GetStatusTagInt32(0, out val32);
            Assert.AreEqual(AdvError.E_ADV_NOFILE, errorCode);
            errorCode = Adv.AdvLib.GetStatusTagInt64(0, out val64);
            Assert.AreEqual(AdvError.E_ADV_NOFILE, errorCode);
            errorCode = Adv.AdvLib.GetStatusTagFloat(0, out valf);
            Assert.AreEqual(AdvError.E_ADV_NOFILE, errorCode);
            errorCode = Adv.AdvLib.GetStatusTagUTF8String(0, out vals);
            Assert.AreEqual(AdvError.E_ADV_NOFILE, errorCode);

        }
    }
}
