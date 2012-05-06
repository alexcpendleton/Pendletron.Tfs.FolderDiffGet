using System.ComponentModel;
using Microsoft.TeamFoundation.Client;
using Pendletron.Tfs.FolderDiffGet.Core;
using Pendletron.Tfs.FolderDiffGet.Core.Wrappers;
using Pendletron.Vsix.Core.Wrappers;

public class DownloadWorker : BackgroundWorker {
	public DownloadWorker(string outputPath)
		: base() {
		WorkerReportsProgress = true;
		WorkerSupportsCancellation = true;
		OutputDirectoryPath = outputPath;
		}

	public string OutputDirectoryPath { get; private set; }

	protected override void OnDoWork(DoWorkEventArgs e) {
		base.OnDoWork(e);
		DownloadFromFolderDiffManager(OutputDirectoryPath);
	}

	public void WriteProgressMessage(string message) {
		ReportProgress(0, message);
	}

	protected void DownloadFromFolderDiffManager(string outputDir) {

		var hatpack = new HatPackage();
		dynamic man = new AccessPrivateWrapper(hatpack._wrapped.FolderDiffManager);
		if (man != null) {
			dynamic toolWindow = new AccessPrivateWrapper(man.FolderDiffToolWindows[0]);
			if (toolWindow != null) {
				dynamic diffControl = new AccessPrivateWrapper(toolWindow.FolderDiffControl);
				if (diffControl != null) {
					var diff = diffControl.FolderDiff;
					if (diff != null) {
						var coll = hatpack.HatterasService.TeamProjectCollection as TfsTeamProjectCollection;

						var getter = new FolderDiffInternalsGetter(diff, coll, outputDir);
						getter.TraceWriter = WriteProgressMessage;
						getter.Go();

					}
				}
			}
		}
	}
}