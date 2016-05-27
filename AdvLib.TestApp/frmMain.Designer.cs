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
            this.rb12BitByte = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbPixel12as12 = new System.Windows.Forms.RadioButton();
            this.rbPixel8 = new System.Windows.Forms.RadioButton();
            this.rbPixel12as16 = new System.Windows.Forms.RadioButton();
            this.rbPixel16 = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rbCamera8 = new System.Windows.Forms.RadioButton();
            this.rbCamera12 = new System.Windows.Forms.RadioButton();
            this.rbCamera16 = new System.Windows.Forms.RadioButton();
            this.cbxCompress = new System.Windows.Forms.CheckBox();
            this.btnVerifyLibrary = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.cbxADV1 = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.nudNormalValue = new System.Windows.Forms.NumericUpDown();
            this.tcTabs = new System.Windows.Forms.TabControl();
            this.tabRecorder = new System.Windows.Forms.TabPage();
            this.tabStructureViewer = new System.Windows.Forms.TabPage();
            this.tabPlayer = new System.Windows.Forms.TabPage();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudNormalValue)).BeginInit();
            this.tcTabs.SuspendLayout();
            this.tabRecorder.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(17, 207);
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
            this.cbxLocationData.Location = new System.Drawing.Point(17, 16);
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
            this.groupBox1.Controls.Add(this.rb12BitByte);
            this.groupBox1.Controls.Add(this.rb16BitUShort);
            this.groupBox1.Controls.Add(this.rb8BitByte);
            this.groupBox1.Controls.Add(this.rb16BitByte);
            this.groupBox1.Location = new System.Drawing.Point(17, 42);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(184, 123);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source Pixel Format";
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
            this.groupBox2.Controls.Add(this.rbPixel12as12);
            this.groupBox2.Controls.Add(this.rbPixel8);
            this.groupBox2.Controls.Add(this.rbPixel12as16);
            this.groupBox2.Controls.Add(this.rbPixel16);
            this.groupBox2.Location = new System.Drawing.Point(207, 42);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(161, 123);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Image Pixel Format";
            // 
            // rbPixel12as12
            // 
            this.rbPixel12as12.AutoSize = true;
            this.rbPixel12as12.Location = new System.Drawing.Point(15, 70);
            this.rbPixel12as12.Name = "rbPixel12as12";
            this.rbPixel12as12.Size = new System.Drawing.Size(134, 17);
            this.rbPixel12as12.TabIndex = 3;
            this.rbPixel12as12.Text = "12-bit (Saved as 12-bit)";
            this.rbPixel12as12.UseVisualStyleBackColor = true;
            // 
            // rbPixel8
            // 
            this.rbPixel8.AutoSize = true;
            this.rbPixel8.Enabled = false;
            this.rbPixel8.Location = new System.Drawing.Point(15, 93);
            this.rbPixel8.Name = "rbPixel8";
            this.rbPixel8.Size = new System.Drawing.Size(122, 17);
            this.rbPixel8.TabIndex = 2;
            this.rbPixel8.Text = "8-bit (Saved as 8-bit)";
            this.rbPixel8.UseVisualStyleBackColor = true;
            this.rbPixel8.CheckedChanged += new System.EventHandler(this.OnPixelFormatChanged);
            // 
            // rbPixel12as16
            // 
            this.rbPixel12as16.AutoSize = true;
            this.rbPixel12as16.Location = new System.Drawing.Point(15, 47);
            this.rbPixel12as16.Name = "rbPixel12as16";
            this.rbPixel12as16.Size = new System.Drawing.Size(134, 17);
            this.rbPixel12as16.TabIndex = 1;
            this.rbPixel12as16.Text = "12-bit (Saved as 16-bit)";
            this.rbPixel12as16.UseVisualStyleBackColor = true;
            this.rbPixel12as16.CheckedChanged += new System.EventHandler(this.OnPixelFormatChanged);
            // 
            // rbPixel16
            // 
            this.rbPixel16.AutoSize = true;
            this.rbPixel16.Checked = true;
            this.rbPixel16.Location = new System.Drawing.Point(15, 24);
            this.rbPixel16.Name = "rbPixel16";
            this.rbPixel16.Size = new System.Drawing.Size(134, 17);
            this.rbPixel16.TabIndex = 0;
            this.rbPixel16.TabStop = true;
            this.rbPixel16.Text = "16-bit (Saved as 16-bit)";
            this.rbPixel16.UseVisualStyleBackColor = true;
            this.rbPixel16.CheckedChanged += new System.EventHandler(this.OnPixelFormatChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rbCamera8);
            this.groupBox3.Controls.Add(this.rbCamera12);
            this.groupBox3.Controls.Add(this.rbCamera16);
            this.groupBox3.Location = new System.Drawing.Point(374, 42);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(122, 123);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Camera Depth";
            // 
            // rbCamera8
            // 
            this.rbCamera8.AutoSize = true;
            this.rbCamera8.Enabled = false;
            this.rbCamera8.Location = new System.Drawing.Point(15, 70);
            this.rbCamera8.Name = "rbCamera8";
            this.rbCamera8.Size = new System.Drawing.Size(45, 17);
            this.rbCamera8.TabIndex = 2;
            this.rbCamera8.Text = "8-bit";
            this.rbCamera8.UseVisualStyleBackColor = true;
            // 
            // rbCamera12
            // 
            this.rbCamera12.AutoSize = true;
            this.rbCamera12.Location = new System.Drawing.Point(15, 47);
            this.rbCamera12.Name = "rbCamera12";
            this.rbCamera12.Size = new System.Drawing.Size(51, 17);
            this.rbCamera12.TabIndex = 1;
            this.rbCamera12.Text = "12-bit";
            this.rbCamera12.UseVisualStyleBackColor = true;
            // 
            // rbCamera16
            // 
            this.rbCamera16.AutoSize = true;
            this.rbCamera16.Checked = true;
            this.rbCamera16.Location = new System.Drawing.Point(15, 24);
            this.rbCamera16.Name = "rbCamera16";
            this.rbCamera16.Size = new System.Drawing.Size(51, 17);
            this.rbCamera16.TabIndex = 0;
            this.rbCamera16.TabStop = true;
            this.rbCamera16.Text = "16-bit";
            this.rbCamera16.UseVisualStyleBackColor = true;
            // 
            // cbxCompress
            // 
            this.cbxCompress.AutoSize = true;
            this.cbxCompress.Location = new System.Drawing.Point(17, 180);
            this.cbxCompress.Name = "cbxCompress";
            this.cbxCompress.Size = new System.Drawing.Size(140, 17);
            this.cbxCompress.TabIndex = 8;
            this.cbxCompress.Text = "Use Image Compression";
            this.cbxCompress.UseVisualStyleBackColor = true;
            // 
            // btnVerifyLibrary
            // 
            this.btnVerifyLibrary.Location = new System.Drawing.Point(364, 207);
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
            this.cbxADV1.Location = new System.Drawing.Point(374, 16);
            this.cbxADV1.Name = "cbxADV1";
            this.cbxADV1.Size = new System.Drawing.Size(117, 17);
            this.cbxADV1.TabIndex = 10;
            this.cbxADV1.Text = "Use ADV Version 1";
            this.cbxADV1.UseVisualStyleBackColor = true;
            this.cbxADV1.CheckedChanged += new System.EventHandler(this.cbxADV1_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(175, 180);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Normal Pixel Value:";
            // 
            // nudNormalValue
            // 
            this.nudNormalValue.Location = new System.Drawing.Point(280, 178);
            this.nudNormalValue.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudNormalValue.Name = "nudNormalValue";
            this.nudNormalValue.Size = new System.Drawing.Size(64, 20);
            this.nudNormalValue.TabIndex = 12;
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
            this.tabRecorder.Controls.Add(this.cbxLocationData);
            this.tabRecorder.Controls.Add(this.nudNormalValue);
            this.tabRecorder.Controls.Add(this.button1);
            this.tabRecorder.Controls.Add(this.label1);
            this.tabRecorder.Controls.Add(this.groupBox1);
            this.tabRecorder.Controls.Add(this.cbxADV1);
            this.tabRecorder.Controls.Add(this.groupBox2);
            this.tabRecorder.Controls.Add(this.btnVerifyLibrary);
            this.tabRecorder.Controls.Add(this.groupBox3);
            this.tabRecorder.Controls.Add(this.cbxCompress);
            this.tabRecorder.Location = new System.Drawing.Point(4, 22);
            this.tabRecorder.Name = "tabRecorder";
            this.tabRecorder.Padding = new System.Windows.Forms.Padding(3);
            this.tabRecorder.Size = new System.Drawing.Size(558, 360);
            this.tabRecorder.TabIndex = 0;
            this.tabRecorder.Text = "Recorder";
            this.tabRecorder.UseVisualStyleBackColor = true;
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
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(587, 403);
            this.Controls.Add(this.tcTabs);
            this.Name = "frmMain";
            this.Text = "AdvLib Test Form";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudNormalValue)).EndInit();
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
		private System.Windows.Forms.RadioButton rbPixel12as16;
		private System.Windows.Forms.RadioButton rbPixel16;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.RadioButton rbCamera8;
		private System.Windows.Forms.RadioButton rbCamera12;
		private System.Windows.Forms.RadioButton rbCamera16;
		private System.Windows.Forms.CheckBox cbxCompress;
		private System.Windows.Forms.Button btnVerifyLibrary;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.CheckBox cbxADV1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown nudNormalValue;
		private System.Windows.Forms.RadioButton rbPixel12as12;
		private System.Windows.Forms.RadioButton rb12BitByte;
        private System.Windows.Forms.TabControl tcTabs;
        private System.Windows.Forms.TabPage tabRecorder;
        private System.Windows.Forms.TabPage tabStructureViewer;
        private System.Windows.Forms.TabPage tabPlayer;
	}
}

