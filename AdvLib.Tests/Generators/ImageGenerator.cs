using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvLib.Tests.Generators
{
    public class ImageGenerator
    {
        public ushort[] GetCurrentImageBytesInt16(int frameId, byte dynaBits)
        {
            return GetImagePattern1BytesInt16(dynaBits);
        }

        public byte[] GetCurrentImageBytes(int frameId, byte dynaBits)
        {
            return GetImagePattern1Bytes(dynaBits);
        }

        internal ushort[] GetImagePattern1BytesInt16(byte dynaBits)
        {
            ushort[] pixels = new ushort[640 * 480];

            // Background values are all half way 0x0FFF / 2 = 0x07FF
            for (int i = 0; i < pixels.Length; i++)
            {
                if (dynaBits == 16)
                    pixels[i] = 0x7FF0;
                else if (dynaBits == 12)
                    pixels[i] = 0x07FF;
            }

            // There is a pixel wide line from top left - down and right with full intensity (0x0FFF)
            for (int x = 0; x < 480; x++)
            {
                if (dynaBits == 16)
                    pixels[(x * 640 + x)] = 0xFFF0;
                else if (dynaBits == 12)
                    pixels[(x * 640 + x)] = 0x0FFF;
            }

            // There is a pixel wide line from top right - down and left with zero intensity (0x0000)
            for (int x = 0; x < 480; x++)
            {
                pixels[(x + 1) * 640 - x - 1] = 0x0000;
            }

            return pixels;
        }

        internal byte[] GetImagePattern1Bytes(byte dynaBits)
        {
            // NOTE: In this TEST example we mock up 12 bit pixels (640, 480), where 

            byte[] pixels;

            if (dynaBits == 12 || dynaBits == 16)
            {
                pixels = new byte[640 * 480 * 2];

                // Background values are all half way 0x0FFF / 2 = 0x07FF. This "scaled" to 16 bit is 0x7FF0
                for (int i = 0; i < pixels.Length / 2; i++)
                {
                    if (dynaBits == 16)
                    {
                        pixels[2 * i] = 0xF0;
                        pixels[2 * i + 1] = 0x7F;
                    }
                    else if (dynaBits == 12)
                    {
                        pixels[2 * i] = 0xFF;
                        pixels[2 * i + 1] = 0x07;
                    }
                }

                // There is a pixel wide line from top left - down and right with full intensity (0x0FFF). This "scaled" to 16 bit is 0xFFF0
                for (int x = 0; x < 480; x++)
                {
                    if (dynaBits == 16)
                    {
                        pixels[2 * (x * 640 + x)] = 0xF0;
                        pixels[2 * (x * 640 + x) + 1] = 0xFF;
                    }
                    else if (dynaBits == 12)
                    {
                        pixels[2 * (x * 640 + x)] = 0xFF;
                        pixels[2 * (x * 640 + x) + 1] = 0x0F;
                    }
                }

                // There is a pixel wide line from top right - down and left with zero intensity (0x0000)
                for (int x = 0; x < 480; x++)
                {
                    pixels[2 * ((x + 1) * 640 - x - 1)] = 0x00;
                    pixels[2 * ((x + 1) * 640 - x - 1) + 1] = 0x00;
                }
            }
            else
            {
                pixels = new byte[640 * 480];

                for (int i = 0; i < pixels.Length; i++)
                {
                    pixels[i] = 0x7F;
                }

                for (int x = 0; x < 480; x++)
                {
                    pixels[x * 640 + x + 1] = 0xFF;
                }

                for (int x = 0; x < 480; x++)
                {
                    pixels[(x + 1) * 640 - x - 1] = 0x00;
                }
            }

            return pixels;
        }

        internal bool VerifyImagePattern1Bytes(byte[] pixels, byte dynaBits)
        {
            if (dynaBits == 12 || dynaBits == 16)
            {
                // There is a pixel wide line from top right - down and left with zero intensity (0x0000)
                for (int x = 0; x < 480; x++)
                {
                    if (pixels[2 * ((x + 1) * 640 - x - 1)] != 0x00)
                    {
                        Console.WriteLine(string.Format("Pixel ({0}, {1}) is supposed to be 0x00 but is instead 0x{2}", x, 2 * (x + 1), Convert.ToString(pixels[2 * ((x + 1) * 640 - x - 1)], 16)));
                        return false;
                    }
                    pixels[2 * ((x + 1) * 640 - x - 1)] = 0x11;

                    if (pixels[2 * ((x + 1) * 640 - x - 1) + 1] != 0x00)
                    {
                        Console.WriteLine(string.Format("Pixel ({0}, {1}) is supposed to be 0x00 but is instead 0x{2}", x, 2 * (x + 1), Convert.ToString(pixels[2 * ((x + 1) * 640 - x - 1) + 1], 16)));
                        return false;
                    }
                    pixels[2 * ((x + 1) * 640 - x - 1) + 1] = 0x11;
                }


                // There is a pixel wide line from top left - down and right with full intensity (0x0FFF). This "scaled" to 16 bit is 0xFFF0
                for (int x = 0; x < 480; x++)
                {
                    if (dynaBits == 16)
                    {
                        if (pixels[2 * (x * 640 + x)] != 0xF0 && pixels[2 * (x * 640 + x)] != 0x11)
                        {
                            Console.WriteLine(string.Format("Pixel ({0}, {1}) is supposed to be 0xF0 but is instead 0x{2}", x + 1, x, Convert.ToString(pixels[2 * (x * 640 + x)], 16)));
                            return false;
                        }
                        pixels[2 * (x * 640 + x)] = 0x11;

                        if (pixels[2 * (x * 640 + x) + 1] != 0xFF && pixels[2 * (x * 640 + x) + 1] != 0x11)
                        {
                            Console.WriteLine(string.Format("Pixel ({0}, {1}) is supposed to be 0xFF but is instead 0x{2}", x + 1, x, Convert.ToString(pixels[2 * (x * 640 + x) + 1], 16)));
                            return false;
                        }
                        pixels[2 * (x * 640 + x) + 1] = 0x11;
                    }
                    else if (dynaBits == 12)
                    {
                        if (pixels[2 * (x * 640 + x)] != 0xFF && pixels[2 * (x * 640 + x)] != 0x11)
                        {
                            Console.WriteLine(string.Format("Pixel ({0}, {1}) is supposed to be 0xFF but is instead 0x{2}", x + 1, x, Convert.ToString(pixels[2 * (x * 640 + x)], 16)));
                            return false;
                        }
                        pixels[2 * (x * 640 + x)] = 0x11;

                        if (pixels[2 * (x * 640 + x) + 1] != 0x0F && pixels[2 * (x * 640 + x) + 1] != 0x11)
                        {
                            Console.WriteLine(string.Format("Pixel ({0}, {1}) is supposed to be 0x0F but is instead 0x{2}", x + 1, x, Convert.ToString(pixels[2 * (x * 640 + x) + 1], 16)));
                            return false;
                        }
                        pixels[2 * (x * 640 + x) + 1] = 0x11;
                    }
                }

               
                // Background values are all half way 0x0FFF / 2 = 0x07FF. This "scaled" to 16 bit is 0x7FF0
                for (int i = 0; i < pixels.Length / 2; i++)
                {
                    if (dynaBits == 16)
                    {
                        if (pixels[2 * i] != 0xF0 && pixels[2 * i] != 0x11)
                        {
                            Console.WriteLine(string.Format("Pixel ({0}) is supposed to be 0xF0 but is instead 0x{1}", i, Convert.ToString(pixels[2 * i], 16)));
                            return false;
                        }
                        pixels[2 * i] = 0x11;

                        if (pixels[2 * i + 1] != 0x7F && pixels[2 * i + 1] != 0x11)
                        {
                            Console.WriteLine(string.Format("Pixel ({0}) is supposed to be 0x7F but is instead 0x{1}", i, Convert.ToString(pixels[2 * i + 1], 16)));
                            return false;
                        }
                        pixels[2 * i + 1] = 0x11;
                    }
                    else if (dynaBits == 12)
                    {
                        if (pixels[2 * i] != 0xFF && pixels[2 * i] != 0x11)
                        {
                            Console.WriteLine(string.Format("Pixel ({0}) is supposed to be 0xFF but is instead 0x{1}", i, Convert.ToString(pixels[2 * i], 16)));
                            return false;
                        }
                        pixels[2 * i] = 0x11;

                        if (pixels[2 * i + 1] != 0x07 && pixels[2 * i + 1] != 0x11)
                        {
                            Console.WriteLine(string.Format("Pixel ({0}) is supposed to be 0x07 but is instead 0x{1}", i, Convert.ToString(pixels[2 * i + 1], 16)));
                            return false;
                        }
                        pixels[2 * i + 1] = 0x11;
                    }
                }
            }
            else
            {

                for (int x = 0; x < 480; x++)
                {
                    if (pixels[(x + 1) * 640 - x - 1] != 0x00)
                    {
                        Console.WriteLine(string.Format("Pixel ({0}, {1}) is supposed to be 0x00 but is instead 0x{2}", x, x + 1, Convert.ToString(pixels[(x + 1) * 640 - x - 1], 16)));
                        return false;
                    }
                    pixels[(x + 1) * 640 - x - 1] = 0x11;
                }

                for (int x = 0; x < 480; x++)
                {
                    if (pixels[x * 640 + x + 1] != 0xFF && pixels[x * 640 + x + 1] != 0x11)
                    {
                        Console.WriteLine(string.Format("Pixel ({0}, {1}) is supposed to be 0xFF but is instead 0x{2}", x + 1, x, Convert.ToString(pixels[x * 640 + x + 1], 16)));
                        return false;
                    }
                    pixels[x * 640 + x + 1] = 0x11;
                }

                for (int i = 0; i < pixels.Length; i++)
                {
                    if (pixels[i] != 0x7F && pixels[i] != 0x11)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        internal bool VerifyImagePattern1UInt32(uint[] pixels, byte dynaBits)
        {
            var bytes = new byte[2 * pixels.Length];
            for (int i = 0; i < pixels.Length; i++)
            {
                bytes[2 * i] = (byte) (pixels[i] & 0xFF);
                bytes[2 * i + 1] = (byte)((pixels[i] >> 8) & 0xFF);
            }

            return VerifyImagePattern1Bytes(bytes, dynaBits);
        }
    }
}
