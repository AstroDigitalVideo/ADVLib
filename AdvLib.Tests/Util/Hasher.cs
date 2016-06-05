using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AdvLib.Tests.Util
{
    public class Hasher
    {
        private string GetHash<T>(string path) where T : HashAlgorithm
        {
            using (HashAlgorithm hasher = Activator.CreateInstance<T>())
            {
                hasher.Initialize();
                try
                {
                    var fileLength = new FileInfo(path).Length;
                    using (var file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    using (var cs = new CryptoStream(file, hasher, CryptoStreamMode.Read))
                    {
                        var buf = new byte[4096];
                        var pos = 0L;
                        var lastReport = DateTime.UtcNow.AddMinutes(-1);
                        while (pos < fileLength)
                        {
                            pos += cs.Read(buf, 0, buf.Length);
                        }
                        cs.Close();

                        // Return an empty string if hashing was stopped.
                        if (pos < fileLength) return "";

                        return BitConverter.ToString(hasher.Hash).Replace("-", "");
                    }
                }
                catch (Exception ex)
                {
                    return "Error: " + ex.Message;
                }
            }
        }

        public string CalcMd5(string path)
        {
            return GetHash<MD5Cng>(path);
        }
    }
}
