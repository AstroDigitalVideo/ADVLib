using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AdvLibException : Exception
{
	public AdvLibException(string message)
		: base(message)
	{ }

	public AdvLibException(string message, Exception innerException)
		: base(message, innerException)
	{ }
}
