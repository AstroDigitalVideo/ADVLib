#region
// The MIT License (MIT)
//
// Copyright (c) 2014 Hristo Pavlov
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion

using AdvLib.TestApp;
using AdvLib.Tests.Generators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Adv;

namespace AdvLibTestApp
{
	public partial class frmMain : Form
	{
	    private ImageGenerator imageGenerator = new ImageGenerator();

		public frmMain()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (cbxADV1.Checked)
				SaveAdvVer1Sample();
			else
				SaveAdvVer2Sample();
		}

		private void SaveAdvVer2Sample()
		{
			string fileName = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + @"Filename2.adv");

		    if (File.Exists(fileName))
            {
                if (MessageBox.Show(string.Format("Output file exists:\r\n\r\n{0}\r\n\r\nOverwrite?", fileName), "Confirmation", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;

                File.Delete(fileName);
            }

		    var config = new AdvGenerationConfig();
		    config.SaveLocationData = cbxLocationData.Checked;

            // Define the image size and bit depth
            config.DynaBits = 16;
            if (rbPixel16.Checked) config.DynaBits = 16;
            else if (rbPixel12.Checked) config.DynaBits = 12;
            else if (rbPixel8.Checked) config.DynaBits = 8;

            config.NormalPixelValue = null;
            if (nudMaxPixelValue.Value > 0) config.NormalPixelValue = (int)nudMaxPixelValue.Value;

            config.Compression = CompressionType.Uncompressed;
            if (cbxCompress.Checked)
                config.Compression = rbLagarith16.Checked ? CompressionType.Lagarith16 : CompressionType.QuickLZ;

            if (rb16BitUShort.Checked)
                config.SourceFormat = AdvSourceDataFormat.Format16BitUShort;
            else if (rb16BitByte.Checked)
                config.SourceFormat = AdvSourceDataFormat.Format16BitLittleEndianByte;
            else if (rb12BitByte.Checked)
                config.SourceFormat = AdvSourceDataFormat.Format12BitPackedByte;
            else if (rb8BitByte.Checked)
                config.SourceFormat = AdvSourceDataFormat.Format8BitByte;
            else if (rb24bitRGB.Checked || rb24bitBGR.Checked)
            {
                config.SourceFormat = AdvSourceDataFormat.Format24BitColour;
                config.BayerPattern = rb24bitRGB.Checked ? BayerPattern.RGB : BayerPattern.BGR;
            }

		    config.NumberOfFrames = GetTotalImages();

            config.ExposureCallback = GetCurrentImageExposure;
            config.TimeStampCallback = GetCurrentImageTimeStamp;
            config.GainCallback = GetCurrentImageGain;
            config.GammaCallback = GetCurrentImageGamma;
            config.MassagesCallback = GetCurrentExampleMassages;
            config.CustomGainCallback = GetCurrentExampleCustomGain;
            config.MainStreamMetadata.Add("Name1", "Христо");
            config.MainStreamMetadata.Add("Name2", "Frédéric");
            config.MainStreamMetadata.Add("Name3", "好的茶");

		    if (cbxZeroTicks.Checked)
		    {
		        config.MainStreamCustomClock = new CustomClockConfig()
		        {
		            ClockFrequency = 1,
		            ClockTicksCallback = () => 0,
		            TicksTimingAccuracy = 1
		        };
		        config.CalibrationStreamCustomClock = new CustomClockConfig()
		        {
		            ClockFrequency = 1,
		            ClockTicksCallback = () => 0,
		            TicksTimingAccuracy = 1
		        };
		    }
            var advGen = new AdvGenerator();
            advGen.GenerateaAdv_V2(config, fileName);

			ActionFileOperation(fileName);
		}

		private void SaveAdvVer1Sample()
		{
            string fileName = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + @"Filename.adv");

            if (File.Exists(fileName))
            {
                if (MessageBox.Show(string.Format("Output file exists:\r\n\r\n{0}\r\n\r\nOverwrite?", fileName), "Confirmation", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;

                File.Delete(fileName);
            }


			var recorder = new Obsolete.AdvVer1.AdvRecorder();

			// First set the values of the standard file metadata
			recorder.FileMetaData.RecorderName = "Genika";
			recorder.FileMetaData.RecorderVersion = "x.y.z";
			recorder.FileMetaData.RecorderTimerFirmwareVersion = "a.b.c";

			recorder.FileMetaData.CameraModel = "Flea3 FL3-FW-03S3M";
			recorder.FileMetaData.CameraSerialNumber = "10210906";
			recorder.FileMetaData.CameraVendorNumber = "Point Grey Research";
			recorder.FileMetaData.CameraSensorInfo = "Sony ICX414AL (1/2\" 648x488 CCD)";
			recorder.FileMetaData.CameraSensorResolution = "648x488";
			recorder.FileMetaData.CameraFirmwareVersion = "1.22.3.0";
			recorder.FileMetaData.CameraFirmwareBuildTime = "Mon Dec 28 20:15:45 2009";
			recorder.FileMetaData.CameraDriverVersion = "2.2.1.6";

			// Then define additional metadata, if required
			recorder.FileMetaData.AddUserTag("TELESCOPE-NAME", "Large Telescope");
			recorder.FileMetaData.AddUserTag("TELESCOPE-FL", "8300");
			recorder.FileMetaData.AddUserTag("TELESCOPE-FD", "6.5");
			recorder.FileMetaData.AddUserTag("CAMERA-DIGITAL-SAMPLIG", "xxx");
			recorder.FileMetaData.AddUserTag("CAMERA-HDR-RESPONSE", "yyy");
			recorder.FileMetaData.AddUserTag("CAMERA-OPTICAL-RESOLUTION", "zzz");

			if (cbxLocationData.Checked)
			{
				recorder.LocationData.LongitudeWgs84 = "150*38'27.7\"";
				recorder.LocationData.LatitudeWgs84 = "-33*39'49.3\"";
				recorder.LocationData.AltitudeMsl = "284.4M";
				recorder.LocationData.MslWgs84Offset = "22.4M";
				recorder.LocationData.GpsHdop = "0.7";
			}

			// Define the image size and bit depth
			byte dynaBits = 16;
			if (rbPixel16.Checked) dynaBits = 16;
			else if (rbPixel12.Checked) dynaBits = 12;
			else if (rbPixel8.Checked) dynaBits = 8;

            recorder.ImageConfig.SetImageParameters(640, 480, dynaBits);

			// By default no status section values will be recorded. The user must enable the ones they need recorded and 
			// can also define additional status parameters to be recorded with each video frame
			recorder.StatusSectionConfig.RecordGain = true;
			recorder.StatusSectionConfig.RecordGamma = true;
			int customTagIdCustomGain = recorder.StatusSectionConfig.AddDefineTag("EXAMPLE-GAIN", AdvTagType.UInt32);
			int customTagIdMessages = recorder.StatusSectionConfig.AddDefineTag("EXAMPLE-MESSAGES", AdvTagType.List16OfAnsiString255);

			recorder.StartRecordingNewFile(fileName);

			Obsolete.AdvStatusEntry status = new Obsolete.AdvStatusEntry();
			status.AdditionalStatusTags = new object[2];

			int imagesCount = GetTotalImages();
			bool useCompression = cbxCompress.Checked;

			for (int i = 0; i < imagesCount; i++)
			{
				// NOTE: Moking up some test data
				uint exposure = GetCurrentImageExposure(i);
				DateTime timestamp = GetCurrentImageTimeStamp(i);
				status.Gain = GetCurrentImageGain(i);
				status.Gamma = GetCurrentImageGamma(i);
				status.AdditionalStatusTags[customTagIdMessages] = GetCurrentExampleMassages(i);
				status.AdditionalStatusTags[customTagIdCustomGain] = GetCurrentExampleCustomGain(i);

				if (rb16BitUShort.Checked)
				{
                    ushort[] imagePixels = imageGenerator.GetCurrentImageBytesInt16(i, dynaBits);

					recorder.AddVideoFrame(
						imagePixels,

						// NOTE: Use with caution! Using compression is slower and may not work at high frame rates 
						// i.e. it may take longer to compress the data than for the next image to arrive on the buffer
						useCompression,

						AdvTimeStamp.FromDateTime(timestamp),
						exposure,
						status);
				}
				else if (rb16BitByte.Checked)
				{
                    byte[] imageBytes = imageGenerator.GetCurrentImageBytes(i, dynaBits);

					recorder.AddVideoFrame(
						imageBytes,

						// NOTE: Use with caution! Using compression is slower and may not work at high frame rates 
						// i.e. it may take longer to compress the data than for the next image to arrive on the buffer
						useCompression,
						AdvImageData.PixelDepth16Bit,
						AdvTimeStamp.FromDateTime(timestamp),
						exposure,
						status);
				}
				else if (rb8BitByte.Checked)
				{
                    byte[] imageBytes = imageGenerator.GetCurrentImageBytes(i, dynaBits);

					recorder.AddVideoFrame(
						imageBytes,

						// NOTE: Use with caution! Using compression is slower and may not work at high frame rates 
						// i.e. it may take longer to compress the data than for the next image to arrive on the buffer
						useCompression,
						AdvImageData.PixelDepth8Bit,
						AdvTimeStamp.FromDateTime(timestamp),
						exposure,
						status);
				}
			}

			recorder.FinishRecording();
		    ActionFileOperation(fileName);
		}

	    private void ActionFileOperation(string fileName)
	    {
            var frm = new frmChooseGeneratedFileAction(fileName);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                switch (frm.SelectedOperation)
                {
                    case frmChooseGeneratedFileAction.FileOperation.ViewStructure:
                        LoadFileStructure(fileName);
                        break;

                    case frmChooseGeneratedFileAction.FileOperation.PlayVideo:
                        PlayFile(fileName);
                        break;
                }
            }
	    }

	    private void LoadFileStructure(string fileName)
	    {
	        using (var file = new AdvFile2(fileName))
	        {
                var pixels = file.GetMainFramePixels(0);    
	        }	        
	    }

        private void PlayFile(string fileName)
        {

        }

		private int GetTotalImages()
		{
			// TODO: In this file conversion example, return the number of images to be recorded
			return 1;
		}

		private uint GetCurrentImageExposure(int frameId)
		{
			// TODO: Get the image exposure in 1/10-th of milliseconds
			return 400;
		}

		private DateTime GetCurrentImageTimeStamp(int frameId)
		{
			// TODO: Get the image timestamp. Alternatevly return windows Ticks or year/month/day/hour/min/sec/milliseconds
		    return new DateTime(2016, 06, 06, 0, 24, 15);
		}

		private float GetCurrentImageGamma(int frameId)
		{
			// TODO: Get the image gamma
			return 1.0f;
		}

		private float GetCurrentImageGain(int frameId)
		{
			// TODO: Get the image gain in dB
			return 36.0f;
		}

		private string[] GetCurrentExampleMassages(int frameId)
		{
			// TODO: Get the image custom defined "EXAMPLE-MESSAGES" value.
			return new string[] { "Message 1", "Message 2", "Message 3" }; ;
		}

		private uint GetCurrentExampleCustomGain(int frameId)
		{
			// TODO: e.g. return an integer gain value reported by the camera which cannot be converted to dB
			return 0x293;
		}

        private void OnImageFormatChanged(object sender, EventArgs e)
		{
			if (rb16BitUShort.Checked)
			{
				rbPixel16.Checked = true;
				rbPixel16.Enabled = true;
				rbPixel12.Enabled = true;
				rbPixel8.Enabled = true;
			}
            else if (rb16BitByte.Checked)
            {
                rbPixel16.Checked = true;
                rbPixel16.Enabled = true;
                rbPixel12.Enabled = false;
                rbPixel8.Enabled = false;
            }
            else if (rb12BitByte.Checked)
			{
                rbPixel12.Checked = true;
				rbPixel16.Enabled = false;
				rbPixel12.Enabled = true;
				rbPixel8.Enabled = false;
			}
			else
			{
				rbPixel8.Checked = true;
				rbPixel16.Enabled = false;
				rbPixel12.Enabled = false;
				rbPixel8.Enabled = true;
			}
		}

		private void btnVerifyLibrary_Click(object sender, EventArgs e)
		{
			try
			{
				string version = Library.GetVersion();
				string platformId = Library.GetPlatformId();
			    string bitness = Library.GetLibraryBitness();
				bool is64Bit = Library.Is64BitProcess();
				string path = Library.GetLibraryPath();

			    var fi = new FileInfo(path);

                MessageBox.Show(this,
                    string.Format("CLR Version: {0}\r\nOperating System: {1}\r\n\r\n", Adv.CrossPlatform.ClrVersion, Adv.CrossPlatform.CurrentOSName) + 
                    string.Format("Current process is {3} bit\r\n{6}: v.{0}{5}\r\nAdvLib.Core Platform: {2}\r\n\r\nLocation: {1}\r\n\r\nLast Modified: {4}",
                    version, Path.GetDirectoryName(path), platformId, is64Bit ? "64" : "32", fi.LastWriteTime.ToString("dd-MM-yyyy HH:mm:ss"), bitness, Path.GetFileName(path)), 
                    "AdvLib Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show(this,ex.Message + "\r\n\r\n" + ex.StackTrace, ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void cbxADV1_CheckedChanged(object sender, EventArgs e)
		{
			if (cbxADV1.Checked)
			{
				rb12BitByte.Enabled = false;
				rb16BitUShort.Checked = true;
				rbPixel16.Checked = true;
			}
			else
			{
				rb12BitByte.Enabled = true;
			}
		}

        private void cbxCompress_CheckedChanged(object sender, EventArgs e)
        {
            gbxCompression.Enabled = cbxCompress.Checked;
        }

	    private int m_TotalTestsToRun = 0;

	    private void StartRunningTests(int totalNumberOfTests)
	    {
	        pbar.Visible = true;
	        pbar.Minimum = 0;
	        pbar.Maximum = totalNumberOfTests;
	        pbar.Value = 0;
	        pbar.ForeColor = SystemColors.Highlight;
	        pbar.Update();

	        m_TotalTestsToRun = totalNumberOfTests;

            lblRanTests.Text = string.Format("0/{0}", totalNumberOfTests);
	        lblRanTests.Update();
	    }

        private void ProgressRunningTests(int numberCompleted, int numberFailed)
        {
            pbar.Value = Math.Min(pbar.Maximum, numberCompleted);
            pbar.Update();

            UpdatePBarColor(numberFailed);

            if (numberFailed > 0)
                lblRanTests.Text = string.Format("{0}/{1}            {2} failed!", numberCompleted, m_TotalTestsToRun, numberFailed);
            else
                lblRanTests.Text = string.Format("{0}/{1}", numberCompleted, m_TotalTestsToRun);

            lblRanTests.Update();
        }

	    private void StopRunningTests(int numberFailed)
	    {
	        pbar.Value = pbar.Maximum;

	        UpdatePBarColor(numberFailed);

            if (numberFailed > 0)
                MessageBox.Show(string.Format("{0} tests failed", numberFailed));

            pbar.Update();
	    }

        private void UpdatePBarColor(int numberFailed)
	    {
            if (numberFailed > 0)
                pbar.ForeColor = Color.Red;
            else
                pbar.ForeColor = Color.Lime;
	    }


	    private void btnRunTests_Click(object sender, EventArgs e)
	    {
            try
            {
                string platformId = Library.GetPlatformId();
                string path = Library.GetLibraryPath();

                var fi = new FileInfo(path);

                lblLibVersion.Text = string.Format("{2}, {0}, {1}", platformId, fi.LastWriteTime.ToString("dd-MM-yyyy HH:mm:ss"), Path.GetFileName(path));
                lblLibVersion.Update();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message + "\r\n\r\n" + ex.StackTrace, ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

	        
            var runner = new NUnitTestRunner();
            runner.RunTests(
                (i) => this.Invoke(new Action<int>(StartRunningTests), i),
                (i, j) => this.Invoke(new Action<int, int>(ProgressRunningTests), i, j),
                (i) => this.Invoke(new Action<int>(StopRunningTests), i));
        }

        private void btnGenSpecExample_Click(object sender, EventArgs e)
        {
            string fileName = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + @"SpecExample.adv.bin");

            if (File.Exists(fileName))
            {
                if (MessageBox.Show(string.Format("Output file exists:\r\n\r\n{0}\r\n\r\nOverwrite?", fileName), "Confirmation", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;

                File.Delete(fileName);
            }

            var advGen = new AdvGenerator();
            advGen.GenerateSpecExampleFile(fileName);
        }
	}
}
