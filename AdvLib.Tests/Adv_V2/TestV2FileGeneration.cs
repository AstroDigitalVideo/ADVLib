using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AdvLib.Tests.Generators;
using NUnit.Framework;

namespace AdvLib.Tests.Adv_V2
{
    [TestFixture]
    public class TestV2FileGeneration
    {
        [Test]
        public void SimpleTest()
        {
            var fileGen = new AdvGenerator();
            var cfg = new AdvGenerationConfig()
            {
                CameraDepth = 16,
                DynaBits = 16,
                SourceFormat = AdvSourceDataFormat.Format16BitUShort,
                NumberOfFrames = 10,
                UsesCompression = false,
                NormalPixelValue = null
            };

            string fileName = Path.GetTempFileName();
            AdvFile2 file = null;
            try
            {
                // Generate
                fileGen.GenerateaAdv_V2(cfg, fileName);

                // Verify
                file = new AdvFile2(fileName);
                
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
    }
}
