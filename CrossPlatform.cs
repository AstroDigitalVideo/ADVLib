using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Adv
{
    // Code based on http://blez.wordpress.com/2012/09/17/determine-os-with-netmono/
    // by blez
    public static class CrossPlatform
    {
        public static string PlatformVersion { get; private set; }

        public static string ClrVersion { get; private set; }

        public static bool IsWindows { get; private set; }

        public static bool IsUnix { get; private set; }

        public static bool IsMac { get; private set; }

        public static bool IsLinux { get; private set; }

        public static bool IsUnknown { get; private set; }

        public static bool Is32bit { get; private set; }

        public static bool Is64bit { get; private set; }

        public static bool Is64BitProcess { get { return (IntPtr.Size == 8); } }

        public static bool Is32BitProcess { get { return (IntPtr.Size == 4); } }

        public static string CurrentOSName { get; private set; }

        static CrossPlatform()
        {
            string name = string.Empty;

            IsWindows = Path.DirectorySeparatorChar == '\\';
            if (IsWindows)
            {
                name = Environment.OSVersion.VersionString;

                name = name.Replace("Microsoft ", "");
                name = name.Replace("  ", " ");
                name = name.Replace(" )", ")");
                name = name.Trim();

                name = name.Replace("NT 6.2", "8 6.2");
                name = name.Replace("NT 6.1", "7 6.1");
                name = name.Replace("NT 6.0", "Vista 6.0");
                name = name.Replace("NT 5.", "XP 5.");
                name = name + (Environment.Is64BitOperatingSystem ? " (64 bit)" : " (32 bit)");

                if (Environment.Is64BitOperatingSystem)
                    Is64bit = true;
                else
                    Is32bit = true;
            }
            else
            {
                string UnixName = ReadProcessOutput("uname");
                if (UnixName.Contains("Darwin"))
                {
                    IsUnix = true;
                    IsMac = true;

                    name = "MacOS X " + ReadProcessOutput("sw_vers", "-productVersion");
                    name = name.Trim();

                    string machine = ReadProcessOutput("uname", "-m");
                    if (machine.Contains("x86_64"))
                        Is64bit = true;
                    else
                        Is32bit = true;

                    name += " " + (Is64bit ? "(64 bit)" : "(32 bit)");
                }
                else if (UnixName.Contains("Linux"))
                {
                    IsUnix = true;
                    IsLinux = true;

                    name = ReadProcessOutput("lsb_release", "-d");
                    name = name.Substring(name.IndexOf(":") + 1);
                    name = name.Trim();

                    string machine = ReadProcessOutput("uname", "-m");
                    if (machine.Contains("x86_64"))
                        Is64bit = true;
                    else
                        Is32bit = true;

                    name += " " + (Is64bit ? "(64 bit)" : "(32 bit)");
                }
                else if (UnixName != "")
                {
                    IsUnix = true;
                }
                else
                {
                    IsUnknown = true;
                }
            }

            OperatingSystem os = Environment.OSVersion;
            PlatformID pid = os.Platform;
            string platformInfo = "Unknown";
            switch (pid)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    platformInfo = "Windows";
                    break;
                case PlatformID.Unix:
                    platformInfo = "Unix";
                    break;
                case PlatformID.MacOSX:
                    platformInfo = "OSX";
                    break;
            }

            CurrentOSName = name;

            PlatformVersion = String.Format("{0} ({1} bit)", platformInfo, Environment.Is64BitOperatingSystem ? "64" : "32");

            bool isMono = Type.GetType("Mono.Runtime") != null;

            ClrVersion = isMono 
                ? "Mono CLR "  + (GetMonoVersion()??Assembly.GetExecutingAssembly().ImageRuntimeVersion) 
                : "Microsoft CLR " + Assembly.GetExecutingAssembly().ImageRuntimeVersion;
        }

        private static string GetMonoVersion()
        {
            try
            {
                Type type = Type.GetType("Mono.Runtime");
                if (type != null)
                {
                    MethodInfo dispalayName = type.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static);
                    if (dispalayName != null)
                        return (string)dispalayName.Invoke(null, null);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

            return null;
        }

        private static string ReadProcessOutput(string name)
        {
            return ReadProcessOutput(name, null);
        }

        private static string ReadProcessOutput(string name, string args)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                if (args != null && args != "") p.StartInfo.Arguments = " " + args;
                p.StartInfo.FileName = name;
                p.Start();
                // Do not wait for the child process to exit before
                // reading to the end of its redirected stream.
                // p.WaitForExit();
                // Read the output stream first and then wait.
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                if (output == null) output = "";
                output = output.Trim();
                Trace.WriteLine(output);
                return output;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.StackTrace);
                return "";
            }
        }
    }
}
