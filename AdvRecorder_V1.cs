/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Adv;

namespace Obsolete
{
	public class AdvFileMetaData
	{
		internal Dictionary<string, string> UserMetaData = new Dictionary<string, string>();

		public string RecorderName { get; set; }
		public string RecorderVersion { get; set; }
		public string RecorderTimerFirmwareVersion { get; set; }

		public string CameraModel { get; set; }
		public string CameraSerialNumber { get; set; }
		public string CameraVendorNumber { get; set; }
		public string CameraSensorInfo { get; set; }
		public string CameraSensorResolution { get; set; }
		public string CameraFirmwareVersion { get; set; }
		public string CameraFirmwareBuildTime { get; set; }
		public string CameraDriverVersion { get; set; }

		public void AddUserTag(string tagName, string tagValue)
		{
			UserMetaData[tagName] = tagValue;
		}
	}

	public class AdvLocationData
	{
		public string LongitudeWgs84 { get; set; }
		public string LatitudeWgs84 { get; set; }
		public string AltitudeMsl { get; set; }
		public string MslWgs84Offset { get; set; }
		public string GpsHdop { get; set; }
	}

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
		/// <param name="imageDynamicBitDepth">The bit depth of the dynamic range of the saved images.</param>
		public void SetImageParameters(ushort width, ushort height, byte imageDynamicBitDepth)
		{
			ImageWidth = width;
			ImageHeight = height;
			ImageBitsPerPixel = imageDynamicBitDepth;
		}
	}

	public class AdvStatusSectionConfig
	{
		public bool RecordSystemTime { get; set; }
		public bool RecordGPSTrackedSatellites { get; set; }
		public bool RecordGPSAlmanacStatus { get; set; }
		public bool RecordGPSAlmanacOffset { get; set; }
		public bool RecordGPSFixStatus { get; set; }
		public bool RecordGain { get; set; }
		public bool RecordShutter { get; set; }
		public bool RecordCameraOffset { get; set; }
		public bool RecordGamma { get; set; }
		public bool RecordVideoCameraFrameId { get; set; }
		public bool RecordUserCommands { get; set; }
		public bool RecordSystemErrors { get; set; }

		internal Dictionary<string, AdvTagType> AdditionalStatusTags = new Dictionary<string, AdvTagType>();

		public int AddDefineTag(string tagName, AdvTagType tagType)
		{
			if (AdditionalStatusTags.ContainsKey(tagName))
				throw new ArgumentException("This tag name as been already added.");

			if (tagType == AdvTagType.UTF8String || tagType == AdvTagType.List16OfUTF8String)
				throw new ArgumentException("UTF8 strings are not supported in the obsolete recorder which is using ADV version 1.");

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
		/// Number of tracked GPS satellites
		/// </summary>
		public byte GPSTrackedSatellites { get; set; }

		/// <summary>
		/// The status of the GPS almanac update 
		/// </summary>
		public AlmanacStatus GPSAlmanacStatus { get; set; }

		/// <summary>
		/// The almanac offset in seconds that was added to the uncorrected time reported by the GPS in order to compute the UTC time
		/// </summary>
		public byte GPSAlmanacOffset { get; set; }

		/// <summary>
		/// The status of the GPS fix
		/// </summary>
		public FixStatus GPSFixStatus { get; set; }

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
		/// The user commands executed since the last recorded frame. Up to 16 lines, each line up to 255 characters.
		/// </summary>
		public string[] UserCommands { get; set; }

		/// <summary>
		/// System errors detected since the last recorded frame. Up to 16 lines, each line up to 255 characters.
		/// </summary>
		public string[] SystemErrors { get; set; }

		/// <summary>
		/// The values of the additional tags. The value types must correspond to the defined tag type. Only the following
		/// .NET types are supported: byte, ushort, uint, ulong, float, string and string[]
		/// </summary>
		public object[] AdditionalStatusTags;
	}

	namespace AdvVer1
	{
		public class AdvRecorder
		{
			private uint m_TAGID_SystemTime;
			private uint m_TAGID_TrackedGPSSatellites;
			private uint m_TAGID_GPSAlmanacStatus;
			private uint m_TAGID_GPSAlmanacOffset;
			private uint m_TAGID_GPSFixStatus;
			private uint m_TAGID_Gain;
			private uint m_TAGID_Shutter;
			private uint m_TAGID_Offset;
			private uint m_TAGID_VideoCameraFrameId;
			private uint m_TAGID_Gamma;
			private uint m_TAGID_UserCommand;
			private uint m_TAGID_SystemError;

			private int m_NumberRecordedFrames = 0;
			private int m_NumberDroppedFrames = 0;
			private long m_FirstRecordedFrameTimestamp = 0;

			public int NumberDroppedFrames
			{
				get { return m_NumberDroppedFrames; }
			}

			private const byte CFG_ADV_LAYOUT_1_UNCOMPRESSED = 1;
			private const byte CFG_ADV_LAYOUT_2_COMPRESSED = 2;
			private const byte CFG_ADV_LAYOUT_3_COMPRESSED = 3;
			private const byte CFG_ADV_LAYOUT_4_UNCOMPRESSED = 4;
			private const byte CFG_ADV_LAYOUT_5_COMPRESSED = 5;

			private Dictionary<string, uint> m_AdditionalStatusSectionTagIds = new Dictionary<string, uint>();

			/// <summary>
			/// The status section configuration to be used for the ADV file when StartRecordingNewFile() is called.
			/// </summary>
			public AdvStatusSectionConfig StatusSectionConfig = new AdvStatusSectionConfig();

			/// <summary>
			/// The file metadata to be saved in the file when StartRecordingNewFile() is called.
			/// </summary>
			public AdvFileMetaData FileMetaData = new AdvFileMetaData();

			/// <summary>
			/// The image configuration to be used for the ADV file when StartRecordingNewFile() is called.
			/// </summary>
			public AdvImageConfig ImageConfig = new AdvImageConfig();

			/// <summary>
			/// The location data to be saved in the file when StartRecordingNewFile() is called.
			/// </summary>
			public AdvLocationData LocationData = new AdvLocationData();

			private string EnsureStringLength(string input)
			{
				if (input == null)
					return string.Empty;
				else if (input.Length > 255)
					return input.Substring(0, 255);
				else
					return input;
			}

			/// <summary>
			/// Creates new ADV file and gets it ready for recording 
			/// </summary>
			/// <param name="fileName"></param>
			public void StartRecordingNewFile(string fileName)
			{
				AdvLib.Obsolete.AdvVer1.NewFile(fileName);

				if (string.IsNullOrEmpty(FileMetaData.RecorderName))
					throw new ArgumentException("FileMetaData.RecorderName must be specified.");
				if (string.IsNullOrEmpty(FileMetaData.RecorderVersion))
					throw new ArgumentException("FileMetaData.RecorderVersion must be specified.");
				if (string.IsNullOrEmpty(FileMetaData.CameraModel))
					throw new ArgumentException("FileMetaData.CameraModel must be specified.");
				if (string.IsNullOrEmpty(FileMetaData.CameraSensorInfo))
					throw new ArgumentException("FileMetaData.CameraSensorInfo must be specified.");

				AdvLib.Obsolete.AdvVer1.AddFileTag("RECORDER-SOFTWARE-VERSION", EnsureStringLength(FileMetaData.RecorderVersion));
				AdvLib.Obsolete.AdvVer1.AddFileTag("TIMER-FIRMWARE-VERSION", EnsureStringLength(FileMetaData.RecorderTimerFirmwareVersion));
				AdvLib.Obsolete.AdvVer1.AddFileTag("ADVLIB-VERSION", "1.0");

				AdvLib.Obsolete.AdvVer1.AddFileTag("RECORDER", EnsureStringLength(FileMetaData.RecorderName));
				AdvLib.Obsolete.AdvVer1.AddFileTag("FSTF-TYPE", "ADV");
				AdvLib.Obsolete.AdvVer1.AddFileTag("ADV-VERSION", "1");

				if (!string.IsNullOrEmpty(LocationData.LongitudeWgs84))
					AdvLib.Obsolete.AdvVer1.AddFileTag("LONGITUDE-WGS84", LocationData.LongitudeWgs84);
				if (!string.IsNullOrEmpty(LocationData.LatitudeWgs84))
					AdvLib.Obsolete.AdvVer1.AddFileTag("LATITUDE-WGS84", LocationData.LatitudeWgs84);
				if (!string.IsNullOrEmpty(LocationData.AltitudeMsl))
					AdvLib.Obsolete.AdvVer1.AddFileTag("ALTITUDE-MSL", LocationData.AltitudeMsl);
				if (!string.IsNullOrEmpty(LocationData.MslWgs84Offset))
					AdvLib.Obsolete.AdvVer1.AddFileTag("MSL-WGS84-OFFSET", LocationData.MslWgs84Offset);
				if (!string.IsNullOrEmpty(LocationData.GpsHdop)) AdvLib.Obsolete.AdvVer1.AddFileTag("GPS-HDOP", LocationData.GpsHdop);

				AdvLib.Obsolete.AdvVer1.AddFileTag("CAMERA-MODEL", EnsureStringLength(FileMetaData.CameraModel));
				AdvLib.Obsolete.AdvVer1.AddFileTag("CAMERA-SERIAL-NO", EnsureStringLength(FileMetaData.CameraSerialNumber));
				AdvLib.Obsolete.AdvVer1.AddFileTag("CAMERA-VENDOR-NAME", EnsureStringLength(FileMetaData.CameraVendorNumber));
				AdvLib.Obsolete.AdvVer1.AddFileTag("CAMERA-SENSOR-INFO", EnsureStringLength(FileMetaData.CameraSensorInfo));
				AdvLib.Obsolete.AdvVer1.AddFileTag("CAMERA-SENSOR-RESOLUTION", EnsureStringLength(FileMetaData.CameraSensorResolution));
				AdvLib.Obsolete.AdvVer1.AddFileTag("CAMERA-FIRMWARE-VERSION", EnsureStringLength(FileMetaData.CameraFirmwareVersion));
				AdvLib.Obsolete.AdvVer1.AddFileTag("CAMERA-FIRMWARE-BUILD-TIME", EnsureStringLength(FileMetaData.CameraFirmwareBuildTime));
				AdvLib.Obsolete.AdvVer1.AddFileTag("CAMERA-DRIVER-VERSION", EnsureStringLength(FileMetaData.CameraDriverVersion));

				foreach (string key in FileMetaData.UserMetaData.Keys)
				{
					AdvLib.Obsolete.AdvVer1.AddFileTag(EnsureStringLength(key), EnsureStringLength(FileMetaData.UserMetaData[key]));
				}

				AdvLib.Obsolete.AdvVer1.DefineImageSection(ImageConfig.ImageWidth, ImageConfig.ImageHeight, ImageConfig.CameraBitsPerPixel);
				AdvLib.Obsolete.AdvVer1.AddOrUpdateImageSectionTag("IMAGE-BYTE-ORDER", ImageConfig.ImageBigEndian ? "BIG-ENDIAN" : "LITTLE-ENDIAN");
				AdvLib.Obsolete.AdvVer1.AddOrUpdateImageSectionTag("IMAGE-DYNABITS", ImageConfig.ImageBitsPerPixel.ToString(CultureInfo.InvariantCulture));

				AdvLib.Obsolete.AdvVer1.DefineImageLayout(CFG_ADV_LAYOUT_1_UNCOMPRESSED, "FULL-IMAGE-RAW", "UNCOMPRESSED", 16, 0, null);
				AdvLib.Obsolete.AdvVer1.DefineImageLayout(CFG_ADV_LAYOUT_2_COMPRESSED, "FULL-IMAGE-DIFFERENTIAL-CODING", "QUICKLZ", 12, 32, "PREV-FRAME");
				AdvLib.Obsolete.AdvVer1.DefineImageLayout(CFG_ADV_LAYOUT_3_COMPRESSED, "FULL-IMAGE-RAW", "QUICKLZ", 16, 0, null);
				AdvLib.Obsolete.AdvVer1.DefineImageLayout(CFG_ADV_LAYOUT_4_UNCOMPRESSED, "FULL-IMAGE-RAW", "UNCOMPRESSED", 8, 0, null);
				AdvLib.Obsolete.AdvVer1.DefineImageLayout(CFG_ADV_LAYOUT_5_COMPRESSED, "FULL-IMAGE-RAW", "QUICKLZ", 8, 0, null);

				if (StatusSectionConfig.RecordSystemTime)
					m_TAGID_SystemTime = AdvLib.Obsolete.AdvVer1.DefineStatusSectionTag("SystemTime", AdvTagType.ULong64);

				if (StatusSectionConfig.RecordGPSTrackedSatellites)
					m_TAGID_TrackedGPSSatellites = AdvLib.Obsolete.AdvVer1.DefineStatusSectionTag("GPSTrackedSatellites", AdvTagType.UInt8);
				if (StatusSectionConfig.RecordGPSAlmanacStatus)
					m_TAGID_GPSAlmanacStatus = AdvLib.Obsolete.AdvVer1.DefineStatusSectionTag("GPSAlmanacStatus", AdvTagType.UInt8);
				if (StatusSectionConfig.RecordGPSAlmanacOffset)
					m_TAGID_GPSAlmanacOffset = AdvLib.Obsolete.AdvVer1.DefineStatusSectionTag("GPSAlmanacOffset", AdvTagType.UInt8);
				if (StatusSectionConfig.RecordGPSFixStatus)
					m_TAGID_GPSFixStatus = AdvLib.Obsolete.AdvVer1.DefineStatusSectionTag("GPSFixStatus", AdvTagType.UInt8);
				if (StatusSectionConfig.RecordGain) m_TAGID_Gain = AdvLib.Obsolete.AdvVer1.DefineStatusSectionTag("Gain", AdvTagType.Real);
				if (StatusSectionConfig.RecordShutter)
					m_TAGID_Shutter = AdvLib.Obsolete.AdvVer1.DefineStatusSectionTag("Shutter", AdvTagType.Real);
				if (StatusSectionConfig.RecordCameraOffset)
					m_TAGID_Offset = AdvLib.Obsolete.AdvVer1.DefineStatusSectionTag("Offset", AdvTagType.Real);
				if (StatusSectionConfig.RecordVideoCameraFrameId)
					m_TAGID_VideoCameraFrameId = AdvLib.Obsolete.AdvVer1.DefineStatusSectionTag("VideoCameraFrameId", AdvTagType.ULong64);
				if (StatusSectionConfig.RecordGamma) m_TAGID_Gamma = AdvLib.Obsolete.AdvVer1.DefineStatusSectionTag("Gamma", AdvTagType.Real);
				if (StatusSectionConfig.RecordUserCommands)
					m_TAGID_UserCommand = AdvLib.Obsolete.AdvVer1.DefineStatusSectionTag("UserCommand", AdvTagType.List16OfAnsiString255);
				if (StatusSectionConfig.RecordSystemErrors)
					m_TAGID_SystemError = AdvLib.Obsolete.AdvVer1.DefineStatusSectionTag("SystemError", AdvTagType.List16OfAnsiString255);

				m_AdditionalStatusSectionTagIds.Clear();

				if (StatusSectionConfig.AdditionalStatusTags.Count > 0)
				{
					foreach (string tagName in StatusSectionConfig.AdditionalStatusTags.Keys)
					{
						uint tagId = AdvLib.Obsolete.AdvVer1.DefineStatusSectionTag(tagName, StatusSectionConfig.AdditionalStatusTags[tagName]);
						m_AdditionalStatusSectionTagIds.Add(tagName, tagId);
					}
				}

				m_NumberRecordedFrames = 0;
				m_NumberDroppedFrames = 0;
				m_FirstRecordedFrameTimestamp = 0;
			}

			/// <summary>
			/// Closes the AVD file and stops any recording to it.
			/// </summary>
			public void FinishRecording()
			{
				AdvLib.Obsolete.AdvVer1.EndFile();
			}

			/// <summary>
			/// Adds a new video frame from a byte array.
			/// </summary>
			/// <param name="pixels">The pixels to be saved. The row-major array is of size Width * Height in 8-bit mode and 2 * Width * Height in little-endian 16-bit mode.</param>
			/// <param name="compress">True if the frame is to be compressed. Please note that compression is CPU and I/O intensive and may not work at high frame rates. Use wisely.</param>
			/// <param name="imageData">The format of the pixels - 8 bit or 16 bit.</param>
			/// <param name="timeStamp">The high accuracy timestamp for the middle of the frame. If the timestamp is not with an accuracy of 1ms then set it as zero. A lower accuracy timestamp can be specified in the SystemTime status value.</param>
			/// <param name="exposureIn10thMilliseconds">The duration of the frame in whole 0.1 ms as determined by the high accuracy timestamping. If high accuracy timestamp is not available then set this to zero. Note that the Shutter status value should be derived from the camera settings rather than from the timestamps.</param>
			/// <param name="metadata">The status metadata to be saved with the video frame.</param>
			public void AddVideoFrame(byte[] pixels, bool compress, AdvImageData imageData, AdvTimeStamp timeStamp, uint exposureIn10thMilliseconds, AdvStatusEntry metadata)
			{
				BeginVideoFrame(timeStamp, exposureIn10thMilliseconds, metadata);

				if (imageData == AdvImageData.PixelDepth16Bit)
				{
					byte layoutIdForCurrentFramerate = compress ? CFG_ADV_LAYOUT_3_COMPRESSED : CFG_ADV_LAYOUT_1_UNCOMPRESSED;
					AdvLib.Obsolete.AdvVer1.FrameAddImageBytes(layoutIdForCurrentFramerate, pixels, 16);
				}
				else if (imageData == AdvImageData.PixelDepth8Bit)
				{
					byte layoutIdForCurrentFramerate = compress ? CFG_ADV_LAYOUT_5_COMPRESSED : CFG_ADV_LAYOUT_4_UNCOMPRESSED;
					AdvLib.Obsolete.AdvVer1.FrameAddImageBytes(layoutIdForCurrentFramerate, pixels, 8);
				}

				AdvLib.Obsolete.AdvVer1.EndFrame();
			}

			/// <summary>
			/// Adds a new video frame from an ushort array.
			/// </summary>
			/// <param name="pixels">The pixels to be saved. The row-major array is of size 2 * Width * Height. This only works in little-endian 16-bit mode.</param>
			/// <param name="compress">True if the frame is to be compressed. Please note that compression is CPU and I/O intensive and may not work at high frame rates. Use wisely.</param>
			/// <param name="timeStamp">The high accuracy timestamp for the middle of the frame. If the timestamp is not with an accuracy of 1ms then set it as zero. A lower accuracy timestamp can be specified in the SystemTime status value.</param>
			/// <param name="exposureIn10thMilliseconds">The duration of the frame in whole 0.1 ms as determined by the high accuracy timestamping. If high accuracy timestamp is not available then set this to zero. Note that the Shutter status value should be derived from the camera settings rather than from the timestamps.</param>
			/// <param name="metadata">The status metadata to be saved with the video frame.</param>
			public void AddVideoFrame(ushort[] pixels, bool compress, AdvTimeStamp timeStamp, uint exposureIn10thMilliseconds,
									  AdvStatusEntry metadata)
			{
				BeginVideoFrame(timeStamp, exposureIn10thMilliseconds, metadata);

				byte layoutIdForCurrentFramerate = compress ? CFG_ADV_LAYOUT_3_COMPRESSED : CFG_ADV_LAYOUT_1_UNCOMPRESSED;

				AdvLib.Obsolete.AdvVer1.FrameAddImage(layoutIdForCurrentFramerate, pixels, 16);

				AdvLib.Obsolete.AdvVer1.EndFrame();
			}

			private void BeginVideoFrame(AdvTimeStamp timeStamp, uint exposureIn10thMilliseconds, AdvStatusEntry metadata)
			{
				long elapsedTimeMilliseconds = 0; // since the first recorded frame was taken
				if (m_NumberRecordedFrames > 0 && m_FirstRecordedFrameTimestamp != 0)
				{
					elapsedTimeMilliseconds = timeStamp.MillisecondsAfterAdvZeroEpoch - m_FirstRecordedFrameTimestamp;
				}
				else if (m_NumberRecordedFrames == 0)
				{
					m_FirstRecordedFrameTimestamp = timeStamp.MillisecondsAfterAdvZeroEpoch;
				}

				bool frameStartedOk = AdvLib.Obsolete.AdvVer1.BeginFrame(timeStamp.MillisecondsAfterAdvZeroEpoch,
																elapsedTimeMilliseconds > 0 ? (uint)elapsedTimeMilliseconds : 0,
																exposureIn10thMilliseconds);
				if (!frameStartedOk)
				{
					// If we can't add the first frame, this may be a file creation issue; otherwise increase the dropped frames counter
					if (m_NumberRecordedFrames > 0)
						m_NumberDroppedFrames++;
					return;
				}

				if (StatusSectionConfig.RecordSystemTime)
					AdvLib.Obsolete.AdvVer1.FrameAddStatusTag64(m_TAGID_SystemTime,
													   metadata.SystemTime.MillisecondsAfterAdvZeroEpoch > 0
														   ? (ulong)metadata.SystemTime.MillisecondsAfterAdvZeroEpoch
														   : 0);

				if (StatusSectionConfig.RecordGPSTrackedSatellites)
					AdvLib.Obsolete.AdvVer1.FrameAddStatusTagUInt8(m_TAGID_TrackedGPSSatellites, metadata.GPSTrackedSatellites);
				if (StatusSectionConfig.RecordGPSAlmanacStatus)
					AdvLib.Obsolete.AdvVer1.FrameAddStatusTagUInt8(m_TAGID_GPSAlmanacStatus, (byte)metadata.GPSAlmanacStatus);
				if (StatusSectionConfig.RecordGPSAlmanacOffset)
					AdvLib.Obsolete.AdvVer1.FrameAddStatusTagUInt8(m_TAGID_GPSAlmanacOffset, metadata.GPSAlmanacOffset);
				if (StatusSectionConfig.RecordGPSFixStatus)
					AdvLib.Obsolete.AdvVer1.FrameAddStatusTagUInt8(m_TAGID_GPSFixStatus, (byte)metadata.GPSFixStatus);
				if (StatusSectionConfig.RecordGain) AdvLib.Obsolete.AdvVer1.FrameAddStatusTagReal(m_TAGID_Gain, metadata.Gain);
				if (StatusSectionConfig.RecordGamma) AdvLib.Obsolete.AdvVer1.FrameAddStatusTagReal(m_TAGID_Gamma, metadata.Gamma);
				if (StatusSectionConfig.RecordShutter) AdvLib.Obsolete.AdvVer1.FrameAddStatusTagReal(m_TAGID_Shutter, metadata.Shutter);
				if (StatusSectionConfig.RecordCameraOffset)
					AdvLib.Obsolete.AdvVer1.FrameAddStatusTagReal(m_TAGID_Offset, metadata.CameraOffset);
				if (StatusSectionConfig.RecordVideoCameraFrameId)
					AdvLib.Obsolete.AdvVer1.FrameAddStatusTag64(m_TAGID_VideoCameraFrameId, metadata.VideoCameraFrameId);

				if (StatusSectionConfig.RecordUserCommands && metadata.UserCommands != null)
				{
					for (int i = 0; i < Math.Min(16, metadata.UserCommands.Count()); i++)
					{
						if (metadata.UserCommands[i] != null)
						{
							if (metadata.UserCommands[i].Length > 255)
								AdvLib.Obsolete.AdvVer1.FrameAddStatusTagMessage(m_TAGID_UserCommand, metadata.UserCommands[i].Substring(0, 255));
							else
								AdvLib.Obsolete.AdvVer1.FrameAddStatusTagMessage(m_TAGID_UserCommand, metadata.UserCommands[i]);
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
								AdvLib.Obsolete.AdvVer1.FrameAddStatusTagMessage(m_TAGID_SystemError, metadata.SystemErrors[i].Substring(0, 255));
							else
								AdvLib.Obsolete.AdvVer1.FrameAddStatusTagMessage(m_TAGID_SystemError, metadata.SystemErrors[i]);
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
							AdvLib.Obsolete.AdvVer1.FrameAddStatusTagUInt8(tagId, (byte)statusTagValue);
							break;

						case AdvTagType.UInt16:
							AdvLib.Obsolete.AdvVer1.FrameAddStatusTag16(tagId, (ushort)statusTagValue);
							break;

						case AdvTagType.UInt32:
							AdvLib.Obsolete.AdvVer1.FrameAddStatusTag32(tagId, (uint)statusTagValue);
							break;

						case AdvTagType.ULong64:
							AdvLib.Obsolete.AdvVer1.FrameAddStatusTag64(tagId, (ulong)statusTagValue);
							break;

						case AdvTagType.Real:
							AdvLib.Obsolete.AdvVer1.FrameAddStatusTagReal(tagId, (float)statusTagValue);
							break;

						case AdvTagType.AnsiString255:
							AdvLib.Obsolete.AdvVer1.FrameAddStatusTag(tagId, (string)statusTagValue);
							break;

						case AdvTagType.List16OfAnsiString255:
							string[] lines = (string[])statusTagValue;
							for (int i = 0; i < Math.Min(16, lines.Count()); i++)
							{
								if (lines[i] != null)
								{
									if (lines[i].Length > 255)
										AdvLib.Obsolete.AdvVer1.FrameAddStatusTagMessage(tagId, lines[i].Substring(0, 255));
									else
										AdvLib.Obsolete.AdvVer1.FrameAddStatusTagMessage(tagId, lines[i]);
								}
							}
							break;
					}
				}
			}

			[DllImport("kernel32.dll", SetLastError = false)]
			private static extern bool SetDllDirectory(string lpPathName);

			/// <summary>
			/// Adds a directory to the search path for AdvLib.Core32.dll and AdvLib.Core64.dll
			/// </summary>
			/// <param name="path">The full path to AdvLib.Core32.dll and AdvLib.Core64.dll</param>
			public void SetNativeDllDirectory(string path)
			{
				SetDllDirectory(path);
			}
		}
	}
}