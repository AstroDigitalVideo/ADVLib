using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

public class AdvRecorder
{
	private const byte CFG_ADV_LAYOUT_1_UNCOMPRESSED = 1;
	private const byte CFG_ADV_LAYOUT_2_COMPRESSED = 2;
	private const byte CFG_ADV_LAYOUT_3_UNCOMPRESSED = 3;
	private const byte CFG_ADV_LAYOUT_4_COMPRESSED = 4;

	public class AdvImageConfig
	{
		public ushort ImageWidth { get; private set; }
		public ushort ImageHeight { get; private set; }
		public byte CameraBitsPerPixel { get; private set; }
		public byte ImageBitsPerPixel { get; private set; }
		public bool ImageBigEndian { get; set; }

		/// <summary>
		/// Sets the image configuration
		/// </summary>
		/// <param name="width">The width of the image in pixels.</param>
		/// <param name="height">The height of the image in pixels.</param>
		/// <param name="cameraBitDepth">The native camera bith depth.</param>
		/// <param name="imageDynamicBitDepth">The bit depth of the dynamic range of the saved images.</param>
		public void SetImageParameters(ushort width, ushort height, byte cameraBitDepth, byte imageDynamicBitDepth)
		{
			if (cameraBitDepth > imageDynamicBitDepth)
				throw new AdvLibException("imageDynamicBitDepth must be greater or equal to cameraBitDepth");

			ImageWidth = width;
			ImageHeight = height;
			CameraBitsPerPixel = cameraBitDepth;
			ImageBitsPerPixel = imageDynamicBitDepth;
		}
	}

	/// <summary>
	/// The image configuration to be used for the ADV file when StartRecordingNewFile() is called.
	/// </summary>
	public AdvImageConfig ImageConfig = new AdvImageConfig();

