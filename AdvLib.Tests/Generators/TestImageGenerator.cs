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
        [TestCase(8)]
        [TestCase(12)]
        [TestCase(16)]
        public void TestPattern1Bytes(byte dynaBits)
        {
            byte[] image = imageGenerator.GetImagePattern1Bytes(dynaBits);
            bool verified = imageGenerator.VerifyImagePattern1Bytes(image, dynaBits);
            Assert.IsTrue(verified);
        }
    }
}
