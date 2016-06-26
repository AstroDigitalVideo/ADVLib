using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Adv
{
    public class DataStreamDefinition
    {
        public int FrameCount;
        public long ClockFrequency;
        public int TimingAccuracy;

        public Dictionary<string, string> MetadataTags = new Dictionary<string, string>();
    }

    public class AdvFile2 : IDisposable
    {
        public DataStreamDefinition MainSteamInfo;
        public DataStreamDefinition CalibrationSteamInfo;

        public Dictionary<string, string> SystemMetadataTags = new Dictionary<string, string>();
        public Dictionary<string, string> UserMetadataTags = new Dictionary<string, string>();

        public int Width;
        public int Height;
        public int DataBpp;
        public int MaxPixelValue;
        public bool IsColourImage;
        public long UtcTimestampAccuracyInNanoseconds;

        public AdvFile2(string fileName)
        {
            AdvFileInfo fileInfo;
            int fileVersionOrErrorCode = AdvLib.AdvOpenFile(fileName, out fileInfo);
            if (fileVersionOrErrorCode == 0)
                throw new AdvLibException(string.Format("'{0}' is not an ADV file.", fileName));
            else if (fileVersionOrErrorCode < 0)
                throw new AdvLibException(string.Format("There was an error opening '{0}'. Error code is: {1}", fileName, fileVersionOrErrorCode));
            else if (fileVersionOrErrorCode != 2)
                throw new AdvLibException(string.Format("'{0}' is not an ADV version 2 file.", fileName));

            MainSteamInfo = new DataStreamDefinition();
            MainSteamInfo.FrameCount = fileInfo.CountMaintFrames;
            MainSteamInfo.ClockFrequency = fileInfo.MainClockFrequency;
            MainSteamInfo.TimingAccuracy = fileInfo.MainStreamAccuracy;

            CalibrationSteamInfo = new DataStreamDefinition();
            CalibrationSteamInfo.FrameCount = fileInfo.CountCalibrationFrames;
            CalibrationSteamInfo.ClockFrequency = fileInfo.CalibrationClockFrequency;
            CalibrationSteamInfo.TimingAccuracy = fileInfo.CalibrationStreamAccuracy;

            Width = fileInfo.Width;
            Height = fileInfo.Height;

            DataBpp = fileInfo.DataBpp;
            MaxPixelValue = fileInfo.MaxPixelValue;
            IsColourImage = fileInfo.IsColourImage;
            UtcTimestampAccuracyInNanoseconds = fileInfo.UtcTimestampAccuracyInNanoseconds;

            LoadTags(fileInfo);
        }

        private void LoadTags(AdvFileInfo fileInfo)
        {
            for (int i = 0; i < fileInfo.MainStreamTagsCount; i++)
            {
                string name;
                string value;
                if (AdvLib.GetMainStreamTag(i, out name, out value))
                    MainSteamInfo.MetadataTags.Add(name, value);
            }

            for (int i = 0; i < fileInfo.CalibrationStreamTagsCount; i++)
            {
                string name;
                string value;
                if (AdvLib.GetCalibrationStreamTag(i, out name, out value))
                    CalibrationSteamInfo.MetadataTags.Add(name, value);
            }

            for (int i = 0; i < fileInfo.SystemMetadataTagsCount; i++)
            {
                string name;
                string value;
                if (AdvLib.GetSystemMetadataTag(i, out name, out value))
                    SystemMetadataTags.Add(name, value);
            }

            for (int i = 0; i < fileInfo.UserMetadataTagsCount; i++)
            {
                string name;
                string value;
                if (AdvLib.GetUserMetadataTag(i, out name, out value))
                    UserMetadataTags.Add(name, value);
            }
        }

        public uint[] GetMainFramePixels(uint frameNo, out AdvFrameInfo frameInfo)
        {
            if (frameNo < MainSteamInfo.FrameCount)
            {
                uint[] pixels = AdvLib.GetFramePixels(0, (int)frameNo, 640, 480, out frameInfo);
                return pixels;
            }
            else
                throw new AdvLibException(string.Format("Main frame number must be bwtween 0 and {0}", MainSteamInfo.FrameCount - 1));
        }

        public uint[] GetMainFramePixels(uint frameNo)
        {
            if (frameNo < MainSteamInfo.FrameCount)
            {
                uint[] pixels = AdvLib.GetFramePixels(0, (int)frameNo, 640, 480);
                return pixels;
            }
            else
                throw new AdvLibException(string.Format("Main frame number must be bwtween 0 and {0}", MainSteamInfo.FrameCount - 1));
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
}

