/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Adv
{
    public enum AdvTagType
    {
        UInt8 = 0,
        UInt16 = 1,
        UInt32 = 2,
        ULong64 = 3,
        Real = 4, // IEEE/REAL*4
        AnsiString255 = 5,
        List16OfAnsiString255 = 6
    };

    public enum Adv2TagType
    {
        Int8 = 0,
        Int16 = 1,
        Int32 = 2,
        Long64 = 3,
        Real = 4,
        UTF8String = 5
    };

    public enum TagPairType
    {
        MainStream = 0,
        CalibrationStream = 1,
        SystemMetadata = 2,
        UserMetadata = 3,
        ImageSection = 4,
        FirstImageLayout = 100
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AdvImageLayoutInfo
    {
        public static AdvImageLayoutInfo Empty = new AdvImageLayoutInfo();

        public int ImageLayoutId;
        public int ImageLayoutTagsCount;
        public byte ImageLayoutBpp;
        public bool IsFullImageRaw;
        public bool Is12BitImagePacked;
        public bool Is8BitColourImage;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct AdvFileInfo
    {
        public int Width;
        public int Height;
        public int CountMaintFrames;
        public int CountCalibrationFrames;
        public int DataBpp;
        public int MaxPixelValue;
        public long MainClockFrequency;
        public int MainStreamAccuracy;
        public long CalibrationClockFrequency;
        public int CalibrationStreamAccuracy;
        public byte MainStreamTagsCount;
        public byte CalibrationStreamTagsCount;
        public byte SystemMetadataTagsCount;
        public byte UserMetadataTagsCount;
        public long UtcTimestampAccuracyInNanoseconds;
        public bool IsColourImage;
        public int ImageLayoutsCount;
        public int StatusTagsCount;
        public int ImageSectionTagsCount;
        public int ErrorStatusTagId;
    };

    public class AdvFrameInfo : AdvFrameInfoNative
    {
        public Dictionary<string, object> Status = new Dictionary<string, object>();

        public int ErrorMessageStrLen = 0;

        public bool HasErrorMessage {
            get { return ErrorMessageStrLen > 0; }
        }

        internal AdvFrameInfo(AdvFrameInfoNative native)
        {
            StartTicksLo = native.StartTicksLo;
            StartTicksHi = native.StartTicksHi;
            EndTicksLo = native.EndTicksLo;
            EndTicksHi = native.EndTicksHi;

            UtcTimestampLo = native.UtcTimestampLo;
            UtcTimestampHi = native.UtcTimestampHi;
            Exposure = native.Exposure;

            Gamma = native.Gamma;
            Gain = native.Gain;
            Shutter = native.Shutter;
            Offset = native.Offset;

            GPSTrackedSattelites = native.GPSTrackedSattelites;
            GPSAlmanacStatus = native.GPSAlmanacStatus;
            GPSFixStatus = native.GPSFixStatus;
            GPSAlmanacOffset = native.GPSAlmanacOffset;

            VideoCameraFrameIdLo = native.VideoCameraFrameIdLo;
            VideoCameraFrameIdHi = native.VideoCameraFrameIdHi;
            HardwareTimerFrameIdLo = native.HardwareTimerFrameIdLo;
            HardwareTimerFrameIdHi = native.HardwareTimerFrameIdHi;
            SystemTimestampLo = native.SystemTimestampLo;
            SystemTimestampHi = native.SystemTimestampHi;
        }

        public ulong HardwareTimerFrameId
        {
            get { return (((ulong)HardwareTimerFrameIdHi) << 32) + (ulong)HardwareTimerFrameIdLo; }
        }


        public ulong VideoCameraFrameId
        {
            get { return (((ulong)VideoCameraFrameIdHi) << 32) + (ulong)VideoCameraFrameIdLo; }
        }

        public bool HasUtcTimeStamp
        {
            get { return UtcTimestampLo != 0 && UtcTimestampHi != 0; }
        }

        public DateTime UtcStartExposureTimeStamp
        {
            get
            {
                ulong nanosecondsElapsed = (((ulong)UtcTimestampHi) << 32) + (ulong)UtcTimestampLo;
                try
                {
                    return new DateTime(REFERENCE_DATETIME.Ticks + (long)(nanosecondsElapsed / 100));
                }
                catch (ArgumentOutOfRangeException)
                {
                    return REFERENCE_DATETIME;
                }
            }
        }

        public DateTime UtcMidExposureTime
        {
            get
            {
                ulong nanosecondsElapsed = (((ulong)UtcTimestampHi) << 32) + (ulong)UtcTimestampLo;
                try
                {
                    return new DateTime(REFERENCE_DATETIME.Ticks + (long)(nanosecondsElapsed / 100)).AddMilliseconds(UtcExposureMilliseconds / 2.0);
                }
                catch (ArgumentOutOfRangeException)
                {
                    return REFERENCE_DATETIME;
                }
            }
        }

        public float UtcExposureMilliseconds
        {
            get { return Exposure / 1000000.0f; }
        }

        public DateTime SystemTimestamp
        {
            get
            {
                ulong nanosecondsElapsed = (((ulong)SystemTimestampHi) << 32) + (ulong)SystemTimestampLo;
                try
                {
                    return new DateTime(REFERENCE_DATETIME.Ticks + (long)(nanosecondsElapsed / 100));
                }
                catch (ArgumentOutOfRangeException)
                {
                    return REFERENCE_DATETIME;
                }
            }
        }

        public ulong TickStampStartTicks
        {
            get { return (((ulong)StartTicksHi) << 32) + (ulong)StartTicksLo; }
        }

        public ulong TickStampEndTicks
        {
            get { return (((ulong)EndTicksHi) << 32) + (ulong)EndTicksLo; }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public class AdvFrameInfoNative
    {
        protected static DateTime REFERENCE_DATETIME = new DateTime(2010, 1, 1, 0, 0, 0, 0);

        public AdvFrameInfoNative()
        {
            StartTicksLo = 0;
            StartTicksHi = 0;
            EndTicksLo = 0;
            EndTicksHi = 0;

            UtcTimestampLo = 0;
            UtcTimestampHi = 0;
            Exposure = 0;

            Gamma = 0f;
            Gain = 0f;
            Shutter = 0f;
            Offset = 0f;
            
            GPSTrackedSattelites = 0;
            GPSAlmanacStatus = 0;
            GPSFixStatus = 0;
            GPSAlmanacOffset = 0;
            
            VideoCameraFrameIdLo = 0;
            VideoCameraFrameIdHi = 0;
            HardwareTimerFrameIdLo = 0;
            HardwareTimerFrameIdHi = 0;

            SystemTimestampLo = 0;
            SystemTimestampHi = 0;
        }

        [FieldOffset(0)]
        public uint StartTicksLo;
        [FieldOffset(4)]
        public uint StartTicksHi;
        [FieldOffset(8)]
        public uint EndTicksLo;
        [FieldOffset(12)]
        public uint EndTicksHi;
        [FieldOffset(16)]
        public uint UtcTimestampLo;
        [FieldOffset(20)]
        public uint UtcTimestampHi;
        [FieldOffset(24)]
        public uint Exposure;
        [FieldOffset(28)]
        public float Gamma;
        [FieldOffset(32)]
        public float Gain;
        [FieldOffset(36)]
        public float Shutter;
        [FieldOffset(40)]
        public float Offset;
        [FieldOffset(44)]
        public byte GPSTrackedSattelites;
        [FieldOffset(45)]
        public byte GPSAlmanacStatus;
        [FieldOffset(46)]
        public byte GPSFixStatus;
        [FieldOffset(47)]
        public byte GPSAlmanacOffset;
        [FieldOffset(48)]
        public uint VideoCameraFrameIdLo;
        [FieldOffset(52)]
        public uint VideoCameraFrameIdHi;
        [FieldOffset(56)]
        public uint HardwareTimerFrameIdLo;
        [FieldOffset(60)]
        public uint HardwareTimerFrameIdHi;
        [FieldOffset(64)]
        public uint SystemTimestampLo;
        [FieldOffset(68)]
        public uint SystemTimestampHi;
    }


    public static class AdvLib
    {

#if !__linux__
        [DllImport("kernel32.dll", SetLastError = false)]
        private static extern bool SetDllDirectory(string lpPathName);

        static AdvLib()
        {
            SetDllDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }
#endif
        internal const string LIBRARY_ADVLIB_CORE32 = "AdvLib.Core32.dll";
        internal const string LIBRARY_ADVLIB_CORE64 = "AdvLib.Core64.dll";
        internal const string LIBRARY_ADVLIB_CORE_UNIX = "AdvCore";

        #region 32bit externals
        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLibraryVersion")]
        private static extern void GetLibraryVersion32([MarshalAs(UnmanagedType.LPArray)]byte[] version);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLibraryPlatformId")]
        private static extern void GetLibraryPlatformId32([MarshalAs(UnmanagedType.LPArray)]byte[] platform);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLibraryBitness")]
        private static extern int GetLibraryBitness32();

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvOpenFile")]
        //unsigned int AdvOpenFile(const char* fileName, AdvLib2::AdvFileInfo* fileInfo);
        private static extern int AdvOpenFile32(string fileName, [In, Out] ref AdvFileInfo fileInfo);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvCloseFile")]
        //unsigned int AdvCloseFile();
        private static extern uint AdvCloseFile32();

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvGetFileVersion")]
        //unsigned int AdvGetFileVersion(const char* fileName);
        private static extern uint AdvGetFileVersion32(string fileName);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_NewFile")]
        //void AdvNewFile(const char* fileName);
        private static extern void AdvVer1_NewFile32(string fileName);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_EndFile")]
        //void AdvEndFile();
        private static extern void AdvVer1_EndFile32();

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvGetCurrentFilePath")]
        //char* AdvGetCurrentFilePath(void);
        private static extern string AdvGetCurrentFilePath32();

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_DefineImageSection")]
        //void AdvDefineImageSection(unsigned short width, unsigned short height, unsigned char dataBpp);
        private static extern void AdvVer1_DefineImageSection32(ushort width, ushort height, byte dataBpp);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "AdvVer1_DefineImageLayout")]
        //void AdvDefineImageLayout(unsigned char layoutId, const char* layoutType, const char* compression, unsigned char layoutBpp, int keyFrame, const char* diffCorrFromBaseFrame);
        private static extern void AdvVer1_DefineImageLayout32(byte layoutId, [MarshalAs(UnmanagedType.LPStr)]string layoutType, [MarshalAs(UnmanagedType.LPStr)]string compression, byte layoutBpp, int keyFrame, [MarshalAs(UnmanagedType.LPStr)]string diffCorrFromBaseFrame);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "AdvVer1_DefineStatusSectionTag")]
        //unsigned int AdvDefineStatusSectionTag(const char* tagName, int tagType);
        private static extern uint AdvVer1_DefineStatusSectionTag32([MarshalAs(UnmanagedType.LPStr)]string tagName, AdvTagType tagType);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "AdvVer1_AddFileTag")]
        //unsigned int AdvAddFileTag(const char* tagName, const char* tagValue);
        private static extern uint AdvVer1_AddFileTag32([MarshalAs(UnmanagedType.LPStr)]string tagName, [MarshalAs(UnmanagedType.LPStr)]string tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "AdvVer1_AddOrUpdateImageSectionTag")]
        //void AdvAddOrUpdateImageSectionTag(const char* tagName, const char* tagValue);
        private static extern uint AdvVer1_AddOrUpdateImageSectionTag32([MarshalAs(UnmanagedType.LPStr)]string tagName, [MarshalAs(UnmanagedType.LPStr)]string tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_BeginFrame")]
        //bool AdvBeginFrame(long long timeStamp, unsigned int elapsedTime, unsigned int exposure);
        private static extern bool AdvVer1_BeginFrame32(long timeStamp, uint elapsedTime, uint exposure);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_FrameAddImage")]
        //void AdvFrameAddImage(unsigned char layoutId, unsigned short* pixels, unsigned char pixelsBpp);
        private static extern void AdvVer1_FrameAddImage32(byte layoutId, [In, MarshalAs(UnmanagedType.LPArray)] ushort[] pixels, byte pixelsBpp);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_FrameAddImageBytes")]
        //void AdvFrameAddImageBytes(unsigned char layoutId, unsigned char* pixels, unsigned char pixelsBpp);
        private static extern void AdvVer1_FrameAddImageBytes32(byte layoutId, [In, MarshalAs(UnmanagedType.LPArray)] byte[] pixels, byte pixelsBpp);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "AdvVer1_FrameAddStatusTag")]
        //void AdvFrameAddStatusTag(unsigned int tagIndex, const char* tagValue);
        private static extern void AdvVer1_FrameAddStatusTag32(uint tagIndex, [MarshalAs(UnmanagedType.LPStr)]string tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "AdvVer1_FrameAddStatusTagMessage")]
        //void AdvFrameAddStatusTagMessage(unsigned int tagIndex, const char* tagValue);
        private static extern void AdvVer1_FrameAddStatusTagMessage32(uint tagIndex, [MarshalAs(UnmanagedType.LPStr)]string tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_FrameAddStatusTagUInt8")]
        //void AdvFrameAddStatusTagUInt8(unsigned int tagIndex, unsigned char tagValue);
        private static extern void AdvVer1_FrameAddStatusTagUInt832(uint tagIndex, byte tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_FrameAddStatusTag16")]
        //void AdvFrameAddStatusTag16(unsigned int tagIndex, unsigned short tagValue);
        private static extern void AdvVer1_FrameAddStatusTag1632(uint tagIndex, ushort tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_FrameAddStatusTagReal")]
        //void AdvFrameAddStatusTagReal(unsigned int tagIndex, float tagValue);
        private static extern void AdvVer1_FrameAddStatusTagReal32(uint tagIndex, float tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_FrameAddStatusTag32")]
        //void AdvFrameAddStatusTag32(unsigned int tagIndex, unsigned long tagValue);
        private static extern void AdvVer1_FrameAddStatusTag3232(uint tagIndex, uint tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_FrameAddStatusTag64")]
        //void AdvFrameAddStatusTag64(unsigned int tagIndex, long long tagValue);
        private static extern void AdvVer1_FrameAddStatusTag6432(uint tagIndex, ulong tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_EndFrame")]
        //void AdvEndFrame();
        private static extern void AdvVer1_EndFrame32();


        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_NewFile")]
        //DLL_PUBLIC void AdvVer2_NewFile(const char* fileName);
        private static extern void AdvVer2_NewFile32(string fileName);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_EndFile")]
        //DLL_PUBLIC ADVRESULT AdvVer2_EndFile();
        private static extern int AdvVer2_EndFile32();

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_SetTicksTimingPrecision")]
        //DLL_PUBLIC void AdvVer2_SetTicksTimingPrecision(int mainStreamAccuracy, int calibrationStreamAccuracy);
        private static extern void AdvVer2_SetTicksTimingPrecision32(int mainStreamAccuracy, int calibrationStreamAccuracy);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_DefineExternalClockForMainStream")]
        //DLL_PUBLIC void AdvVer2_DefineExternalClockForMainStream(__int64 clockFrequency, int ticksTimingAccuracy);
        private static extern void AdvVer2_DefineExternalClockForMainStream32(long clockFrequency, int ticksTimingAccuracy);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_DefineExternalClockForCalibrationStream")]
        //DLL_PUBLIC void AAdvVer2_DefineExternalClockForCalibrationStream(__int64 clockFrequency, int ticksTimingAccuracy);
        private static extern void AdvVer2_DefineExternalClockForCalibrationStream32(long clockFrequency, int ticksTimingAccuracy);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_AddMainStreamTag")]
        //DLL_PUBLIC unsigned int AdvVer2_AddMainStreamTag(const char* tagName, const char* tagValue);
        private static extern int AdvVer2_AddMainStreamTag32([MarshalAs(UnmanagedType.LPArray)]byte[] tagName, [MarshalAs(UnmanagedType.LPArray)]byte[] tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_AddCalibrationStreamTag")]
        //DLL_PUBLIC unsigned int AdvVer2_AddCalibrationStreamTag(const char* tagName, const char* tagValue);
        private static extern int AdvVer2_AddCalibrationStreamTag32([MarshalAs(UnmanagedType.LPArray)]byte[] tagName, [MarshalAs(UnmanagedType.LPArray)]byte[] tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_BeginFrameWithTicks")]
        //ADVRESULT AdvBeginFrame(unsigned char streamId, __int64 startFrameTicks, __int64 endFrameTicks, __int64 elapsedTicksSinceFirstFrame, __int64 utcStartTimeNanosecondsSinceAdvZeroEpoch, unsigned int utcExposureNanoseconds));
        private static extern int AdvVer2_BeginFrame32(uint streamId, long startTicks, long endTicks, long elapsedTicksFromStart, ulong utcStartTimeNanosecondsSinceAdvZeroEpoch, uint utcExposureNanoseconds);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_BeginFrame")]
        //ADVRESULT AdvBeginFrame(unsigned char streamId, __int64 utcStartTimeNanosecondsSinceAdvZeroEpoch, unsigned int utcExposureNanoseconds));
        private static extern int AdvVer2_BeginFrame32(uint streamId, ulong utcStartTimeNanosecondsSinceAdvZeroEpoch, uint utcExposureNanoseconds);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_DefineImageSection")]
        //ADVRESULT AdvVer2_DefineImageSection(unsigned short width, unsigned short height, unsigned char dataBpp);
        private static extern int AdvVer2_DefineImageSection32(ushort width, ushort height, byte dataBpp);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_DefineStatusSection")]
        //ADVRESULT AdvVer2_DefineStatusSection(__int64 utcTimestampAccuracyInNanoseconds);
        private static extern int AdvVer2_DefineStatusSection32(long utcTimestampAccuracyInNanoseconds);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_DefineImageLayout")]
        //ADVRESULT AdvVer2_DefineImageLayout(unsigned char layoutId, const char* layoutType, const char* compression, unsigned char layoutBpp);
        private static extern int AdvVer2_DefineImageLayout32(byte layoutId, [MarshalAs(UnmanagedType.LPStr)]string layoutType, [MarshalAs(UnmanagedType.LPStr)]string compression, byte layoutBpp);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_DefineStatusSectionTag")]
        //ADVRESULT AdvVer2_DefineStatusSectionTag(const char* tagName, int tagType, unsigned int* addedTagId);
        private static extern int AdvVer2_DefineStatusSectionTag32([MarshalAs(UnmanagedType.LPArray)]byte[] tagName, Adv2TagType tagType, ref uint addedTagId);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_AddFileTag")]
        //ADVRESULT AdvVer2_AddFileTag(const char* tagName, const char* tagValue);
        private static extern int AdvVer2_AddFileTag32([MarshalAs(UnmanagedType.LPArray)]byte[] tagName, [MarshalAs(UnmanagedType.LPArray)]byte[] tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_AddUserTag")]
        //ADVRESULT AdvVer2_AddUserTag(const char* tagName, const char* tagValue);
        private static extern int AdvVer2_AddUserTag32([MarshalAs(UnmanagedType.LPArray)]byte[] tagName, [MarshalAs(UnmanagedType.LPArray)]byte[] tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_AddOrUpdateImageSectionTag")]
        //ADVRESULT AdvVer2_AddOrUpdateImageSectionTag(const char* tagName, const char* tagValue);
        private static extern int AdvVer2_AddOrUpdateImageSectionTag32([MarshalAs(UnmanagedType.LPArray)]byte[] tagName, [MarshalAs(UnmanagedType.LPArray)]byte[] tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTagUTF8String")]
        //ADVRESULT AdvVer2_FrameAddStatusTagUTF8String(unsigned int tagIndex, const char* tagValue);
        private static extern int AdvVer2_FrameAddStatusTagUTF8String32(uint tagIndex, [MarshalAs(UnmanagedType.LPArray)]byte[] tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTagUInt8")]
        //ADVRESULT AdvVer2_FrameAddStatusTagUInt8(unsigned int tagIndex, unsigned char tagValue);
        private static extern int AdvVer2_FrameAddStatusTagUInt8_32(uint tagIndex, byte tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTag16")]
        //ADVRESULT AdvVer2_FrameAddStatusTag16(unsigned int tagIndex, unsigned short tagValue);
        private static extern int AdvVer2_FrameAddStatusTag16_32(uint tagIndex, short tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTagReal")]
        //ADVRESULT AdvVer2_FrameAddStatusTagReal(unsigned int tagIndex, float tagValue);
        private static extern int AdvVer2_FrameAddStatusTagReal_32(uint tagIndex, float tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTag32")]
        //ADVRESULT AdvVer2_FrameAddStatusTag32(unsigned int tagIndex, unsigned long tagValue);
        private static extern int AdvVer2_FrameAddStatusTag32_32(uint tagIndex, int tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTag64")]
        //ADVRESULT AdvVer2_FrameAddStatusTag64(unsigned int tagIndex, long long tagValue);
        private static extern int AdvVer2_FrameAddStatusTag64_32(uint tagIndex, long tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddImage")]
        //ADVRESULT AdvVer2_FrameAddImage(unsigned char layoutId, unsigned short* pixels, unsigned char pixelsBpp);
        private static extern int AdvVer2_FrameAddImage_32(byte layoutId, [In, MarshalAs(UnmanagedType.LPArray)] ushort[] pixels, byte pixelsBpp);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddImageBytes")]
        //ADVRESULT AdvVer2_FrameAddImageBytes(unsigned char layoutId, unsigned char* pixels, unsigned char pixelsBpp);
        private static extern int AdvVer2_FrameAddImageBytes_32(byte layoutId, [In, MarshalAs(UnmanagedType.LPArray)] byte[] pixels, byte pixelsBpp);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_EndFrame")]
        //ADVRESULT AdvVer2_EndFrame();
        private static extern int AdvVer2_EndFrame32();

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetFramePixels")]
        //ADVRESULT AdvVer2_GetFramePixels(int streamId, int frameNo, unsigned int* pixels, AdvLib2::AdvFrameInfo* frameInfo, int* systemErrorLen);
        private static extern int AdvVer2_GetFramePixels32(int streamId, int frameNo, [In, Out] uint[] pixels, [In, Out] AdvFrameInfoNative frameInfo, [In, Out] int systemErrorLen);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetTagPairValues")]
        //ADVRESULT AdvVer2_GetTagPairValues(TagPairType tagPairType, int tagId, char* tagName, char* tagValue)
        private static extern int AdvVer2_GetTagPairValues32(TagPairType tagPairType, int tagId, [In, Out] byte[] tagName, [In, Out] byte[] tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetTagPairSizes")]
        //ADVRESULT AdvVer2_GetTagPairSizes(TagPairType tagPairType, int tagId, int* tagNameSize, int* tagValueSize);
        private static extern int AdvVer2_GetTagPairSizes32(TagPairType tagPairType, int tagId, ref int tagNameSize, ref int tagValueSize);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetStatusTagSizeUTF8String")]
        //ADVRESULT AdvVer2_GetStatusTagSizeUTF8String(unsigned int tagIndex, int* tagValueSize);
        private static extern int AdvVer2_GetStatusTagSizeUTF8String32(uint tagId, ref int tagValueSize);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetStatusTagUTF8String")]
        //ADVRESULT AdvVer2_GetStatusTagUTF8String(unsigned int tagIndex, char* tagValue);
        private static extern int AdvVer2_GetStatusTagUTF8String32(uint tagId, [In, Out] byte[] tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetStatusTagUInt8")]
        //ADVRESULT AdvVer2_GetStatusTagUInt8(unsigned int tagIndex, unsigned char* tagValue);
        private static extern int AdvVer2_GetStatusTagUInt8_32(uint tagId, ref byte tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetStatusTag16")]
        //ADVRESULT AdvVer2_GetStatusTag16(unsigned int tagIndex, unsigned short* tagValue);
        private static extern int AdvVer2_GetStatusTag16_32(uint tagId, ref short tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetStatusTagReal")]
        //ADVRESULT AdvVer2_GetStatusTagReal(unsigned int tagIndex, float* tagValue);
        private static extern int AdvVer2_GetStatusTagReal_32(uint tagId, ref float tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetStatusTag32")]
        //ADVRESULT AdvVer2_GetStatusTag32(unsigned int tagIndex, unsigned int* tagValue);
        private static extern int AdvVer2_GetStatusTag32_32(uint tagId, ref int tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetStatusTag64")]
        //ADVRESULT AdvVer2_GetStatusTag64(unsigned int tagIndex, __int64* tagValue);
        private static extern int AdvVer2_GetStatusTag64_32(uint tagId, ref long tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetStatusTagNameSize")]
        //DLL_PUBLIC ADVRESULT AdvVer2_GetStatusTagNameSize(unsigned int tagId, int* tagNameSize);
        private static extern int AdvVer2_GetStatusTagNameSize32(uint tagId, ref int tagNameSize);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetStatusTagInfo")]
        //DLL_PUBLIC ADVRESULT AdvVer2_GetStatusTagInfo(unsigned int tagId, int* tagNameSize);
        private static extern int AdvVer2_GetStatusTagInfo32(uint tagId,[In, Out] byte[] tagName, ref Adv2TagType tagType);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetImageLayoutInfo")]
        //DLL_PUBLIC ADVRESULT AdvVer2_GetImageLayoutInfo(int layoutIndex, AdvLib2::AdvImageLayoutInfo* imageLayoutInfo);
        private static extern int AdvVer2_GetImageLayoutInfo32(int layoutIndex, ref AdvImageLayoutInfo imageLayoutInfo);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetLastSystemSpecificFileError")]       
        //DLL_PUBLIC int AdvVer2_GetLastSystemSpecificFileError();
        private static extern int AdvVer2_GetLastSystemSpecificFileError32();

        #endregion 

        #region 64bit externals
        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLibraryVersion")]
        private static extern void GetLibraryVersion64([MarshalAs(UnmanagedType.LPArray)]byte[] version);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLibraryPlatformId")]
        private static extern void GetLibraryPlatformId64([MarshalAs(UnmanagedType.LPArray)]byte[] platform);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLibraryBitness")]
        private static extern int GetLibraryBitness64();

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvOpenFile")]
        //unsigned int AdvOpenFile(const char* fileName, AdvLib2::AdvFileInfo* fileInfo);
        private static extern int AdvOpenFile64(string fileName, [In, Out] ref AdvFileInfo fileInfo);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvCloseFile")]
        //unsigned int AdvCloseFile();
        private static extern uint AdvCloseFile64();

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvGetFileVersion")]
        //unsigned int AdvGetFileVersion(const char* fileName);
        private static extern uint AdvGetFileVersion64(string fileName);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_NewFile")]
        //void AdvNewFile(const char* fileName);
        private static extern void AdvVer1_NewFile64(string fileName);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_EndFile")]
        //void AdvEndFile();
        private static extern void AdvVer1_EndFile64();

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvGetCurrentFilePath")]
        //char* AdvGetCurrentFilePath(void);
        private static extern string AdvGetCurrentFilePath64();

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_DefineImageSection")]
        //void AdvDefineImageSection(unsigned short width, unsigned short height, unsigned char dataBpp);
        private static extern void AdvVer1_DefineImageSection64(ushort width, ushort height, byte dataBpp);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "AdvVer1_DefineImageLayout")]
        //void AdvDefineImageLayout(unsigned char layoutId, const char* layoutType, const char* compression, unsigned char layoutBpp, int keyFrame, const char* diffCorrFromBaseFrame);
        private static extern void AdvVer1_DefineImageLayout64(byte layoutId, [MarshalAs(UnmanagedType.LPStr)]string layoutType, [MarshalAs(UnmanagedType.LPStr)]string compression, byte layoutBpp, int keyFrame, [MarshalAs(UnmanagedType.LPStr)]string diffCorrFromBaseFrame);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "AdvVer1_DefineStatusSectionTag")]
        //unsigned int AdvDefineStatusSectionTag(const char* tagName, int tagType);
        private static extern uint AdvVer1_DefineStatusSectionTag64([MarshalAs(UnmanagedType.LPStr)]string tagName, AdvTagType tagType);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "AdvVer1_AddFileTag")]
        //unsigned int AdvAddFileTag(const char* tagName, const char* tagValue);
        private static extern uint AdvVer1_AddFileTag64([MarshalAs(UnmanagedType.LPStr)]string tagName, [MarshalAs(UnmanagedType.LPStr)]string tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "AdvVer1_AddOrUpdateImageSectionTag")]
        //void AdvAddOrUpdateImageSectionTag(const char* tagName, const char* tagValue);
        private static extern uint AdvVer1_AddOrUpdateImageSectionTag64([MarshalAs(UnmanagedType.LPStr)]string tagName, [MarshalAs(UnmanagedType.LPStr)]string tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_BeginFrame")]
        //bool AdvBeginFrame(long long timeStamp, unsigned int elapsedTime, unsigned int exposure);
        private static extern bool AdvVer1_BeginFrame64(long timeStamp, uint elapsedTime, uint exposure);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_FrameAddImage")]
        //void AdvFrameAddImage(unsigned char layoutId, unsigned short* pixels, unsigned char pixelsBpp);
        private static extern void AdvVer1_FrameAddImage64(byte layoutId, [In, MarshalAs(UnmanagedType.LPArray)] ushort[] pixels, byte pixelsBpp);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_FrameAddImageBytes")]
        //void AdvFrameAddImageBytes(unsigned char layoutId, unsigned char* pixels, unsigned char pixelsBpp);
        private static extern void AdvVer1_FrameAddImageBytes64(byte layoutId, [In, MarshalAs(UnmanagedType.LPArray)] byte[] pixels, byte pixelsBpp);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "AdvVer1_FrameAddStatusTag")]
        //void AdvFrameAddStatusTag(unsigned int tagIndex, const char* tagValue);
        private static extern void AdvVer1_FrameAddStatusTag64(uint tagIndex, [MarshalAs(UnmanagedType.LPStr)]string tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "AdvVer1_FrameAddStatusTagMessage")]
        //void AdvFrameAddStatusTagMessage(unsigned int tagIndex, const char* tagValue);
        private static extern void AdvVer1_FrameAddStatusTagMessage64(uint tagIndex, [MarshalAs(UnmanagedType.LPStr)]string tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_FrameAddStatusTagUInt8")]
        //void AdvFrameAddStatusTagUInt8(unsigned int tagIndex, unsigned char tagValue);
        private static extern void AdvVer1_FrameAddStatusTagUInt864(uint tagIndex, byte tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_FrameAddStatusTag16")]
        //void AdvFrameAddStatusTag16(unsigned int tagIndex, unsigned short tagValue);
        private static extern void AdvVer1_FrameAddStatusTag1664(uint tagIndex, ushort tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_FrameAddStatusTagReal")]
        //void AdvFrameAddStatusTagReal(unsigned int tagIndex, float tagValue);
        private static extern void AdvVer1_FrameAddStatusTagReal64(uint tagIndex, float tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_FrameAddStatusTag32")]
        //void AdvFrameAddStatusTag32(unsigned int tagIndex, unsigned long tagValue);
        private static extern void AdvVer1_FrameAddStatusTag3264(uint tagIndex, uint tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_FrameAddStatusTag64")]
        //void AdvFrameAddStatusTag64(unsigned int tagIndex, long long tagValue);
        private static extern void AdvVer1_FrameAddStatusTag6464(uint tagIndex, ulong tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_EndFrame")]
        //void AdvEndFrame();
        private static extern void AdvVer1_EndFrame64();

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_NewFile")]
        //DLL_PUBLIC void AdvVer2_NewFile(const char* fileName);
        private static extern void AdvVer2_NewFile64(string fileName);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_EndFile")]
        //DLL_PUBLIC ADVRESULT AdvVer2_EndFile();
        private static extern int AdvVer2_EndFile64();

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_SetTicksTimingPrecision")]
        //DLL_PUBLIC void AdvVer2_SetTicksTimingPrecision(int mainStreamAccuracy, int calibrationStreamAccuracy);
        private static extern void AdvVer2_SetTicksTimingPrecision64(int mainStreamAccuracy, int calibrationStreamAccuracy);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_DefineExternalClockForMainStream")]
        //DLL_PUBLIC void AdvVer2_DefineExternalClockForMainStream(__int64 clockFrequency, int ticksTimingAccuracy);
        private static extern void AdvVer2_DefineExternalClockForMainStream64(long clockFrequency, int ticksTimingAccuracy);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_DefineExternalClockForCalibrationStream")]
        //DLL_PUBLIC void AAdvVer2_DefineExternalClockForCalibrationStream(__int64 clockFrequency, int ticksTimingAccuracy);
        private static extern void AdvVer2_DefineExternalClockForCalibrationStream64(long clockFrequency, int ticksTimingAccuracy);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_AddMainStreamTag")]
        //ADVRESULT AdvVer2_AddMainStreamTag(const char* tagName, const char* tagValue);
        private static extern int AdvVer2_AddMainStreamTag64([MarshalAs(UnmanagedType.LPArray)]byte[] tagName, [MarshalAs(UnmanagedType.LPArray)]byte[] tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_AddCalibrationStreamTag")]
        //ADVRESULT AdvVer2_AddCalibrationStreamTag(const char* tagName, const char* tagValue);
        private static extern int AdvVer2_AddCalibrationStreamTag64([MarshalAs(UnmanagedType.LPArray)]byte[] tagName, [MarshalAs(UnmanagedType.LPArray)]byte[] tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_BeginFrameWithTicks")]
        //ADVRESULT AdvBeginFrame(unsigned char streamId, __int64 startFrameTicks, __int64 endFrameTicks, __int64 elapsedTicksSinceFirstFrame, __int64 utcStartTimeNanosecondsSinceAdvZeroEpoch, unsigned int utcExposureNanoseconds);
        private static extern int AdvVer2_BeginFrame64(uint streamId, long startTicks, long endTicks, long elapsedTicksFromStart, ulong utcStartTimeNanosecondsSinceAdvZeroEpoch, uint utcExposureNanoseconds);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_BeginFrame")]
        //ADVRESULT AdvBeginFrame(unsigned char streamId, __int64 utcStartTimeNanosecondsSinceAdvZeroEpoch, unsigned int utcExposureNanoseconds));
        private static extern int AdvVer2_BeginFrame64(uint streamId, ulong utcStartTimeNanosecondsSinceAdvZeroEpoch, uint utcExposureNanoseconds);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_DefineImageSection")]
        //ADVRESULT AdvVer2_DefineImageSection(unsigned short width, unsigned short height, unsigned char dataBpp);
        private static extern int AdvVer2_DefineImageSection64(ushort width, ushort height, byte dataBpp);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_DefineStatusSection")]
        //ADVRESULT AdvVer2_DefineStatusSection(__int64 utcTimestampAccuracyInNanoseconds);
        private static extern int AdvVer2_DefineStatusSection64(long utcTimestampAccuracyInNanoseconds);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_DefineImageLayout")]
        //ADVRESULT AdvVer2_DefineImageLayout(unsigned char layoutId, const char* layoutType, const char* compression, unsigned char layoutBpp);
        private static extern int AdvVer2_DefineImageLayout64(byte layoutId, [MarshalAs(UnmanagedType.LPStr)]string layoutType, [MarshalAs(UnmanagedType.LPStr)]string compression, byte layoutBpp);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_DefineStatusSectionTag")]
        //ADVRESULT AdvVer2_DefineStatusSectionTag(const char* tagName, int tagType, unsigned int* addedTagId);
        private static extern int AdvVer2_DefineStatusSectionTag64([MarshalAs(UnmanagedType.LPArray)]byte[] tagName, Adv2TagType tagType, ref uint addedTagId);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_AddFileTag")]
        //ADVRESULT AdvVer2_AddFileTag(const char* tagName, const char* tagValue);
        private static extern int AdvVer2_AddFileTag64([MarshalAs(UnmanagedType.LPArray)]byte[] tagName, [MarshalAs(UnmanagedType.LPArray)]byte[] tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_AddUserTag")]
        //ADVRESULT AdvVer2_AddUserTag(const char* tagName, const char* tagValue);
        private static extern int AdvVer2_AddUserTag64([MarshalAs(UnmanagedType.LPArray)]byte[] tagName, [MarshalAs(UnmanagedType.LPArray)]byte[] tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_AddOrUpdateImageSectionTag")]
        //ADVRESULT AdvVer2_AddOrUpdateImageSectionTag(const char* tagName, const char* tagValue);
        private static extern int AdvVer2_AddOrUpdateImageSectionTag64([MarshalAs(UnmanagedType.LPArray)]byte[] tagName, [MarshalAs(UnmanagedType.LPArray)]byte[] tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTagUTF8String")]
        //ADVRESULT AdvVer2_FrameAddStatusTagUTF8String(unsigned int tagIndex, const char* tagValue);
        private static extern int AdvVer2_FrameAddStatusTagUTF8String64(uint tagIndex, [MarshalAs(UnmanagedType.LPArray)]byte[] tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTagUInt8")]
        //ADVRESULT AdvVer2_FrameAddStatusTagUInt8(unsigned int tagIndex, unsigned char tagValue);
        private static extern int AdvVer2_FrameAddStatusTagUInt8_64(uint tagIndex, byte tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTag16")]
        //ADVRESULT AdvVer2_FrameAddStatusTag16(unsigned int tagIndex, unsigned short tagValue);
        private static extern int AdvVer2_FrameAddStatusTag16_64(uint tagIndex, short tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTagReal")]
        //ADVRESULT AdvVer2_FrameAddStatusTagReal(unsigned int tagIndex, float tagValue);
        private static extern int AdvVer2_FrameAddStatusTagReal_64(uint tagIndex, float tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTag32")]
        //ADVRESULT AdvVer2_FrameAddStatusTag32(unsigned int tagIndex, unsigned long tagValue);
        private static extern int AdvVer2_FrameAddStatusTag32_64(uint tagIndex, int tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTag64")]
        //ADVRESULT AdvVer2_FrameAddStatusTag64(unsigned int tagIndex, long long tagValue);
        private static extern int AdvVer2_FrameAddStatusTag64_64(uint tagIndex, long tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddImage")]
        //ADVRESULT AdvVer2_FrameAddImage(unsigned char layoutId, unsigned short* pixels, unsigned char pixelsBpp);
        private static extern int AdvVer2_FrameAddImage_64(byte layoutId, [In, MarshalAs(UnmanagedType.LPArray)] ushort[] pixels, byte pixelsBpp);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddImageBytes")]
        //ADVRESULT AdvVer2_FrameAddImageBytes(unsigned char layoutId, unsigned char* pixels, unsigned char pixelsBpp);
        private static extern int AdvVer2_FrameAddImageBytes_64(byte layoutId, [In, MarshalAs(UnmanagedType.LPArray)] byte[] pixels, byte pixelsBpp);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_EndFrame")]
        //ADVRESULT AdvVer2_EndFrame();
        private static extern int AdvVer2_EndFrame64();

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetFramePixels")]
        //ADVRESULT AdvVer2_GetFramePixels(int streamId, int frameNo, unsigned int* pixels, AdvLib2::AdvFrameInfo* frameInfo, int* systemErrorLen);
        private static extern int AdvVer2_GetFramePixels64(int streamId, int frameNo, [In, Out] uint[] pixels, [In, Out] AdvFrameInfoNative frameInfo, [In, Out] int systemErrorLen);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetTagPairValues")]
        //ADVRESULT AdvVer2_GetTagPairValues(TagPairType tagPairType, int tagId, char* tagName, char* tagValue)
        private static extern int AdvVer2_GetTagPairValues64(TagPairType tagPairType, int tagId, [In, Out] byte[] tagName, [In, Out] byte[] tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetTagPairSizes")]
        //ADVRESULT AdvVer2_GetTagPairSizes(TagPairType tagPairType, int tagId, int* tagNameSize, int* tagValueSize);
        private static extern int AdvVer2_GetTagPairSizes64(TagPairType tagPairType, int tagId, ref int tagNameSize, ref int tagValueSize);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetStatusTagSizeUTF8String")]
        //ADVRESULT AdvVer2_GetStatusTagSizeUTF8String(unsigned int tagIndex, int* tagValueSize);
        private static extern int AdvVer2_GetStatusTagSizeUTF8String64(uint tagId, ref int tagValueSize);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetStatusTagUTF8String")]
        //ADVRESULT AdvVer2_GetStatusTagUTF8String(unsigned int tagIndex, char* tagValue);
        private static extern int AdvVer2_GetStatusTagUTF8String64(uint tagId, [In, Out] byte[] tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetStatusTagUInt8")]
        //ADVRESULT AdvVer2_GetStatusTagUInt8(unsigned int tagIndex, unsigned char* tagValue);
        private static extern int AdvVer2_GetStatusTagUInt8_64(uint tagId, ref byte tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetStatusTag16")]
        //ADVRESULT AdvVer2_GetStatusTag16(unsigned int tagIndex, unsigned short* tagValue);
        private static extern int AdvVer2_GetStatusTag16_64(uint tagId, ref short tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetStatusTagReal")]
        //ADVRESULT AdvVer2_GetStatusTagReal(unsigned int tagIndex, float* tagValue);
        private static extern int AdvVer2_GetStatusTagReal_64(uint tagId, ref float tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetStatusTag32")]
        //ADVRESULT AdvVer2_GetStatusTag32(unsigned int tagIndex, unsigned int* tagValue);
        private static extern int AdvVer2_GetStatusTag32_64(uint tagId, ref int tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetStatusTag64")]
        //ADVRESULT AdvVer2_GetStatusTag64(unsigned int tagIndex, __int64* tagValue);
        private static extern int AdvVer2_GetStatusTag64_64(uint tagId, ref long tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetStatusTagNameSize")]
        //DLL_PUBLIC ADVRESULT AdvVer2_GetStatusTagNameSize(unsigned int tagId, int* tagNameSize);
        private static extern int AdvVer2_GetStatusTagNameSize64(uint tagId, ref int tagNameSize);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetStatusTagInfo")]
        //DLL_PUBLIC ADVRESULT AdvVer2_GetStatusTagInfo(unsigned int tagId, int* tagNameSize);
        private static extern int AdvVer2_GetStatusTagInfo64(uint tagId, [In, Out] byte[] tagName, ref Adv2TagType tagType);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetImageLayoutInfo")]
        //DLL_PUBLIC ADVRESULT AdvVer2_GetImageLayoutInfo(int layoutIndex, AdvLib2::AdvImageLayoutInfo* imageLayoutInfo);
        private static extern int AdvVer2_GetImageLayoutInfo64(int layoutIndex, ref AdvImageLayoutInfo imageLayoutInfo);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetLastSystemSpecificFileError")]
        //DLL_PUBLIC int AdvVer2_GetLastSystemSpecificFileError();
        private static extern int AdvVer2_GetLastSystemSpecificFileError64();

        #endregion

        #region UNIX externals
        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLibraryVersion")]
        private static extern void GetLibraryVersionUnix([MarshalAs(UnmanagedType.LPArray)]byte[] version);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLibraryPlatformId")]
        private static extern void GetLibraryPlatformIdUnix([MarshalAs(UnmanagedType.LPArray)]byte[] platform);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLibraryBitness")]
        private static extern int GetLibraryBitnessUnix();

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvOpenFile")]
        //unsigned int AdvOpenFile(const char* fileName, AdvLib2::AdvFileInfo* fileInfo);
        private static extern int AdvOpenFileUnix(string fileName, [In, Out] ref AdvFileInfo fileInfo);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvCloseFile")]
        //unsigned int AdvCloseFile();
        private static extern uint AdvCloseFileUnix();

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvGetFileVersion")]
        //unsigned int AdvGetFileVersion(const char* fileName);
        private static extern uint AdvGetFileVersionUnix(string fileName);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_NewFile")]
        //void AdvNewFile(const char* fileName);
        private static extern void AdvVer1_NewFileUnix(string fileName);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_EndFile")]
        //void AdvEndFile();
        private static extern void AdvVer1_EndFileUnix();

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvGetCurrentFilePath")]
        //char* AdvGetCurrentFilePath(void);
        private static extern string AdvGetCurrentFilePathUnix();

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_DefineImageSection")]
        //void AdvDefineImageSection(unsigned short width, unsigned short height, unsigned char dataBpp);
        private static extern void AdvVer1_DefineImageSectionUnix(ushort width, ushort height, byte dataBpp);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "AdvVer1_DefineImageLayout")]
        //void AdvDefineImageLayout(unsigned char layoutId, const char* layoutType, const char* compression, unsigned char layoutBpp, int keyFrame, const char* diffCorrFromBaseFrame);
        private static extern void AdvVer1_DefineImageLayoutUnix(byte layoutId, [MarshalAs(UnmanagedType.LPStr)]string layoutType, [MarshalAs(UnmanagedType.LPStr)]string compression, byte layoutBpp, int keyFrame, [MarshalAs(UnmanagedType.LPStr)]string diffCorrFromBaseFrame);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "AdvVer1_DefineStatusSectionTag")]
        //unsigned int AdvDefineStatusSectionTag(const char* tagName, int tagType);
        private static extern uint AdvVer1_DefineStatusSectionTagUnix([MarshalAs(UnmanagedType.LPStr)]string tagName, AdvTagType tagType);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "AdvVer1_AddFileTag")]
        //unsigned int AdvAddFileTag(const char* tagName, const char* tagValue);
        private static extern uint AdvVer1_AddFileTagUnix([MarshalAs(UnmanagedType.LPStr)]string tagName, [MarshalAs(UnmanagedType.LPStr)]string tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "AdvVer1_AddOrUpdateImageSectionTag")]
        //void AdvAddOrUpdateImageSectionTag(const char* tagName, const char* tagValue);
        private static extern uint AdvVer1_AddOrUpdateImageSectionTagUnix([MarshalAs(UnmanagedType.LPStr)]string tagName, [MarshalAs(UnmanagedType.LPStr)]string tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_BeginFrame")]
        //bool AdvBeginFrame(long long timeStamp, unsigned int elapsedTime, unsigned int exposure);
        private static extern bool AdvVer1_BeginFrameUnix(long timeStamp, uint elapsedTime, uint exposure);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_FrameAddImage")]
        //void AdvFrameAddImage(unsigned char layoutId, unsigned short* pixels, unsigned char pixelsBpp);
        private static extern void AdvVer1_FrameAddImageUnix(byte layoutId, [In, MarshalAs(UnmanagedType.LPArray)] ushort[] pixels, byte pixelsBpp);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_FrameAddImageBytes")]
        //void AdvFrameAddImageBytes(unsigned char layoutId, unsigned char* pixels, unsigned char pixelsBpp);
        private static extern void AdvVer1_FrameAddImageBytesUnix(byte layoutId, [In, MarshalAs(UnmanagedType.LPArray)] byte[] pixels, byte pixelsBpp);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "AdvVer1_FrameAddStatusTag")]
        //void AdvFrameAddStatusTag(unsigned int tagIndex, const char* tagValue);
        private static extern void AdvVer1_FrameAddStatusTagUnix(uint tagIndex, [MarshalAs(UnmanagedType.LPStr)]string tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "AdvVer1_FrameAddStatusTagMessage")]
        //void AdvFrameAddStatusTagMessage(unsigned int tagIndex, const char* tagValue);
        private static extern void AdvVer1_FrameAddStatusTagMessageUnix(uint tagIndex, [MarshalAs(UnmanagedType.LPStr)]string tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_FrameAddStatusTagUInt8")]
        //void AdvFrameAddStatusTagUInt8(unsigned int tagIndex, unsigned char tagValue);
        private static extern void AdvVer1_FrameAddStatusTagUInt8Unix(uint tagIndex, byte tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_FrameAddStatusTag16")]
        //void AdvFrameAddStatusTag16(unsigned int tagIndex, unsigned short tagValue);
        private static extern void AdvVer1_FrameAddStatusTag16Unix(uint tagIndex, ushort tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_FrameAddStatusTagReal")]
        //void AdvFrameAddStatusTagReal(unsigned int tagIndex, float tagValue);
        private static extern void AdvVer1_FrameAddStatusTagRealUnix(uint tagIndex, float tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_FrameAddStatusTag32")]
        //void AdvFrameAddStatusTag32(unsigned int tagIndex, unsigned long tagValue);
        private static extern void AdvVer1_FrameAddStatusTag32Unix(uint tagIndex, uint tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_FrameAddStatusTag64")]
        //void AdvFrameAddStatusTagUnix(unsigned int tagIndex, long long tagValue);
        private static extern void AdvVer1_FrameAddStatusTag64Unix(uint tagIndex, ulong tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer1_EndFrame")]
        //ADVRESULT AdvEndFrame();
        private static extern int AdvVer1_EndFrameUnix();

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_NewFile")]
        //DLL_PUBLIC void AdvVer2_NewFile(const char* fileName);
        private static extern void AdvVer2_NewFileUnix(string fileName);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_EndFile")]
        //DLL_PUBLIC ADVRESULT AdvVer2_EndFile();
        private static extern int AdvVer2_EndFileUnix();

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_SetTicksTimingPrecision")]
        //DLL_PUBLIC void AdvVer2_SetTicksTimingPrecision(int mainStreamAccuracy, int calibrationStreamAccuracy);
        private static extern void AdvVer2_SetTicksTimingPrecisionUnix(int mainStreamAccuracy, int calibrationStreamAccuracy);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_DefineExternalClockForMainStream")]
        //DLL_PUBLIC void AdvVer2_DefineExternalClockForMainStream(__int64 clockFrequency, int ticksTimingAccuracy);
        private static extern void AdvVer2_DefineExternalClockForMainStreamUnix(long clockFrequency, int ticksTimingAccuracy);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_DefineExternalClockForCalibrationStream")]
        //DLL_PUBLIC void AAdvVer2_DefineExternalClockForCalibrationStream(__int64 clockFrequency, int ticksTimingAccuracy);
        private static extern void AdvVer2_DefineExternalClockForCalibrationStreamUnix(long clockFrequency, int ticksTimingAccuracy);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_AddMainStreamTag")]
        //ADVRESULT AdvVer2_AddMainStreamTag(const char* tagName, const char* tagValue);
        private static extern int AdvVer2_AddMainStreamTagUnix([MarshalAs(UnmanagedType.LPArray)]byte[] tagName, [MarshalAs(UnmanagedType.LPArray)]byte[] tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_AddCalibrationStreamTag")]
        //ADVRESULT AdvVer2_AddCalibrationStreamTag(const char* tagName, const char* tagValue);
        private static extern int AdvVer2_AddCalibrationStreamTagUnix([MarshalAs(UnmanagedType.LPArray)]byte[] tagName, [MarshalAs(UnmanagedType.LPArray)]byte[] tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_BeginFrameWithTicks")]
        //ADVRESULT AdvBeginFrame(unsigned char streamId, __int64 startFrameTicks, __int64 endFrameTicks, __int64 elapsedTicksSinceFirstFrame, __int64 utcStartTimeNanosecondsSinceAdvZeroEpoch, unsigned int utcExposureNanoseconds);
        private static extern int AdvVer2_BeginFrameUnix(uint streamId, long startTicks, long endTicks, long elapsedTicksFromStart, ulong utcStartTimeNanosecondsSinceAdvZeroEpoch, uint utcExposureNanoseconds);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_BeginFrame")]
        //ADVRESULT AdvBeginFrame(unsigned char streamId, __int64 utcStartTimeNanosecondsSinceAdvZeroEpoch, unsigned int utcExposureNanoseconds));
        private static extern int AdvVer2_BeginFrameUnix(uint streamId, ulong utcStartTimeNanosecondsSinceAdvZeroEpoch, uint utcExposureNanoseconds);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_DefineImageSection")]
        //ADVRESULT AdvVer2_DefineImageSection(unsigned short width, unsigned short height, unsigned char dataBpp);
        private static extern int AdvVer2_DefineImageSectionUnix(ushort width, ushort height, byte dataBpp);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_DefineStatusSection")]
        //ADVRESULT AdvVer2_DefineStatusSection(__int64 utcTimestampAccuracyInNanoseconds);
        private static extern int AdvVer2_DefineStatusSectionUnix(long utcTimestampAccuracyInNanoseconds);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_DefineImageLayout")]
        //ADVRESULT AdvVer2_DefineImageLayout(unsigned char layoutId, const char* layoutType, const char* compression, unsigned char layoutBpp);
        private static extern int AdvVer2_DefineImageLayoutUnix(byte layoutId, [MarshalAs(UnmanagedType.LPStr)]string layoutType, [MarshalAs(UnmanagedType.LPStr)]string compression, byte layoutBpp);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_DefineStatusSectionTag")]
        //ADVRESULT AdvVer2_DefineStatusSectionTag(const char* tagName, int tagType, unsigned int* addedTagId);
        private static extern int AdvVer2_DefineStatusSectionTagUnix([MarshalAs(UnmanagedType.LPArray)]byte[] tagName, Adv2TagType tagType, ref uint addedTagId);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_AddFileTag")]
        //ADVRESULT AdvVer2_AddFileTag(const char* tagName, const char* tagValue);
        private static extern int AdvVer2_AddFileTagUnix([MarshalAs(UnmanagedType.LPArray)]byte[] tagName, [MarshalAs(UnmanagedType.LPArray)]byte[] tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_AddUserTag")]
        //ADVRESULT AdvVer2_AddUserTag(const char* tagName, const char* tagValue);
        private static extern int AdvVer2_AddUserTagUnix([MarshalAs(UnmanagedType.LPArray)]byte[] tagName, [MarshalAs(UnmanagedType.LPArray)]byte[] tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_AddOrUpdateImageSectionTag")]
        //ADVRESULT AdvVer2_AddOrUpdateImageSectionTag(const char* tagName, const char* tagValue);
        private static extern int AdvVer2_AddOrUpdateImageSectionTagUnix([MarshalAs(UnmanagedType.LPArray)]byte[] tagName, [MarshalAs(UnmanagedType.LPArray)]byte[] tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTagUTF8String")]
        //ADVRESULT AdvVer2_FrameAddStatusTagUTF8String(unsigned int tagIndex, const char* tagValue);
        private static extern int AdvVer2_FrameAddStatusTagUTF8StringUnix(uint tagIndex, [MarshalAs(UnmanagedType.LPArray)]byte[] tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTagUInt8")]
        //ADVRESULT AdvVer2_FrameAddStatusTagUInt8(unsigned int tagIndex, unsigned char tagValue);
        private static extern int AdvVer2_FrameAddStatusTagUInt8_Unix(uint tagIndex, byte tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTag16")]
        //ADVRESULT AdvVer2_FrameAddStatusTag16(unsigned int tagIndex, unsigned short tagValue);
        private static extern int AdvVer2_FrameAddStatusTag16_Unix(uint tagIndex, short tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTagReal")]
        //ADVRESULT AdvVer2_FrameAddStatusTagReal(unsigned int tagIndex, float tagValue);
        private static extern int AdvVer2_FrameAddStatusTagReal_Unix(uint tagIndex, float tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTag32")]
        //ADVRESULT AdvVer2_FrameAddStatusTag32(unsigned int tagIndex, unsigned long tagValue);
        private static extern int AdvVer2_FrameAddStatusTag32_Unix(uint tagIndex, int tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTag64")]
        //ADVRESULT AdvVer2_FrameAddStatusTagUnix(unsigned int tagIndex, long long tagValue);
        private static extern int AdvVer2_FrameAddStatusTag64_Unix(uint tagIndex, long tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddImage")]
        //ADVRESULT AdvVer2_FrameAddImage(unsigned char layoutId, unsigned short* pixels, unsigned char pixelsBpp);
        private static extern int AdvVer2_FrameAddImage_Unix(byte layoutId, [In, MarshalAs(UnmanagedType.LPArray)] ushort[] pixels, byte pixelsBpp);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddImageBytes")]
        //ADVRESULT AdvVer2_FrameAddImageBytes(unsigned char layoutId, unsigned char* pixels, unsigned char pixelsBpp);
        private static extern int AdvVer2_FrameAddImageBytes_Unix(byte layoutId, [In, MarshalAs(UnmanagedType.LPArray)] byte[] pixels, byte pixelsBpp);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_EndFrame")]
        //ADVRESULT AdvVer2_EndFrame();
        private static extern int AdvVer2_EndFrameUnix();

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetFramePixels")]
        //ADVRESULT AdvVer2_GetFramePixels(int streamId, int frameNo, unsigned int* pixels, AdvLib2::AdvFrameInfo* frameInfo, int* systemErrorLen);
        private static extern int AdvVer2_GetFramePixelsUnix(int streamId, int frameNo, [In, Out] uint[] pixels, [In, Out] AdvFrameInfoNative frameInfo, [In, Out] int systemErrorLen);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetTagPairValues")]
        //ADVRESULT AdvVer2_GetTagPairValues(TagPairType tagPairType, int tagId, char* tagName, char* tagValue)
        private static extern int AdvVer2_GetTagPairValuesUnix(TagPairType tagPairType, int tagId, [In, Out] byte[] tagName, [In, Out] byte[] tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetTagPairSizes")]
        //ADVRESULT AdvVer2_GetTagPairSizes(TagPairType tagPairType, int tagId, int* tagNameSize, int* tagValueSize);
        private static extern int AdvVer2_GetTagPairSizesUnix(TagPairType tagPairType, int tagId, ref int tagNameSize, ref int tagValueSize);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetStatusTagSizeUTF8String")]
        //ADVRESULT AdvVer2_GetStatusTagSizeUTF8String(unsigned int tagIndex, int* tagValueSize);
        private static extern int AdvVer2_GetStatusTagSizeUTF8StringUnix(uint tagId, ref int tagValueSize);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetStatusTagUTF8String")]
        //ADVRESULT AdvVer2_GetStatusTagUTF8String(unsigned int tagIndex, char* tagValue);
        private static extern int AdvVer2_GetStatusTagUTF8StringUnix(uint tagId, [In, Out] byte[] tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetStatusTagUInt8")]
        //ADVRESULT AdvVer2_GetStatusTagUInt8(unsigned int tagIndex, unsigned char* tagValue);
        private static extern int AdvVer2_GetStatusTagUInt8_Unix(uint tagId, ref byte tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetStatusTag16")]
        //ADVRESULT AdvVer2_GetStatusTag16(unsigned int tagIndex, unsigned short* tagValue);
        private static extern int AdvVer2_GetStatusTag16_Unix(uint tagId, ref short tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetStatusTagReal")]
        //ADVRESULT AdvVer2_GetStatusTagReal(unsigned int tagIndex, float* tagValue);
        private static extern int AdvVer2_GetStatusTagReal_Unix(uint tagId, ref float tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetStatusTag32")]
        //ADVRESULT AdvVer2_GetStatusTag32(unsigned int tagIndex, unsigned int* tagValue);
        private static extern int AdvVer2_GetStatusTag32_Unix(uint tagId, ref int tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetStatusTag64")]
        //ADVRESULT AdvVer2_GetStatusTag64(unsigned int tagIndex, __int64* tagValue);
        private static extern int AdvVer2_GetStatusTag64_Unix(uint tagId, ref long tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetStatusTagNameSize")]
        //DLL_PUBLIC ADVRESULT AdvVer2_GetStatusTagNameSize(unsigned int tagId, int* tagNameSize);
        private static extern int AdvVer2_GetStatusTagNameSizeUnix(uint tagId, ref int tagNameSize);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetStatusTagInfo")]
        //DLL_PUBLIC ADVRESULT AdvVer2_GetStatusTagInfo(unsigned int tagId, int* tagNameSize);
        private static extern int AdvVer2_GetStatusTagInfoUnix(uint tagId, [In, Out] byte[] tagName, ref Adv2TagType tagType);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetImageLayoutInfo")]
        //DLL_PUBLIC ADVRESULT AdvVer2_GetImageLayoutInfo(int layoutIndex, AdvLib2::AdvImageLayoutInfo* imageLayoutInfo);
        private static extern int AdvVer2_GetImageLayoutInfoUnix(int layoutIndex, ref AdvImageLayoutInfo imageLayoutInfo);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetLastSystemSpecificFileError")]
        //DLL_PUBLIC int AdvVer2_GetLastSystemSpecificFileError();
        private static extern int AdvVer2_GetLastSystemSpecificFileErrorUnix();

        #endregion

        public static string AdvGetCurrentFilePath()
        {
            if (!CrossPlatform.IsWindows)
                return AdvGetCurrentFilePathUnix();
            else if (Is64Bit())
                return AdvGetCurrentFilePath64();
            else
                return AdvGetCurrentFilePath32();
        }

        public static class Obsolete
        {
            public static class AdvVer1
            {
                public static void NewFile(string fileName)
                {
                    if (!CrossPlatform.IsWindows)
                        AdvVer1_NewFileUnix(fileName);
                    else if (Is64Bit())
                        AdvVer1_NewFile64(fileName);
                    else
                        AdvVer1_NewFile32(fileName);
                }

                public static void EndFile()
                {
                    if (!CrossPlatform.IsWindows)
                        AdvVer1_EndFileUnix();
                    else if (Is64Bit())
                        AdvVer1_EndFile64();
                    else
                        AdvVer1_EndFile32();
                }

                public static void DefineImageSection(ushort width, ushort height, byte dataBpp)
                {
                    if (!CrossPlatform.IsWindows)
                        AdvVer1_DefineImageSectionUnix(width, height, dataBpp);
                    else if (Is64Bit())
                        AdvVer1_DefineImageSection64(width, height, dataBpp);
                    else
                        AdvVer1_DefineImageSection32(width, height, dataBpp);
                }

                public static void DefineImageLayout(byte layoutId, string layoutType, string compression, byte layoutBpp, int keyFrame, string diffCorrFromBaseFrame)
                {
                    if (!CrossPlatform.IsWindows)
                        AdvVer1_DefineImageLayoutUnix(layoutId, layoutType, compression, layoutBpp, keyFrame, diffCorrFromBaseFrame);
                    else if (Is64Bit())
                        AdvVer1_DefineImageLayout64(layoutId, layoutType, compression, layoutBpp, keyFrame, diffCorrFromBaseFrame);
                    else
                        AdvVer1_DefineImageLayout32(layoutId, layoutType, compression, layoutBpp, keyFrame, diffCorrFromBaseFrame);
                }

                public static uint DefineStatusSectionTag(string tagName, AdvTagType tagType)
                {
                    if (!CrossPlatform.IsWindows)
                        return AdvVer1_DefineStatusSectionTagUnix(tagName, tagType);
                    else if (Is64Bit())
                        return AdvVer1_DefineStatusSectionTag64(tagName, tagType);
                    else
                        return AdvVer1_DefineStatusSectionTag32(tagName, tagType);
                }

                public static uint AddFileTag(string tagName, string tagValue)
                {
                    if (!CrossPlatform.IsWindows)
                        return AdvVer1_AddFileTagUnix(tagName, tagValue);
                    else if (Is64Bit())
                        return AdvVer1_AddFileTag64(tagName, tagValue);
                    else
                        return AdvVer1_AddFileTag32(tagName, tagValue);
                }

                public static uint AddOrUpdateImageSectionTag(string tagName, string tagValue)
                {
                    if (!CrossPlatform.IsWindows)
                        return AdvVer1_AddOrUpdateImageSectionTagUnix(tagName, tagValue);
                    else if (Is64Bit())
                        return AdvVer1_AddOrUpdateImageSectionTag64(tagName, tagValue);
                    else
                        return AdvVer1_AddOrUpdateImageSectionTag32(tagName, tagValue);
                }

                public static bool BeginFrame(long timeStamp, uint elapsedTime, uint exposure)
                {
                    if (!CrossPlatform.IsWindows)
                        return AdvVer1_BeginFrameUnix(timeStamp, elapsedTime, exposure);
                    else if (Is64Bit())
                        return AdvVer1_BeginFrame64(timeStamp, elapsedTime, exposure);
                    else
                        return AdvVer1_BeginFrame32(timeStamp, elapsedTime, exposure);
                }

                public static void FrameAddImage(byte layoutId, ushort[] pixels, byte pixelsBpp)
                {
                    if (!CrossPlatform.IsWindows)
                        AdvVer1_FrameAddImageUnix(layoutId, pixels, pixelsBpp);
                    else if (Is64Bit())
                        AdvVer1_FrameAddImage64(layoutId, pixels, pixelsBpp);
                    else
                        AdvVer1_FrameAddImage32(layoutId, pixels, pixelsBpp);
                }

                public static void FrameAddImageBytes(byte layoutId, byte[] pixels, byte pixelsBpp)
                {
                    if (!CrossPlatform.IsWindows)
                        AdvVer1_FrameAddImageBytesUnix(layoutId, pixels, pixelsBpp);
                    else if (Is64Bit())
                        AdvVer1_FrameAddImageBytes64(layoutId, pixels, pixelsBpp);
                    else
                        AdvVer1_FrameAddImageBytes32(layoutId, pixels, pixelsBpp);
                }

                public static void FrameAddStatusTag(uint tagIndex, string tagValue)
                {
                    if (!CrossPlatform.IsWindows)
                        AdvVer1_FrameAddStatusTagUnix(tagIndex, tagValue);
                    else if (Is64Bit())
                        AdvVer1_FrameAddStatusTag64(tagIndex, tagValue);
                    else
                        AdvVer1_FrameAddStatusTag32(tagIndex, tagValue);
                }

                public static void FrameAddStatusTagMessage(uint tagIndex, string tagValue)
                {
                    if (!CrossPlatform.IsWindows)
                        AdvVer1_FrameAddStatusTagMessageUnix(tagIndex, tagValue);
                    else if (Is64Bit())
                        AdvVer1_FrameAddStatusTagMessage64(tagIndex, tagValue);
                    else
                        AdvVer1_FrameAddStatusTagMessage32(tagIndex, tagValue);
                }

                public static void FrameAddStatusTagUInt8(uint tagIndex, byte tagValue)
                {
                    if (!CrossPlatform.IsWindows)
                        AdvVer1_FrameAddStatusTagUInt8Unix(tagIndex, tagValue);
                    else if (Is64Bit())
                        AdvVer1_FrameAddStatusTagUInt864(tagIndex, tagValue);
                    else
                        AdvVer1_FrameAddStatusTagUInt832(tagIndex, tagValue);
                }

                public static void FrameAddStatusTag16(uint tagIndex, ushort tagValue)
                {
                    if (!CrossPlatform.IsWindows)
                        AdvVer1_FrameAddStatusTag16Unix(tagIndex, tagValue);
                    else if (Is64Bit())
                        AdvVer1_FrameAddStatusTag1664(tagIndex, tagValue);
                    else
                        AdvVer1_FrameAddStatusTag1632(tagIndex, tagValue);
                }

                public static void FrameAddStatusTagReal(uint tagIndex, float tagValue)
                {
                    if (!CrossPlatform.IsWindows)
                        AdvVer1_FrameAddStatusTagRealUnix(tagIndex, tagValue);
                    else if (Is64Bit())
                        AdvVer1_FrameAddStatusTagReal64(tagIndex, tagValue);
                    else
                        AdvVer1_FrameAddStatusTagReal32(tagIndex, tagValue);
                }

                public static void FrameAddStatusTag32(uint tagIndex, uint tagValue)
                {
                    if (!CrossPlatform.IsWindows)
                        AdvVer1_FrameAddStatusTag32Unix(tagIndex, tagValue);
                    else if (Is64Bit())
                        AdvVer1_FrameAddStatusTag3264(tagIndex, tagValue);
                    else
                        AdvVer1_FrameAddStatusTag3232(tagIndex, tagValue);
                }

                public static void FrameAddStatusTag64(uint tagIndex, ulong tagValue)
                {
                    if (!CrossPlatform.IsWindows)
                        AdvVer1_FrameAddStatusTag64Unix(tagIndex, tagValue);
                    else if (Is64Bit())
                        AdvVer1_FrameAddStatusTag6464(tagIndex, tagValue);
                    else
                        AdvVer1_FrameAddStatusTag6432(tagIndex, tagValue);
                }

                public static void EndFrame()
                {
                    if (!CrossPlatform.IsWindows)
                        AdvVer1_EndFrameUnix();
                    else if (Is64Bit())
                        AdvVer1_EndFrame64();
                    else
                        AdvVer1_EndFrame32();
                }
            }
        }

        public static string GetLibraryVersion()
        {
            byte[] version = new byte[256];
            if (!CrossPlatform.IsWindows)
                GetLibraryVersionUnix(version);
            else if (Is64Bit())
                GetLibraryVersion64(version);
            else
                GetLibraryVersion32(version);

            return Encoding.ASCII.GetString(version).Trim('\0');
        }

        public static string GetLibraryPlatformId()
        {
            byte[] platform = new byte[256];
            if (!CrossPlatform.IsWindows)
                GetLibraryPlatformIdUnix(platform);
            else if (Is64Bit())
                GetLibraryPlatformId64(platform);
            else
                GetLibraryPlatformId32(platform);

            return Encoding.ASCII.GetString(platform).Trim('\0');
        }

        public static string GetLibraryBitness()
        {
            int bitness = 0;
            if (!CrossPlatform.IsWindows)
                bitness = GetLibraryBitnessUnix();
            else if (Is64Bit())
                bitness = GetLibraryBitness64();
            else
                bitness = GetLibraryBitness32();

            return bitness > 0 ? string.Format(" ({0} bit)", bitness) : string.Empty;
        }

        public static int AdvOpenFile(string fileName, out AdvFileInfo fileInfo)
        {
            fileInfo = new AdvFileInfo();

            if (!CrossPlatform.IsWindows)
                return AdvOpenFileUnix(fileName, ref fileInfo);
            else if (Is64Bit())
                return AdvOpenFile64(fileName, ref fileInfo);
            else
                return AdvOpenFile32(fileName, ref fileInfo);
        }

        public static uint AdvCloseFile()
        {
            if (!CrossPlatform.IsWindows)
                return AdvCloseFileUnix();
            else if (Is64Bit())
                return AdvCloseFile64();
            else
                return AdvCloseFile32();
        }

        public static uint AdvGetFileVersion(string fileName)
        {
            if (!CrossPlatform.IsWindows)
                return AdvGetFileVersionUnix(fileName);
            else if (Is64Bit())
                return AdvGetFileVersion64(fileName);
            else
                return AdvGetFileVersion32(fileName);
        }

        public static bool Is64Bit()
        {
            //Check whether we are running on a 32 or 64bit system.
            if (IntPtr.Size == 8)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public static void NewFile(string fileName)
        {
            if (!CrossPlatform.IsWindows)
                AdvVer2_NewFileUnix(fileName);
            else if (Is64Bit())
                AdvVer2_NewFile64(fileName);
            else
                AdvVer2_NewFile32(fileName);
        }

        public static int EndFile()
        {
            if (!CrossPlatform.IsWindows)
                return AdvVer2_EndFileUnix();
            else if (Is64Bit())
                return AdvVer2_EndFile64();
            else
                return AdvVer2_EndFile32();
        }

        private static byte[] StringToUTF8Bytes(string str)
        {
            if (String.IsNullOrEmpty(str))
                return new byte[0];
            else
                return Encoding.UTF8.GetBytes(str + "\0");
        }

        public static void SetTimingPrecision(int mainAccuracy, int calibrationAccuracy)
        {
            if (!CrossPlatform.IsWindows)
                AdvVer2_SetTicksTimingPrecisionUnix(mainAccuracy, calibrationAccuracy);
            else if (Is64Bit())
                AdvVer2_SetTicksTimingPrecision64(mainAccuracy, calibrationAccuracy);
            else
                AdvVer2_SetTicksTimingPrecision32(mainAccuracy, calibrationAccuracy);
        }

        public static int AddOrUpdateMainStreamTag(string tagName, string tagValue)
        {
            if (!CrossPlatform.IsWindows)
                return AdvVer2_AddMainStreamTagUnix(StringToUTF8Bytes(tagName), StringToUTF8Bytes(tagValue));
            else if (Is64Bit())
                return AdvVer2_AddMainStreamTag64(StringToUTF8Bytes(tagName), StringToUTF8Bytes(tagValue));
            else
                return AdvVer2_AddMainStreamTag32(StringToUTF8Bytes(tagName), StringToUTF8Bytes(tagValue));
        }

        public static int AddOrUpdateCalibrationStreamTag(string tagName, string tagValue)
        {
            if (!CrossPlatform.IsWindows)
                return AdvVer2_AddCalibrationStreamTagUnix(StringToUTF8Bytes(tagName), StringToUTF8Bytes(tagValue));
            else if (Is64Bit())
                return AdvVer2_AddCalibrationStreamTag64(StringToUTF8Bytes(tagName), StringToUTF8Bytes(tagValue));
            else
                return AdvVer2_AddCalibrationStreamTag32(StringToUTF8Bytes(tagName), StringToUTF8Bytes(tagValue));
        }

        public static int BeginFrame(byte streamId, long startTicks, long endTicks, long elapsedTicksFromStart, ulong utcStartTimeNanosecondsSinceAdvZeroEpoch, uint utcExposureNanoseconds)
        {
            try
            {
                if (!CrossPlatform.IsWindows)
                    return AdvVer2_BeginFrameUnix(streamId, startTicks, endTicks, elapsedTicksFromStart, utcStartTimeNanosecondsSinceAdvZeroEpoch, utcExposureNanoseconds);
                else if (Is64Bit())
                    return AdvVer2_BeginFrame64(streamId, startTicks, endTicks, elapsedTicksFromStart, utcStartTimeNanosecondsSinceAdvZeroEpoch, utcExposureNanoseconds);
                else
                    return AdvVer2_BeginFrame32(streamId, startTicks, endTicks, elapsedTicksFromStart, utcStartTimeNanosecondsSinceAdvZeroEpoch, utcExposureNanoseconds);
            }
            catch (Exception ex)
            {
                AdvError.HandleException(ex);

                return AdvError.E_FAIL;
            }
        }

        public static int BeginFrame(byte streamId, ulong utcStartTimeNanosecondsSinceAdvZeroEpoch, uint utcExposureNanoseconds)
        {
            try
            {
                if (!CrossPlatform.IsWindows)
                    return AdvVer2_BeginFrameUnix(streamId, utcStartTimeNanosecondsSinceAdvZeroEpoch, utcExposureNanoseconds);
                else if (Is64Bit())
                    return AdvVer2_BeginFrame64(streamId, utcStartTimeNanosecondsSinceAdvZeroEpoch, utcExposureNanoseconds);
                else
                    return AdvVer2_BeginFrame32(streamId, utcStartTimeNanosecondsSinceAdvZeroEpoch, utcExposureNanoseconds);
            }
            catch (Exception ex)
            {
                AdvError.HandleException(ex);

                return AdvError.E_FAIL;
            }
        }

        public static void DefineExternalClockForMainStream(long clockFrequency, int ticksTimingAccuracy)
        {
            if (!CrossPlatform.IsWindows)
                AdvVer2_DefineExternalClockForMainStreamUnix(clockFrequency, ticksTimingAccuracy);
            else if (Is64Bit())
                AdvVer2_DefineExternalClockForMainStream64(clockFrequency, ticksTimingAccuracy);
            else
                AdvVer2_DefineExternalClockForMainStream32(clockFrequency, ticksTimingAccuracy);
        }

        public static void DefineExternalClockForCalibrationStream(long clockFrequency, int ticksTimingAccuracy)
        {
            if (!CrossPlatform.IsWindows)
                AdvVer2_DefineExternalClockForCalibrationStreamUnix(clockFrequency, ticksTimingAccuracy);
            else if (Is64Bit())
                AdvVer2_DefineExternalClockForCalibrationStream64(clockFrequency, ticksTimingAccuracy);
            else
                AdvVer2_DefineExternalClockForCalibrationStream32(clockFrequency, ticksTimingAccuracy);
        }

        public static int DefineImageSection(ushort width, ushort height, byte dataBpp)
        {
            if (!CrossPlatform.IsWindows)
                return AdvVer2_DefineImageSectionUnix(width, height, dataBpp);
            else if (Is64Bit())
                return AdvVer2_DefineImageSection64(width, height, dataBpp);
            else
                return AdvVer2_DefineImageSection32(width, height, dataBpp);
        }

        public static int DefineStatusSection(long utcTimestampAccuracyInNanoseconds)
        {
            if (!CrossPlatform.IsWindows)
                return AdvVer2_DefineStatusSectionUnix(utcTimestampAccuracyInNanoseconds);
            else if (Is64Bit())
                return AdvVer2_DefineStatusSection64(utcTimestampAccuracyInNanoseconds);
            else
                return AdvVer2_DefineStatusSection32(utcTimestampAccuracyInNanoseconds);
        }

        public static int DefineImageLayout(byte layoutId, string layoutType, string compression, byte layoutBpp)
        {
            if (!CrossPlatform.IsWindows)
                return AdvVer2_DefineImageLayoutUnix(layoutId, layoutType, compression, layoutBpp);
            else if (Is64Bit())
                return AdvVer2_DefineImageLayout64(layoutId, layoutType, compression, layoutBpp);
            else
                return AdvVer2_DefineImageLayout32(layoutId, layoutType, compression, layoutBpp);
        }

        public static int DefineStatusSectionTag(string tagName, Adv2TagType tagType, out uint addedTagId)
        {
            addedTagId = uint.MaxValue;

            if (!CrossPlatform.IsWindows)
                return AdvVer2_DefineStatusSectionTagUnix(StringToUTF8Bytes(tagName), tagType, ref addedTagId);
            else if (Is64Bit())
                return AdvVer2_DefineStatusSectionTag64(StringToUTF8Bytes(tagName), tagType, ref addedTagId);
            else
                return AdvVer2_DefineStatusSectionTag32(StringToUTF8Bytes(tagName), tagType, ref addedTagId);
        }

        public static int AddOrUpdateFileTag(string tagName, string tagValue)
        {
            if (!CrossPlatform.IsWindows)
                return AdvVer2_AddFileTagUnix(StringToUTF8Bytes(tagName), StringToUTF8Bytes(tagValue));
            else if (Is64Bit())
                return AdvVer2_AddFileTag64(StringToUTF8Bytes(tagName), StringToUTF8Bytes(tagValue));
            else
                return AdvVer2_AddFileTag32(StringToUTF8Bytes(tagName), StringToUTF8Bytes(tagValue));
        }

        public static int AddOrUpdateUserTag(string tagName, string tagValue)
        {
            if (!CrossPlatform.IsWindows)
                return AdvVer2_AddUserTagUnix(StringToUTF8Bytes(tagName), StringToUTF8Bytes(tagValue));
            else if (Is64Bit())
                return AdvVer2_AddUserTag64(StringToUTF8Bytes(tagName), StringToUTF8Bytes(tagValue));
            else
                return AdvVer2_AddUserTag32(StringToUTF8Bytes(tagName), StringToUTF8Bytes(tagValue));
        }

        public static int AddOrUpdateImageSectionTag(string tagName, string tagValue)
        {
            if (!CrossPlatform.IsWindows)
                return AdvVer2_AddOrUpdateImageSectionTagUnix(StringToUTF8Bytes(tagName), StringToUTF8Bytes(tagValue));
            else if (Is64Bit())
                return AdvVer2_AddOrUpdateImageSectionTag64(StringToUTF8Bytes(tagName), StringToUTF8Bytes(tagValue));
            else
                return AdvVer2_AddOrUpdateImageSectionTag32(StringToUTF8Bytes(tagName), StringToUTF8Bytes(tagValue));
        }

        public static int FrameAddStatusTagUTF8String(uint tagIndex, string tagValue)
        {
            if (!CrossPlatform.IsWindows)
                return AdvVer2_FrameAddStatusTagUTF8StringUnix(tagIndex, StringToUTF8Bytes(tagValue));
            else if (Is64Bit())
                return AdvVer2_FrameAddStatusTagUTF8String64(tagIndex, StringToUTF8Bytes(tagValue));
            else
                return AdvVer2_FrameAddStatusTagUTF8String32(tagIndex, StringToUTF8Bytes(tagValue));
        }

        public static int FrameAddStatusTagUInt8(uint tagIndex, byte tagValue)
        {
            if (!CrossPlatform.IsWindows)
                return AdvVer2_FrameAddStatusTagUInt8_Unix(tagIndex, tagValue);
            else if (Is64Bit())
                return AdvVer2_FrameAddStatusTagUInt8_64(tagIndex, tagValue);
            else
                return AdvVer2_FrameAddStatusTagUInt8_32(tagIndex, tagValue);
        }

        public static int FrameAddStatusTagInt16(uint tagIndex, short tagValue)
        {
            if (!CrossPlatform.IsWindows)
                return AdvVer2_FrameAddStatusTag16_Unix(tagIndex, tagValue);
            else if (Is64Bit())
                return AdvVer2_FrameAddStatusTag16_64(tagIndex, tagValue);
            else
                return AdvVer2_FrameAddStatusTag16_32(tagIndex, tagValue);
        }

        public static int FrameAddStatusTagFloat(uint tagIndex, float tagValue)
        {
            if (!CrossPlatform.IsWindows)
                return AdvVer2_FrameAddStatusTagReal_Unix(tagIndex, tagValue);
            else if (Is64Bit())
                return AdvVer2_FrameAddStatusTagReal_64(tagIndex, tagValue);
            else
                return AdvVer2_FrameAddStatusTagReal_32(tagIndex, tagValue);
        }

        public static int FrameAddStatusTagInt32(uint tagIndex, int tagValue)
        {
            if (!CrossPlatform.IsWindows)
                return AdvVer2_FrameAddStatusTag32_Unix(tagIndex, tagValue);
            else if (Is64Bit())
                return AdvVer2_FrameAddStatusTag32_64(tagIndex, tagValue);
            else
                return AdvVer2_FrameAddStatusTag32_32(tagIndex, tagValue);
        }

        public static int FrameAddStatusTagInt64(uint tagIndex, long tagValue)
        {
            if (!CrossPlatform.IsWindows)
                return AdvVer2_FrameAddStatusTag64_Unix(tagIndex, tagValue);
            else if (Is64Bit())
                return AdvVer2_FrameAddStatusTag64_64(tagIndex, tagValue);
            else
                return AdvVer2_FrameAddStatusTag64_32(tagIndex, tagValue);
        }

        public static int FrameAddImage(byte layoutId, ushort[] pixels, byte pixelsBpp)
        {
            var rv = 0;
            if (!CrossPlatform.IsWindows)
                rv = AdvVer2_FrameAddImage_Unix(layoutId, pixels, pixelsBpp);
            else if (Is64Bit())
                rv = AdvVer2_FrameAddImage_64(layoutId, pixels, pixelsBpp);
            else
                rv = AdvVer2_FrameAddImage_32(layoutId, pixels, pixelsBpp);

            return rv;
        }

        public static int FrameAddImageBytes(byte layoutId, byte[] pixels, byte pixelsBpp)
        {
            var rv = 0;
            if (!CrossPlatform.IsWindows)
                rv = AdvVer2_FrameAddImageBytes_Unix(layoutId, pixels, pixelsBpp);
            else if (Is64Bit())
                rv = AdvVer2_FrameAddImageBytes_64(layoutId, pixels, pixelsBpp);
            else
                rv = AdvVer2_FrameAddImageBytes_32(layoutId, pixels, pixelsBpp);

            return rv;
        }

        public static int EndFrame()
        {
            if (!CrossPlatform.IsWindows)
                return AdvVer2_EndFrameUnix();
            else if (Is64Bit())
                return AdvVer2_EndFrame64();
            else
                return AdvVer2_EndFrame32();
        }

        public static uint[] GetFramePixels(int streamId, int frameNo, int width, int height)
        {
            AdvFrameInfo frameInfo;
            uint[] pixels;
            int errorCode = GetFramePixels(streamId, frameNo, width, height, out frameInfo, out pixels);
            if (errorCode == AdvError.S_OK)
                return pixels;
            else
                return null;
        }

        public static int GetFramePixels(int streamId, int frameNo, int width, int height, out AdvFrameInfo frameInfo, out uint[] pixels)
        {
			pixels = new uint[width * height];
			var frameInfoNative = new AdvFrameInfoNative();

			byte[] systemError = new byte[256 * 16];
            int errorMessageLen = 0;
            int errorCode = -1;
            if (!CrossPlatform.IsWindows)
                errorCode = AdvVer2_GetFramePixelsUnix(streamId, frameNo, pixels, frameInfoNative, errorMessageLen);
            else if (Is64Bit())
                errorCode = AdvVer2_GetFramePixels64(streamId, frameNo, pixels, frameInfoNative, errorMessageLen);
            else
                errorCode = AdvVer2_GetFramePixels32(streamId, frameNo, pixels, frameInfoNative, errorMessageLen);

            if (errorCode != AdvError.S_OK)
            {
                frameInfo = null;
                return errorCode;
            }

            frameInfo = new AdvFrameInfo(frameInfoNative);

            if (errorMessageLen > 0)
                frameInfo.ErrorMessageStrLen = errorMessageLen;

            return AdvError.S_OK;
        }

        public static int GetMainStreamTag(int tagId, out string tagName, out string tagValue)
        {
            return GetAdvTagPair(TagPairType.MainStream, tagId, out tagName, out tagValue);
        }

        public static int GetCalibrationStreamTag(int tagId, out string tagName, out string tagValue)
        {
            return GetAdvTagPair(TagPairType.CalibrationStream, tagId, out tagName, out tagValue);
        }

        public static int GetSystemMetadataTag(int tagId, out string tagName, out string tagValue)
        {
            return GetAdvTagPair(TagPairType.SystemMetadata, tagId, out tagName, out tagValue);
        }

        public static int GetUserMetadataTag(int tagId, out string tagName, out string tagValue)
        {
            return GetAdvTagPair(TagPairType.UserMetadata, tagId, out tagName, out tagValue);
        }

        public static int GetImageSectionTag(int tagId, out string tagName, out string tagValue)
        {
            return GetAdvTagPair(TagPairType.ImageSection, tagId, out tagName, out tagValue);
        }

        public static int GetImageLayoutTag(int imageLayoutId, int tagId, out string tagName, out string tagValue)
        {
            return GetAdvTagPair(TagPairType.FirstImageLayout + imageLayoutId, tagId, out tagName, out tagValue);
        }

        private static int GetAdvTagPair(TagPairType tagType, int tagId, out string tagName, out string tagValue)
        {
            tagName = null;
            tagValue = null;

            int rv = -1;
            int nameSize = 0;
            int valueSize = 0;
            if (!CrossPlatform.IsWindows)
                rv = AdvVer2_GetTagPairSizesUnix(tagType, tagId, ref nameSize, ref valueSize);
            else if (Is64Bit())
                rv = AdvVer2_GetTagPairSizes64(tagType, tagId, ref nameSize, ref valueSize);
            else
                rv = AdvVer2_GetTagPairSizes32(tagType, tagId, ref nameSize, ref valueSize);
            
            if (rv != AdvError.S_OK)
                return rv;

            var tagNameBT = new byte[2 * nameSize + 1];
            var tagValueBT = new byte[2 * valueSize + 1];


            if (!CrossPlatform.IsWindows)
                rv = AdvVer2_GetTagPairValuesUnix(tagType, tagId, tagNameBT, tagValueBT);
            else if (Is64Bit())
                rv = AdvVer2_GetTagPairValues64(tagType, tagId, tagNameBT, tagValueBT);
            else
                rv = AdvVer2_GetTagPairValues32(tagType, tagId, tagNameBT, tagValueBT);

            if (rv != AdvError.S_OK)
                return rv;

            tagName = GetStringFromUTF8Bytes(tagNameBT);
            tagValue = GetStringFromUTF8Bytes(tagValueBT);

            return AdvError.S_OK;
        }

        public static int GetStatusTagInfo(uint tagId, out Adv2TagType? tagType, out string tagName)
        {
            int nameSize = 0;
            int errorCode = -1;
            tagType = null;
            tagName = null;

            if (!CrossPlatform.IsWindows)
                errorCode = AdvVer2_GetStatusTagNameSizeUnix(tagId, ref nameSize);
            else if (Is64Bit())
                errorCode = AdvVer2_GetStatusTagNameSize64(tagId, ref nameSize);
            else
                errorCode = AdvVer2_GetStatusTagNameSize32(tagId, ref nameSize);

            if (errorCode != AdvError.S_OK)
                return errorCode;

            var tagNameBT = new byte[2 * nameSize + 1];

            Adv2TagType tt = Adv2TagType.Int8;
            if (!CrossPlatform.IsWindows)
                errorCode = AdvVer2_GetStatusTagInfoUnix(tagId, tagNameBT, ref tt);
            else if (Is64Bit())
                errorCode = AdvVer2_GetStatusTagInfo64(tagId, tagNameBT, ref tt);
            else
                errorCode = AdvVer2_GetStatusTagInfo32(tagId, tagNameBT, ref tt);

            if (errorCode != AdvError.S_OK)
                return errorCode;

            tagName = GetStringFromUTF8Bytes(tagNameBT);
            tagType = tt;

            return AdvError.S_OK;
        }

        public static int GetStatusTagUTF8String(uint tagId, out string tagValue)
        {
            int valueSize = 0;
            int errorCode = -1;
            tagValue = null;
            if (!CrossPlatform.IsWindows)
                errorCode = AdvVer2_GetStatusTagSizeUTF8StringUnix(tagId, ref valueSize);
            else if (Is64Bit())
                errorCode = AdvVer2_GetStatusTagSizeUTF8String64(tagId, ref valueSize);
            else
                errorCode = AdvVer2_GetStatusTagSizeUTF8String32(tagId, ref valueSize);

            if (errorCode != AdvError.S_OK)
                return errorCode;

            var tagValueBT = new byte[2 * valueSize + 1];


            if (!CrossPlatform.IsWindows)
                errorCode = AdvVer2_GetStatusTagUTF8StringUnix(tagId, tagValueBT);
            else if (Is64Bit())
                errorCode = AdvVer2_GetStatusTagUTF8String64(tagId, tagValueBT);
            else
                errorCode = AdvVer2_GetStatusTagUTF8String32(tagId, tagValueBT);

            if (errorCode != AdvError.S_OK)
                return errorCode;

            tagValue = GetStringFromUTF8Bytes(tagValueBT);
            return AdvError.S_OK;
        }


        public static int GetStatusTagUInt8(uint tagId, out byte? tagValue)
        {
            int errorCode = -1;
            byte rv = 0;
            tagValue = 0;
            if (!CrossPlatform.IsWindows)
                errorCode = AdvVer2_GetStatusTagUInt8_Unix(tagId, ref rv);
            else if (Is64Bit())
                errorCode = AdvVer2_GetStatusTagUInt8_64(tagId, ref rv);
            else
                errorCode = AdvVer2_GetStatusTagUInt8_32(tagId, ref rv);

            if (errorCode != AdvError.S_OK)
                return errorCode;

            tagValue = rv;
            return AdvError.S_OK;
        }

        public static int GetStatusTagInt16(uint tagId, out short? tagValue)
        {
            int errorCode = -1;
            short rv = 0;
            tagValue = null;
            if (!CrossPlatform.IsWindows)
                errorCode = AdvVer2_GetStatusTag16_Unix(tagId, ref rv);
            else if (Is64Bit())
                errorCode = AdvVer2_GetStatusTag16_64(tagId, ref rv);
            else
                errorCode = AdvVer2_GetStatusTag16_32(tagId, ref rv);

            if (errorCode != AdvError.S_OK)
                return errorCode;

            tagValue = rv;
            return AdvError.S_OK;
        }

        public static int GetStatusTagFloat(uint tagId, out float? tagValue)
        {
            int errorCode = -1;
            float rv = 0;
            tagValue = null;
            if (!CrossPlatform.IsWindows)
                errorCode = AdvVer2_GetStatusTagReal_Unix(tagId, ref rv);
            else if (Is64Bit())
                errorCode = AdvVer2_GetStatusTagReal_64(tagId, ref rv);
            else
                errorCode = AdvVer2_GetStatusTagReal_32(tagId, ref rv);

            if (errorCode != AdvError.S_OK)
                return errorCode;

            tagValue = rv;
            return AdvError.S_OK;
        }

        public static int GetStatusTagInt32(uint tagId, out int? tagValue)
        {
            int errorCode = -1;
            int rv = 0;
            tagValue = null;
            if (!CrossPlatform.IsWindows)
                errorCode = AdvVer2_GetStatusTag32_Unix(tagId, ref rv);
            else if (Is64Bit())
                errorCode = AdvVer2_GetStatusTag32_64(tagId, ref rv);
            else
                errorCode = AdvVer2_GetStatusTag32_32(tagId, ref rv);

            if (errorCode != AdvError.S_OK)
                return errorCode;

            tagValue = rv;
            return AdvError.S_OK;
        }

        public static int GetStatusTagInt64(uint tagId, out long? tagValue)
        {
            int errorCode = -1;
            long rv = 0;
            tagValue = null;
            if (!CrossPlatform.IsWindows)
                errorCode = AdvVer2_GetStatusTag64_Unix(tagId, ref rv);
            else if (Is64Bit())
                errorCode = AdvVer2_GetStatusTag64_64(tagId, ref rv);
            else
                errorCode = AdvVer2_GetStatusTag64_32(tagId, ref rv);

            if (errorCode != AdvError.S_OK)
                return errorCode;

            tagValue = rv;
            return AdvError.S_OK;
        }

        internal static string GetStringFromUTF8Bytes(byte[] chars)
        {
            string str = Encoding.UTF8.GetString(chars);
            return str.Substring(0, str.IndexOf('\0'));
        }

        public static int GetImageLayoutInfo(int layoutIndex, out AdvImageLayoutInfo imageLayoutInfo)
        {
            int errorCode = -1;
            imageLayoutInfo = new AdvImageLayoutInfo();
            if (!CrossPlatform.IsWindows)
                errorCode = AdvVer2_GetImageLayoutInfoUnix(layoutIndex, ref imageLayoutInfo);
            else if (Is64Bit())
                errorCode = AdvVer2_GetImageLayoutInfo64(layoutIndex, ref imageLayoutInfo);
            else
                errorCode = AdvVer2_GetImageLayoutInfo32(layoutIndex, ref imageLayoutInfo);

            if (errorCode != AdvError.S_OK)
            {
                imageLayoutInfo = AdvImageLayoutInfo.Empty;
                return errorCode;
            }

            return AdvError.S_OK;
        }

        public static int GetLastSystemSpecificFileError()
        {
            if (!CrossPlatform.IsWindows)
                return AdvVer2_GetLastSystemSpecificFileErrorUnix();
            else if (Is64Bit())
                return AdvVer2_GetLastSystemSpecificFileError64();
            else
                return AdvVer2_GetLastSystemSpecificFileError32();
        }
    }
}

