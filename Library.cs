using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Library
{
	public static string GetVersion()
	{
		return AdvLib.GetLibraryVersion();
	}

	public static string GetPlatformId()
	{
		return AdvLib.GetLibraryPlatformId();
	}

	public static bool Is64BitProcess()
	{
		return AdvLib.Is64Bit();
	}
}