	/// <summary>
	/// Creates new ADV file and gets it ready for recording 
	/// </summary>
	/// <param name="fileName"></param>
	public void StartRecordingNewFile(string fileName)
	{
		AdvLib.NewFile(fileName);

		AdvLib.SetTimingPrecision(1000, 1, 1000, 1);

		AdvLib.AddMainStreamTag("Name1", "Христо");
		AdvLib.AddMainStreamTag("Name2", "Frédéric");

		AdvLib.AddCalibrationStreamTag("Name1", "好的茶");

		//if (string.IsNullOrEmpty(FileMetaData.RecorderName)) throw new ArgumentException("FileMetaData.RecorderName must be specified.");
		//if (string.IsNullOrEmpty(FileMetaData.RecorderVersion)) throw new ArgumentException("FileMetaData.RecorderVersion must be specified.");
		//if (string.IsNullOrEmpty(FileMetaData.CameraModel)) throw new ArgumentException("FileMetaData.CameraModel must be specified.");
		//if (string.IsNullOrEmpty(FileMetaData.CameraSensorInfo)) throw new ArgumentException("FileMetaData.CameraSensorInfo must be specified.");

		//AdvLib.AdvVer1_AddFileTag("RECORDER-SOFTWARE-VERSION", EnsureStringLength(FileMetaData.RecorderVersion));
		//AdvLib.AdvVer1_AddFileTag("TIMER-FIRMWARE-VERSION", EnsureStringLength(FileMetaData.RecorderTimerFirmwareVersion));
		//AdvLib.AdvVer1_AddFileTag("ADVLIB-VERSION", "1.0");

		//AdvLib.AdvVer1_AddFileTag("RECORDER", EnsureStringLength(FileMetaData.RecorderName));
		//AdvLib.AdvVer1_AddFileTag("FSTF-TYPE", "ADV");
		//AdvLib.AdvVer1_AddFileTag("ADV-VERSION", "1");

		//if (!string.IsNullOrEmpty(LocationData.LongitudeWgs84)) AdvLib.AdvVer1_AddFileTag("LONGITUDE-WGS84", LocationData.LongitudeWgs84);
		//if (!string.IsNullOrEmpty(LocationData.LatitudeWgs84)) AdvLib.AdvVer1_AddFileTag("LATITUDE-WGS84", LocationData.LatitudeWgs84);
		//if (!string.IsNullOrEmpty(LocationData.AltitudeMsl)) AdvLib.AdvVer1_AddFileTag("ALTITUDE-MSL", LocationData.AltitudeMsl);
		//if (!string.IsNullOrEmpty(LocationData.MslWgs84Offset)) AdvLib.AdvVer1_AddFileTag("MSL-WGS84-OFFSET", LocationData.MslWgs84Offset);
		//if (!string.IsNullOrEmpty(LocationData.GpsHdop)) AdvLib.AdvVer1_AddFileTag("GPS-HDOP", LocationData.GpsHdop);

		//AdvLib.AdvVer1_AddFileTag("CAMERA-MODEL", EnsureStringLength(FileMetaData.CameraModel));
		//AdvLib.AdvVer1_AddFileTag("CAMERA-SERIAL-NO", EnsureStringLength(FileMetaData.CameraSerialNumber));
		//AdvLib.AdvVer1_AddFileTag("CAMERA-VENDOR-NAME", EnsureStringLength(FileMetaData.CameraVendorNumber));
		//AdvLib.AdvVer1_AddFileTag("CAMERA-SENSOR-INFO", EnsureStringLength(FileMetaData.CameraSensorInfo));
		//AdvLib.AdvVer1_AddFileTag("CAMERA-SENSOR-RESOLUTION", EnsureStringLength(FileMetaData.CameraSensorResolution));
		//AdvLib.AdvVer1_AddFileTag("CAMERA-FIRMWARE-VERSION", EnsureStringLength(FileMetaData.CameraFirmwareVersion));
		//AdvLib.AdvVer1_AddFileTag("CAMERA-FIRMWARE-BUILD-TIME", EnsureStringLength(FileMetaData.CameraFirmwareBuildTime));
		//AdvLib.AdvVer1_AddFileTag("CAMERA-DRIVER-VERSION", EnsureStringLength(FileMetaData.CameraDriverVersion));

		//foreach (string key in FileMetaData.UserMetaData.Keys)
		//{
		//	AdvLib.AdvVer1_AddFileTag(EnsureStringLength(key), EnsureStringLength(FileMetaData.UserMetaData[key]));
		//}

		AdvLib.DefineImageSection(ImageConfig.ImageWidth, ImageConfig.ImageHeight, ImageConfig.CameraBitsPerPixel);
		AdvLib.AddOrUpdateImageSectionTag("IMAGE-BYTE-ORDER", ImageConfig.ImageBigEndian ? "BIG-ENDIAN" : "LITTLE-ENDIAN");
		AdvLib.AddOrUpdateImageSectionTag("IMAGE-DYNABITS", ImageConfig.ImageBitsPerPixel.ToString(CultureInfo.InvariantCulture));

		AdvLib.DefineImageLayout(CFG_ADV_LAYOUT_1_UNCOMPRESSED, "FULL-IMAGE-RAW", "UNCOMPRESSED", 16, 0, null);
		AdvLib.DefineImageLayout(CFG_ADV_LAYOUT_2_COMPRESSED, "FULL-IMAGE-RAW", "LAGARITH16", 16, 0, null);
		AdvLib.DefineImageLayout(CFG_ADV_LAYOUT_3_UNCOMPRESSED, "FULL-IMAGE-RAW", "UNCOMPRESSED", 8, 0, null);
		AdvLib.DefineImageLayout(CFG_ADV_LAYOUT_4_COMPRESSED, "FULL-IMAGE-RAW", "QUICKLZ", 8, 0, null);

		//if (StatusSectionConfig.RecordSystemTime) m_TAGID_SystemTime = AdvLib.AdvVer1_DefineStatusSectionTag("SystemTime", AdvTagType.ULong64);

		//if (StatusSectionConfig.RecordGPSTrackedSatellites) m_TAGID_TrackedGPSSatellites = AdvLib.AdvVer1_DefineStatusSectionTag("GPSTrackedSatellites", AdvTagType.UInt8);
		//if (StatusSectionConfig.RecordGPSAlmanacStatus) m_TAGID_GPSAlmanacStatus = AdvLib.AdvVer1_DefineStatusSectionTag("GPSAlmanacStatus", AdvTagType.UInt8);
		//if (StatusSectionConfig.RecordGPSAlmanacOffset) m_TAGID_GPSAlmanacOffset = AdvLib.AdvVer1_DefineStatusSectionTag("GPSAlmanacOffset", AdvTagType.UInt8);
		//if (StatusSectionConfig.RecordGPSFixStatus) m_TAGID_GPSFixStatus = AdvLib.AdvVer1_DefineStatusSectionTag("GPSFixStatus", AdvTagType.UInt8);
		//if (StatusSectionConfig.RecordGain) m_TAGID_Gain = AdvLib.AdvVer1_DefineStatusSectionTag("Gain", AdvTagType.Real);
		//if (StatusSectionConfig.RecordShutter) m_TAGID_Shutter = AdvLib.AdvVer1_DefineStatusSectionTag("Shutter", AdvTagType.Real);
		//if (StatusSectionConfig.RecordCameraOffset) m_TAGID_Offset = AdvLib.AdvVer1_DefineStatusSectionTag("Offset", AdvTagType.Real);
		//if (StatusSectionConfig.RecordVideoCameraFrameId) m_TAGID_VideoCameraFrameId = AdvLib.AdvVer1_DefineStatusSectionTag("VideoCameraFrameId", AdvTagType.ULong64);
		//if (StatusSectionConfig.RecordGamma) m_TAGID_Gamma = AdvLib.AdvVer1_DefineStatusSectionTag("Gamma", AdvTagType.Real);
		//if (StatusSectionConfig.RecordUserCommands) m_TAGID_UserCommand = AdvLib.AdvVer1_DefineStatusSectionTag("UserCommand", AdvTagType.List16OfAnsiString255);
		//if (StatusSectionConfig.RecordSystemErrors) m_TAGID_SystemError = AdvLib.AdvVer1_DefineStatusSectionTag("SystemError", AdvTagType.List16OfAnsiString255);

		//m_AdditionalStatusSectionTagIds.Clear();

		//if (StatusSectionConfig.AdditionalStatusTags.Count > 0)
		//{
		//	foreach (string tagName in StatusSectionConfig.AdditionalStatusTags.Keys)
		//	{
		//		uint tagId = AdvLib.AdvVer1_DefineStatusSectionTag(tagName, StatusSectionConfig.AdditionalStatusTags[tagName]);
		//		m_AdditionalStatusSectionTagIds.Add(tagName, tagId);
		//	}
		//}

		//m_NumberRecordedFrames = 0;
		//m_NumberDroppedFrames = 0;
		//m_FirstRecordedFrameTimestamp = 0;

		AdvLib.BeginFrame(0, 0, 0, 0);
	}

	/// <summary>
	/// Closes the AVD file and stops any recording to it.
	/// </summary>
	public void StopRecording()
	{
		AdvLib.EndFile();
	}
}
