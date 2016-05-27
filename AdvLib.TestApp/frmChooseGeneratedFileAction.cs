using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace AdvLib.TestApp
{
    public partial class frmChooseGeneratedFileAction : Form
    {
        public enum FileOperation
        {
            None,
            ViewStructure,
            PlayVideo
        }

        public FileOperation SelectedOperation = FileOperation.None;

        public frmChooseGeneratedFileAction()
        {
            InitializeComponent();
        }

        public frmChooseGeneratedFileAction(string fileName)
            : this()
        {
            lblFilePath.Text = fileName;
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            SelectedOperation = FileOperation.ViewStructure;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            SelectedOperation = FileOperation.PlayVideo;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
