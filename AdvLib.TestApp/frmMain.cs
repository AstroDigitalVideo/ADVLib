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
using System.Diagnostics;
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

		    AdvError.ShowMessageBoxErrorMessage = true;
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
		    config.SystemErrorsCallback = GetCurrentSystemErrors;
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
				status.AdditionalStatusTags[customTagIdMessages] = "Test Message";
				status.AdditionalStatusTags[customTagIdCustomGain] = 36.0f;

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
			return 10;
		}

        private string GetCurrentSystemErrors(int frameId)
        {
            if (frameId % 2 == 1)
                return "Odd Frames Sample Error Message";
            else
                return null;
        }

		private uint GetCurrentImageExposure(int frameId)
		{
			// TODO: Get the image exposure in 1/10-th of milliseconds
			return 10000;
		}

		private DateTime GetCurrentImageTimeStamp(int frameId)
		{
			// TODO: Get the image timestamp. Alternatevly return windows Ticks or year/month/day/hour/min/sec/milliseconds
		    return new DateTime(2016, 06, 06, 0, 24, 15).AddSeconds(frameId);
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

            pbar.Error = numberFailed > 0;

            if (numberFailed > 0)
                lblRanTests.Text = string.Format("{0}/{1}            {2} failed!", numberCompleted, m_TotalTestsToRun, numberFailed);
            else
                lblRanTests.Text = string.Format("{0}/{1}", numberCompleted, m_TotalTestsToRun);

            lblRanTests.Update();
        }

	    private void StopRunningTests(int numberFailed)
	    {
	        pbar.Value = pbar.Maximum;

            pbar.Error = numberFailed > 0;

            if (numberFailed > 0)
                MessageBox.Show(string.Format("{0} tests failed", numberFailed));

            pbar.Update();
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
                cbxCopyToClipboard.Checked,
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

            using (var loadedFile = new AdvFile2(fileName))
            {
                Trace.WriteLine(loadedFile.Width);
            }
        }

	    private AdvFile2 loadedFile;

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog(this) == DialogResult.OK)
            {
                if (loadedFile != null)
                {
                    loadedFile.Dispose();
                    loadedFile = null;
                }
                loadedFile = new AdvFile2(openFileDialog2.FileName);
                tbxFileName.Text = openFileDialog2.FileName;
                lblWidth.Text = loadedFile.Width.ToString();
                lblHeight.Text = loadedFile.Height.ToString();
                lblBPP.Text = string.Format("{0} bit, {1}", loadedFile.DataBpp, loadedFile.IsColourImage ? "Colour" : "Monochrome");
                lblMaxPixelValue.Text = loadedFile.MaxPixelValue.ToString();
                lblUtcAccuracy.Text = GetUtcAccuracy(loadedFile.UtcTimestampAccuracyInNanoseconds);

                lblMainFrames.Text = loadedFile.MainSteamInfo.FrameCount.ToString();
                lblMainClockFreq.Text = string.Format("{0} Hz", loadedFile.MainSteamInfo.ClockFrequency);
                lblMainFrameTSAccu.Text = GetFrameTimeStampAccuracy(loadedFile.MainSteamInfo.TimingAccuracy * 1.0 / loadedFile.MainSteamInfo.ClockFrequency, loadedFile.MainSteamInfo.TimingAccuracy);
                LoadMetadata(lvMainStream, loadedFile.MainSteamInfo.MetadataTags);

                lblCalibFrames.Text = loadedFile.CalibrationSteamInfo.FrameCount.ToString();
                lblCalibClockFreq.Text = string.Format("{0} Hz", loadedFile.CalibrationSteamInfo.ClockFrequency);
                lblCalibFrameTSAccu.Text = GetFrameTimeStampAccuracy(loadedFile.CalibrationSteamInfo.TimingAccuracy * 1.0 / loadedFile.CalibrationSteamInfo.ClockFrequency, loadedFile.MainSteamInfo.TimingAccuracy);
                LoadMetadata(lvCalibStream, loadedFile.CalibrationSteamInfo.MetadataTags);

                LoadMetadata(lvImageSectionTags, loadedFile.ImageSectionTags);
                lblCountLayouts.Text = loadedFile.ImageLayouts.Count.ToString();
                lvImageLayoutTags.Tag = loadedFile.ImageLayouts;
                LoadImageLayoutTags(loadedFile.ImageLayouts.Count - 1);

                LoadStatusTags(loadedFile.StatusTagDefinitions);

                LoadMetadata(lvSystemMedata, loadedFile.SystemMetadataTags);
                LoadMetadata(lvUserMetadata, loadedFile.UserMetadataTags);

                if (loadedFile.MainSteamInfo.FrameCount > 0)
                {
                    // Make sure we can read a frame, if any
                    AdvFrameInfo frameInfo;
                    loadedFile.GetMainFramePixels(0, out frameInfo);
                }

                if (loadedFile.MainIndex.Count > 0)
                {
                    var sb = new StringBuilder();
                    sb.AppendFormat("FrameOffset\t\tBytesCount\tElapsedTicks\r\n");
                    foreach (var ie in loadedFile.MainIndex)
                    {
                        sb.AppendFormat("0x{0}\t0x{1}\t{2}\r\n",
                            Convert.ToString(ie.FrameOffset, 16).PadLeft(16, '0'),
                            Convert.ToString(ie.BytesCount, 16).PadLeft(8, '0'), ie.ElapsedTicks);
                    }
                    tbxMainIndex.Text = sb.ToString();
                }

                if (loadedFile.CalibrationIndex.Count > 0)
                {
                    var sb = new StringBuilder();
                    sb.AppendFormat("FrameOffset\t\tBytesCount\tElapsedTicks\r\n");
                    foreach (var ie in loadedFile.CalibrationIndex)
                    {
                        sb.AppendFormat("0x{0}\t0x{1}\t{2}\r\n", Convert.ToString(ie.FrameOffset, 16).PadLeft(16, '0'), Convert.ToString(ie.BytesCount, 16).PadLeft(8, '0'), ie.ElapsedTicks);
                    }
                    tbxCalibIndex.Text = sb.ToString();
                }
            }
        }

	    private int currLayoutId;

	    private void LoadImageLayoutTags(int imageLayoutId)
	    {
	        var layoutData = lvImageLayoutTags.Tag as List<ImageLayoutDefinition>;
            lvImageLayoutTags.Items.Clear();
	        lblImageLayoutId.Text = "";
            lblImageLayoutBPP.Text = "";

	        if (layoutData != null)
	        {
	            currLayoutId = imageLayoutId;
	            if (currLayoutId >= 0 && currLayoutId < layoutData.Count)
	            {
                    lblImageLayoutId.Text = layoutData[currLayoutId].LayoutId.ToString();
                    lblImageLayoutBPP.Text = string.Format("{0} bit", layoutData[currLayoutId].Bpp);
	                LoadMetadata(lvImageLayoutTags, layoutData[currLayoutId].ImageLayoutTags);
	            }

                btnPrevLayout.Enabled = currLayoutId > 0;
                btnNextLayout.Enabled = currLayoutId < layoutData.Count -1;
	        }
	        else
	        {
	            btnPrevLayout.Enabled = false;
                btnNextLayout.Enabled = false;
	        }
	    }

        private void btnPrevLayout_Click(object sender, EventArgs e)
        {
            LoadImageLayoutTags(Math.Max(0, currLayoutId - 1));
        }

        private void btnNextLayout_Click(object sender, EventArgs e)
        {
            var layoutData = lvImageLayoutTags.Tag as List<ImageLayoutDefinition>;
            if (layoutData != null)
            {
                LoadImageLayoutTags(Math.Min(layoutData.Count - 1, currLayoutId + 1));
            }
        }

	    private void LoadMetadata(ListView lv, Dictionary<string, string> data)
	    {
            lv.Items.Clear();

	        foreach (var entry in data)
	        {
	            var itm = lv.Items.Add(entry.Key);
	            itm.SubItems.Add(entry.Value);
	        }
	    }

        private void LoadStatusTags(List<Tuple<string, uint, Adv2TagType>> data)
        {
            lvlStatusTags.Items.Clear();

            foreach (var entry in data)
            {
                var itm = lvlStatusTags.Items.Add(entry.Item1);
                itm.SubItems.Add(entry.Item2.ToString());
                itm.SubItems.Add(entry.Item3.ToString());
            }
        }

	    private string GetUtcAccuracy(long nanoSecs)
	    {
            var secs = nanoSecs / 1e9;
            if (secs >= 0.1)
            {
                return string.Format("{0} sec ({1})", secs.ToString("0.00").TrimEnd(new char[] { '0', '.' }), nanoSecs);
            }
	        var miliSecs = nanoSecs /1e6;
	        if (miliSecs >= 0.01)
	        {
                return string.Format("{0} ms ({1})", miliSecs.ToString("0.00").TrimEnd(new char[] { '0', '.' }), nanoSecs);
	        }
            var microSec = nanoSecs / 1e3;
            if (microSec >= 0.01)
            {
                return string.Format("{0} micro sec ({1})", microSec.ToString("0.00").TrimEnd(new char[] { '0', '.' }), nanoSecs);
            }

            return string.Format("{0} ns", nanoSecs);
	    }

	    private string GetFrameTimeStampAccuracy(double accuSec, int timeAccuTicks)
	    {
            if (accuSec >= 0.1)
            {
                return string.Format("{0} sec ({1})", accuSec.ToString("0.00").TrimEnd(new char[] { '0', '.' }), timeAccuTicks);
            }
            var miliSecs = accuSec * 1e3;
            if (miliSecs >= 0.01)
            {
                return string.Format("{0} ms ({1})", miliSecs.ToString("0.00").TrimEnd(new char[] { '0', '.' }), timeAccuTicks);
            }
            var microSec = accuSec * 1e6;
            if (microSec >= 0.01)
            {
                return string.Format("{0} micro sec ({1})", microSec.ToString("0.00").TrimEnd(new char[] { '0', '.' }), timeAccuTicks);
            }

            var nanoSec = accuSec * 1e9;
            return string.Format("{0} ns ({1})", nanoSec.ToString("0.000000").TrimEnd(new char[] { '0', '.' }), timeAccuTicks);
	    }
	}
}
