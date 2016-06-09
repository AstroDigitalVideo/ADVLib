using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace AdvLib.Tests.Generators
{
    [TestFixture]
    public class TestImageGenerator
    {
        private ImageGenerator imageGenerator;

        public TestImageGenerator()
        {
            imageGenerator = new ImageGenerator();
        }

        [Test]
        public void Test8BitGeneration()
        {
            byte[] pixBytes = imageGenerator.GetImagePattern1Bytes(8);
            ushort[] pixShorts = imageGenerator.GetImagePattern1BytesInt16(8);

            Assert.AreEqual(pixShorts.Length, pixBytes.Length);

            for (int i = 0; i < pixBytes.Length; i++)
            {
                Assert.AreEqual(pixBytes[i], pixShorts[i]);   
            }
        }
        
        [Test]
        [TestCase(12)]
        [TestCase(16)]
        public void Test16BitGeneration(byte dynaBits)
        {
            byte[] pixBytes = imageGenerator.GetImagePattern1Bytes(dynaBits);
            ushort[] pixShorts = imageGenerator.GetImagePattern1BytesInt16(dynaBits);

            Assert.AreEqual(pixShorts.Length * 2, pixBytes.Length);

            for (int i = 0; i < pixShorts.Length; i++)
            {
                int valFromByte = pixBytes[2 * i] + (pixBytes[2 * i + 1] << 8); // Little Endian
                int valFromShort = pixShorts[i];
                Assert.AreEqual(valFromByte, valFromShort);
            }
        }

        
        [Test]
        [TestCase(8)]
        [TestCase(12)]
        [TestCase(16)]
        public void TestPattern1Bytes(byte dynaBits)
        {
            byte[] image = imageGenerator.GetImagePattern1Bytes(dynaBits);
            bool verified = imageGenerator.VerifyImagePattern1Bytes(image, dynaBits);
            Assert.IsTrue(verified);
        }

        [Test]
        [TestCase(8)]
        [TestCase(12)]
        [TestCase(16)]
        public void TestPattern1Int16(byte dynaBits)
        {
            ushort[] image = imageGenerator.GetImagePattern1BytesInt16(dynaBits);
            bool verified = imageGenerator.VerifyImagePattern1UInt16(image, dynaBits);
            Assert.IsTrue(verified);
        }
    }
}
