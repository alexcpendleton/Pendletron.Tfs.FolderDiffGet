using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ookii.Dialogs;

namespace Pendletron.Pendletron_Tfs_FolderDiffGet_Vsix.UI {
	public partial class DownloadForm : Form {
		protected VistaFolderBrowserDialog _folderBrowser = new VistaFolderBrowserDialog();
		public DownloadForm() {
			InitializeComponent();
		}

		public event EventHandler Cancel = (sender, args) => { };
		public event EventHandler<DownloadEventArgs> Download = (sender, args) => { };

		private void uxBrowse_Click(object sender, EventArgs e) {
			HandleFolderBrowser();
		}

		protected void HandleFolderBrowser()
		{
			var result = _folderBrowser.ShowDialog();

			if (result == DialogResult.OK) {
				OnPathSelected(_folderBrowser.SelectedPath);
			}
		}

		protected void OnPathSelected(string path)
		{
			uxOutputDirectory.Text = path;
		}

		private void uxDownload_Click(object sender, EventArgs e) {
			var args = new DownloadEventArgs(uxOutputDirectory.Text.Trim());
			Download(this, args);
			Close();
		}

		private void uxCancel_Click(object sender, EventArgs e) {
			Cancel(sender, e);
			Close();
		}
	}
}
