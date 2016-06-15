﻿namespace AdvLibTestApp
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
            this.cbxZeroTicks = new System.Windows.Forms.CheckBox();
            this.tabStructureViewer = new System.Windows.Forms.TabPage();
            this.tabPlayer = new System.Windows.Forms.TabPage();
            this.rb24bitRGB = new System.Windows.Forms.RadioButton();
            this.rb24bitBGR = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxPixelValue)).BeginInit();
            this.tcTabs.SuspendLayout();
            this.tabRecorder.SuspendLayout();
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
            // 
            // btnVerifyLibrary
            // 
            this.btnVerifyLibrary.Location = new System.Drawing.Point(364, 244);
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
            this.tabStructureViewer.Location = new System.Drawing.Point(4, 22);
            this.tabStructureViewer.Name = "tabStructureViewer";
            this.tabStructureViewer.Padding = new System.Windows.Forms.Padding(3);
            this.tabStructureViewer.Size = new System.Drawing.Size(558, 360);
            this.tabStructureViewer.TabIndex = 1;
            this.tabStructureViewer.Text = "Structure Viewer";
            this.tabStructureViewer.UseVisualStyleBackColor = true;
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
	}
}

