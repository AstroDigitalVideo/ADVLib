using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AdvFile2
{
    public AdvFile2(string fileName)
    {
        int fileVersionOrErrorCode = AdvLib.AdvOpenFile(fileName);
        if (fileVersionOrErrorCode == 0)
            throw new AdvLibException(string.Format("'{0}' is not an ADV file.", fileName));
        else if (fileVersionOrErrorCode < 0)
            throw new AdvLibException(string.Format("There was an error opening '{0}'. Error code is: {1}", fileName, fileVersionOrErrorCode));
        else if (fileVersionOrErrorCode != 2)
            throw new AdvLibException(string.Format("'{0}' is not an ADV version 2 file.", fileName));
    }
}
