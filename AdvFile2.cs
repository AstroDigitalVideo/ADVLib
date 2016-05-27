using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AdvFile2
{
    public AdvFile2(string fileName)
    {
        uint fileVersion = AdvLib.AdvOpenFile(fileName);
        if (fileVersion == 0)
            throw new AdvLibException(string.Format("'{0}' is not an ADV file.", fileName));
        else if (fileVersion != 2)
            throw new AdvLibException(string.Format("'{0}' is not an ADV version 2 file.", fileName));
    }
}
