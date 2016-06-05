using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;

namespace Adv
{

#if !__linux__
    internal class NativeMethods
    {
        [DllImport("Kernel32.dll")]
        public static extern void QueryPerformanceCounter(ref long ticks);

        [DllImport("Kernel32.dll")]
        public static extern void QueryPerformanceFrequency(ref long frequency);
    }
#endif

    public enum ResponseMode
    {
        Unknown = 0,
        Logarithmic = 1,
        Linear = 2
    }

    public enum VideoStandard
    {
        Unknown = 0,
        PAL = 1,
        NTSC = 2,
        Digital = 3
    }

    public enum BayerPattern
    {
        RGGB, // Camera produces RGGB encoded Bayer array images 
        CMYG, // Camera produces CMYG encoded Bayer array images 
        CMYG2, //Camera produces CMYG2 encoded Bayer array images 
        LRGB // Camera produces Kodak TRUESENSE Bayer LRGB array images 
    }

    public class AdvRecorder
    {
        private const byte CFG_ADV_LAYOUT_1_UNCOMPRESSED = 1;
        private const byte CFG_ADV_LAYOUT_2_COMPRESSED = 2;
        private const byte CFG_ADV_LAYOUT_3_UNCOMPRESSED = 3;
        private const byte CFG_ADV_LAYOUT_4_UNCOMPRESSED = 4;
        private const byte CFG_ADV_LAYOUT_5_COMPRESSED = 5;

        private const byte MAIN_STREAM_ID = 0;
        private const byte CALIBRATION_STREAM_ID = 1;

        private int m_NumberRecordedFrames = 0;
        private int m_NumberDroppedFrames = 0;
        private long m_FirstRecordedFrameTimestamp = 0;
        private long m_PrevFrameEndTimestampAutoTicks = 0;

        public int NumberDroppedFrames
        {
            get { return m_NumberDroppedFrames; }
        }

        private Dictionary<string, uint> m_AdditionalStatusSectionTagIds = new Dictionary<string, uint>();

        private uint m_TAGID_UTCStartTimestamp;
        private uint m_TAGID_UTCEndTimestamp;
        private uint m_TAGID_SystemTime;
        private uint m_TAGID_TrackedSatellites;
        private uint m_TAGID_AlmanacStatus;
        private uint m_TAGID_AlmanacOffset;
        private uint m_TAGID_FixStatus;
        private uint m_TAGID_Gain;
        private uint m_TAGID_Shutter;
        private uint m_TAGID_Offset;
        private uint m_TAGID_VideoCameraFrameId;
        private uint m_TAGID_HardwareTimerFrameId;
        private uint m_TAGID_Gamma;
        private uint m_TAGID_UserCommand;
        private uint m_TAGID_SystemError;
        private uint m_TAGID_AoiTop;
        private uint m_TAGID_AoiLeft;
        private uint m_TAGID_AoiWidth;
        private uint m_TAGID_AoiHeight;

        public class AdvImageConfig
        {
            public ushort ImageWidth { get; private set; }
            public ushort ImageHeight { get; private set; }
            public byte CameraBitsPerPixel { get; private set; }
            public int? ImagePixelNormalValue { get; private set; }
            public int ImageBitsPerPixel { get; private set; }
            public bool ImageBigEndian { get; set; }
            public string ImageBayerPattern { get; set; }

            /// <summary>
            /// Sets the image configuration. Here are some examples: 
            /// 
            /// 8 bit camera, running with x4 frames on the fly integration, pixel values saved as 16 bit:
            /// Camera Bit Depth: 8 bit
            /// Image Bits Per Pixel: 16 bit
            /// Pixel Normal Value: 0x3FC (1020)
            /// 
            /// 14 bit camera with image saved as 16 bit:
            /// 
            /// Camera Bit Depth: 14 bit
            /// Image Bits Per Pixel: 16 bit
            /// Pixel Normal Value: null
            /// 
            /// 12 bit camera with image saved as 12 bit (2 pixels in 3 bytes):
            /// 
            /// Camera Bit Depth: 12 bit
            /// Image Bits Per Pixel: 12 bit
            /// Pixel Normal Value: null
            /// </summary>
            /// <param name="width">The width of the image in pixels.</param>
            /// <param name="height">The height of the image in pixels.</param>
            /// <param name="cameraBitDepth">The native camera bit depth.</param>
            /// <param name="imageBitsPerPixel">The bit depth of the pixel values saved in the image.</param>
            /// <param name="pixelNormalValue">The upper limit of the dynamic range of the image expressed as the maximum pixel value (or <b>null</b> if not applicable).</param>
            public void SetImageParameters(ushort width, ushort height, byte cameraBitDepth, byte imageBitsPerPixel, int? pixelNormalValue)
            {
                ImageWidth = width;
                ImageHeight = height;
                CameraBitsPerPixel = cameraBitDepth;
                ImageBitsPerPixel = imageBitsPerPixel;
                ImagePixelNormalValue = null;

                if (pixelNormalValue.HasValue && pixelNormalValue.Value != 0)
                {
                    int normValBitDepth = (int)Math.Ceiling(Math.Log(pixelNormalValue.Value, 2));
                    if (cameraBitDepth > normValBitDepth)
                        throw new AdvLibException(string.Format("pixelNormalValue {0} must be greater or equal to the cameraBitDepth ({1} bit) max value of {2}", pixelNormalValue, cameraBitDepth, Math.Pow(2, cameraBitDepth)));

                    ImagePixelNormalValue = pixelNormalValue;
                }
            }

