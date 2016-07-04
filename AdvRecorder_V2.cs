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

    public enum PreferredCompression
    {
        Lagarith16,
        QuickLZ
    }

    public enum BayerPattern
    {
        MONOCHROME = 0,
        RGGB, // Camera produces RGGB encoded Bayer array images. Defined in ASCOM.
        CMYG, // Camera produces CMYG encoded Bayer array images. Defined in ASCOM. 
        CMYG2, //Camera produces CMYG2 encoded Bayer array images. Defined in ASCOM.
        LRGB, // Camera produces Kodak TRUESENSE Bayer LRGB array images. Defined in ASCOM. 
        GRBG, // Variant of RGGB. Defined for SER files.
        GBRG, // Variant of RGGB. Defined for SER files.
        YCMY, // Variant of CMY. Defined for SER files.
        YMCY, // Variant of CMY. Defined for SER files.
        MYYC, // Variant of CMY. Defined for SER files.
        RGB,
        BGR
    }

    public class AdvRecorder
    {
        private const byte CFG_ADV_LAYOUT_1_RAW_UNCOMPRESSED = 1;
        private const byte CFG_ADV_LAYOUT_2_RAW_COMPRESSED_LTH16 = 2;
        private const byte CFG_ADV_LAYOUT_3_RAW_COMPRESSED_QLZ = 3;
        private const byte CFG_ADV_LAYOUT_4_RAW_UNCOMPRESSED = 4;
        private const byte CFG_ADV_LAYOUT_5_RAW_UNCOMPRESSED = 5;
        private const byte CFG_ADV_LAYOUT_6_RAW_COMPRESSED_QLZ = 6;
        private const byte CFG_ADV_LAYOUT_7_RAW_COMPRESSED_LTH16 = 7;
        private const byte CFG_ADV_LAYOUT_8_PACKED12_UNCOMPRESSED = 8;
        private const byte CFG_ADV_LAYOUT_9_PACKED12_COMPRESSED_LTH16 = 9;
        private const byte CFG_ADV_LAYOUT_10_PACKED12_COMPRESSED_QLZ = 10;
        private const byte CFG_ADV_LAYOUT_11_COLOUR24_UNCOMPRESSED = 11;
        private const byte CFG_ADV_LAYOUT_12_PACKED24_COMPRESSED_QLZ = 12;
        private const byte CFG_ADV_LAYOUT_13_PACKED24_COMPRESSED_LTH16 = 13;

        private const byte MAIN_STREAM_ID = 0;
        private const byte CALIBRATION_STREAM_ID = 1;

        private static byte[] STREAM_IDS = new []{ MAIN_STREAM_ID, CALIBRATION_STREAM_ID };

        private int m_NumberRecordedFrames = 0;
        private int m_NumberDroppedFrames = 0;
        private long m_FirstRecordedFrameTimestamp = 0;
        private long? m_PrevFrameEndTimestampAutoTicks = 0;

        public int NumberDroppedFrames
        {
            get { return m_NumberDroppedFrames; }
        }

        private Dictionary<string, uint> m_AdditionalStatusSectionTagIds = new Dictionary<string, uint>();

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
        private uint m_TAGID_SystemError;

        public class AdvImageConfig
        {
            public ushort ImageWidth { get; private set; }
            public ushort ImageHeight { get; private set; }
            public int? ImagePixelMaxValue { get; private set; }
            public byte ImageBitsPerPixel { get; private set; }
            public bool ImageBigEndian { get; set; }
            public BayerPattern ImageBayerPattern { get; set; }

            /// <summary>
            /// Sets the image configuration. Here are some examples: 
            /// 
            /// 8 bit camera, running with x4 frames on the fly integration, pixel values saved as 16 bit:
            /// Image Bits Per Pixel: 16 bit
            /// Pixel Max Value: 0x3FC (1020)
            /// 
            /// 14 bit camera with image saved as 16 bit (unscaled):
            /// 
            /// Image Bits Per Pixel: 16 bit
            /// Pixel Max Value: 0x3FFF (16383)
            /// 
            /// 14 bit camera with image scaled to and saved as 16 bit:
            /// 
            /// Image Bits Per Pixel: 16 bit
            /// Pixel Max Value: null
            /// 
            /// 16 bit camera with image saved as 16 bit:
            /// 
            /// Image Bits Per Pixel: 16 bit
            /// Pixel Max Value: null
            ///            
            /// 12 bit camera with image saved as 12 bit (2 pixels in 3 bytes):
            /// 
            /// Image Bits Per Pixel: 12 bit
            /// Pixel Max Value: null
            /// </summary>
            /// <param name="width">The width of the image in pixels.</param>
            /// <param name="height">The height of the image in pixels.</param>
            /// <param name="imageBitsPerPixel">The bit depth of the pixel values saved in the image.</param>
            /// <param name="pixelMaxValue">The upper limit of the dynamic range of the image expressed as the maximum pixel value (or <b>null</b> if not applicable).</param>
            public void SetImageParameters(ushort width, ushort height, byte imageBitsPerPixel, int? pixelMaxValue)
            {
                ImageWidth = width;
                ImageHeight = height;
                ImageBitsPerPixel = imageBitsPerPixel;
                ImagePixelMaxValue = null;

                if (pixelMaxValue.HasValue && pixelMaxValue.Value != 0)
                {
                    int normValBitDepth = (int)Math.Ceiling(Math.Log(pixelMaxValue.Value, 2));
                    if (imageBitsPerPixel < normValBitDepth)
                        throw new AdvLibException(string.Format("pixelMaxValue {0} must be less or equal to the imageBitsPerPixel's ({1} bit) max value of {2}", pixelMaxValue, imageBitsPerPixel, Math.Pow(2, imageBitsPerPixel)));

                    ImagePixelMaxValue = pixelMaxValue;
                }
            }

            /// <summary>
            /// Sets the bayer pattern used by the CCD colour chip to one of the predefined values
            /// </summary>
            /// <param name="pattern">The bayer pattern to be set</param>
            public void SetBayerPattern(BayerPattern pattern)
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
            public bool RecordSystemErrors { get; set; }

            internal Dictionary<string, Adv2TagType> AdditionalStatusTags = new Dictionary<string, Adv2TagType>();

            public int AddDefineTag(string tagName, Adv2TagType tagType)
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
            /// The status of the UTC time satellite fix
            /// </summary>
            public FixStatus FixStatus { get; set; }


            public int AlmanacOffset { get; set; }


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
            public long VideoCameraFrameId { get; set; }

            /// <summary>
            /// The id of the frame as labeled by the hardware timer (when used)
            /// </summary>
            public long HardwareTimerFrameId { get; set; }

            /// <summary>
            /// System errors detected since the last recorded frame. 
            /// </summary>
            public string SystemErrors { get; set; }

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
            internal Dictionary<string, string> MainStreamMetaData = new Dictionary<string, string>();
            internal Dictionary<string, string> CalibrationStreamMetaData = new Dictionary<string, string>();

            public string RecorderSoftwareName { get; set; }
            public string RecorderSoftwareVersion { get; set; }
            public string RecorderHardwareName { get; set; }
            public string RecorderHardwareVersion { get; set; }

            public string CameraModel { get; set; }
            public string CameraSensorInfo { get; set; }

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

            public void AddMainStreamTag(string tagName, string tagValue)
            {
                MainStreamMetaData[tagName] = tagValue;
            }

            public void AddCalibrationStreamTag(string tagName, string tagValue)
            {
                CalibrationStreamMetaData[tagName] = tagValue;
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

        public enum AdvStream
        {
            MainStream = 0,
            CalibrationStream = 1
        }

        private bool m_BayerPatternIsRGBorGBR;
        private long m_MainClockFrequency;
        private int m_MainTicksTimingAccuracy;
        private Func<long> m_GetMainClockTicksCallback = null;

        private long m_CalibrationClockFrequency;
        private int m_CalibrationTicksTimingAccuracy;
        private Func<long> m_GetCalibrationClockTicksCallback = null;

        public void DefineCustomClock(AdvStream advStream, long clockFrequency, int clockAcuracy, Func<long> getClockTicksCallback)
        {
            if (advStream == AdvStream.MainStream)
            {
                m_MainClockFrequency = clockFrequency;
                m_MainTicksTimingAccuracy = clockAcuracy;
                m_GetMainClockTicksCallback = getClockTicksCallback;
            }
            else if (advStream == AdvStream.CalibrationStream)
            {
                m_CalibrationClockFrequency = clockFrequency;
                m_CalibrationTicksTimingAccuracy = clockAcuracy;
                m_GetCalibrationClockTicksCallback = getClockTicksCallback;
            }
        }

        /// <summary>
        /// Creates new ADV file and gets it ready for recording 
        /// </summary>
        /// <param name="fileName"></param>
        public void StartRecordingNewFile(string fileName, long utcTimestampAccuracyInNanoseconds, bool createNew = false)
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

            if (m_GetMainClockTicksCallback != null)
                AdvLib.DefineExternalClockForMainStream(m_MainClockFrequency, m_MainTicksTimingAccuracy);
            
            if (m_GetCalibrationClockTicksCallback != null)
                AdvLib.DefineExternalClockForCalibrationStream(m_CalibrationClockFrequency, m_CalibrationTicksTimingAccuracy);

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

            foreach (string key in FileMetaData.MainStreamMetaData.Keys)
                AdvLib.AddMainStreamTag(key, FileMetaData.MainStreamMetaData[key]);

            foreach (string key in FileMetaData.CalibrationStreamMetaData.Keys)
                AdvLib.AddCalibrationStreamTag(key, FileMetaData.CalibrationStreamMetaData[key]);

            foreach (string key in FileMetaData.UserMetaData.Keys)
                AdvLib.AddUserTag(key, FileMetaData.UserMetaData[key]);

            AdvLib.DefineImageSection(ImageConfig.ImageWidth, ImageConfig.ImageHeight, ImageConfig.ImageBitsPerPixel);
            AdvLib.DefineStatusSection(utcTimestampAccuracyInNanoseconds);
            AdvLib.AddOrUpdateImageSectionTag("IMAGE-BYTE-ORDER", ImageConfig.ImageBigEndian ? "BIG-ENDIAN" : "LITTLE-ENDIAN");
            AdvLib.AddOrUpdateImageSectionTag("IMAGE-BITPIX", ImageConfig.ImageBitsPerPixel.ToString(CultureInfo.InvariantCulture));
            if (ImageConfig.ImagePixelMaxValue.HasValue) AdvLib.AddOrUpdateImageSectionTag("IMAGE-MAX-PIXEL-VALUE", ImageConfig.ImagePixelMaxValue.Value.ToString(CultureInfo.InvariantCulture));
            if (ImageConfig.ImageBayerPattern > 0)
            {
                AdvLib.AddOrUpdateImageSectionTag("IMAGE-BAYER-PATTERN", ImageConfig.ImageBayerPattern.ToString());
                m_BayerPatternIsRGBorGBR = ImageConfig.ImageBayerPattern == BayerPattern.BGR ||
                                           ImageConfig.ImageBayerPattern == BayerPattern.RGB;
            }

            AdvLib.DefineImageLayout(CFG_ADV_LAYOUT_1_RAW_UNCOMPRESSED, "FULL-IMAGE-RAW", "UNCOMPRESSED", 16);
            AdvLib.DefineImageLayout(CFG_ADV_LAYOUT_2_RAW_COMPRESSED_LTH16, "FULL-IMAGE-RAW", "LAGARITH16", 16);
            AdvLib.DefineImageLayout(CFG_ADV_LAYOUT_3_RAW_COMPRESSED_QLZ, "FULL-IMAGE-RAW", "QUICKLZ", 16);
            AdvLib.DefineImageLayout(CFG_ADV_LAYOUT_4_RAW_UNCOMPRESSED, "FULL-IMAGE-RAW", "UNCOMPRESSED", 12);
            AdvLib.DefineImageLayout(CFG_ADV_LAYOUT_5_RAW_UNCOMPRESSED, "FULL-IMAGE-RAW", "UNCOMPRESSED", 8);
            AdvLib.DefineImageLayout(CFG_ADV_LAYOUT_6_RAW_COMPRESSED_QLZ, "FULL-IMAGE-RAW", "QUICKLZ", 8);
            AdvLib.DefineImageLayout(CFG_ADV_LAYOUT_7_RAW_COMPRESSED_LTH16, "FULL-IMAGE-RAW", "LAGARITH16", 8);
            AdvLib.DefineImageLayout(CFG_ADV_LAYOUT_8_PACKED12_UNCOMPRESSED, "12BIT-IMAGE-PACKED", "UNCOMPRESSED", 12);
            AdvLib.DefineImageLayout(CFG_ADV_LAYOUT_9_PACKED12_COMPRESSED_LTH16, "12BIT-IMAGE-PACKED", "LAGARITH16", 12);
            AdvLib.DefineImageLayout(CFG_ADV_LAYOUT_10_PACKED12_COMPRESSED_QLZ, "12BIT-IMAGE-PACKED", "QUICKLZ", 12);
            AdvLib.DefineImageLayout(CFG_ADV_LAYOUT_11_COLOUR24_UNCOMPRESSED, "8BIT-COLOR-IMAGE", "UNCOMPRESSED", 8);
            AdvLib.DefineImageLayout(CFG_ADV_LAYOUT_12_PACKED24_COMPRESSED_QLZ, "8BIT-COLOR-IMAGE", "QUICKLZ", 8);
            AdvLib.DefineImageLayout(CFG_ADV_LAYOUT_13_PACKED24_COMPRESSED_LTH16, "8BIT-COLOR-IMAGE", "LAGARITH16", 8);

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

            if (StatusSectionConfig.RecordSystemTime) m_TAGID_SystemTime = AdvLib.DefineStatusSectionTag("SystemTime", Adv2TagType.Long64);

            if (StatusSectionConfig.RecordTrackedSatellites) m_TAGID_TrackedSatellites = AdvLib.DefineStatusSectionTag("TrackedSatellites", Adv2TagType.Int8);
            if (StatusSectionConfig.RecordAlmanacStatus) m_TAGID_AlmanacStatus = AdvLib.DefineStatusSectionTag("AlmanacStatus", Adv2TagType.Int8);
            if (StatusSectionConfig.RecordAlmanacOffset) m_TAGID_AlmanacOffset = AdvLib.DefineStatusSectionTag("AlmanacOffset", Adv2TagType.Int8);
            if (StatusSectionConfig.RecordFixStatus) m_TAGID_FixStatus = AdvLib.DefineStatusSectionTag("SatelliteFixStatus", Adv2TagType.Int8);
            if (StatusSectionConfig.RecordGamma) m_TAGID_Gamma = AdvLib.DefineStatusSectionTag("Gamma", Adv2TagType.Real);
            if (StatusSectionConfig.RecordGain) m_TAGID_Gain = AdvLib.DefineStatusSectionTag("Gain", Adv2TagType.Real);
            if (StatusSectionConfig.RecordShutter) m_TAGID_Shutter = AdvLib.DefineStatusSectionTag("Shutter", Adv2TagType.Real);
            if (StatusSectionConfig.RecordCameraOffset) m_TAGID_Offset = AdvLib.DefineStatusSectionTag("Offset", Adv2TagType.Real);

            if (StatusSectionConfig.RecordVideoCameraFrameId) m_TAGID_VideoCameraFrameId = AdvLib.DefineStatusSectionTag("VideoCameraFrameId", Adv2TagType.Long64);
            if (StatusSectionConfig.RecordHardwareTimerFrameId) m_TAGID_HardwareTimerFrameId = AdvLib.DefineStatusSectionTag("HardwareTimerFrameId", Adv2TagType.Long64);
            if (StatusSectionConfig.RecordSystemErrors) m_TAGID_SystemError = AdvLib.DefineStatusSectionTag("Error", Adv2TagType.UTF8String);

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
        }

        /// <summary>
        /// Closes the AVD file and stops any recording to it.
        /// </summary>
        public void FinishRecording()
        {
            AdvLib.EndFile();
        }

        private void AddFrame(AdvStream advStream, ushort[] pixels, bool compressIfPossible, PreferredCompression? preferredCompression, long? startClockTicks, long? endClockTicks, AdvTimeStamp startUtcTimeStamp, AdvTimeStamp endUtcTimeStamp, AdvStatusEntry metadata, AdvImageData imageData)
        {
            if (m_BayerPatternIsRGBorGBR)
                throw new InvalidOperationException("To add a 24bit colour image in an RGB or BGR ImageBayerPattern the pixel data must be passed as a byte[].");

            BeginVideoFrame(advStream, startClockTicks, endClockTicks, startUtcTimeStamp, endUtcTimeStamp, metadata);

            byte layoutIdForCurrentFramerate = GetImageLayoutId(compressIfPossible, true, imageData, preferredCompression);

            if (layoutIdForCurrentFramerate == CFG_ADV_LAYOUT_11_COLOUR24_UNCOMPRESSED || layoutIdForCurrentFramerate == CFG_ADV_LAYOUT_12_PACKED24_COMPRESSED_QLZ)
                throw new InvalidOperationException(String.Format("This image layout ({0}) cannot be used when pixels are passed as ushort[]", layoutIdForCurrentFramerate));

            AdvLib.FrameAddImage(layoutIdForCurrentFramerate, pixels, 16);

            AdvLib.EndFrame();
        }

        public void AddVideoFrame(ushort[] pixels, bool compressIfPossible, PreferredCompression? preferredCompression, AdvTimeStamp startUtcTimeStamp, AdvTimeStamp endUtcTimeStamp, AdvStatusEntry metadata, AdvImageData imageData)
        {
            long? ticks = null;
            if (m_GetMainClockTicksCallback != null) ticks = m_GetMainClockTicksCallback();

            AddFrame(AdvStream.MainStream, pixels, compressIfPossible, preferredCompression, m_PrevFrameEndTimestampAutoTicks, ticks, startUtcTimeStamp, endUtcTimeStamp, metadata, imageData);

            m_PrevFrameEndTimestampAutoTicks = ticks;
        }

        public void AddVideoFrame(ushort[] pixels, bool compressIfPossible, PreferredCompression? preferredCompression, AdvStatusEntry metadata, AdvImageData imageData)
        {
            AddVideoFrame(pixels, compressIfPossible, preferredCompression, new AdvTimeStamp(), new AdvTimeStamp(), metadata, imageData);
        }

        public void AddCalibrationFrame(ushort[] pixels, bool compressIfPossible, PreferredCompression? preferredCompression, AdvTimeStamp startUtcTimeStamp, AdvTimeStamp endUtcTimeStamp, AdvStatusEntry metadata, AdvImageData imageData)
        {
            long? ticks = null;
            if (m_GetCalibrationClockTicksCallback != null) ticks = m_GetCalibrationClockTicksCallback();

            AddFrame(AdvStream.CalibrationStream, pixels, compressIfPossible, preferredCompression, m_PrevFrameEndTimestampAutoTicks, ticks, startUtcTimeStamp, endUtcTimeStamp, metadata, imageData);

            m_PrevFrameEndTimestampAutoTicks = ticks;
        }

        public void AddCalibrationFrame(ushort[] pixels, bool compressIfPossible, PreferredCompression? preferredCompression, AdvStatusEntry metadata, AdvImageData imageData)
        {
            AddCalibrationFrame(pixels, compressIfPossible, preferredCompression, new AdvTimeStamp(), new AdvTimeStamp(), metadata, imageData);
        }

        private void AddFrame(AdvStream advStream, byte[] pixels, bool compressIfPossible, PreferredCompression? preferredCompression, long? startClockTicks, long? endClockTicks, AdvTimeStamp startUtcTimeStamp, AdvTimeStamp endUtcTimeStamp, AdvStatusEntry metadata, AdvImageData imageData)
        {
            if (ImageConfig.ImageBitsPerPixel == 12 && imageData != AdvImageData.PixelData12Bit)
                throw new InvalidOperationException("12bit pixel data can be only saved as 12bit byte array (2 pixels saved in 3 bytes)");
            
            if (imageData == AdvImageData.PixelData24BitColor && !m_BayerPatternIsRGBorGBR)
                throw new InvalidOperationException("24bit colour pixel data can be only saved with an RGB or BGR ImageBayerPattern.");
            
            BeginVideoFrame(advStream, startClockTicks, endClockTicks, startUtcTimeStamp, endUtcTimeStamp, metadata);

            byte layoutIdForCurrentFramerate = GetImageLayoutId(compressIfPossible, false, imageData, preferredCompression);

            if (m_BayerPatternIsRGBorGBR && layoutIdForCurrentFramerate != CFG_ADV_LAYOUT_11_COLOUR24_UNCOMPRESSED && layoutIdForCurrentFramerate != CFG_ADV_LAYOUT_12_PACKED24_COMPRESSED_QLZ)
                throw new InvalidOperationException("When an RGB or BGR ImageBayerPattern is in use an 8BIT-COLOR-IMAGE image layout must be selected.");

            AdvLib.FrameAddImageBytes(layoutIdForCurrentFramerate, pixels, ImageConfig.ImageBitsPerPixel);

            AdvLib.EndFrame();
        }

        public void AddVideoFrame(byte[] pixels, bool compressIfPossible, PreferredCompression? preferredCompression, AdvTimeStamp startUtcTimeStamp, AdvTimeStamp endUtcTimeStamp, AdvStatusEntry metadata, AdvImageData imageData)
        {
            long? ticks = null;
            if (m_GetMainClockTicksCallback != null) ticks = m_GetMainClockTicksCallback();

            AddFrame(AdvStream.MainStream, pixels, compressIfPossible, preferredCompression, m_PrevFrameEndTimestampAutoTicks, ticks, startUtcTimeStamp, endUtcTimeStamp, metadata, imageData);

            m_PrevFrameEndTimestampAutoTicks = ticks;
        }

        public void AddVideoFrame(byte[] pixels, bool compressIfPossible, PreferredCompression? preferredCompression, AdvStatusEntry metadata, AdvImageData imageData)
        {
            AddVideoFrame(pixels, compressIfPossible, preferredCompression, new AdvTimeStamp(), new AdvTimeStamp(), metadata, imageData);
        }

        public void AddCalibrationFrame(byte[] pixels, bool compressIfPossible, PreferredCompression? preferredCompression, AdvTimeStamp startUtcTimeStamp, AdvTimeStamp endUtcTimeStamp, AdvStatusEntry metadata, AdvImageData imageData)
        {
            long? ticks = null;
            if (m_GetCalibrationClockTicksCallback != null) ticks = m_GetCalibrationClockTicksCallback();

            AddFrame(AdvStream.CalibrationStream, pixels, compressIfPossible, preferredCompression, m_PrevFrameEndTimestampAutoTicks, ticks, startUtcTimeStamp, endUtcTimeStamp, metadata, imageData);

            m_PrevFrameEndTimestampAutoTicks = ticks;
        }

        public void AddCalibrationFrame(byte[] pixels, bool compressIfPossible, PreferredCompression? preferredCompression, AdvStatusEntry metadata, AdvImageData imageData)
        {
            AddCalibrationFrame(pixels, compressIfPossible, preferredCompression, new AdvTimeStamp(), new AdvTimeStamp(), metadata, imageData);
        }

        private byte GetImageLayoutId(bool useCompression, bool inputAs16BitArray, AdvImageData imageData, PreferredCompression? preferredCompression)
        {
            if (inputAs16BitArray)
            {
                if (imageData != AdvImageData.PixelDepth16Bit) throw new NotSupportedException();

                if (ImageConfig.ImageBitsPerPixel == 12)
                {
                    // Input data come as 16bit pixels but actual bit depth is 12bit. We can use a 12bit packed layout    
                    return useCompression
                        ? (preferredCompression.HasValue && preferredCompression.Value == PreferredCompression.Lagarith16
                                ? CFG_ADV_LAYOUT_9_PACKED12_COMPRESSED_LTH16  /* "12BIT-IMAGE-PACKED", "QUICKLZ", 8, 0 */
                                : CFG_ADV_LAYOUT_10_PACKED12_COMPRESSED_QLZ   /* "12BIT-IMAGE-PACKED", "QUICKLZ", 8, 0 */)
                        : CFG_ADV_LAYOUT_8_PACKED12_UNCOMPRESSED; /* "12BIT-IMAGE-PACKED", "UNCOMPRESSED", 8 */
                }

                // When the input data is 16bit we always save as 16 bit regardless of the ImageBitsPerPixel
                return useCompression
                    ? (preferredCompression.HasValue && preferredCompression.Value == PreferredCompression.QuickLZ
                            ? CFG_ADV_LAYOUT_3_RAW_COMPRESSED_QLZ   /* "FULL-IMAGE-RAW", "QUICKLZ", 16, 0 */
                            : CFG_ADV_LAYOUT_2_RAW_COMPRESSED_LTH16 /* "FULL-IMAGE-RAW", "LAGARITH16", 16, 0 */)
                    : CFG_ADV_LAYOUT_1_RAW_UNCOMPRESSED; /* "FULL-IMAGE-RAW", "UNCOMPRESSED", 16 */
            }

            // When the input data is byte array we choose an image layout based on the ImageBitsPerPixel
            if (ImageConfig.ImageBitsPerPixel <= 8)
            {
                if (imageData != AdvImageData.PixelDepth8Bit) throw new NotSupportedException();

                return useCompression
                    ? (preferredCompression.HasValue && preferredCompression.Value == PreferredCompression.Lagarith16
                            ? CFG_ADV_LAYOUT_7_RAW_COMPRESSED_LTH16  /* "FULL-IMAGE-RAW", "QUICKLZ", 8, 0 */
                            : CFG_ADV_LAYOUT_6_RAW_COMPRESSED_QLZ    /* "FULL-IMAGE-RAW", "QUICKLZ", 8, 0 */)
                    : CFG_ADV_LAYOUT_5_RAW_UNCOMPRESSED; /* "FULL-IMAGE-RAW", "UNCOMPRESSED", 8 */
            }
            else if (ImageConfig.ImageBitsPerPixel == 12)
            {
                if (imageData == AdvImageData.PixelDepth16Bit) 
                    return CFG_ADV_LAYOUT_4_RAW_UNCOMPRESSED; /* "FULL-IMAGE-RAW", "UNCOMPRESSED", 12 */
                else if (imageData == AdvImageData.PixelData12Bit)
                    // NOTE: Think about this more. What is the difference between wanting to use a 12bit packed layout and passing the 12bit input data as bytes (2 per pixel) or shorts (1 per pixel)
                    // Should the input actually contain packed 12bit data (3 bytes per 2 pixels) and if not then how is this case going to be supported??
                {
                    return useCompression
                        ? (preferredCompression.HasValue && preferredCompression.Value == PreferredCompression.Lagarith16
                                ? CFG_ADV_LAYOUT_9_PACKED12_COMPRESSED_LTH16  /* "12BIT-IMAGE-PACKED", "QUICKLZ", 8, 0 */
                                : CFG_ADV_LAYOUT_10_PACKED12_COMPRESSED_QLZ   /* "12BIT-IMAGE-PACKED", "QUICKLZ", 8, 0 */)
                        : CFG_ADV_LAYOUT_8_PACKED12_UNCOMPRESSED; /* "12BIT-IMAGE-PACKED", "UNCOMPRESSED", 8 */
                }
                else
                    throw new NotSupportedException("TODO");
            }
            else
            {
                // NOTE: 16bit data presented as little endian byte array
                if (imageData != AdvImageData.PixelDepth16Bit) throw new NotSupportedException();

                // TODO: What about big endian data?

                return useCompression
                    ? (preferredCompression.HasValue && preferredCompression.Value == PreferredCompression.QuickLZ
                            ? CFG_ADV_LAYOUT_3_RAW_COMPRESSED_QLZ   /* "FULL-IMAGE-RAW", "QUICKLZ", 16, 0 */
                            : CFG_ADV_LAYOUT_2_RAW_COMPRESSED_LTH16 /* "FULL-IMAGE-RAW", "LAGARITH16", 16, 0 */)
                    : CFG_ADV_LAYOUT_1_RAW_UNCOMPRESSED; /* "FULL-IMAGE-RAW", "UNCOMPRESSED", 16 */
            }
        }

        private void BeginVideoFrame(AdvStream advStream, long? startClockTicks, long? endClockTicks, AdvTimeStamp startUtcTimeStamp, AdvTimeStamp endUtcTimeStamp, AdvStatusEntry metadata)
        {
            byte streamId = STREAM_IDS[(int) advStream];
            bool frameStartedOk = false;

            if (startClockTicks.HasValue && endClockTicks.HasValue)
            {
                switch(streamId)
                {
                    case MAIN_STREAM_ID:
                        if (m_GetMainClockTicksCallback == null)
                            throw new AdvLibException("A custom clock for the Main Stream must be defined when providing start and end clock ticks.");
                        break;

                    case CALIBRATION_STREAM_ID:
                        if (m_GetCalibrationClockTicksCallback == null)
                            throw new AdvLibException("A custom clock for the Calibration Stream must be defined when providing start and end clock ticks.");
                        break;

                    default:
                        throw new IndexOutOfRangeException();
                }

                long elapsedTicks = 0; // since the first recorded frame was taken
                if (m_NumberRecordedFrames > 0 && m_FirstRecordedFrameTimestamp != 0)
                {
                    elapsedTicks = startClockTicks.Value - m_FirstRecordedFrameTimestamp;
                }
                else if (m_NumberRecordedFrames == 0)
                {
                    m_FirstRecordedFrameTimestamp = startClockTicks.Value;
                }

                frameStartedOk = AdvLib.BeginFrame(streamId, startClockTicks.Value, endClockTicks.Value, elapsedTicks > 0L ? elapsedTicks : 0L, startUtcTimeStamp.NanosecondsAfterAdvZeroEpoch, (uint)(endUtcTimeStamp.NanosecondsAfterAdvZeroEpoch - startUtcTimeStamp.NanosecondsAfterAdvZeroEpoch));
            }
            else
            {
                switch (streamId)
                {
                    case MAIN_STREAM_ID:
                        if (m_GetMainClockTicksCallback != null)
                            throw new AdvLibException("Must provide start and end clock ticks when a custom clock for the Main Stream is defined.");
                        break;

                    case CALIBRATION_STREAM_ID:
                        if (m_GetCalibrationClockTicksCallback != null)
                            throw new AdvLibException("Must provide start and end clock ticks when a custom clock for the Calibration Stream is defined.");
                        break;

                    default:
                        throw new IndexOutOfRangeException();
                }

                frameStartedOk = AdvLib.BeginFrame(streamId, startUtcTimeStamp.NanosecondsAfterAdvZeroEpoch, (uint)(endUtcTimeStamp.NanosecondsAfterAdvZeroEpoch - startUtcTimeStamp.NanosecondsAfterAdvZeroEpoch));
            }

            if (!frameStartedOk)
            {
                // If we can't add the first frame, this may be a file creation issue; otherwise increase the dropped frames counter
                if (m_NumberRecordedFrames > 0)
                    m_NumberDroppedFrames++;
                return;
            }

            if (StatusSectionConfig.RecordSystemTime)
                AdvLib.FrameAddStatusTagInt64(m_TAGID_SystemTime,
                                                   metadata.SystemTime.NanosecondsAfterAdvZeroEpoch > 0
                                                       ? (long)metadata.SystemTime.NanosecondsAfterAdvZeroEpoch
                                                       : 0);

            if (StatusSectionConfig.RecordTrackedSatellites) AdvLib.FrameAddStatusTagUInt8(m_TAGID_TrackedSatellites, metadata.TrackedSatellites);
            if (StatusSectionConfig.RecordAlmanacStatus) AdvLib.FrameAddStatusTagUInt8(m_TAGID_AlmanacStatus, (byte)metadata.AlmanacStatus);
            if (StatusSectionConfig.RecordAlmanacOffset) AdvLib.FrameAddStatusTagUInt8(m_TAGID_AlmanacOffset, (byte)metadata.AlmanacOffset);
            if (StatusSectionConfig.RecordFixStatus) AdvLib.FrameAddStatusTagUInt8(m_TAGID_FixStatus, (byte)metadata.FixStatus);
            if (StatusSectionConfig.RecordGain) AdvLib.FrameAddStatusTagFloat(m_TAGID_Gain, metadata.Gain);
            if (StatusSectionConfig.RecordGamma) AdvLib.FrameAddStatusTagFloat(m_TAGID_Gamma, metadata.Gamma);
            if (StatusSectionConfig.RecordShutter) AdvLib.FrameAddStatusTagFloat(m_TAGID_Shutter, metadata.Shutter);
            if (StatusSectionConfig.RecordCameraOffset) AdvLib.FrameAddStatusTagFloat(m_TAGID_Offset, metadata.CameraOffset);
            if (StatusSectionConfig.RecordVideoCameraFrameId) AdvLib.FrameAddStatusTagInt64(m_TAGID_VideoCameraFrameId, metadata.VideoCameraFrameId);

            if (StatusSectionConfig.RecordHardwareTimerFrameId) AdvLib.FrameAddStatusTagInt64(m_TAGID_HardwareTimerFrameId, metadata.HardwareTimerFrameId);

            if (StatusSectionConfig.RecordSystemErrors && metadata.SystemErrors != null)
            {
                AdvLib.FrameAddStatusTagUTF8String(m_TAGID_SystemError, metadata.SystemErrors);
            }

            int additionalStatusTagId = -1;
            foreach (string tagName in StatusSectionConfig.AdditionalStatusTags.Keys)
            {
                uint tagId = m_AdditionalStatusSectionTagIds[tagName];
                additionalStatusTagId++;
                object statusTagValue = metadata.AdditionalStatusTags[additionalStatusTagId];

                switch (StatusSectionConfig.AdditionalStatusTags[tagName])
                {
                    case Adv2TagType.Int8:
                        AdvLib.FrameAddStatusTagUInt8(tagId, (byte)statusTagValue);
                        break;

                    case Adv2TagType.Int16:
                        AdvLib.FrameAddStatusTagInt16(tagId, (short)statusTagValue);
                        break;

                    case Adv2TagType.Int32:
                        AdvLib.FrameAddStatusTagInt32(tagId, (int)statusTagValue);
                        break;

                    case Adv2TagType.Long64:
                        AdvLib.FrameAddStatusTagInt64(tagId, (long)statusTagValue);
                        break;

                    case Adv2TagType.Real:
                        AdvLib.FrameAddStatusTagFloat(tagId, (float)statusTagValue);
                        break;

                    case Adv2TagType.UTF8String:
                        AdvLib.FrameAddStatusTagUTF8String(tagId, (string)statusTagValue);
                        break;
                }
            }
        }
    }
}
