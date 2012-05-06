using System;
using System.Diagnostics;
using Microsoft.TeamFoundation.Client;
using System.Collections.Generic;
using System.IO;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace Pendletron.Tfs.FolderDiffGet.Core {
	public abstract class BaseFolderDiffGetter : IFolderDiffGetter {
		/// <summary>
		/// The connection to the Tfs team project collection.
		/// </summary>
		protected TfsTeamProjectCollection _collection = null;

		public BaseFolderDiffGetter(string collectionUri, string srcPath, string targetPath, string outputDir) {
			TeamCollectionUrl = collectionUri;
			SourcePath = srcPath;
			TargetPath = targetPath;
			OutputDirectory = outputDir;
			ViewSame = false;
			ViewDifferent = true;
			ViewSourceOnly = false;
			ViewTargetOnly = true;
			TraceWriter = DefaultTraceWriter;
		}

		/// <summary>
		/// Gets or sets the source path to compare (server or local)
		/// </summary>
		public string SourcePath { get; set; }

		/// <summary>
		/// Gets or sets the target path to compare (server or local)
		/// </summary>
		public string TargetPath { get; set; }

		/// <summary>
		/// Gets or sets the directory where the different files will be downloaded.
		/// </summary>
		public string OutputDirectory { get; set; }

		/// <summary>
		/// Gets or sets the URL of the TFS Team Project Collection 
		/// </summary>
		public string TeamCollectionUrl { get; set; }

		/// <summary>
		/// Gets or sets whether to download files that are the same. False by default.
		/// </summary>
		public bool ViewSame { get; set; }

		/// <summary>
		/// Gets or sets whether to download files that are different in the compare. True by default.
		/// </summary>
		public bool ViewDifferent { get; set; }

		/// <summary>
		/// Gets or sets whether to download files that are only in the source path. False by default
		/// </summary>
		public bool ViewSourceOnly { get; set; }

		/// <summary>
		/// Gets or sets whether to download files that are only in the target path. True by default
		/// </summary>
		public bool ViewTargetOnly { get; set; }


		public Action<string> TraceWriter { get; set; }

		public virtual void DefaultTraceWriter(string message) {
			Trace.WriteLine(message);
		}

		/// <summary>
		/// Writes a message to the TraceListener (if available.)
		/// </summary>
		/// <param name="message">The message to write.</param>
		public virtual void WriteToTrace(string message) {
			if(TraceWriter != null)
			{
				TraceWriter(message);
			}
		}
		/// <summary>
		/// Formats and writes a message to the TraceListener (if available.)
		/// </summary>
		/// <param name="message">The message to write, with formatting placeholders.</param>
		/// <param name="args">The formatting arguments.</param>
		/// <remarks>Does a String.Format(message, args) and calls WriteToTrace with the result.</remarks>
		public virtual void WriteToTrace(string message, params object[] args) {
			WriteToTrace(String.Format(message, args));
		}

		/// <summary>
		/// Setups up and connects to the TfsTeamProjectCollection, uses UICredentialsProvider
		/// </summary>
		public virtual void SetupProjectCollection() {
			if (_collection == null)
			{
				_collection = new TfsTeamProjectCollection(new Uri(TeamCollectionUrl), new UICredentialsProvider());
			}
			_collection.EnsureAuthenticated();
		}

		public virtual void Go()
		{
			var files = GetDifferentFilePaths();
			DownloadFiles(files);
		}

		public virtual void Dispose() {
			if (_collection != null) {
				_collection.Dispose();
			}
		}

		public virtual void CopyLocalFile(IFolderDiffEntry diff, string outputPath)
		{
			string sourcePath = diff.Path2;
			if(File.Exists(sourcePath))
			{
				WriteToTrace("Copying local path: '{0}'", sourcePath);
				string dir = Path.GetDirectoryName(outputPath);
				if (!Directory.Exists(dir))
				{
					Directory.CreateDirectory(dir);
				}
				File.Copy(sourcePath, outputPath, true);
			}
		}

		private VersionControlServer _vcs = null;
		public VersionControlServer Vcs
		{
			get
			{
				if (_vcs == null)
				{
					_vcs = _collection.GetService<VersionControlServer>();
				}
				return _vcs;
			}
		}

		public virtual void DownloadFromTfs(IFolderDiffEntry diff, string outputPath)
		{
			int deletionID = 0; // TODO: what is this?
			WriteToTrace("Downloading file from TFS: '{0}'", diff.Path2);
			Vcs.DownloadFile(diff.Path2, deletionID, diff.Path2VersionSpec, outputPath);
		}

		public virtual bool IsDirectory(IFolderDiffEntry item)
		{
			return item.ItemType == ItemType.Folder;
		}

		public abstract List<IFolderDiffEntry> GetDifferentFilePaths();
		
		public virtual void DownloadFiles(List<IFolderDiffEntry> filesToGet)
		{
			SetupProjectCollection();

			var vcs = _collection.GetService<VersionControlServer>();
			foreach (var diff in filesToGet)
			{
				string outputFilePath = diff.Path2;
				outputFilePath = outputFilePath.Replace(TargetPath, "");
				if(outputFilePath.StartsWith("/"))
				{
					outputFilePath = outputFilePath.Remove(0, 1);
				}
				if (outputFilePath.StartsWith("\\"))
				{
					outputFilePath = outputFilePath.Remove(0, 1);
				}
				outputFilePath = Path.Combine(OutputDirectory, outputFilePath);
				outputFilePath = Path.GetFullPath(outputFilePath);

				if (IsDirectory(diff))
				{
					if (!Directory.Exists(outputFilePath)) {
						WriteToTrace("Creating directory: '{0}'", outputFilePath);
						Directory.CreateDirectory(outputFilePath);
					}
				}
				else
				{
					if(diff.IsPath2Local)
					{
						// Just copy the file to the new path
						CopyLocalFile(diff, outputFilePath);
					}
					else
					{
						DownloadFromTfs(diff, outputFilePath);
					}
				}
			}
			WriteToTrace("Finished.");
		}
	}
}