            /// <summary>
            /// Sets the bayer pattern used by the CCD colour chip to one of the predefined values
            /// </summary>
            /// <param name="pattern">The bayer pattern to be set</param>
            public void SetBayerPattern(BayerPattern pattern)
            {
                ImageBayerPattern = pattern.ToString();
            }

            /// <summary>
            /// Sets the bayer pattern used by the CCD colour chip to a user defined value
            /// </summary>
            /// <param name="pattern">The bayer pattern to be set</param>
            public void SetBayerPattern(string pattern)
            {
                ImageBayerPattern = pattern;
            }
        }

        public class NtpServersInfo
        {
            internal List<string> Hosts = new List<string>();
            internal List<string> ServerDetails = new List<string>();

            public void AddServer(string host, string serverDetail)
            {
                Hosts.Add(host);
                ServerDetails.Add(serverDetail);
            }
        }

        public class AdvStatusSectionConfig
        {
            public bool RecordSystemTime { get; set; }
            public bool RecordTrackedSatellites { get; set; }
            public bool RecordAlmanacStatus { get; set; }
            public bool RecordAlmanacOffset { get; set; }
            public bool RecordFixStatus { get; set; }
            public bool RecordGain { get; set; }
            public bool RecordShutter { get; set; }
            public bool RecordCameraOffset { get; set; }
            public bool RecordGamma { get; set; }
            public bool RecordVideoCameraFrameId { get; set; }
            public bool RecordHardwareTimerFrameId { get; set; }
            public bool RecordUserCommands { get; set; }
            public bool RecordSystemErrors { get; set; }
            public bool RecordAOIPosition { get; set; }

            internal Dictionary<string, AdvTagType> AdditionalStatusTags = new Dictionary<string, AdvTagType>();

            public int AddDefineTag(string tagName, AdvTagType tagType)
            {
                if (AdditionalStatusTags.ContainsKey(tagName))
                    throw new ArgumentException("This tag name as been already added.");

                AdditionalStatusTags.Add(tagName, tagType);

                return AdditionalStatusTags.Count - 1;
            }
        }

        public class AdvStatusEntry
        {
            /// <summary>
            /// Lower accuracy system timestamp for the frame. Could be used as a backup time reference in case of a problem with the main timing hardware.
            /// </summary>
            public AdvTimeStamp SystemTime { get; set; }

            /// <summary>
            /// Number of tracked satellites when obtaining a UTC timestamp
            /// </summary>
            public byte TrackedSatellites { get; set; }

            /// <summary>
            /// The status of the almanac update when obtaining a UTC timestamp
            /// </summary>
            public AlmanacStatus AlmanacStatus { get; set; }

            /// <summary>
            /// The almanac offset in seconds that was added to the uncorrected time reported by the satellites in order to compute the UTC time
            /// </summary>
            public byte AlmanacOffset { get; set; }

            /// <summary>
            /// The status of the UTC time satellite fix
            /// </summary>
            public FixStatus FixStatus { get; set; }

            /// <summary>
            /// The gain of the camera in dB
            /// </summary>
            public float Gain { get; set; }

            /// <summary>
            /// Camera shutter speed in seconds
            /// </summary>
            public float Shutter { get; set; }

            /// <summary>
            /// The gamma correction applied to the produced image
            /// </summary>
            public float Gamma { get; set; }

            /// <summary>
            /// The offset in percentage applied to the produced image
            /// </summary>
            public float CameraOffset { get; set; }

            /// <summary>
            /// The id of the frame as labeled by the camera frame counter
            /// </summary>
            public ulong VideoCameraFrameId { get; set; }

            /// <summary>
            /// The id of the frame as labeled by the hardware timer (when used)
            /// </summary>
            public ulong HardwareTimerFrameId { get; set; }

            /// <summary>
            /// The user commands executed since the last recorded frame. Up to 16 lines, each line up to 255 characters.
            /// </summary>
            public string[] UserCommands { get; set; }

            /// <summary>
            /// System errors detected since the last recorded frame. Up to 16 lines, each line up to 255 characters.
            /// </summary>
            public string[] SystemErrors { get; set; }


            /// <summary>
            /// The top position of the top-left corner of the Area of Interest (AOI), also called Region of Interest (ROI)
            /// </summary>
            public ushort AoiTop { get; set; }

            /// <summary>
            /// The left position of the top-left corner of the Area of Interest (AOI), also called Region of Interest (ROI)
            /// </summary>
            public ushort AoiLeft { get; set; }

            /// <summary>
            /// The width of the Area of Interest (AOI), also called Region of Interest (ROI)
            /// </summary>
            public ushort AoiWidth { get; set; }

            /// <summary>
            /// The height of the Area of Interest (AOI), also called Region of Interest (ROI)
            /// </summary>
            public ushort AoiHeight { get; set; }

            /// <summary>
            /// The values of the additional tags. The value types must correspond to the defined tag type. Only the following
            /// .NET types are supported: byte, ushort, uint, ulong, float, string and string[]
            /// </summary>
            public object[] AdditionalStatusTags;
        }

        public class AdvFileMetaData
        {
            public AdvFileMetaData()
            {
                NativeFrameRate = double.NaN;
                EffectiveFrameRate = double.NaN;
                EffectivePixelSizeX = double.NaN;
                EffectivePixelSizeY = double.NaN;

                NtpServers = new NtpServersInfo();
            }

