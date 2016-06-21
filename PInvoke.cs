/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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
        List16OfAnsiString255 = 6,
        UTF8String = 7,
        List16OfUTF8String = 8,
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct AdvFileInfo
    {
        public int Width;
        public int Height;
        public int CountMaintFrames;
        public int CountCalibrationFrames;
        public int Bpp;
        public int MaxPixelVal;
    };

    [StructLayout(LayoutKind.Explicit)]
    public class AdvFrameInfoNative
    {
        private static DateTime REFERENCE_DATETIME = new DateTime(2010, 1, 1, 0, 0, 0, 0);

        public AdvFrameInfoNative()
        {
            StartTicksLo = 0;
            StartTicksHi = 0;
            EndTicksLo = 0;
            EndTicksHi = 0;

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
            Temperature = 0;

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
        public float Gamma;
        [FieldOffset(20)]
        public float Gain;
        [FieldOffset(24)]
        public float Shutter;
        [FieldOffset(28)]
        public float Offset;
        [FieldOffset(32)]
        public byte GPSTrackedSattelites;
        [FieldOffset(33)]
        public byte GPSAlmanacStatus;
        [FieldOffset(34)]
        public byte GPSFixStatus;
        [FieldOffset(35)]
        public byte GPSAlmanacOffset;
        [FieldOffset(36)]
        public uint VideoCameraFrameIdLo;
        [FieldOffset(40)]
        public uint VideoCameraFrameIdHi;
        [FieldOffset(44)]
        public float Temperature;
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
        //unsigned int AdvOpenFile(const char* fileName);
        private static extern int AdvOpenFile32(string fileName);

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
        //DLL_PUBLIC void AdvVer2_EndFile();
        private static extern void AdvVer2_EndFile32();

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
        //bool AdvBeginFrame(unsigned char streamId, __int64 startFrameTicks, __int64 endFrameTicks, __int64 elapsedTicksSinceFirstFrame, __int64 utcStartTimeNanosecondsSinceAdvZeroEpoch, unsigned int utcExposureNanoseconds));
        private static extern bool AdvVer2_BeginFrame32(uint streamId, long startTicks, long endTicks, long elapsedTicksFromStart, ulong utcStartTimeNanosecondsSinceAdvZeroEpoch, uint utcExposureNanoseconds);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_BeginFrame")]
        //bool AdvBeginFrame(unsigned char streamId, __int64 utcStartTimeNanosecondsSinceAdvZeroEpoch, unsigned int utcExposureNanoseconds));
        private static extern bool AdvVer2_BeginFrame32(uint streamId, ulong utcStartTimeNanosecondsSinceAdvZeroEpoch, uint utcExposureNanoseconds);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_DefineImageSection")]
        //void AdvVer2_DefineImageSection(unsigned short width, unsigned short height, unsigned char dataBpp);
        private static extern void AdvVer2_DefineImageSection32(ushort width, ushort height, byte dataBpp);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_DefineStatusSection")]
        //void AdvVer2_DefineStatusSection(__int64 utcTimestampAccuracyInNanoseconds);
        private static extern void AdvVer2_DefineStatusSection32(long utcTimestampAccuracyInNanoseconds);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_DefineImageLayout")]
        //void AdvVer2_DefineImageLayout(unsigned char layoutId, const char* layoutType, const char* compression, unsigned char layoutBpp);
        private static extern void AdvVer2_DefineImageLayout32(byte layoutId, [MarshalAs(UnmanagedType.LPStr)]string layoutType, [MarshalAs(UnmanagedType.LPStr)]string compression, byte layoutBpp);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_DefineStatusSectionTag")]
        //unsigned int AdvVer2_DefineStatusSectionTag(const char* tagName, int tagType);
        private static extern uint AdvVer2_DefineStatusSectionTag32([MarshalAs(UnmanagedType.LPStr)]string tagName, AdvTagType tagType);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_AddFileTag")]
        //unsigned int AdvVer2_AddFileTag(const char* tagName, const char* tagValue);
        private static extern uint AdvVer2_AddFileTag32([MarshalAs(UnmanagedType.LPStr)]string tagName, [MarshalAs(UnmanagedType.LPStr)]string tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_AddOrUpdateImageSectionTag")]
        //void AdvVer2_AddOrUpdateImageSectionTag(const char* tagName, const char* tagValue);
        private static extern uint AdvVer2_AddOrUpdateImageSectionTag32([MarshalAs(UnmanagedType.LPStr)]string tagName, [MarshalAs(UnmanagedType.LPStr)]string tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTagUTF8String")]
        //void AdvVer2_FrameAddStatusTagUTF8String(unsigned int tagIndex, const char* tagValue);
        private static extern void AdvVer2_FrameAddStatusTagUTF8String32(uint tagIndex, [MarshalAs(UnmanagedType.LPStr)]string tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTagMessage")]
        //void AdvVer2_FrameAddStatusTagMessage(unsigned int tagIndex, const char* tagValue);
        private static extern void AdvVer2_FrameAddStatusTagMessage32(uint tagIndex, [MarshalAs(UnmanagedType.LPStr)]string tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTagUInt8")]
        //void AdvVer2_FrameAddStatusTagUInt8(unsigned int tagIndex, unsigned char tagValue);
        private static extern void AdvVer2_FrameAddStatusTagUInt8_32(uint tagIndex, byte tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTag16")]
        //void AdvVer2_FrameAddStatusTag16(unsigned int tagIndex, unsigned short tagValue);
        private static extern void AdvVer2_FrameAddStatusTag16_32(uint tagIndex, ushort tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTagReal")]
        //void AdvVer2_FrameAddStatusTagReal(unsigned int tagIndex, float tagValue);
        private static extern void AdvVer2_FrameAddStatusTagReal_32(uint tagIndex, float tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTag32")]
        //void AdvVer2_FrameAddStatusTag32(unsigned int tagIndex, unsigned long tagValue);
        private static extern void AdvVer2_FrameAddStatusTag32_32(uint tagIndex, uint tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTag64")]
        //void AdvVer2_FrameAddStatusTag64(unsigned int tagIndex, long long tagValue);
        private static extern void AdvVer2_FrameAddStatusTag64_32(uint tagIndex, ulong tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddImage")]
        //HRESULT AdvVer2_FrameAddImage(unsigned char layoutId, unsigned short* pixels, unsigned char pixelsBpp);
        private static extern int AdvVer2_FrameAddImage_32(byte layoutId, [In, MarshalAs(UnmanagedType.LPArray)] ushort[] pixels, byte pixelsBpp);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddImageBytes")]
        //HRESULT AdvVer2_FrameAddImageBytes(unsigned char layoutId, unsigned char* pixels, unsigned char pixelsBpp);
        private static extern int AdvVer2_FrameAddImageBytes_32(byte layoutId, [In, MarshalAs(UnmanagedType.LPArray)] byte[] pixels, byte pixelsBpp);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_EndFrame")]
        //void AdvVer2_EndFrame();
        private static extern void AdvVer2_EndFrame32();

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetMainStreamInfo")]
        //void AdvVer2_GetMainStreamInfo(long* numFrames, __int64* mainClockFrequency, long* mainStreamAccuracy);
        private static extern void AdvVer2_GetMainStreamInfo32(ref int numFrames, ref long mainClockFrequency, ref int mainStreamAccuracy);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetCalibrationStreamInfo")]
        //void AdvVer2_GetCalibrationStreamInfo(long* numFrames, __int64* calibrationClockFrequency, long* calibrationStreamAccuracy);
        private static extern void AdvVer2_GetCalibrationStreamInfo32(ref int numFrames, ref long calibrationClockFrequency, ref int calibrationStreamAccuracy);

        [DllImport(LIBRARY_ADVLIB_CORE32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetFramePixels")]
        //HRESULT AdvVer2_GetFramePixels(int streamId, int frameNo, unsigned int* pixels, AdvLib2::AdvFrameInfo* frameInfo, char* systemError);
        private static extern int AdvVer2_GetFramePixels32(int streamId, int frameNo, [In, Out] uint[] pixels, [In, Out] AdvFrameInfoNative frameInfo, [In, Out] byte[] systemError);
        #endregion 

        #region 64bit externals
        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLibraryVersion")]
        private static extern void GetLibraryVersion64([MarshalAs(UnmanagedType.LPArray)]byte[] version);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLibraryPlatformId")]
        private static extern void GetLibraryPlatformId64([MarshalAs(UnmanagedType.LPArray)]byte[] platform);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLibraryBitness")]
        private static extern int GetLibraryBitness64();

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvOpenFile")]
        //unsigned int AdvOpenFile(const char* fileName);
        private static extern int AdvOpenFile64(string fileName);

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
        //DLL_PUBLIC void AdvVer2_EndFile();
        private static extern void AdvVer2_EndFile64();

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
        //DLL_PUBLIC unsigned int AdvVer2_AddMainStreamTag(const char* tagName, const char* tagValue);
        private static extern int AdvVer2_AddMainStreamTag64([MarshalAs(UnmanagedType.LPArray)]byte[] tagName, [MarshalAs(UnmanagedType.LPArray)]byte[] tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_AddCalibrationStreamTag")]
        //DLL_PUBLIC unsigned int AdvVer2_AddCalibrationStreamTag(const char* tagName, const char* tagValue);
        private static extern int AdvVer2_AddCalibrationStreamTag64([MarshalAs(UnmanagedType.LPArray)]byte[] tagName, [MarshalAs(UnmanagedType.LPArray)]byte[] tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_BeginFrameWithTicks")]
        //bool AdvBeginFrame(unsigned char streamId, __int64 startFrameTicks, __int64 endFrameTicks, __int64 elapsedTicksSinceFirstFrame, __int64 utcStartTimeNanosecondsSinceAdvZeroEpoch, unsigned int utcExposureNanoseconds);
        private static extern bool AdvVer2_BeginFrame64(uint streamId, long startTicks, long endTicks, long elapsedTicksFromStart, ulong utcStartTimeNanosecondsSinceAdvZeroEpoch, uint utcExposureNanoseconds);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_BeginFrame")]
        //bool AdvBeginFrame(unsigned char streamId, __int64 utcStartTimeNanosecondsSinceAdvZeroEpoch, unsigned int utcExposureNanoseconds));
        private static extern bool AdvVer2_BeginFrame64(uint streamId, ulong utcStartTimeNanosecondsSinceAdvZeroEpoch, uint utcExposureNanoseconds);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_DefineImageSection")]
        //void AdvVer2_DefineImageSection(unsigned short width, unsigned short height, unsigned char dataBpp);
        private static extern void AdvVer2_DefineImageSection64(ushort width, ushort height, byte dataBpp);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_DefineStatusSection")]
        //void AdvVer2_DefineStatusSection(__int64 utcTimestampAccuracyInNanoseconds);
        private static extern void AdvVer2_DefineStatusSection64(long utcTimestampAccuracyInNanoseconds);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_DefineImageLayout")]
        //void AdvVer2_DefineImageLayout(unsigned char layoutId, const char* layoutType, const char* compression, unsigned char layoutBpp);
        private static extern void AdvVer2_DefineImageLayout64(byte layoutId, [MarshalAs(UnmanagedType.LPStr)]string layoutType, [MarshalAs(UnmanagedType.LPStr)]string compression, byte layoutBpp);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_DefineStatusSectionTag")]
        //unsigned int AdvVer2_DefineStatusSectionTag(const char* tagName, int tagType);
        private static extern uint AdvVer2_DefineStatusSectionTag64([MarshalAs(UnmanagedType.LPStr)]string tagName, AdvTagType tagType);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_AddFileTag")]
        //unsigned int AdvVer2_AddFileTag(const char* tagName, const char* tagValue);
        private static extern uint AdvVer2_AddFileTag64([MarshalAs(UnmanagedType.LPStr)]string tagName, [MarshalAs(UnmanagedType.LPStr)]string tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_AddOrUpdateImageSectionTag")]
        //void AdvVer2_AddOrUpdateImageSectionTag(const char* tagName, const char* tagValue);
        private static extern uint AdvVer2_AddOrUpdateImageSectionTag64([MarshalAs(UnmanagedType.LPStr)]string tagName, [MarshalAs(UnmanagedType.LPStr)]string tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTagUTF8String")]
        //void AdvVer2_FrameAddStatusTagUTF8String(unsigned int tagIndex, const char* tagValue);
        private static extern void AdvVer2_FrameAddStatusTagUTF8String64(uint tagIndex, [MarshalAs(UnmanagedType.LPStr)]string tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTagMessage")]
        //void AdvVer2_FrameAddStatusTagMessage(unsigned int tagIndex, const char* tagValue);
        private static extern void AdvVer2_FrameAddStatusTagMessage64(uint tagIndex, [MarshalAs(UnmanagedType.LPStr)]string tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTagUInt8")]
        //void AdvVer2_FrameAddStatusTagUInt8(unsigned int tagIndex, unsigned char tagValue);
        private static extern void AdvVer2_FrameAddStatusTagUInt8_64(uint tagIndex, byte tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTag16")]
        //void AdvVer2_FrameAddStatusTag16(unsigned int tagIndex, unsigned short tagValue);
        private static extern void AdvVer2_FrameAddStatusTag16_64(uint tagIndex, ushort tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTagReal")]
        //void AdvVer2_FrameAddStatusTagReal(unsigned int tagIndex, float tagValue);
        private static extern void AdvVer2_FrameAddStatusTagReal_64(uint tagIndex, float tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTag32")]
        //void AdvVer2_FrameAddStatusTag32(unsigned int tagIndex, unsigned long tagValue);
        private static extern void AdvVer2_FrameAddStatusTag32_64(uint tagIndex, uint tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTag64")]
        //void AdvVer2_FrameAddStatusTag64(unsigned int tagIndex, long long tagValue);
        private static extern void AdvVer2_FrameAddStatusTag64_64(uint tagIndex, ulong tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddImage")]
        //HRESULT AdvVer2_FrameAddImage(unsigned char layoutId, unsigned short* pixels, unsigned char pixelsBpp);
        private static extern int AdvVer2_FrameAddImage_64(byte layoutId, [In, MarshalAs(UnmanagedType.LPArray)] ushort[] pixels, byte pixelsBpp);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddImageBytes")]
        //HRESULT AdvVer2_FrameAddImageBytes(unsigned char layoutId, unsigned char* pixels, unsigned char pixelsBpp);
        private static extern int AdvVer2_FrameAddImageBytes_64(byte layoutId, [In, MarshalAs(UnmanagedType.LPArray)] byte[] pixels, byte pixelsBpp);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_EndFrame")]
        //void AdvVer2_EndFrame();
        private static extern void AdvVer2_EndFrame64();

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetMainStreamInfo")]
        //void AdvVer2_GetMainStreamInfo(long* numFrames, __int64* mainClockFrequency, long* mainStreamAccuracy);
        private static extern void AdvVer2_GetMainStreamInfo64(ref int numFrames, ref long mainClockFrequency, ref int mainStreamAccuracy);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetCalibrationStreamInfo")]
        //void AdvVer2_GetCalibrationStreamInfo(long* numFrames, __int64* calibrationClockFrequency, long* calibrationStreamAccuracy);
        private static extern void AdvVer2_GetCalibrationStreamInfo64(ref int numFrames, ref long calibrationClockFrequency, ref int calibrationStreamAccuracy);

        [DllImport(LIBRARY_ADVLIB_CORE64, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetFramePixels")]
        //HRESULT AdvVer2_GetFramePixels(int streamId, int frameNo, unsigned int* pixels, AdvLib2::AdvFrameInfo* frameInfo, char* systemError);
        private static extern int AdvVer2_GetFramePixels64(int streamId, int frameNo, [In, Out] uint[] pixels, [In, Out] AdvFrameInfoNative frameInfo, [In, Out] byte[] systemError);

        #endregion

        #region UNIX externals
        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLibraryVersion")]
        private static extern void GetLibraryVersionUnix([MarshalAs(UnmanagedType.LPArray)]byte[] version);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLibraryPlatformId")]
        private static extern void GetLibraryPlatformIdUnix([MarshalAs(UnmanagedType.LPArray)]byte[] platform);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLibraryBitness")]
        private static extern int GetLibraryBitnessUnix();

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvOpenFile")]
        //unsigned int AdvOpenFile(const char* fileName);
        private static extern int AdvOpenFileUnix(string fileName);

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
        //void AdvEndFrame();
        private static extern void AdvVer1_EndFrameUnix();

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_NewFile")]
        //DLL_PUBLIC void AdvVer2_NewFile(const char* fileName);
        private static extern void AdvVer2_NewFileUnix(string fileName);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_EndFile")]
        //DLL_PUBLIC void AdvVer2_EndFile();
        private static extern void AdvVer2_EndFileUnix();

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
        //DLL_PUBLIC unsigned int AdvVer2_AddMainStreamTag(const char* tagName, const char* tagValue);
        private static extern int AdvVer2_AddMainStreamTagUnix([MarshalAs(UnmanagedType.LPArray)]byte[] tagName, [MarshalAs(UnmanagedType.LPArray)]byte[] tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_AddCalibrationStreamTag")]
        //DLL_PUBLIC unsigned int AdvVer2_AddCalibrationStreamTag(const char* tagName, const char* tagValue);
        private static extern int AdvVer2_AddCalibrationStreamTagUnix([MarshalAs(UnmanagedType.LPArray)]byte[] tagName, [MarshalAs(UnmanagedType.LPArray)]byte[] tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_BeginFrameWithTicks")]
        //bool AdvBeginFrame(unsigned char streamId, __int64 startFrameTicks, __int64 endFrameTicks, __int64 elapsedTicksSinceFirstFrame, __int64 utcStartTimeNanosecondsSinceAdvZeroEpoch, unsigned int utcExposureNanoseconds);
        private static extern bool AdvVer2_BeginFrameUnix(uint streamId, long startTicks, long endTicks, long elapsedTicksFromStart, ulong utcStartTimeNanosecondsSinceAdvZeroEpoch, uint utcExposureNanoseconds);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_BeginFrame")]
        //bool AdvBeginFrame(unsigned char streamId, __int64 utcStartTimeNanosecondsSinceAdvZeroEpoch, unsigned int utcExposureNanoseconds));
        private static extern bool AdvVer2_BeginFrameUnix(uint streamId, ulong utcStartTimeNanosecondsSinceAdvZeroEpoch, uint utcExposureNanoseconds);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_DefineImageSection")]
        //void AdvVer2_DefineImageSection(unsigned short width, unsigned short height, unsigned char dataBpp);
        private static extern void AdvVer2_DefineImageSectionUnix(ushort width, ushort height, byte dataBpp);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_DefineStatusSection")]
        //void AdvVer2_DefineStatusSection(__int64 utcTimestampAccuracyInNanoseconds);
        private static extern void AdvVer2_DefineStatusSectionUnix(long utcTimestampAccuracyInNanoseconds);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_DefineImageLayout")]
        //void AdvVer2_DefineImageLayout(unsigned char layoutId, const char* layoutType, const char* compression, unsigned char layoutBpp);
        private static extern void AdvVer2_DefineImageLayoutUnix(byte layoutId, [MarshalAs(UnmanagedType.LPStr)]string layoutType, [MarshalAs(UnmanagedType.LPStr)]string compression, byte layoutBpp);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_DefineStatusSectionTag")]
        //unsigned int AdvVer2_DefineStatusSectionTag(const char* tagName, int tagType);
        private static extern uint AdvVer2_DefineStatusSectionTagUnix([MarshalAs(UnmanagedType.LPStr)]string tagName, AdvTagType tagType);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_AddFileTag")]
        //unsigned int AdvVer2_AddFileTag(const char* tagName, const char* tagValue);
        private static extern uint AdvVer2_AddFileTagUnix([MarshalAs(UnmanagedType.LPStr)]string tagName, [MarshalAs(UnmanagedType.LPStr)]string tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_AddOrUpdateImageSectionTag")]
        //void AdvVer2_AddOrUpdateImageSectionTag(const char* tagName, const char* tagValue);
        private static extern uint AdvVer2_AddOrUpdateImageSectionTagUnix([MarshalAs(UnmanagedType.LPStr)]string tagName, [MarshalAs(UnmanagedType.LPStr)]string tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTagUTF8String")]
        //void AdvVer2_FrameAddStatusTagUTF8String(unsigned int tagIndex, const char* tagValue);
        private static extern void AdvVer2_FrameAddStatusTagUTF8StringUnix(uint tagIndex, [MarshalAs(UnmanagedType.LPStr)]string tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTagMessage")]
        //void AdvVer2_FrameAddStatusTagMessage(unsigned int tagIndex, const char* tagValue);
        private static extern void AdvVer2_FrameAddStatusTagMessageUnix(uint tagIndex, [MarshalAs(UnmanagedType.LPStr)]string tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTagUInt8")]
        //void AdvVer2_FrameAddStatusTagUInt8(unsigned int tagIndex, unsigned char tagValue);
        private static extern void AdvVer2_FrameAddStatusTagUInt8_Unix(uint tagIndex, byte tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTag16")]
        //void AdvVer2_FrameAddStatusTag16(unsigned int tagIndex, unsigned short tagValue);
        private static extern void AdvVer2_FrameAddStatusTag16_Unix(uint tagIndex, ushort tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTagReal")]
        //void AdvVer2_FrameAddStatusTagReal(unsigned int tagIndex, float tagValue);
        private static extern void AdvVer2_FrameAddStatusTagReal_Unix(uint tagIndex, float tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTag32")]
        //void AdvVer2_FrameAddStatusTag32(unsigned int tagIndex, unsigned long tagValue);
        private static extern void AdvVer2_FrameAddStatusTag32_Unix(uint tagIndex, uint tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddStatusTag64")]
        //void AdvVer2_FrameAddStatusTagUnix(unsigned int tagIndex, long long tagValue);
        private static extern void AdvVer2_FrameAddStatusTag64_Unix(uint tagIndex, ulong tagValue);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddImage")]
        //HRESULT AdvVer2_FrameAddImage(unsigned char layoutId, unsigned short* pixels, unsigned char pixelsBpp);
        private static extern int AdvVer2_FrameAddImage_Unix(byte layoutId, [In, MarshalAs(UnmanagedType.LPArray)] ushort[] pixels, byte pixelsBpp);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_FrameAddImageBytes")]
        //HRESULT AdvVer2_FrameAddImageBytes(unsigned char layoutId, unsigned char* pixels, unsigned char pixelsBpp);
        private static extern int AdvVer2_FrameAddImageBytes_Unix(byte layoutId, [In, MarshalAs(UnmanagedType.LPArray)] byte[] pixels, byte pixelsBpp);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_EndFrame")]
        //void AdvVer2_EndFrame();
        private static extern void AdvVer2_EndFrameUnix();

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetMainStreamInfo")]
        //void AdvVer2_GetMainStreamInfo(long* numFrames, __int64* mainClockFrequency, long* mainStreamAccuracy);
        private static extern void AdvVer2_GetMainStreamInfoUnix(ref int numFrames, ref long mainClockFrequency, ref int mainStreamAccuracy);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetCalibrationStreamInfo")]
        //void AdvVer2_GetCalibrationStreamInfo(long* numFrames, __int64* calibrationClockFrequency, long* calibrationStreamAccuracy);
        private static extern void AdvVer2_GetCalibrationStreamInfoUnix(ref int numFrames, ref long calibrationClockFrequency, ref int calibrationStreamAccuracy);

        [DllImport(LIBRARY_ADVLIB_CORE_UNIX, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AdvVer2_GetFramePixels")]
        //HRESULT AdvVer2_GetFramePixels(int streamId, int frameNo, unsigned int* pixels, AdvLib2::AdvFrameInfo* frameInfo, char* systemError);
        private static extern int AdvVer2_GetFramePixelsUnix(int streamId, int frameNo, [In, Out] uint[] pixels, [In, Out] AdvFrameInfoNative frameInfo, [In, Out] byte[] systemError);

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

        public static int AdvOpenFile(string fileName)
        {
            if (!CrossPlatform.IsWindows)
                return AdvOpenFileUnix(fileName);
            else if (Is64Bit())
                return AdvOpenFile64(fileName);
            else
                return AdvOpenFile32(fileName);
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

        public static void EndFile()
        {
            if (!CrossPlatform.IsWindows)
                AdvVer2_EndFileUnix();
            else if (Is64Bit())
                AdvVer2_EndFile64();
            else
                AdvVer2_EndFile32();
        }

        private static byte[] StringToUTF8Bytes(string str)
        {
            if (String.IsNullOrEmpty(str))
                return new byte[0];
            else
                return Encoding.UTF8.GetBytes(str);
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

        public static void AddMainStreamTag(string tagName, string tagValue)
        {
            if (!CrossPlatform.IsWindows)
                AdvVer2_AddMainStreamTagUnix(StringToUTF8Bytes(tagName), StringToUTF8Bytes(tagValue));
            else if (Is64Bit())
                AdvVer2_AddMainStreamTag64(StringToUTF8Bytes(tagName), StringToUTF8Bytes(tagValue));
            else
                AdvVer2_AddMainStreamTag32(StringToUTF8Bytes(tagName), StringToUTF8Bytes(tagValue));
        }

        public static void AddCalibrationStreamTag(string tagName, string tagValue)
        {
            if (!CrossPlatform.IsWindows)
                AdvVer2_AddCalibrationStreamTagUnix(StringToUTF8Bytes(tagName), StringToUTF8Bytes(tagValue));
            else if (Is64Bit())
                AdvVer2_AddCalibrationStreamTag64(StringToUTF8Bytes(tagName), StringToUTF8Bytes(tagValue));
            else
                AdvVer2_AddCalibrationStreamTag32(StringToUTF8Bytes(tagName), StringToUTF8Bytes(tagValue));
        }

        public static bool BeginFrame(byte streamId, long startTicks, long endTicks, long elapsedTicksFromStart, ulong utcStartTimeNanosecondsSinceAdvZeroEpoch, uint utcExposureNanoseconds)
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
                Trace.WriteLine(ex);

                return false;
            }
        }

        public static bool BeginFrame(byte streamId, ulong utcStartTimeNanosecondsSinceAdvZeroEpoch, uint utcExposureNanoseconds)
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
                Trace.WriteLine(ex);

                return false;
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

        public static void DefineImageSection(ushort width, ushort height, byte dataBpp)
        {
            if (!CrossPlatform.IsWindows)
                AdvVer2_DefineImageSectionUnix(width, height, dataBpp);
            else if (Is64Bit())
                AdvVer2_DefineImageSection64(width, height, dataBpp);
            else
                AdvVer2_DefineImageSection32(width, height, dataBpp);
        }

        public static void DefineStatusSection(long utcTimestampAccuracyInNanoseconds)
        {
            if (!CrossPlatform.IsWindows)
                AdvVer2_DefineStatusSectionUnix(utcTimestampAccuracyInNanoseconds);
            else if (Is64Bit())
                AdvVer2_DefineStatusSection64(utcTimestampAccuracyInNanoseconds);
            else
                AdvVer2_DefineStatusSection32(utcTimestampAccuracyInNanoseconds);
        }

        public static void DefineImageLayout(byte layoutId, string layoutType, string compression, byte layoutBpp)
        {
            if (!CrossPlatform.IsWindows)
                AdvVer2_DefineImageLayoutUnix(layoutId, layoutType, compression, layoutBpp);
            else if (Is64Bit())
                AdvVer2_DefineImageLayout64(layoutId, layoutType, compression, layoutBpp);
            else
                AdvVer2_DefineImageLayout32(layoutId, layoutType, compression, layoutBpp);
        }

        public static uint DefineStatusSectionTag(string tagName, AdvTagType tagType)
        {
            if (!CrossPlatform.IsWindows)
                return AdvVer2_DefineStatusSectionTagUnix(tagName, tagType);
            else if (Is64Bit())
                return AdvVer2_DefineStatusSectionTag64(tagName, tagType);
            else
                return AdvVer2_DefineStatusSectionTag32(tagName, tagType);
        }

        public static uint AddFileTag(string tagName, string tagValue)
        {
            if (!CrossPlatform.IsWindows)
                return AdvVer2_AddFileTagUnix(tagName, tagValue);
            else if (Is64Bit())
                return AdvVer2_AddFileTag64(tagName, tagValue);
            else
                return AdvVer2_AddFileTag32(tagName, tagValue);
        }

        public static uint AddOrUpdateImageSectionTag(string tagName, string tagValue)
        {
            if (!CrossPlatform.IsWindows)
                return AdvVer2_AddOrUpdateImageSectionTagUnix(tagName, tagValue);
            else if (Is64Bit())
                return AdvVer2_AddOrUpdateImageSectionTag64(tagName, tagValue);
            else
                return AdvVer2_AddOrUpdateImageSectionTag32(tagName, tagValue);
        }

        public static void FrameAddStatusTagUTF8String(uint tagIndex, string tagValue)
        {
            if (!CrossPlatform.IsWindows)
                AdvVer2_FrameAddStatusTagUTF8StringUnix(tagIndex, tagValue);
            else if (Is64Bit())
                AdvVer2_FrameAddStatusTagUTF8String64(tagIndex, tagValue);
            else
                AdvVer2_FrameAddStatusTagUTF8String32(tagIndex, tagValue);
        }

        public static void FrameAddStatusTagMessage(uint tagIndex, string tagValue)
        {
            if (!CrossPlatform.IsWindows)
                AdvVer2_FrameAddStatusTagMessageUnix(tagIndex, tagValue);
            else if (Is64Bit())
                AdvVer2_FrameAddStatusTagMessage64(tagIndex, tagValue);
            else
                AdvVer2_FrameAddStatusTagMessage32(tagIndex, tagValue);
        }

        public static void FrameAddStatusTagUInt8(uint tagIndex, byte tagValue)
        {
            if (!CrossPlatform.IsWindows)
                AdvVer2_FrameAddStatusTagUInt8_Unix(tagIndex, tagValue);
            else if (Is64Bit())
                AdvVer2_FrameAddStatusTagUInt8_64(tagIndex, tagValue);
            else
                AdvVer2_FrameAddStatusTagUInt8_32(tagIndex, tagValue);
        }

        public static void FrameAddStatusTag16(uint tagIndex, ushort tagValue)
        {
            if (!CrossPlatform.IsWindows)
                AdvVer2_FrameAddStatusTag16_Unix(tagIndex, tagValue);
            else if (Is64Bit())
                AdvVer2_FrameAddStatusTag16_64(tagIndex, tagValue);
            else
                AdvVer2_FrameAddStatusTag16_32(tagIndex, tagValue);
        }

        public static void FrameAddStatusTagReal(uint tagIndex, float tagValue)
        {
            if (!CrossPlatform.IsWindows)
                AdvVer2_FrameAddStatusTagReal_Unix(tagIndex, tagValue);
            else if (Is64Bit())
                AdvVer2_FrameAddStatusTagReal_64(tagIndex, tagValue);
            else
                AdvVer2_FrameAddStatusTagReal_32(tagIndex, tagValue);
        }

        public static void FrameAddStatusTag32(uint tagIndex, uint tagValue)
        {
            if (!CrossPlatform.IsWindows)
                AdvVer2_FrameAddStatusTag32_Unix(tagIndex, tagValue);
            else if (Is64Bit())
                AdvVer2_FrameAddStatusTag32_64(tagIndex, tagValue);
            else
                AdvVer2_FrameAddStatusTag32_32(tagIndex, tagValue);
        }

        public static void FrameAddStatusTag64(uint tagIndex, ulong tagValue)
        {
            if (!CrossPlatform.IsWindows)
                AdvVer2_FrameAddStatusTag64_Unix(tagIndex, tagValue);
            else if (Is64Bit())
                AdvVer2_FrameAddStatusTag64_64(tagIndex, tagValue);
            else
                AdvVer2_FrameAddStatusTag64_32(tagIndex, tagValue);
        }

        public static void FrameAddImage(byte layoutId, ushort[] pixels, byte pixelsBpp)
        {
            var rv = 0;
            if (!CrossPlatform.IsWindows)
                rv = AdvVer2_FrameAddImage_Unix(layoutId, pixels, pixelsBpp);
            else if (Is64Bit())
                rv = AdvVer2_FrameAddImage_64(layoutId, pixels, pixelsBpp);
            else
                rv = AdvVer2_FrameAddImage_32(layoutId, pixels, pixelsBpp);

            if (rv != 0) throw new AdvLibException("AdvVer2_FrameAddImage failed!");
        }

        public static void FrameAddImageBytes(byte layoutId, byte[] pixels, byte pixelsBpp)
        {
            var rv = 0;
            if (!CrossPlatform.IsWindows)
                rv = AdvVer2_FrameAddImageBytes_Unix(layoutId, pixels, pixelsBpp);
            else if (Is64Bit())
                rv = AdvVer2_FrameAddImageBytes_64(layoutId, pixels, pixelsBpp);
            else
                rv = AdvVer2_FrameAddImageBytes_32(layoutId, pixels, pixelsBpp);

            if (rv != 0) throw new AdvLibException("AdvVer2_FrameAddImageBytes failed!");
        }

        public static void EndFrame()
        {
            if (!CrossPlatform.IsWindows)
                AdvVer2_EndFrameUnix();
            else if (Is64Bit())
                AdvVer2_EndFrame64();
            else
                AdvVer2_EndFrame32();
        }

        public static void GetMainStreamInfo(ref int numFrames, ref long mainClockFrequency, ref int mainStreamAccuracy)
        {
            if (!CrossPlatform.IsWindows)
                AdvVer2_GetMainStreamInfoUnix(ref numFrames, ref mainClockFrequency, ref mainStreamAccuracy);
            else if (Is64Bit())
                AdvVer2_GetMainStreamInfo64(ref numFrames, ref mainClockFrequency, ref mainStreamAccuracy);
            else
                AdvVer2_GetMainStreamInfo32(ref numFrames, ref mainClockFrequency, ref mainStreamAccuracy);
        }

        public static void GetCalibrationStreamInfo(ref int numFrames, ref long mainClockFrequency, ref int mainStreamAccuracy)
        {
            if (!CrossPlatform.IsWindows)
                AdvVer2_GetCalibrationStreamInfoUnix(ref numFrames, ref mainClockFrequency, ref mainStreamAccuracy);
            else if (Is64Bit())
                AdvVer2_GetCalibrationStreamInfo64(ref numFrames, ref mainClockFrequency, ref mainStreamAccuracy);
            else
                AdvVer2_GetCalibrationStreamInfo32(ref numFrames, ref mainClockFrequency, ref mainStreamAccuracy);
        }

        public static uint[] GetFramePixels(int streamId, int frameNo, int width, int height)
        {
			uint[] pixels = new uint[width * height];
			var frameInfo = new AdvFrameInfoNative();
			byte[] systemError = new byte[256 * 16];

            if (!CrossPlatform.IsWindows)
                AdvVer2_GetFramePixelsUnix(streamId, frameNo, pixels, frameInfo, systemError);
            else if (Is64Bit())
                AdvVer2_GetFramePixels64(streamId, frameNo, pixels, frameInfo, systemError);
            else
                AdvVer2_GetFramePixels32(streamId, frameNo, pixels, frameInfo, systemError);

            return pixels;
        }
    }
}

