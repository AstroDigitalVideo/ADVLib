using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

public class Library
{
	[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
	public static extern IntPtr GetModuleHandle(string lpModuleName);

	[DllImport("kernel32.dll", SetLastError = true)]
	[PreserveSig]
	public static extern uint GetModuleFileName([In] IntPtr hModule, [Out] StringBuilder lpFilename, [In] [MarshalAs(UnmanagedType.U4)] int nSize);

	public static string GetVersion()
	{
		return AdvLib.GetLibraryVersion();
	}

	public static string GetPlatformId()
	{
		return AdvLib.GetLibraryPlatformId();
	}

    public static string GetLibraryBitness()
	{
        return AdvLib.GetLibraryBitness();
	}
 
	public static string GetLibraryPath()
	{
		string dllName = Is64BitProcess() ? AdvLib.LIBRARY_ADVLIB_CORE64 : AdvLib.LIBRARY_ADVLIB_CORE32;
		IntPtr handle = GetModuleHandle(dllName);

		var outputStr = new StringBuilder(1024);
		GetModuleFileName(handle, outputStr, 1024);

		return outputStr.ToString();
	}

	public static bool Is64BitProcess()
	{
		return AdvLib.Is64Bit();
	}
}
