using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DataStreamDefinition
{
    public int FrameCount;
    public long ClockFrequency;
    public int TimingAccuracy;
}

public class AdvFile2
public class AdvFile2 : IDisposable
{
    public DataStreamDefinition MainSteamInfo;
    public DataStreamDefinition CalibrationSteamInfo;

    public AdvFile2(string fileName)
    {
        int fileVersionOrErrorCode = AdvLib.AdvOpenFile(fileName);
        if (fileVersionOrErrorCode == 0)
            throw new AdvLibException(string.Format("'{0}' is not an ADV file.", fileName));
        else if (fileVersionOrErrorCode < 0)
            throw new AdvLibException(string.Format("There was an error opening '{0}'. Error code is: {1}", fileName, fileVersionOrErrorCode));
        else if (fileVersionOrErrorCode != 2)
            throw new AdvLibException(string.Format("'{0}' is not an ADV version 2 file.", fileName));

        MainSteamInfo = new DataStreamDefinition();
        AdvLib.GetMainStreamInfo(ref MainSteamInfo.FrameCount, ref MainSteamInfo.ClockFrequency, ref MainSteamInfo.TimingAccuracy);

        CalibrationSteamInfo = new DataStreamDefinition();
        AdvLib.GetCalibrationStreamInfo(ref CalibrationSteamInfo.FrameCount, ref CalibrationSteamInfo.ClockFrequency, ref CalibrationSteamInfo.TimingAccuracy);
    }

    public bool Close()
    {
        uint closedVer = AdvLib.AdvCloseFile();
        return closedVer > 1;
    }

    public void Dispose()
    {
        Close();
    }
}
