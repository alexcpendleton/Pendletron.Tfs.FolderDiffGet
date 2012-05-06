using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Pendletron.Tfs.FolderDiffGet.Core
{
	public interface IFolderDiffGetter : IDisposable
	{
		/// <summary>
		/// Gets or sets the source path to compare (server or local)
		/// </summary>
		string SourcePath { get; set; }

		/// <summary>
		/// Gets or sets the target path to compare (server or local)
		/// </summary>
		string TargetPath { get; set; }

		/// <summary>
		/// Gets or sets the directory where the different files will be downloaded.
		/// </summary>
		string OutputDirectory { get; set; }

		/// <summary>
		/// Gets or sets the URL of the TFS Team Project Collection 
		/// </summary>
		string TeamCollectionUrl { get; set; }

		/// <summary>
		/// Gets or sets whether to download files that are the same. False by default.
		/// </summary>
		bool ViewSame { get; set; }

		/// <summary>
		/// Gets or sets whether to download files that are different in the compare. True by default.
		/// </summary>
		bool ViewDifferent { get; set; }

		/// <summary>
		/// Gets or sets whether to download files that are only in the source path. False by default
		/// </summary>
		bool ViewSourceOnly { get; set; }

		/// <summary>
		/// Gets or sets whether to download files that are only in the target path. True by default
		/// </summary>
		bool ViewTargetOnly { get; set; }

		/// <summary>
		/// Writes a message to the TraceListener (if available.)
		/// </summary>
		/// <param name="message">The message to write.</param>
		void WriteToTrace(string message);

		/// <summary>
		/// Formats and writes a message to the TraceListener (if available.)
		/// </summary>
		/// <param name="message">The message to write, with formatting placeholders.</param>
		/// <param name="args">The formatting arguments.</param>
		/// <remarks>Does a String.Format(message, args) and calls WriteToTrace with the result.</remarks>
		void WriteToTrace(string message, params object[] args);

		Action<string> TraceWriter { get; set; }

		/// <summary>
		/// Setups up and connects to the TfsTeamProjectCollection, uses UICredentialsProvider
		/// </summary>
		void SetupProjectCollection();

		/// <summary>
		/// Runs the diff and get process.
		/// </summary>
		void Go();

		/// <summary>
		/// Parses the results of the process output and gets the files.
		/// </summary>
		List<IFolderDiffEntry> GetDifferentFilePaths();

		/// <summary>
		/// Gets all files in the parsed results.
		/// </summary>
		/// <param name="parsed">The parsed results.</param>
		void DownloadFiles(List<IFolderDiffEntry> files);

		/// <summary>
		/// Disposes the process and the Tfs collection connection.
		/// </summary>
		void Dispose();
	}
}