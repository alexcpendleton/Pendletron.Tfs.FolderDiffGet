using System;

namespace Pendletron.Pendletron_Tfs_FolderDiffGet_Vsix.UI
{
	public class DownloadEventArgs : EventArgs {
		public DownloadEventArgs(string path) {
			Path = path;
		}

		public string Path { get; set; }
	}
}