            internal Dictionary<string, string> UserMetaData = new Dictionary<string, string>();

            public string RecorderSoftwareName { get; set; }
            public string RecorderSoftwareVersion { get; set; }
            public string RecorderHardwareName { get; set; }
            public string RecorderHardwareVersion { get; set; }

            public string CameraModel { get; set; }
            public string CameraSensorInfo { get; set; }
            public int CameraBitPix { get; set; }

            public int BinningX { get; set; }
            public int BinningY { get; set; }

            /// <summary>
            /// Effective pixel size in µm (taking any binning into account)
            /// </summary>
            public double EffectivePixelSizeX { get; set; }

            /// <summary>
            /// Effective pixel size in µm (taking any binning into account)
            /// </summary>
            public double EffectivePixelSizeY { get; set; }

            public ResponseMode GainResponseMode { get; set; }
            public ResponseMode OffsetResponseMode { get; set; }

            public VideoStandard NativeVideoStandard { get; set; }
            public double NativeFrameRate { get; set; }
            public double EffectiveFrameRate { get; set; }

            public int OsdPositionTop { get; set; }
            public int OsdPositionBottom { get; set; }
            public int OsdPositionLeft { get; set; }
            public int OsdPositionRight { get; set; }

            public NtpServersInfo NtpServers { get; set; }

            public void AddUserTag(string tagName, string tagValue)
            {
                UserMetaData[tagName] = tagValue;
            }
        }

        public class AdvLocationData
        {
            public AdvLocationData()
            {
                Longitude = double.NaN;
                Latitude = double.NaN;
                Altitude = double.NaN;
            }

            public double Longitude { get; private set; }
            public double Latitude { get; private set; }
            public double Altitude { get; private set; }

            public string SiteName { get; set; }

            /// <summary>
            /// The position reference e.g. 'WGS84'
            /// </summary>
            public string PositionReference { get; private set; }

            /// <summary>
            /// An indication of the accuracy of the position. For GPS derived positions this could be HDOP value, for fixed coordinates this could be for example '+/-1m'
            /// </summary>
            public string PositionAccuracy { get; private set; }

            /// <summary>
            /// Sets the location.
            /// </summary>
            /// <param name="longitude">Longitude in degrees- positive to the east and negative to the west.</param>
            /// <param name="latitude">Latitude in degrees - positive to the north and negative to the south.</param>
            /// <param name="alitude">Altitude in meters.</param>
            /// <param name="positionReference">The position reference e.g. 'WGS84'</param>
            /// <param name="positionAccuracy">An indication of the accuracy of the position. For GPS derived positions this could be HDOP value, for fixed coordinates this could be for example '+/-1m'</param>
            public void SetLocation(double longitude, double latitude, double alitude, string positionReference = "WGS84", string positionAccuracy = null)
            {
                Longitude = longitude;
                Latitude = latitude;
                Altitude = alitude;
                PositionReference = positionReference;
                PositionAccuracy = PositionAccuracy;
            }
        }

        /// <summary>
        /// The image configuration to be used for the ADV file when StartRecordingNewFile() is called.
        /// </summary>
        public AdvImageConfig ImageConfig = new AdvImageConfig();

        public AdvFileMetaData FileMetaData = new AdvFileMetaData();

        public AdvLocationData LocationData = new AdvLocationData();

        public AdvStatusSectionConfig StatusSectionConfig = new AdvStatusSectionConfig();

