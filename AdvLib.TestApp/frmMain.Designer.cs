namespace AdvLibTestApp
{
	partial class frmMain
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.button1 = new System.Windows.Forms.Button();
            this.cbxLocationData = new System.Windows.Forms.CheckBox();
            this.rb16BitUShort = new System.Windows.Forms.RadioButton();
            this.rb16BitByte = new System.Windows.Forms.RadioButton();
            this.rb8BitByte = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rb24bitBGR = new System.Windows.Forms.RadioButton();
            this.rb24bitRGB = new System.Windows.Forms.RadioButton();
            this.rb12BitByte = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbPixel8 = new System.Windows.Forms.RadioButton();
            this.rbPixel12 = new System.Windows.Forms.RadioButton();
            this.nudMaxPixelValue = new System.Windows.Forms.NumericUpDown();
            this.rbPixel16 = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.cbxCompress = new System.Windows.Forms.CheckBox();
            this.btnVerifyLibrary = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.cbxADV1 = new System.Windows.Forms.CheckBox();
            this.tcTabs = new System.Windows.Forms.TabControl();
            this.tabRecorder = new System.Windows.Forms.TabPage();
            this.cbxCopyToClipboard = new System.Windows.Forms.CheckBox();
            this.btnGenSpecExample = new System.Windows.Forms.Button();
            this.lblLibVersion = new System.Windows.Forms.Label();
            this.lblRanTests = new System.Windows.Forms.Label();
            this.btnRunTests = new System.Windows.Forms.Button();
            this.gbxCompression = new System.Windows.Forms.GroupBox();
            this.rbLagarith16 = new System.Windows.Forms.RadioButton();
            this.rbQuickLZ = new System.Windows.Forms.RadioButton();
            this.cbxZeroTicks = new System.Windows.Forms.CheckBox();
            this.tabStructureViewer = new System.Windows.Forms.TabPage();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.tabPlayer = new System.Windows.Forms.TabPage();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.tcStructureViewer = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.tabImageSection = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblWidth = new System.Windows.Forms.Label();
            this.lblHeight = new System.Windows.Forms.Label();
            this.lblBPP = new System.Windows.Forms.Label();
            this.lblMaxPixelValue = new System.Windows.Forms.Label();
            this.lblUtcAccuracy = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tbxFileName = new System.Windows.Forms.TextBox();
            this.tabStatusSection = new System.Windows.Forms.TabPage();
            this.tabMetadata = new System.Windows.Forms.TabPage();
            this.tabMainIndex = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lvMainStream = new System.Windows.Forms.ListView();
            this.lblMainFrames = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblMainClockFreq = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.tagName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tagValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblMainFrameTSAccu = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lblCalibFrameTSAccu = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lblCalibClockFreq = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.lblCalibFrames = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.lvCalibStream = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvImageLayoutTags = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label6 = new System.Windows.Forms.Label();
            this.lvImageSectionTags = new System.Windows.Forms.ListView();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label12 = new System.Windows.Forms.Label();
            this.btnPrevLayout = new System.Windows.Forms.Button();
            this.btnNextLayout = new System.Windows.Forms.Button();
            this.lblImageLayoutId = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.lblImageLayoutBPP = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.lblCountLayouts = new System.Windows.Forms.Label();
            this.lvlStatusTags = new System.Windows.Forms.ListView();
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label14 = new System.Windows.Forms.Label();
            this.lvSystemMedata = new System.Windows.Forms.ListView();
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvUserMetadata = new System.Windows.Forms.ListView();
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tbxMainIndex = new System.Windows.Forms.TextBox();
            this.pbar = new AdvLib.TestApp.UnitTestProgressBar();
            this.tabCalibrationIndex = new System.Windows.Forms.TabPage();
            this.tbxCalibIndex = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxPixelValue)).BeginInit();
            this.tcTabs.SuspendLayout();
            this.tabRecorder.SuspendLayout();
            this.gbxCompression.SuspendLayout();
            this.tabStructureViewer.SuspendLayout();
            this.tcStructureViewer.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.tabImageSection.SuspendLayout();
            this.tabStatusSection.SuspendLayout();
            this.tabMetadata.SuspendLayout();
            this.tabMainIndex.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tabCalibrationIndex.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(17, 244);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(132, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Generate Test File";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // cbxLocationData
            // 
            this.cbxLocationData.AutoSize = true;
            this.cbxLocationData.Location = new System.Drawing.Point(399, 33);
            this.cbxLocationData.Name = "cbxLocationData";
            this.cbxLocationData.Size = new System.Drawing.Size(115, 17);
            this.cbxLocationData.TabIndex = 1;
            this.cbxLocationData.Text = "Save location data";
            this.cbxLocationData.UseVisualStyleBackColor = true;
            // 
            // rb16BitUShort
            // 
            this.rb16BitUShort.AutoSize = true;
            this.rb16BitUShort.Checked = true;
            this.rb16BitUShort.Location = new System.Drawing.Point(18, 24);
            this.rb16BitUShort.Name = "rb16BitUShort";
            this.rb16BitUShort.Size = new System.Drawing.Size(95, 17);
            this.rb16BitUShort.TabIndex = 2;
            this.rb16BitUShort.TabStop = true;
            this.rb16BitUShort.Text = "16-bit, ushort[] ";
            this.rb16BitUShort.UseVisualStyleBackColor = true;
            this.rb16BitUShort.CheckedChanged += new System.EventHandler(this.OnImageFormatChanged);
            // 
            // rb16BitByte
            // 
            this.rb16BitByte.AutoSize = true;
            this.rb16BitByte.Location = new System.Drawing.Point(18, 47);
            this.rb16BitByte.Name = "rb16BitByte";
            this.rb16BitByte.Size = new System.Drawing.Size(142, 17);
            this.rb16BitByte.TabIndex = 3;
            this.rb16BitByte.Text = "16-bit, little-endian, byte[]";
            this.rb16BitByte.UseVisualStyleBackColor = true;
            this.rb16BitByte.CheckedChanged += new System.EventHandler(this.OnImageFormatChanged);
            // 
            // rb8BitByte
            // 
            this.rb8BitByte.AutoSize = true;
            this.rb8BitByte.Location = new System.Drawing.Point(18, 93);
            this.rb8BitByte.Name = "rb8BitByte";
            this.rb8BitByte.Size = new System.Drawing.Size(77, 17);
            this.rb8BitByte.TabIndex = 4;
            this.rb8BitByte.Text = "8-bit, byte[]";
            this.rb8BitByte.UseVisualStyleBackColor = true;
            this.rb8BitByte.CheckedChanged += new System.EventHandler(this.OnImageFormatChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rb24bitBGR);
            this.groupBox1.Controls.Add(this.rb24bitRGB);
            this.groupBox1.Controls.Add(this.rb12BitByte);
            this.groupBox1.Controls.Add(this.rb16BitUShort);
            this.groupBox1.Controls.Add(this.rb8BitByte);
            this.groupBox1.Controls.Add(this.rb16BitByte);
            this.groupBox1.Location = new System.Drawing.Point(17, 33);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(184, 171);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Raw Source Pixel Format";
            // 
            // rb24bitBGR
            // 
            this.rb24bitBGR.AutoSize = true;
            this.rb24bitBGR.Location = new System.Drawing.Point(18, 139);
            this.rb24bitBGR.Name = "rb24bitBGR";
            this.rb24bitBGR.Size = new System.Drawing.Size(109, 17);
            this.rb24bitBGR.TabIndex = 7;
            this.rb24bitBGR.Text = "24-bit BGR, byte[]";
            this.rb24bitBGR.UseVisualStyleBackColor = true;
            this.rb24bitBGR.CheckedChanged += new System.EventHandler(this.OnImageFormatChanged);
            // 
            // rb24bitRGB
            // 
            this.rb24bitRGB.AutoSize = true;
            this.rb24bitRGB.Location = new System.Drawing.Point(18, 116);
            this.rb24bitRGB.Name = "rb24bitRGB";
            this.rb24bitRGB.Size = new System.Drawing.Size(109, 17);
            this.rb24bitRGB.TabIndex = 6;
            this.rb24bitRGB.Text = "24-bit RGB, byte[]";
            this.rb24bitRGB.UseVisualStyleBackColor = true;
            this.rb24bitRGB.CheckedChanged += new System.EventHandler(this.OnImageFormatChanged);
            // 
            // rb12BitByte
            // 
            this.rb12BitByte.AutoSize = true;
            this.rb12BitByte.Location = new System.Drawing.Point(18, 70);
            this.rb12BitByte.Name = "rb12BitByte";
            this.rb12BitByte.Size = new System.Drawing.Size(125, 17);
            this.rb12BitByte.TabIndex = 5;
            this.rb12BitByte.Text = "12-bit, packed, byte[]";
            this.rb12BitByte.UseVisualStyleBackColor = true;
            this.rb12BitByte.CheckedChanged += new System.EventHandler(this.OnImageFormatChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbPixel8);
            this.groupBox2.Controls.Add(this.rbPixel12);
            this.groupBox2.Controls.Add(this.nudMaxPixelValue);
            this.groupBox2.Controls.Add(this.rbPixel16);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(207, 33);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(161, 171);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Image BPP (Bits per Pixel)";
            // 
            // rbPixel8
            // 
            this.rbPixel8.AutoSize = true;
            this.rbPixel8.Location = new System.Drawing.Point(15, 70);
            this.rbPixel8.Name = "rbPixel8";
            this.rbPixel8.Size = new System.Drawing.Size(45, 17);
            this.rbPixel8.TabIndex = 2;
            this.rbPixel8.Text = "8-bit";
            this.rbPixel8.UseVisualStyleBackColor = true;
            // 
            // rbPixel12
            // 
            this.rbPixel12.AutoSize = true;
            this.rbPixel12.Location = new System.Drawing.Point(15, 47);
            this.rbPixel12.Name = "rbPixel12";
            this.rbPixel12.Size = new System.Drawing.Size(51, 17);
            this.rbPixel12.TabIndex = 1;
            this.rbPixel12.Text = "12-bit";
            this.rbPixel12.UseVisualStyleBackColor = true;
            // 
            // nudMaxPixelValue
            // 
            this.nudMaxPixelValue.Location = new System.Drawing.Point(15, 128);
            this.nudMaxPixelValue.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudMaxPixelValue.Name = "nudMaxPixelValue";
            this.nudMaxPixelValue.Size = new System.Drawing.Size(64, 20);
            this.nudMaxPixelValue.TabIndex = 12;
            // 
            // rbPixel16
            // 
            this.rbPixel16.AutoSize = true;
            this.rbPixel16.Checked = true;
            this.rbPixel16.Location = new System.Drawing.Point(15, 24);
            this.rbPixel16.Name = "rbPixel16";
            this.rbPixel16.Size = new System.Drawing.Size(51, 17);
            this.rbPixel16.TabIndex = 0;
            this.rbPixel16.TabStop = true;
            this.rbPixel16.Text = "16-bit";
            this.rbPixel16.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 107);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Max Pixel Value:";
            // 
            // cbxCompress
            // 
            this.cbxCompress.AutoSize = true;
            this.cbxCompress.Location = new System.Drawing.Point(399, 81);
            this.cbxCompress.Name = "cbxCompress";
            this.cbxCompress.Size = new System.Drawing.Size(140, 17);
            this.cbxCompress.TabIndex = 8;
            this.cbxCompress.Text = "Use Image Compression";
            this.cbxCompress.UseVisualStyleBackColor = true;
            this.cbxCompress.CheckedChanged += new System.EventHandler(this.cbxCompress_CheckedChanged);
            // 
            // btnVerifyLibrary
            // 
            this.btnVerifyLibrary.Location = new System.Drawing.Point(170, 244);
            this.btnVerifyLibrary.Name = "btnVerifyLibrary";
            this.btnVerifyLibrary.Size = new System.Drawing.Size(132, 23);
            this.btnVerifyLibrary.TabIndex = 9;
            this.btnVerifyLibrary.Text = "Verify Library";
            this.btnVerifyLibrary.UseVisualStyleBackColor = true;
            this.btnVerifyLibrary.Click += new System.EventHandler(this.btnVerifyLibrary_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "dll";
            this.openFileDialog1.FileName = "Open Adv.Core dll";
            this.openFileDialog1.Filter = "Dynamic Link Libraries (*.dll)|*.dll";
            // 
            // cbxADV1
            // 
            this.cbxADV1.AutoSize = true;
            this.cbxADV1.Location = new System.Drawing.Point(399, 178);
            this.cbxADV1.Name = "cbxADV1";
            this.cbxADV1.Size = new System.Drawing.Size(117, 17);
            this.cbxADV1.TabIndex = 10;
            this.cbxADV1.Text = "Use ADV Version 1";
            this.cbxADV1.UseVisualStyleBackColor = true;
            this.cbxADV1.CheckedChanged += new System.EventHandler(this.cbxADV1_CheckedChanged);
            // 
            // tcTabs
            // 
            this.tcTabs.Controls.Add(this.tabRecorder);
            this.tcTabs.Controls.Add(this.tabStructureViewer);
            this.tcTabs.Controls.Add(this.tabPlayer);
            this.tcTabs.Location = new System.Drawing.Point(12, 12);
            this.tcTabs.Name = "tcTabs";
            this.tcTabs.SelectedIndex = 0;
            this.tcTabs.Size = new System.Drawing.Size(566, 386);
            this.tcTabs.TabIndex = 13;
            // 
            // tabRecorder
            // 
            this.tabRecorder.Controls.Add(this.cbxCopyToClipboard);
            this.tabRecorder.Controls.Add(this.btnGenSpecExample);
            this.tabRecorder.Controls.Add(this.lblLibVersion);
            this.tabRecorder.Controls.Add(this.lblRanTests);
            this.tabRecorder.Controls.Add(this.pbar);
            this.tabRecorder.Controls.Add(this.btnRunTests);
            this.tabRecorder.Controls.Add(this.gbxCompression);
            this.tabRecorder.Controls.Add(this.cbxZeroTicks);
            this.tabRecorder.Controls.Add(this.cbxLocationData);
            this.tabRecorder.Controls.Add(this.button1);
            this.tabRecorder.Controls.Add(this.groupBox1);
            this.tabRecorder.Controls.Add(this.cbxADV1);
            this.tabRecorder.Controls.Add(this.groupBox2);
            this.tabRecorder.Controls.Add(this.btnVerifyLibrary);
            this.tabRecorder.Controls.Add(this.cbxCompress);
            this.tabRecorder.Location = new System.Drawing.Point(4, 22);
            this.tabRecorder.Name = "tabRecorder";
            this.tabRecorder.Padding = new System.Windows.Forms.Padding(3);
            this.tabRecorder.Size = new System.Drawing.Size(558, 360);
            this.tabRecorder.TabIndex = 0;
            this.tabRecorder.Text = "Recorder";
            this.tabRecorder.UseVisualStyleBackColor = true;
            // 
            // cbxCopyToClipboard
            // 
            this.cbxCopyToClipboard.AutoSize = true;
            this.cbxCopyToClipboard.Checked = true;
            this.cbxCopyToClipboard.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxCopyToClipboard.Location = new System.Drawing.Point(16, 340);
            this.cbxCopyToClipboard.Name = "cbxCopyToClipboard";
            this.cbxCopyToClipboard.Size = new System.Drawing.Size(147, 17);
            this.cbxCopyToClipboard.TabIndex = 20;
            this.cbxCopyToClipboard.Text = "Copy Results to Clipboard";
            this.cbxCopyToClipboard.UseVisualStyleBackColor = true;
            // 
            // btnGenSpecExample
            // 
            this.btnGenSpecExample.Location = new System.Drawing.Point(387, 244);
            this.btnGenSpecExample.Name = "btnGenSpecExample";
            this.btnGenSpecExample.Size = new System.Drawing.Size(152, 23);
            this.btnGenSpecExample.TabIndex = 19;
            this.btnGenSpecExample.Text = "Generate Spec Example File";
            this.btnGenSpecExample.UseVisualStyleBackColor = true;
            this.btnGenSpecExample.Click += new System.EventHandler(this.btnGenSpecExample_Click);
            // 
            // lblLibVersion
            // 
            this.lblLibVersion.AutoSize = true;
            this.lblLibVersion.Location = new System.Drawing.Point(167, 295);
            this.lblLibVersion.Name = "lblLibVersion";
            this.lblLibVersion.Size = new System.Drawing.Size(0, 13);
            this.lblLibVersion.TabIndex = 18;
            // 
            // lblRanTests
            // 
            this.lblRanTests.AutoSize = true;
            this.lblRanTests.Location = new System.Drawing.Point(167, 341);
            this.lblRanTests.Name = "lblRanTests";
            this.lblRanTests.Size = new System.Drawing.Size(0, 13);
            this.lblRanTests.TabIndex = 17;
            // 
            // btnRunTests
            // 
            this.btnRunTests.Location = new System.Drawing.Point(17, 313);
            this.btnRunTests.Name = "btnRunTests";
            this.btnRunTests.Size = new System.Drawing.Size(132, 23);
            this.btnRunTests.TabIndex = 15;
            this.btnRunTests.Text = "Run All Tests";
            this.btnRunTests.UseVisualStyleBackColor = true;
            this.btnRunTests.Click += new System.EventHandler(this.btnRunTests_Click);
            // 
            // gbxCompression
            // 
            this.gbxCompression.Controls.Add(this.rbLagarith16);
            this.gbxCompression.Controls.Add(this.rbQuickLZ);
            this.gbxCompression.Enabled = false;
            this.gbxCompression.Location = new System.Drawing.Point(416, 100);
            this.gbxCompression.Name = "gbxCompression";
            this.gbxCompression.Size = new System.Drawing.Size(123, 68);
            this.gbxCompression.TabIndex = 14;
            this.gbxCompression.TabStop = false;
            // 
            // rbLagarith16
            // 
            this.rbLagarith16.AutoSize = true;
            this.rbLagarith16.Location = new System.Drawing.Point(13, 42);
            this.rbLagarith16.Name = "rbLagarith16";
            this.rbLagarith16.Size = new System.Drawing.Size(75, 17);
            this.rbLagarith16.TabIndex = 1;
            this.rbLagarith16.Text = "Lagarith16";
            this.rbLagarith16.UseVisualStyleBackColor = true;
            // 
            // rbQuickLZ
            // 
            this.rbQuickLZ.AutoSize = true;
            this.rbQuickLZ.Checked = true;
            this.rbQuickLZ.Location = new System.Drawing.Point(13, 19);
            this.rbQuickLZ.Name = "rbQuickLZ";
            this.rbQuickLZ.Size = new System.Drawing.Size(66, 17);
            this.rbQuickLZ.TabIndex = 0;
            this.rbQuickLZ.TabStop = true;
            this.rbQuickLZ.Text = "QuickLZ";
            this.rbQuickLZ.UseVisualStyleBackColor = true;
            // 
            // cbxZeroTicks
            // 
            this.cbxZeroTicks.AutoSize = true;
            this.cbxZeroTicks.Location = new System.Drawing.Point(399, 58);
            this.cbxZeroTicks.Name = "cbxZeroTicks";
            this.cbxZeroTicks.Size = new System.Drawing.Size(128, 17);
            this.cbxZeroTicks.TabIndex = 13;
            this.cbxZeroTicks.Text = "Save zero clock ticks";
            this.cbxZeroTicks.UseVisualStyleBackColor = true;
            // 
            // tabStructureViewer
            // 
            this.tabStructureViewer.Controls.Add(this.tcStructureViewer);
            this.tabStructureViewer.Location = new System.Drawing.Point(4, 22);
            this.tabStructureViewer.Name = "tabStructureViewer";
            this.tabStructureViewer.Padding = new System.Windows.Forms.Padding(3);
            this.tabStructureViewer.Size = new System.Drawing.Size(558, 360);
            this.tabStructureViewer.TabIndex = 1;
            this.tabStructureViewer.Text = "Structure Viewer";
            this.tabStructureViewer.UseVisualStyleBackColor = true;
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Location = new System.Drawing.Point(9, 6);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(132, 23);
            this.btnOpenFile.TabIndex = 10;
            this.btnOpenFile.Text = "Open File";
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // tabPlayer
            // 
            this.tabPlayer.Location = new System.Drawing.Point(4, 22);
            this.tabPlayer.Name = "tabPlayer";
            this.tabPlayer.Size = new System.Drawing.Size(558, 360);
            this.tabPlayer.TabIndex = 2;
            this.tabPlayer.Text = "Player";
            this.tabPlayer.UseVisualStyleBackColor = true;
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.DefaultExt = "adv";
            this.openFileDialog2.FileName = "openFileDialog2";
            this.openFileDialog2.Filter = "ADV (*.adv)|*.adv";
            // 
            // tcStructureViewer
            // 
            this.tcStructureViewer.Controls.Add(this.tabGeneral);
            this.tcStructureViewer.Controls.Add(this.tabImageSection);
            this.tcStructureViewer.Controls.Add(this.tabStatusSection);
            this.tcStructureViewer.Controls.Add(this.tabMetadata);
            this.tcStructureViewer.Controls.Add(this.tabMainIndex);
            this.tcStructureViewer.Controls.Add(this.tabCalibrationIndex);
            this.tcStructureViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcStructureViewer.Location = new System.Drawing.Point(3, 3);
            this.tcStructureViewer.Name = "tcStructureViewer";
            this.tcStructureViewer.SelectedIndex = 0;
            this.tcStructureViewer.Size = new System.Drawing.Size(552, 354);
            this.tcStructureViewer.TabIndex = 11;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.groupBox4);
            this.tabGeneral.Controls.Add(this.groupBox3);
            this.tabGeneral.Controls.Add(this.tbxFileName);
            this.tabGeneral.Controls.Add(this.lblUtcAccuracy);
            this.tabGeneral.Controls.Add(this.label7);
            this.tabGeneral.Controls.Add(this.lblMaxPixelValue);
            this.tabGeneral.Controls.Add(this.lblBPP);
            this.tabGeneral.Controls.Add(this.lblHeight);
            this.tabGeneral.Controls.Add(this.lblWidth);
            this.tabGeneral.Controls.Add(this.label5);
            this.tabGeneral.Controls.Add(this.label4);
            this.tabGeneral.Controls.Add(this.label3);
            this.tabGeneral.Controls.Add(this.label2);
            this.tabGeneral.Controls.Add(this.btnOpenFile);
            this.tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(544, 328);
            this.tabGeneral.TabIndex = 0;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // tabImageSection
            // 
            this.tabImageSection.Controls.Add(this.lblCountLayouts);
            this.tabImageSection.Controls.Add(this.lblImageLayoutBPP);
            this.tabImageSection.Controls.Add(this.label18);
            this.tabImageSection.Controls.Add(this.lblImageLayoutId);
            this.tabImageSection.Controls.Add(this.label16);
            this.tabImageSection.Controls.Add(this.btnNextLayout);
            this.tabImageSection.Controls.Add(this.btnPrevLayout);
            this.tabImageSection.Controls.Add(this.label12);
            this.tabImageSection.Controls.Add(this.lvImageSectionTags);
            this.tabImageSection.Controls.Add(this.label6);
            this.tabImageSection.Controls.Add(this.lvImageLayoutTags);
            this.tabImageSection.Location = new System.Drawing.Point(4, 22);
            this.tabImageSection.Name = "tabImageSection";
            this.tabImageSection.Padding = new System.Windows.Forms.Padding(3);
            this.tabImageSection.Size = new System.Drawing.Size(544, 328);
            this.tabImageSection.TabIndex = 1;
            this.tabImageSection.Text = "Image Section";
            this.tabImageSection.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Width:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Height:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(367, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "BPP:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(130, 65);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Max Pixel Value:";
            // 
            // lblWidth
            // 
            this.lblWidth.AutoSize = true;
            this.lblWidth.ForeColor = System.Drawing.Color.Navy;
            this.lblWidth.Location = new System.Drawing.Point(47, 65);
            this.lblWidth.Name = "lblWidth";
            this.lblWidth.Size = new System.Drawing.Size(0, 13);
            this.lblWidth.TabIndex = 16;
            // 
            // lblHeight
            // 
            this.lblHeight.AutoSize = true;
            this.lblHeight.ForeColor = System.Drawing.Color.Navy;
            this.lblHeight.Location = new System.Drawing.Point(48, 88);
            this.lblHeight.Name = "lblHeight";
            this.lblHeight.Size = new System.Drawing.Size(0, 13);
            this.lblHeight.TabIndex = 17;
            // 
            // lblBPP
            // 
            this.lblBPP.AutoSize = true;
            this.lblBPP.ForeColor = System.Drawing.Color.Navy;
            this.lblBPP.Location = new System.Drawing.Point(395, 65);
            this.lblBPP.Name = "lblBPP";
            this.lblBPP.Size = new System.Drawing.Size(0, 13);
            this.lblBPP.TabIndex = 18;
            // 
            // lblMaxPixelValue
            // 
            this.lblMaxPixelValue.AutoSize = true;
            this.lblMaxPixelValue.ForeColor = System.Drawing.Color.Navy;
            this.lblMaxPixelValue.Location = new System.Drawing.Point(212, 65);
            this.lblMaxPixelValue.Name = "lblMaxPixelValue";
            this.lblMaxPixelValue.Size = new System.Drawing.Size(0, 13);
            this.lblMaxPixelValue.TabIndex = 19;
            // 
            // lblUtcAccuracy
            // 
            this.lblUtcAccuracy.AutoSize = true;
            this.lblUtcAccuracy.ForeColor = System.Drawing.Color.Navy;
            this.lblUtcAccuracy.Location = new System.Drawing.Point(212, 88);
            this.lblUtcAccuracy.Name = "lblUtcAccuracy";
            this.lblUtcAccuracy.Size = new System.Drawing.Size(0, 13);
            this.lblUtcAccuracy.TabIndex = 21;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(135, 88);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(80, 13);
            this.label7.TabIndex = 20;
            this.label7.Text = "UTC Accuracy:";
            // 
            // tbxFileName
            // 
            this.tbxFileName.Location = new System.Drawing.Point(9, 35);
            this.tbxFileName.Name = "tbxFileName";
            this.tbxFileName.ReadOnly = true;
            this.tbxFileName.Size = new System.Drawing.Size(529, 20);
            this.tbxFileName.TabIndex = 22;
            // 
            // tabStatusSection
            // 
            this.tabStatusSection.Controls.Add(this.label14);
            this.tabStatusSection.Controls.Add(this.lvlStatusTags);
            this.tabStatusSection.Location = new System.Drawing.Point(4, 22);
            this.tabStatusSection.Name = "tabStatusSection";
            this.tabStatusSection.Size = new System.Drawing.Size(544, 328);
            this.tabStatusSection.TabIndex = 2;
            this.tabStatusSection.Text = "Status Section";
            this.tabStatusSection.UseVisualStyleBackColor = true;
            // 
            // tabMetadata
            // 
            this.tabMetadata.Controls.Add(this.lvUserMetadata);
            this.tabMetadata.Controls.Add(this.lvSystemMedata);
            this.tabMetadata.Location = new System.Drawing.Point(4, 22);
            this.tabMetadata.Name = "tabMetadata";
            this.tabMetadata.Size = new System.Drawing.Size(544, 328);
            this.tabMetadata.TabIndex = 3;
            this.tabMetadata.Text = "File Metadata";
            this.tabMetadata.UseVisualStyleBackColor = true;
            // 
            // tabMainIndex
            // 
            this.tabMainIndex.Controls.Add(this.tbxMainIndex);
            this.tabMainIndex.Location = new System.Drawing.Point(4, 22);
            this.tabMainIndex.Name = "tabMainIndex";
            this.tabMainIndex.Size = new System.Drawing.Size(544, 328);
            this.tabMainIndex.TabIndex = 4;
            this.tabMainIndex.Text = "Main Index";
            this.tabMainIndex.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lblMainFrameTSAccu);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.lblMainClockFreq);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.lblMainFrames);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.lvMainStream);
            this.groupBox3.Location = new System.Drawing.Point(9, 115);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(265, 204);
            this.groupBox3.TabIndex = 23;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Main Stream Info";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lblCalibFrameTSAccu);
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Controls.Add(this.lblCalibClockFreq);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.lblCalibFrames);
            this.groupBox4.Controls.Add(this.label15);
            this.groupBox4.Controls.Add(this.lvCalibStream);
            this.groupBox4.Location = new System.Drawing.Point(280, 115);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(258, 204);
            this.groupBox4.TabIndex = 24;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Calibration Stream Info";
            // 
            // lvMainStream
            // 
            this.lvMainStream.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.tagName,
            this.tagValue});
            this.lvMainStream.Location = new System.Drawing.Point(7, 80);
            this.lvMainStream.Name = "lvMainStream";
            this.lvMainStream.Size = new System.Drawing.Size(252, 118);
            this.lvMainStream.TabIndex = 0;
            this.lvMainStream.UseCompatibleStateImageBehavior = false;
            this.lvMainStream.View = System.Windows.Forms.View.Details;
            // 
            // lblMainFrames
            // 
            this.lblMainFrames.AutoSize = true;
            this.lblMainFrames.ForeColor = System.Drawing.Color.Navy;
            this.lblMainFrames.Location = new System.Drawing.Point(55, 25);
            this.lblMainFrames.Name = "lblMainFrames";
            this.lblMainFrames.Size = new System.Drawing.Size(0, 13);
            this.lblMainFrames.TabIndex = 18;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 25);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(44, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Frames:";
            // 
            // lblMainClockFreq
            // 
            this.lblMainClockFreq.AutoSize = true;
            this.lblMainClockFreq.ForeColor = System.Drawing.Color.Navy;
            this.lblMainClockFreq.Location = new System.Drawing.Point(143, 25);
            this.lblMainClockFreq.Name = "lblMainClockFreq";
            this.lblMainClockFreq.Size = new System.Drawing.Size(0, 13);
            this.lblMainClockFreq.TabIndex = 20;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(107, 25);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(37, 13);
            this.label9.TabIndex = 19;
            this.label9.Text = "Clock:";
            // 
            // tagName
            // 
            this.tagName.Text = "Name";
            this.tagName.Width = 124;
            // 
            // tagValue
            // 
            this.tagValue.Text = "Value";
            this.tagValue.Width = 111;
            // 
            // lblMainFrameTSAccu
            // 
            this.lblMainFrameTSAccu.AutoSize = true;
            this.lblMainFrameTSAccu.ForeColor = System.Drawing.Color.Navy;
            this.lblMainFrameTSAccu.Location = new System.Drawing.Point(143, 46);
            this.lblMainFrameTSAccu.Name = "lblMainFrameTSAccu";
            this.lblMainFrameTSAccu.Size = new System.Drawing.Size(0, 13);
            this.lblMainFrameTSAccu.TabIndex = 22;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(107, 46);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(35, 13);
            this.label10.TabIndex = 21;
            this.label10.Text = "Accu:";
            // 
            // lblCalibFrameTSAccu
            // 
            this.lblCalibFrameTSAccu.AutoSize = true;
            this.lblCalibFrameTSAccu.ForeColor = System.Drawing.Color.Navy;
            this.lblCalibFrameTSAccu.Location = new System.Drawing.Point(142, 46);
            this.lblCalibFrameTSAccu.Name = "lblCalibFrameTSAccu";
            this.lblCalibFrameTSAccu.Size = new System.Drawing.Size(0, 13);
            this.lblCalibFrameTSAccu.TabIndex = 29;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(106, 46);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(35, 13);
            this.label11.TabIndex = 28;
            this.label11.Text = "Accu:";
            // 
            // lblCalibClockFreq
            // 
            this.lblCalibClockFreq.AutoSize = true;
            this.lblCalibClockFreq.ForeColor = System.Drawing.Color.Navy;
            this.lblCalibClockFreq.Location = new System.Drawing.Point(142, 25);
            this.lblCalibClockFreq.Name = "lblCalibClockFreq";
            this.lblCalibClockFreq.Size = new System.Drawing.Size(0, 13);
            this.lblCalibClockFreq.TabIndex = 27;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(106, 25);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(37, 13);
            this.label13.TabIndex = 26;
            this.label13.Text = "Clock:";
            // 
            // lblCalibFrames
            // 
            this.lblCalibFrames.AutoSize = true;
            this.lblCalibFrames.ForeColor = System.Drawing.Color.Navy;
            this.lblCalibFrames.Location = new System.Drawing.Point(54, 25);
            this.lblCalibFrames.Name = "lblCalibFrames";
            this.lblCalibFrames.Size = new System.Drawing.Size(0, 13);
            this.lblCalibFrames.TabIndex = 25;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(14, 25);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(44, 13);
            this.label15.TabIndex = 24;
            this.label15.Text = "Frames:";
            // 
            // lvCalibStream
            // 
            this.lvCalibStream.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lvCalibStream.Location = new System.Drawing.Point(6, 80);
            this.lvCalibStream.Name = "lvCalibStream";
            this.lvCalibStream.Size = new System.Drawing.Size(252, 118);
            this.lvCalibStream.TabIndex = 23;
            this.lvCalibStream.UseCompatibleStateImageBehavior = false;
            this.lvCalibStream.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 124;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Value";
            this.columnHeader2.Width = 111;
            // 
            // lvImageLayoutTags
            // 
            this.lvImageLayoutTags.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
            this.lvImageLayoutTags.Location = new System.Drawing.Point(276, 53);
            this.lvImageLayoutTags.Name = "lvImageLayoutTags";
            this.lvImageLayoutTags.Size = new System.Drawing.Size(262, 269);
            this.lvImageLayoutTags.TabIndex = 1;
            this.lvImageLayoutTags.UseCompatibleStateImageBehavior = false;
            this.lvImageLayoutTags.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Name";
            this.columnHeader3.Width = 124;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Value";
            this.columnHeader4.Width = 111;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(102, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Image Section Tags";
            // 
            // lvImageSectionTags
            // 
            this.lvImageSectionTags.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader6});
            this.lvImageSectionTags.Location = new System.Drawing.Point(8, 28);
            this.lvImageSectionTags.Name = "lvImageSectionTags";
            this.lvImageSectionTags.Size = new System.Drawing.Size(262, 294);
            this.lvImageSectionTags.TabIndex = 14;
            this.lvImageSectionTags.UseCompatibleStateImageBehavior = false;
            this.lvImageSectionTags.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Name";
            this.columnHeader5.Width = 124;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Value";
            this.columnHeader6.Width = 111;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(276, 12);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(79, 13);
            this.label12.TabIndex = 15;
            this.label12.Text = "Image Layouts:";
            // 
            // btnPrevLayout
            // 
            this.btnPrevLayout.Location = new System.Drawing.Point(468, 7);
            this.btnPrevLayout.Name = "btnPrevLayout";
            this.btnPrevLayout.Size = new System.Drawing.Size(32, 23);
            this.btnPrevLayout.TabIndex = 16;
            this.btnPrevLayout.Text = "<";
            this.btnPrevLayout.UseVisualStyleBackColor = true;
            this.btnPrevLayout.Click += new System.EventHandler(this.btnPrevLayout_Click);
            // 
            // btnNextLayout
            // 
            this.btnNextLayout.Location = new System.Drawing.Point(506, 7);
            this.btnNextLayout.Name = "btnNextLayout";
            this.btnNextLayout.Size = new System.Drawing.Size(32, 23);
            this.btnNextLayout.TabIndex = 17;
            this.btnNextLayout.Text = ">";
            this.btnNextLayout.UseVisualStyleBackColor = true;
            this.btnNextLayout.Click += new System.EventHandler(this.btnNextLayout_Click);
            // 
            // lblImageLayoutId
            // 
            this.lblImageLayoutId.AutoSize = true;
            this.lblImageLayoutId.ForeColor = System.Drawing.Color.Navy;
            this.lblImageLayoutId.Location = new System.Drawing.Point(304, 35);
            this.lblImageLayoutId.Name = "lblImageLayoutId";
            this.lblImageLayoutId.Size = new System.Drawing.Size(0, 13);
            this.lblImageLayoutId.TabIndex = 20;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(276, 35);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(19, 13);
            this.label16.TabIndex = 19;
            this.label16.Text = "Id:";
            // 
            // lblImageLayoutBPP
            // 
            this.lblImageLayoutBPP.AutoSize = true;
            this.lblImageLayoutBPP.ForeColor = System.Drawing.Color.Navy;
            this.lblImageLayoutBPP.Location = new System.Drawing.Point(384, 35);
            this.lblImageLayoutBPP.Name = "lblImageLayoutBPP";
            this.lblImageLayoutBPP.Size = new System.Drawing.Size(0, 13);
            this.lblImageLayoutBPP.TabIndex = 22;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(356, 35);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(31, 13);
            this.label18.TabIndex = 21;
            this.label18.Text = "BPP:";
            // 
            // lblCountLayouts
            // 
            this.lblCountLayouts.AutoSize = true;
            this.lblCountLayouts.ForeColor = System.Drawing.Color.Navy;
            this.lblCountLayouts.Location = new System.Drawing.Point(353, 12);
            this.lblCountLayouts.Name = "lblCountLayouts";
            this.lblCountLayouts.Size = new System.Drawing.Size(0, 13);
            this.lblCountLayouts.TabIndex = 23;
            // 
            // lvlStatusTags
            // 
            this.lvlStatusTags.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9});
            this.lvlStatusTags.Location = new System.Drawing.Point(8, 28);
            this.lvlStatusTags.Name = "lvlStatusTags";
            this.lvlStatusTags.Size = new System.Drawing.Size(533, 297);
            this.lvlStatusTags.TabIndex = 15;
            this.lvlStatusTags.UseCompatibleStateImageBehavior = false;
            this.lvlStatusTags.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Name";
            this.columnHeader7.Width = 156;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Id";
            this.columnHeader8.Width = 55;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Type";
            this.columnHeader9.Width = 154;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 9);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(102, 13);
            this.label14.TabIndex = 16;
            this.label14.Text = "Image Section Tags";
            // 
            // lvSystemMedata
            // 
            this.lvSystemMedata.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader10,
            this.columnHeader11});
            this.lvSystemMedata.Location = new System.Drawing.Point(3, 3);
            this.lvSystemMedata.Name = "lvSystemMedata";
            this.lvSystemMedata.Size = new System.Drawing.Size(538, 244);
            this.lvSystemMedata.TabIndex = 15;
            this.lvSystemMedata.UseCompatibleStateImageBehavior = false;
            this.lvSystemMedata.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Name";
            this.columnHeader10.Width = 240;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "Value";
            this.columnHeader11.Width = 274;
            // 
            // lvUserMetadata
            // 
            this.lvUserMetadata.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader12,
            this.columnHeader13});
            this.lvUserMetadata.Location = new System.Drawing.Point(3, 253);
            this.lvUserMetadata.Name = "lvUserMetadata";
            this.lvUserMetadata.Size = new System.Drawing.Size(538, 72);
            this.lvUserMetadata.TabIndex = 16;
            this.lvUserMetadata.UseCompatibleStateImageBehavior = false;
            this.lvUserMetadata.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "Name";
            this.columnHeader12.Width = 240;
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "Value";
            this.columnHeader13.Width = 274;
            // 
            // tbxMainIndex
            // 
            this.tbxMainIndex.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbxMainIndex.Location = new System.Drawing.Point(0, 0);
            this.tbxMainIndex.Multiline = true;
            this.tbxMainIndex.Name = "tbxMainIndex";
            this.tbxMainIndex.ReadOnly = true;
            this.tbxMainIndex.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbxMainIndex.Size = new System.Drawing.Size(544, 328);
            this.tbxMainIndex.TabIndex = 0;
            // 
            // pbar
            // 
            this.pbar.Error = false;
            this.pbar.Location = new System.Drawing.Point(167, 313);
            this.pbar.Name = "pbar";
            this.pbar.Size = new System.Drawing.Size(372, 23);
            this.pbar.TabIndex = 16;
            this.pbar.Visible = false;
            // 
            // tabCalibrationIndex
            // 
            this.tabCalibrationIndex.Controls.Add(this.tbxCalibIndex);
            this.tabCalibrationIndex.Location = new System.Drawing.Point(4, 22);
            this.tabCalibrationIndex.Name = "tabCalibrationIndex";
            this.tabCalibrationIndex.Size = new System.Drawing.Size(544, 328);
            this.tabCalibrationIndex.TabIndex = 5;
            this.tabCalibrationIndex.Text = "Calibration Index";
            this.tabCalibrationIndex.UseVisualStyleBackColor = true;
            // 
            // tbxCalibIndex
            // 
            this.tbxCalibIndex.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbxCalibIndex.Location = new System.Drawing.Point(0, 0);
            this.tbxCalibIndex.Multiline = true;
            this.tbxCalibIndex.Name = "tbxCalibIndex";
            this.tbxCalibIndex.ReadOnly = true;
            this.tbxCalibIndex.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbxCalibIndex.Size = new System.Drawing.Size(544, 328);
            this.tbxCalibIndex.TabIndex = 1;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(581, 405);
            this.Controls.Add(this.tcTabs);
            this.Name = "frmMain";
            this.Text = "AdvLib Test Form";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxPixelValue)).EndInit();
            this.tcTabs.ResumeLayout(false);
            this.tabRecorder.ResumeLayout(false);
            this.tabRecorder.PerformLayout();
            this.gbxCompression.ResumeLayout(false);
            this.gbxCompression.PerformLayout();
            this.tabStructureViewer.ResumeLayout(false);
            this.tcStructureViewer.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tabGeneral.PerformLayout();
            this.tabImageSection.ResumeLayout(false);
            this.tabImageSection.PerformLayout();
            this.tabStatusSection.ResumeLayout(false);
            this.tabStatusSection.PerformLayout();
            this.tabMetadata.ResumeLayout(false);
            this.tabMainIndex.ResumeLayout(false);
            this.tabMainIndex.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.tabCalibrationIndex.ResumeLayout(false);
            this.tabCalibrationIndex.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.CheckBox cbxLocationData;
		private System.Windows.Forms.RadioButton rb16BitUShort;
		private System.Windows.Forms.RadioButton rb16BitByte;
		private System.Windows.Forms.RadioButton rb8BitByte;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton rbPixel8;
		private System.Windows.Forms.RadioButton rbPixel12;
        private System.Windows.Forms.RadioButton rbPixel16;
		private System.Windows.Forms.CheckBox cbxCompress;
		private System.Windows.Forms.Button btnVerifyLibrary;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.CheckBox cbxADV1;
		private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudMaxPixelValue;
		private System.Windows.Forms.RadioButton rb12BitByte;
        private System.Windows.Forms.TabControl tcTabs;
        private System.Windows.Forms.TabPage tabRecorder;
        private System.Windows.Forms.TabPage tabStructureViewer;
        private System.Windows.Forms.TabPage tabPlayer;
        private System.Windows.Forms.CheckBox cbxZeroTicks;
        private System.Windows.Forms.RadioButton rb24bitBGR;
        private System.Windows.Forms.RadioButton rb24bitRGB;
        private System.Windows.Forms.GroupBox gbxCompression;
        private System.Windows.Forms.RadioButton rbLagarith16;
        private System.Windows.Forms.RadioButton rbQuickLZ;
        private System.Windows.Forms.Button btnRunTests;
        private AdvLib.TestApp.UnitTestProgressBar pbar;
        private System.Windows.Forms.Label lblRanTests;
        private System.Windows.Forms.Label lblLibVersion;
        private System.Windows.Forms.Button btnGenSpecExample;
        private System.Windows.Forms.CheckBox cbxCopyToClipboard;
        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
        private System.Windows.Forms.TabControl tcStructureViewer;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.TabPage tabImageSection;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblMaxPixelValue;
        private System.Windows.Forms.Label lblBPP;
        private System.Windows.Forms.Label lblHeight;
        private System.Windows.Forms.Label lblWidth;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbxFileName;
        private System.Windows.Forms.Label lblUtcAccuracy;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TabPage tabStatusSection;
        private System.Windows.Forms.TabPage tabMetadata;
        private System.Windows.Forms.TabPage tabMainIndex;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lblMainClockFreq;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblMainFrames;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ListView lvMainStream;
        private System.Windows.Forms.ColumnHeader tagName;
        private System.Windows.Forms.ColumnHeader tagValue;
        private System.Windows.Forms.Label lblMainFrameTSAccu;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblCalibFrameTSAccu;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lblCalibClockFreq;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label lblCalibFrames;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ListView lvCalibStream;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button btnNextLayout;
        private System.Windows.Forms.Button btnPrevLayout;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ListView lvImageSectionTags;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListView lvImageLayoutTags;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Label lblImageLayoutBPP;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label lblImageLayoutId;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label lblCountLayouts;
        private System.Windows.Forms.ListView lvlStatusTags;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ListView lvUserMetadata;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.ListView lvSystemMedata;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.TextBox tbxMainIndex;
        private System.Windows.Forms.TabPage tabCalibrationIndex;
        private System.Windows.Forms.TextBox tbxCalibIndex;
	}
}

