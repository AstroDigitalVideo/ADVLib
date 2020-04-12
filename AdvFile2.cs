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

    public class ImageLayoutDefinition
    {
        public int LayoutId;
        public byte Bpp;

        public Dictionary<string, string> ImageLayoutTags = new Dictionary<string, string>();
    }

    public class AdvFile2 : IDisposable
    {
        public DataStreamDefinition MainSteamInfo;
        public DataStreamDefinition CalibrationSteamInfo;

        public List<ImageLayoutDefinition> ImageLayouts = new List<ImageLayoutDefinition>();
 
        public Dictionary<string, string> SystemMetadataTags = new Dictionary<string, string>();
        public Dictionary<string, string> UserMetadataTags = new Dictionary<string, string>();
        public Dictionary<string, string> ImageSectionTags = new Dictionary<string, string>();

        public List<AdvIndexEntry> MainIndex = new List<AdvIndexEntry>(); 
        public List<AdvIndexEntry> CalibrationIndex = new List<AdvIndexEntry>(); 

        public List<Tuple<string, uint, Adv2TagType>> StatusTagDefinitions = new List<Tuple<string, uint, Adv2TagType>>(); 

        public int Width;
        public int Height;
        public int DataBpp;
        public int MaxPixelValue;
        public bool IsColourImage;
        public long UtcTimestampAccuracyInNanoseconds;

        public AdvFile2(string fileName)
        {
            AdvFileInfo fileInfo;
            int fileVersionOrErrorCode = AdvLib.OpenFile(fileName, out fileInfo);
            if (fileVersionOrErrorCode == 0)
                throw new AdvLibException(string.Format("'{0}' is not an ADV file.", fileName));
            else if (fileVersionOrErrorCode < 0)
                throw new AdvLibException(string.Format("There was an error opening '{0}'. {1}", fileName, AdvError.ResolveErrorMessage(fileVersionOrErrorCode)));
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

            LoadImageLayoutInfo(fileInfo);
            LoadTags(fileInfo);
            EnsureStatusTagDefinitions(fileInfo);

            AdvIndexEntry[] mainIndex;
            AdvIndexEntry[] calibIndex;
            AdvLib.GetIndexEntries(MainSteamInfo.FrameCount, CalibrationSteamInfo.FrameCount, out mainIndex, out calibIndex);
            MainIndex = mainIndex.ToList();
            CalibrationIndex = calibIndex.ToList();
        }

        private void LoadImageLayoutInfo(AdvFileInfo fileInfo)
        {
            for (int i = 0; i < fileInfo.ImageLayoutsCount; i++)
            {
                AdvImageLayoutInfo info;
                int errorCode = AdvLib.GetImageLayoutInfo(i, out info);
                AdvError.Check(errorCode);

                var layout = new ImageLayoutDefinition()
                {
                    LayoutId = info.ImageLayoutId,
                    Bpp = info.ImageLayoutBpp
                };
                
                for (int j = 0; j < info.ImageLayoutTagsCount; j++)
                {
                    string name;
                    string value;
                    if (AdvLib.GetImageLayoutTag(info.ImageLayoutId, j, out name, out value) == AdvError.S_OK)
                        layout.ImageLayoutTags.Add(name, value);
                }

                ImageLayouts.Add(layout);
            }
        }

        private void LoadTags(AdvFileInfo fileInfo)
        {
            for (int i = 0; i < fileInfo.MainStreamTagsCount; i++)
            {
                string name;
                string value;
                if (AdvLib.GetMainStreamTag(i, out name, out value) == AdvError.S_OK)
                    MainSteamInfo.MetadataTags.Add(name, value);
            }

            for (int i = 0; i < fileInfo.CalibrationStreamTagsCount; i++)
            {
                string name;
                string value;
                if (AdvLib.GetCalibrationStreamTag(i, out name, out value) == AdvError.S_OK)
                    CalibrationSteamInfo.MetadataTags.Add(name, value);
            }

            for (int i = 0; i < fileInfo.SystemMetadataTagsCount; i++)
            {
                string name;
                string value;
                if (AdvLib.GetSystemMetadataTag(i, out name, out value) == AdvError.S_OK)
                    SystemMetadataTags.Add(name, value);
            }

            for (int i = 0; i < fileInfo.UserMetadataTagsCount; i++)
            {
                string name;
                string value;
                if (AdvLib.GetUserMetadataTag(i, out name, out value) == AdvError.S_OK)
                    UserMetadataTags.Add(name, value);
            }

            for (int i = 0; i < fileInfo.ImageSectionTagsCount; i++)
            {
                string name;
                string value;
                if (AdvLib.GetImageSectionTag(i, out name, out value) == AdvError.S_OK)
                    ImageSectionTags.Add(name, value);
            }
        }

        private void EnsureStatusTagDefinitions(AdvFileInfo fileInfo)
        {
            for (uint i = 0; i < fileInfo.StatusTagsCount; i++)
            {
                Adv2TagType? tagType;
                string tagName;
                if (AdvLib.GetStatusTagInfo(i, out tagType, out tagName) == AdvError.S_OK &&
                    !string.IsNullOrEmpty(tagName) &&
                    tagType != null)
                {
                    StatusTagDefinitions.Add(new Tuple<string, uint, Adv2TagType>(tagName, i, tagType.Value));
                }                    
            }            
        }

        public uint[] GetMainFramePixels(uint frameNo, out AdvFrameInfo frameInfo)
        {
            if (frameNo < MainSteamInfo.FrameCount)
            {
                uint[] pixels;
                int errorCode = AdvLib.GetFramePixels(0, (int)frameNo, Width, Height, out frameInfo, out pixels);
                AdvError.Check(errorCode); 
                foreach (var entry in StatusTagDefinitions)
                {
                    byte? val8;
                    short? val16;
                    int? val32;
                    long? val64;
                    float? valf;
                    string vals;

                    switch (entry.Item3)
                    {
                        case Adv2TagType.Int8:
                            if (AdvLib.GetStatusTagUInt8(entry.Item2, out val8) == AdvError.S_OK && val8.HasValue)
                                frameInfo.Status.Add(entry.Item1, val8.Value);
                            break;
                        case Adv2TagType.Int16:
                            if (AdvLib.GetStatusTagInt16(entry.Item2, out val16) == AdvError.S_OK && val16.HasValue)
                                frameInfo.Status.Add(entry.Item1, val16.Value);
                            break;
                        case Adv2TagType.Int32:
                            if (AdvLib.GetStatusTagInt32(entry.Item2, out val32) == AdvError.S_OK && val32.HasValue)
                                frameInfo.Status.Add(entry.Item1, val32.Value);
                            break;
                        case Adv2TagType.Long64:
                            if (AdvLib.GetStatusTagInt64(entry.Item2, out val64) == AdvError.S_OK && val64.HasValue)
                                frameInfo.Status.Add(entry.Item1, val64.Value);
                            break;
                        case Adv2TagType.Real:
                            if (AdvLib.GetStatusTagFloat(entry.Item2, out valf) == AdvError.S_OK && valf.HasValue)
                                frameInfo.Status.Add(entry.Item1, valf.Value);
                            break;
                        case Adv2TagType.UTF8String:
                            if (AdvLib.GetStatusTagUTF8String(entry.Item2, out vals) == AdvError.S_OK)
                                frameInfo.Status.Add(entry.Item1, vals);
                            break;
                    }                    
                }
                return pixels;
            }
            else
                throw new AdvLibException(string.Format("Main frame number must be bwtween 0 and {0}", MainSteamInfo.FrameCount - 1));
        }

        public uint[] GetMainFramePixels(uint frameNo)
        {
            if (frameNo < MainSteamInfo.FrameCount)
            {
                uint[] pixels = AdvLib.GetFramePixels(0, (int)frameNo, Width, Height);
                return pixels;
            }
            else
                throw new AdvLibException(string.Format("Main frame number must be bwtween 0 and {0}", MainSteamInfo.FrameCount - 1));
        }

        public uint[] GetStackedMainFramePixels(uint frameNo, uint framesToStack, bool isSlidingIntegration, out AdvFrameInfo frameInfo)
        {
            if (frameNo < MainSteamInfo.FrameCount)
            {
                uint[] pixels = AdvLib.GetStackedFramePixels(0, (int)frameNo, (int)framesToStack, Width, Height, isSlidingIntegration, out frameInfo);
                return pixels;
            }
            else
                throw new AdvLibException(string.Format("Main frame number must be bwtween 0 and {0}", MainSteamInfo.FrameCount - 1));
        }

        public uint[] GetCalibrationFramePixels(uint frameNo, out AdvFrameInfo frameInfo)
        {
            if (frameNo < CalibrationSteamInfo.FrameCount)
            {
                uint[] pixels;
                int errorCode = AdvLib.GetFramePixels(1, (int)frameNo, Width, Height, out frameInfo, out pixels);
                AdvError.Check(errorCode);
                foreach (var entry in StatusTagDefinitions)
                {
                    byte? val8;
                    short? val16;
                    int? val32;
                    long? val64;
                    float? valf;
                    string vals;

                    switch (entry.Item3)
                    {
                        case Adv2TagType.Int8:
                            if (AdvLib.GetStatusTagUInt8(entry.Item2, out val8) == AdvError.S_OK && val8.HasValue)
                                frameInfo.Status.Add(entry.Item1, val8.Value);
                            break;
                        case Adv2TagType.Int16:
                            if (AdvLib.GetStatusTagInt16(entry.Item2, out val16) == AdvError.S_OK && val16.HasValue)
                                frameInfo.Status.Add(entry.Item1, val16.Value);
                            break;
                        case Adv2TagType.Int32:
                            if (AdvLib.GetStatusTagInt32(entry.Item2, out val32) == AdvError.S_OK && val32.HasValue)
                                frameInfo.Status.Add(entry.Item1, val32.Value);
                            break;
                        case Adv2TagType.Long64:
                            if (AdvLib.GetStatusTagInt64(entry.Item2, out val64) == AdvError.S_OK && val64.HasValue)
                                frameInfo.Status.Add(entry.Item1, val64.Value);
                            break;
                        case Adv2TagType.Real:
                            if (AdvLib.GetStatusTagFloat(entry.Item2, out valf) == AdvError.S_OK && valf.HasValue)
                                frameInfo.Status.Add(entry.Item1, valf.Value);
                            break;
                        case Adv2TagType.UTF8String:
                            if (AdvLib.GetStatusTagUTF8String(entry.Item2, out vals) == AdvError.S_OK)
                                frameInfo.Status.Add(entry.Item1, vals);
                            break;
                    }
                }
                return pixels;
            }
            else
                throw new AdvLibException(string.Format("Calibration frame number must be bwtween 0 and {0}", CalibrationSteamInfo.FrameCount - 1));
        }

        public uint[] GetCalibrationFramePixels(uint frameNo)
        {
            if (frameNo < CalibrationSteamInfo.FrameCount)
            {
                uint[] pixels = AdvLib.GetFramePixels(1, (int)frameNo, Width, Height);
                return pixels;
            }
            else
                throw new AdvLibException(string.Format("Calibration frame number must be bwtween 0 and {0}", CalibrationSteamInfo.FrameCount - 1));
        }

        public bool Close()
        {
            uint closedVer = AdvLib.CloseFile();
            return closedVer > 1;
        }

        public void Dispose()
        {
            Close();
        }
    }
}