        /// <summary>
        /// Creates new ADV file and gets it ready for recording 
        /// </summary>
        /// <param name="fileName"></param>
        public void StartRecordingNewFile(string fileName, bool createNew = false)
        {
            fileName = Path.GetFullPath(fileName);

            if (File.Exists(fileName))
            {
                if (createNew)
                    File.Delete(fileName);
                else
                    throw new AdvLibException(string.Format("{0} already exists.", fileName));
            }

            AdvLib.NewFile(fileName);

            AdvLib.SetTimingPrecision(1000, 1, 1000, 1);

            AdvLib.AddMainStreamTag("Name1", "Христо");
            AdvLib.AddMainStreamTag("Name2", "Frédéric");
            AdvLib.AddCalibrationStreamTag("Name1", "好的茶");

            if (string.IsNullOrEmpty(FileMetaData.RecorderSoftwareName)) throw new ArgumentException("FileMetaData.RecorderSoftwareName must be specified.");
            if (string.IsNullOrEmpty(FileMetaData.RecorderSoftwareVersion)) throw new ArgumentException("FileMetaData.RecorderSoftwareVersion must be specified.");
            if (string.IsNullOrEmpty(FileMetaData.CameraModel)) throw new ArgumentException("FileMetaData.CameraModel must be specified.");
            if (string.IsNullOrEmpty(FileMetaData.CameraSensorInfo)) throw new ArgumentException("FileMetaData.CameraSensorInfo must be specified.");

            AdvLib.AddFileTag("FSTF-TYPE", "ADV");
            AdvLib.AddFileTag("ADV-VERSION", "2");

            AdvLib.AddFileTag("RECORDER-SOFTWARE", FileMetaData.RecorderSoftwareName);
            AdvLib.AddFileTag("RECORDER-SOFTWARE-VERSION", FileMetaData.RecorderSoftwareVersion);
            if (!string.IsNullOrEmpty(FileMetaData.RecorderHardwareName)) AdvLib.AddFileTag("RECORDER-HARDWARE", FileMetaData.RecorderHardwareName);
            if (!string.IsNullOrEmpty(FileMetaData.RecorderHardwareVersion)) AdvLib.AddFileTag("RECORDER-HARDWARE-VERSION", FileMetaData.RecorderHardwareVersion);
            AdvLib.AddFileTag("ADVLIB-VERSION", "2.0");

            if (!double.IsNaN(LocationData.Longitude)) AdvLib.AddFileTag("LONGITUDE", LocationData.Longitude.ToString(CultureInfo.InvariantCulture));
            if (!double.IsNaN(LocationData.Latitude)) AdvLib.AddFileTag("LATITUDE", LocationData.Latitude.ToString(CultureInfo.InvariantCulture));
            if (!double.IsNaN(LocationData.Altitude)) AdvLib.AddFileTag("ALTITUDE", LocationData.Altitude.ToString(CultureInfo.InvariantCulture));
            if (!string.IsNullOrEmpty(LocationData.SiteName)) AdvLib.AddFileTag("POSITION-SITE-NAME", LocationData.SiteName);
            if (!string.IsNullOrEmpty(LocationData.PositionReference)) AdvLib.AddFileTag("POSITION-REFERENCE", LocationData.PositionReference);
            if (!string.IsNullOrEmpty(LocationData.PositionAccuracy)) AdvLib.AddFileTag("POSITION-ACCURACY", LocationData.PositionAccuracy);

            if (!string.IsNullOrEmpty(FileMetaData.CameraModel)) AdvLib.AddFileTag("CAMERA-MODEL", FileMetaData.CameraModel);
            if (!string.IsNullOrEmpty(FileMetaData.CameraSensorInfo)) AdvLib.AddFileTag("CAMERA-SENSOR-INFO", FileMetaData.CameraSensorInfo);
            if (FileMetaData.CameraBitPix > 0) AdvLib.AddFileTag("CAMERA-BITPIX", FileMetaData.CameraBitPix.ToString(CultureInfo.InvariantCulture));

            if (FileMetaData.BinningX > 0) AdvLib.AddFileTag("BINNING-X", FileMetaData.BinningX.ToString(CultureInfo.InvariantCulture));
            if (FileMetaData.BinningY > 0) AdvLib.AddFileTag("BINNING-Y", FileMetaData.BinningY.ToString(CultureInfo.InvariantCulture));
            if (!double.IsNaN(FileMetaData.EffectivePixelSizeX)) AdvLib.AddFileTag("EFFECTIVE-PIXEL-SIZE-X", FileMetaData.EffectivePixelSizeX.ToString(CultureInfo.InvariantCulture));
            if (!double.IsNaN(FileMetaData.EffectivePixelSizeY)) AdvLib.AddFileTag("EFFECTIVE-PIXEL-SIZE-Y", FileMetaData.EffectivePixelSizeY.ToString(CultureInfo.InvariantCulture));

            if (FileMetaData.GainResponseMode != ResponseMode.Unknown) AdvLib.AddFileTag("RESPONSE-MODE-GAIN", FileMetaData.GainResponseMode.ToString());
            if (FileMetaData.OffsetResponseMode != ResponseMode.Unknown) AdvLib.AddFileTag("RESPONSE-MODE-OFFSET", FileMetaData.OffsetResponseMode.ToString());

            if (FileMetaData.NativeVideoStandard != VideoStandard.Unknown) AdvLib.AddFileTag("NATIVE-VIDEO-STANDARD", FileMetaData.NativeVideoStandard.ToString());
            if (!double.IsNaN(FileMetaData.NativeFrameRate)) AdvLib.AddFileTag("NATIVE-FRAME-RATE", FileMetaData.NativeFrameRate.ToString(CultureInfo.InvariantCulture));
            if (!double.IsNaN(FileMetaData.EffectiveFrameRate)) AdvLib.AddFileTag("EFFECTIVE-FRAME-RATE", FileMetaData.EffectiveFrameRate.ToString(CultureInfo.InvariantCulture));

            if (FileMetaData.OsdPositionTop != 0) AdvLib.AddFileTag("OSD-POSITION-TOP", FileMetaData.OsdPositionTop.ToString());
            if (FileMetaData.OsdPositionLeft != 0) AdvLib.AddFileTag("OSD-POSITION-LEFT", FileMetaData.OsdPositionLeft.ToString());
            if (FileMetaData.OsdPositionBottom != 0) AdvLib.AddFileTag("OSD-POSITION-BOTTOM", FileMetaData.OsdPositionBottom.ToString());
            if (FileMetaData.OsdPositionRight != 0) AdvLib.AddFileTag("OSD-POSITION-RIGHT", FileMetaData.OsdPositionRight.ToString());

            if (FileMetaData.NtpServers.Hosts.Count > 0)
            {
                AdvLib.AddFileTag("NTP-SERVER-LIST", string.Join(";", FileMetaData.NtpServers.Hosts.ToArray()));
                AdvLib.AddFileTag("NTP-SERVER-DETAILS", string.Join(";", FileMetaData.NtpServers.ServerDetails.ToArray()));
            }

            foreach (string key in FileMetaData.UserMetaData.Keys)
            {
                AdvLib.AddFileTag(key, FileMetaData.UserMetaData[key]);
            }

            AdvLib.DefineImageSection(ImageConfig.ImageWidth, ImageConfig.ImageHeight, ImageConfig.CameraBitsPerPixel);
            AdvLib.AddOrUpdateImageSectionTag("IMAGE-BYTE-ORDER", ImageConfig.ImageBigEndian ? "BIG-ENDIAN" : "LITTLE-ENDIAN");
            AdvLib.AddOrUpdateImageSectionTag("IMAGE-BITPIX", ImageConfig.ImageBitsPerPixel.ToString(CultureInfo.InvariantCulture));
            if (ImageConfig.ImagePixelNormalValue.HasValue) AdvLib.AddOrUpdateImageSectionTag("IMAGE-PIX-NORM-VAL", ImageConfig.ImagePixelNormalValue.Value.ToString(CultureInfo.InvariantCulture));
            if (!string.IsNullOrEmpty(ImageConfig.ImageBayerPattern)) AdvLib.AddOrUpdateImageSectionTag("IMAGE-BAYER-PATTERN", ImageConfig.ImageBayerPattern);

            AdvLib.DefineImageLayout(CFG_ADV_LAYOUT_1_UNCOMPRESSED, "FULL-IMAGE-RAW", "UNCOMPRESSED", 16, 0, null);
            AdvLib.DefineImageLayout(CFG_ADV_LAYOUT_2_COMPRESSED, "FULL-IMAGE-RAW", "LAGARITH16", 16, 0, null);
            AdvLib.DefineImageLayout(CFG_ADV_LAYOUT_3_UNCOMPRESSED, "FULL-IMAGE-RAW", "UNCOMPRESSED", 12, 0, null);
            AdvLib.DefineImageLayout(CFG_ADV_LAYOUT_4_UNCOMPRESSED, "FULL-IMAGE-RAW", "UNCOMPRESSED", 8, 0, null);
            AdvLib.DefineImageLayout(CFG_ADV_LAYOUT_5_COMPRESSED, "FULL-IMAGE-RAW", "QUICKLZ", 8, 0, null);

            //SystemTime - The system clock reading at the time the frame has been recorded in the file (as a backup time)

            //TrackedSatellites - Number of GPS satellites tracked at the moment when the frame was timed by the GPS-based hardware timer (if applicable) 
            //AlmanacStatus - The almanac status (if applicable). Suggested values: Uncertain or Good
            //AlmanacOffset - The determined almanac offset (and applied to the timestamp) by the GPS-based hardware timer (if applicable)
            //FixStatus - P-Fix, G-Fix, No fix or No fix but Certain (if applicable)

            //Gamma - Current gamma applied by the camera
            //Gain - Current gain applied by the camera (in dB)
            //Shutter - Current camera shutter speed (in seconds)
            //Offset - The offset as human readable value (e.g. '5.4 %' or '12.34')

            //VideoCameraFrameId - A frame counter provided by the video camera (if supported)
            //HardwareTimerFrameId - A frame counter provided by the hardware timer (if supported)

            //UserCommand - A list of commands issued by the user during the generation of the current video frame. The commands are saved as free text (for example: Changed gain)
            //SystemError - A list of errors that were detected by the system during the generation of the current video frame. 


            m_TAGID_UTCStartTimestamp = AdvLib.DefineStatusSectionTag("UTCStartTimeStamp", AdvTagType.ULong64);
            m_TAGID_UTCEndTimestamp = AdvLib.DefineStatusSectionTag("UTCEndTimeStamp", AdvTagType.ULong64);

            if (StatusSectionConfig.RecordSystemTime) m_TAGID_SystemTime = AdvLib.DefineStatusSectionTag("SystemTime", AdvTagType.ULong64);

            if (StatusSectionConfig.RecordTrackedSatellites) m_TAGID_TrackedSatellites = AdvLib.DefineStatusSectionTag("TrackedSatellites", AdvTagType.UInt8);
            if (StatusSectionConfig.RecordAlmanacStatus) m_TAGID_AlmanacStatus = AdvLib.DefineStatusSectionTag("AlmanacStatus", AdvTagType.UInt8);
            if (StatusSectionConfig.RecordAlmanacOffset) m_TAGID_AlmanacOffset = AdvLib.DefineStatusSectionTag("AlmanacOffset", AdvTagType.UInt8);
            if (StatusSectionConfig.RecordFixStatus) m_TAGID_FixStatus = AdvLib.DefineStatusSectionTag("FixStatus", AdvTagType.UInt8);
            if (StatusSectionConfig.RecordGamma) m_TAGID_Gamma = AdvLib.DefineStatusSectionTag("Gamma", AdvTagType.Real);
            if (StatusSectionConfig.RecordGain) m_TAGID_Gain = AdvLib.DefineStatusSectionTag("Gain", AdvTagType.Real);
            if (StatusSectionConfig.RecordShutter) m_TAGID_Shutter = AdvLib.DefineStatusSectionTag("Shutter", AdvTagType.Real);
            if (StatusSectionConfig.RecordCameraOffset) m_TAGID_Offset = AdvLib.DefineStatusSectionTag("Offset", AdvTagType.Real);

            if (StatusSectionConfig.RecordAOIPosition)
            {
                m_TAGID_AoiTop = AdvLib.DefineStatusSectionTag("AOI-TOP", AdvTagType.UInt16);
                m_TAGID_AoiLeft = AdvLib.DefineStatusSectionTag("AOI-LEFT", AdvTagType.UInt16);
                m_TAGID_AoiWidth = AdvLib.DefineStatusSectionTag("AOI-WIDTH", AdvTagType.UInt16);
                m_TAGID_AoiHeight = AdvLib.DefineStatusSectionTag("AOI-HEIGHT", AdvTagType.UInt16);
            }

            if (StatusSectionConfig.RecordVideoCameraFrameId) m_TAGID_VideoCameraFrameId = AdvLib.DefineStatusSectionTag("VideoCameraFrameId", AdvTagType.ULong64);
            if (StatusSectionConfig.RecordHardwareTimerFrameId) m_TAGID_HardwareTimerFrameId = AdvLib.DefineStatusSectionTag("HardwareTimerFrameId", AdvTagType.ULong64);
            if (StatusSectionConfig.RecordUserCommands) m_TAGID_UserCommand = AdvLib.DefineStatusSectionTag("UserCommand", AdvTagType.List16OfAnsiString255);
            if (StatusSectionConfig.RecordSystemErrors) m_TAGID_SystemError = AdvLib.DefineStatusSectionTag("SystemError", AdvTagType.List16OfAnsiString255);

            m_AdditionalStatusSectionTagIds.Clear();

            if (StatusSectionConfig.AdditionalStatusTags.Count > 0)
            {
                foreach (string tagName in StatusSectionConfig.AdditionalStatusTags.Keys)
                {
                    uint tagId = AdvLib.DefineStatusSectionTag(tagName, StatusSectionConfig.AdditionalStatusTags[tagName]);
                    m_AdditionalStatusSectionTagIds.Add(tagName, tagId);
                }
            }

            m_NumberRecordedFrames = 0;
            m_NumberDroppedFrames = 0;
            m_FirstRecordedFrameTimestamp = 0;
            m_PrevFrameEndTimestampAutoTicks = 0;

            if (!AdvLib.BeginFrame(0, 0, 0, 0))
                throw new AdvLibException(string.Format("Cannot start recording '{0}'", fileName));
        }

        /// <summary>
        /// Closes the AVD file and stops any recording to it.
        /// </summary>
        public void FinishRecording()
        {
            AdvLib.EndFile();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="streamId"></param>
        /// <param name="pixels"></param>
        /// <param name="compressIfPossible"></param>
        /// <param name="startClockTicks"></param>
        /// <param name="endClockTicks"></param>
        /// <param name="startUtcTimeStamp"></param>
        /// <param name="endUtcTimeStamp"></param>
        /// <param name="metadata"></param>
        /// <param name="imageData"></param>
        public void AddFrame(byte streamId, ushort[] pixels, bool compressIfPossible, long startClockTicks, long endClockTicks, AdvTimeStamp startUtcTimeStamp, AdvTimeStamp endUtcTimeStamp, AdvStatusEntry metadata)
        {
            BeginVideoFrame(streamId, startClockTicks, endClockTicks, startUtcTimeStamp, endUtcTimeStamp, metadata);

            byte layoutIdForCurrentFramerate = GetImageLayoutId(compressIfPossible);

            AdvLib.FrameAddImage(layoutIdForCurrentFramerate, pixels, 16);

            AdvLib.EndFrame();
        }

        public void AddVideoFrame(ushort[] pixels, bool compressIfPossible, AdvTimeStamp startUtcTimeStamp, AdvTimeStamp endUtcTimeStamp, AdvStatusEntry metadata)
        {
            long ticks = 0;
#if !__linux__	
            NativeMethods.QueryPerformanceCounter(ref ticks);
#endif

            AddFrame(MAIN_STREAM_ID, pixels, compressIfPossible, ticks, m_PrevFrameEndTimestampAutoTicks, startUtcTimeStamp, endUtcTimeStamp, metadata);

            m_PrevFrameEndTimestampAutoTicks = ticks;
        }

        public void AddVideoFrame(ushort[] pixels, bool compressIfPossible, AdvStatusEntry metadata)
        {
            AddVideoFrame(pixels, compressIfPossible, new AdvTimeStamp(), new AdvTimeStamp(), metadata);
        }

        public void AddCalibrationFrame(ushort[] pixels, bool compressIfPossible, AdvTimeStamp startUtcTimeStamp, AdvTimeStamp endUtcTimeStamp, AdvStatusEntry metadata)
        {
            long ticks = 0;
#if !__linux__	
            NativeMethods.QueryPerformanceCounter(ref ticks);
#endif
            AddFrame(CALIBRATION_STREAM_ID, pixels, compressIfPossible, ticks, m_PrevFrameEndTimestampAutoTicks, startUtcTimeStamp, endUtcTimeStamp, metadata);

            m_PrevFrameEndTimestampAutoTicks = ticks;
        }

        public void AddCalibrationFrame(ushort[] pixels, bool compressIfPossible, AdvStatusEntry metadata)
        {
            AddCalibrationFrame(pixels, compressIfPossible, new AdvTimeStamp(), new AdvTimeStamp(), metadata);
        }

        public void AddFrame(byte streamId, byte[] pixels, bool compressIfPossible, long startClockTicks, long endClockTicks, AdvTimeStamp startUtcTimeStamp, AdvTimeStamp endUtcTimeStamp, AdvStatusEntry metadata, AdvImageData imageData)
        {
            if (ImageConfig.ImageBitsPerPixel == 12 && imageData != AdvImageData.PixelData12Bit)
                throw new InvalidOperationException("12bit pixel data can be only saved as 12bit byte array (2 pixels saved in 3 bytes)");

            BeginVideoFrame(streamId, startClockTicks, endClockTicks, startUtcTimeStamp, endUtcTimeStamp, metadata);

            byte layoutIdForCurrentFramerate = GetImageLayoutId(compressIfPossible);

            AdvLib.FrameAddImageBytes(layoutIdForCurrentFramerate, pixels, 16);

            AdvLib.EndFrame();
        }

        public void AddVideoFrame(byte[] pixels, bool compressIfPossible, AdvTimeStamp startUtcTimeStamp, AdvTimeStamp endUtcTimeStamp, AdvStatusEntry metadata, AdvImageData imageData)
        {
            long ticks = 0;
#if !__linux__	
			NativeMethods.QueryPerformanceCounter(ref ticks);
#endif

            AddFrame(MAIN_STREAM_ID, pixels, compressIfPossible, ticks, m_PrevFrameEndTimestampAutoTicks, startUtcTimeStamp, endUtcTimeStamp, metadata, imageData);

            m_PrevFrameEndTimestampAutoTicks = ticks;
        }

        public void AddVideoFrame(byte[] pixels, bool compressIfPossible, AdvStatusEntry metadata, AdvImageData imageData)
        {
            AddVideoFrame(pixels, compressIfPossible, new AdvTimeStamp(), new AdvTimeStamp(), metadata, imageData);
        }

        public void AddCalibrationFrame(byte[] pixels, bool compressIfPossible, AdvTimeStamp startUtcTimeStamp, AdvTimeStamp endUtcTimeStamp, AdvStatusEntry metadata, AdvImageData imageData)
        {
            long ticks = 0;
#if !__linux__
			NativeMethods.QueryPerformanceCounter(ref ticks);
#endif

            AddFrame(CALIBRATION_STREAM_ID, pixels, compressIfPossible, ticks, m_PrevFrameEndTimestampAutoTicks, startUtcTimeStamp, endUtcTimeStamp, metadata, imageData);

            m_PrevFrameEndTimestampAutoTicks = ticks;
        }

        public void AddCalibrationFrame(byte[] pixels, bool compressIfPossible, AdvStatusEntry metadata, AdvImageData imageData)
        {
            AddCalibrationFrame(pixels, compressIfPossible, new AdvTimeStamp(), new AdvTimeStamp(), metadata, imageData);
        }

        private byte GetImageLayoutId(bool useCompression)
        {
            if (ImageConfig.ImageBitsPerPixel <= 8)
            {
                return useCompression
                           ? CFG_ADV_LAYOUT_5_COMPRESSED /* "FULL-IMAGE-RAW", "QUICKLZ", 8, 0 */
                           : CFG_ADV_LAYOUT_4_UNCOMPRESSED; /* "FULL-IMAGE-RAW", "UNCOMPRESSED", 8 */
            }
            else if (ImageConfig.ImageBitsPerPixel == 12)
            {
                return CFG_ADV_LAYOUT_3_UNCOMPRESSED; /* "FULL-IMAGE-RAW", "UNCOMPRESSED", 12 */
            }
            else
            {
                return useCompression
                           ? CFG_ADV_LAYOUT_2_COMPRESSED /* "FULL-IMAGE-RAW", "LAGARITH16", 16, 0 */
                           : CFG_ADV_LAYOUT_1_UNCOMPRESSED; /* "FULL-IMAGE-RAW", "UNCOMPRESSED", 16 */
            }
        }

        private void BeginVideoFrame(byte streamId, long startClockTicks, long endClockTicks, AdvTimeStamp startUtcTimeStamp, AdvTimeStamp endUtcTimeStamp, AdvStatusEntry metadata)
        {
            long elapsedTicks = 0; // since the first recorded frame was taken
            if (m_NumberRecordedFrames > 0 && m_FirstRecordedFrameTimestamp != 0)
            {
                elapsedTicks = startClockTicks - m_FirstRecordedFrameTimestamp;
            }
            else if (m_NumberRecordedFrames == 0)
            {
                m_FirstRecordedFrameTimestamp = startClockTicks;
            }

            bool frameStartedOk = AdvLib.BeginFrame(streamId, startClockTicks, endClockTicks, elapsedTicks > 0L ? elapsedTicks : 0L);
            if (!frameStartedOk)
            {
                // If we can't add the first frame, this may be a file creation issue; otherwise increase the dropped frames counter
                if (m_NumberRecordedFrames > 0)
                    m_NumberDroppedFrames++;
                return;
            }

            if (startUtcTimeStamp.MillisecondsAfterAdvZeroEpoch > 0) AdvLib.FrameAddStatusTag64(m_TAGID_UTCStartTimestamp, (ulong)startUtcTimeStamp.MillisecondsAfterAdvZeroEpoch);
            if (endUtcTimeStamp.MillisecondsAfterAdvZeroEpoch > 0) AdvLib.FrameAddStatusTag64(m_TAGID_UTCEndTimestamp, (ulong)endUtcTimeStamp.MillisecondsAfterAdvZeroEpoch);

            if (StatusSectionConfig.RecordSystemTime)
                AdvLib.FrameAddStatusTag64(m_TAGID_SystemTime,
                                                   metadata.SystemTime.MillisecondsAfterAdvZeroEpoch > 0
                                                       ? (ulong)metadata.SystemTime.MillisecondsAfterAdvZeroEpoch
                                                       : 0);

            if (StatusSectionConfig.RecordTrackedSatellites) AdvLib.FrameAddStatusTagUInt8(m_TAGID_TrackedSatellites, metadata.TrackedSatellites);
            if (StatusSectionConfig.RecordAlmanacStatus) AdvLib.FrameAddStatusTagUInt8(m_TAGID_AlmanacStatus, (byte)metadata.AlmanacStatus);
            if (StatusSectionConfig.RecordAlmanacOffset) AdvLib.FrameAddStatusTagUInt8(m_TAGID_AlmanacOffset, metadata.AlmanacOffset);
            if (StatusSectionConfig.RecordFixStatus) AdvLib.FrameAddStatusTagUInt8(m_TAGID_FixStatus, (byte)metadata.FixStatus);
            if (StatusSectionConfig.RecordGain) AdvLib.FrameAddStatusTagReal(m_TAGID_Gain, metadata.Gain);
            if (StatusSectionConfig.RecordGamma) AdvLib.FrameAddStatusTagReal(m_TAGID_Gamma, metadata.Gamma);
            if (StatusSectionConfig.RecordShutter) AdvLib.FrameAddStatusTagReal(m_TAGID_Shutter, metadata.Shutter);
            if (StatusSectionConfig.RecordCameraOffset) AdvLib.FrameAddStatusTagReal(m_TAGID_Offset, metadata.CameraOffset);
            if (StatusSectionConfig.RecordVideoCameraFrameId) AdvLib.FrameAddStatusTag64(m_TAGID_VideoCameraFrameId, metadata.VideoCameraFrameId);

            if (StatusSectionConfig.RecordHardwareTimerFrameId) AdvLib.FrameAddStatusTag64(m_TAGID_HardwareTimerFrameId, metadata.HardwareTimerFrameId);
            if (StatusSectionConfig.RecordAOIPosition)
            {
                AdvLib.FrameAddStatusTag16(m_TAGID_AoiTop, metadata.AoiTop);
                AdvLib.FrameAddStatusTag16(m_TAGID_AoiLeft, metadata.AoiLeft);
                AdvLib.FrameAddStatusTag16(m_TAGID_AoiWidth, metadata.AoiWidth);
                AdvLib.FrameAddStatusTag16(m_TAGID_AoiHeight, metadata.AoiHeight);
            }

            if (StatusSectionConfig.RecordUserCommands && metadata.UserCommands != null)
            {
                for (int i = 0; i < Math.Min(16, metadata.UserCommands.Count()); i++)
                {
                    if (metadata.UserCommands[i] != null)
                    {
                        if (metadata.UserCommands[i].Length > 255)
                            AdvLib.FrameAddStatusTagMessage(m_TAGID_UserCommand, metadata.UserCommands[i].Substring(0, 255));
                        else
                            AdvLib.FrameAddStatusTagMessage(m_TAGID_UserCommand, metadata.UserCommands[i]);
                    }
                }
            }

            if (StatusSectionConfig.RecordSystemErrors && metadata.SystemErrors != null)
            {
                for (int i = 0; i < Math.Min(16, metadata.SystemErrors.Count()); i++)
                {
                    if (metadata.SystemErrors[i] != null)
                    {
                        if (metadata.SystemErrors[i].Length > 255)
                            AdvLib.FrameAddStatusTagMessage(m_TAGID_SystemError, metadata.SystemErrors[i].Substring(0, 255));
                        else
                            AdvLib.FrameAddStatusTagMessage(m_TAGID_SystemError, metadata.SystemErrors[i]);
                    }
                }
            }

            int additionalStatusTagId = -1;
            foreach (string tagName in StatusSectionConfig.AdditionalStatusTags.Keys)
            {
                uint tagId = m_AdditionalStatusSectionTagIds[tagName];
                additionalStatusTagId++;
                object statusTagValue = metadata.AdditionalStatusTags[additionalStatusTagId];

                switch (StatusSectionConfig.AdditionalStatusTags[tagName])
                {
                    case AdvTagType.UInt8:
                        AdvLib.FrameAddStatusTagUInt8(tagId, (byte)statusTagValue);
                        break;

                    case AdvTagType.UInt16:
                        AdvLib.FrameAddStatusTag16(tagId, (ushort)statusTagValue);
                        break;

                    case AdvTagType.UInt32:
                        AdvLib.FrameAddStatusTag32(tagId, (uint)statusTagValue);
                        break;

                    case AdvTagType.ULong64:
                        AdvLib.FrameAddStatusTag64(tagId, (ulong)statusTagValue);
                        break;

                    case AdvTagType.Real:
                        AdvLib.FrameAddStatusTagReal(tagId, (float)statusTagValue);
                        break;

                    case AdvTagType.AnsiString255:
                    case AdvTagType.UTF8String:
                        AdvLib.FrameAddStatusTagUTF8String(tagId, (string)statusTagValue);
                        break;

                    case AdvTagType.List16OfAnsiString255:
                    case AdvTagType.List16OfUTF8String:
                        string[] lines = (string[])statusTagValue;
                        if (lines != null)
                        {
                            for (int i = 0; i < Math.Min(16, lines.Count()); i++)
                            {
                                if (lines[i] != null)
                                {
                                    if (lines[i].Length > 255)
                                        AdvLib.FrameAddStatusTagMessage(tagId, lines[i].Substring(0, 255));
                                    else
                                        AdvLib.FrameAddStatusTagMessage(tagId, lines[i]);
                                }
                            }
                        }
                        break;
                }
            }
        }
    }
}
