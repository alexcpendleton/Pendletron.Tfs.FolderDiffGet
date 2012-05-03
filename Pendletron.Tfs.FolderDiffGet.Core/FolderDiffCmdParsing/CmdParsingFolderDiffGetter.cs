using System;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Pendletron.Tfs.FolderDiffGet.Core.FolderDiffCmdParsing
{
	public class CmdParsingFolderDiffGetter : TextParsingFolderDiffGetter
	{
        public CmdParsingFolderDiffGetter(string collectionUri, string srcPath, string targetPath, string outputDir)
			:base(collectionUri, srcPath, targetPath, outputDir, "")
        {
            FolderDiffOutputFilePath = "FolderDiffOutput.txt";
            CommandPromptPath = @"c:\windows\system32\cmd.exe";
            VcVarsBatPath = @"C:\Program Files (x86)\Microsoft Visual Studio 10.0\VC\vcvarsall.bat";
			SummaryText = "Summary: ";
            SleepTime = 5000;
        }

		/// <summary>
		/// Gets or sets the time (in milliseconds) to wait between checks to see if the command prompt has finished executing. Default is 5000 (5 seconds)
		/// </summary>
        public int SleepTime { get; set; }

		/// <summary>
		/// Gets or sets the file name of the folder diff summary file.
		/// </summary>
        public string FolderDiffOutputFilePath { get; set; }
		/// <summary>
		/// Gets or sets the path to the vcvars.bat file for the location to tf.exe
		/// </summary>
        public string VcVarsBatPath { get; set; }

		/// <summary>
		/// Gets or sets the path to the command prompt. 
		/// </summary>
        public string CommandPromptPath { get; set; }

		/// <summary>
		/// Gets or sets the text for the Summary line, used for determining the end of the output. "Summary:" by default.
		/// </summary>
		public string SummaryText { get; set; }

		/// <summary>
		/// The connection to the Tfs team project collection.
		/// </summary>
		protected TfsTeamProjectCollection _collection = null;
		/// <summary>
		/// The process that runs the command prompt for tf.exe
		/// </summary>
		protected Process proc = new Process();
		/// <summary>
		/// Determines if the process is finished.
		/// </summary>
		protected bool _completelyFinished = false;
		/// <summary>
		/// The current output from the command line process.
		/// </summary>
		protected StringBuilder _output = new StringBuilder();
		/// <summary>
		/// Whether the process's end text has been detected (Summary: ), but the next line is actually the last.
		/// </summary>
		protected bool _endOnNext = false;
		
		/// <summary>
		/// Runs tf.exe with the necessary arguments. 
		/// </summary>
		protected virtual void RunCommandLineFolderDiff()
        {
            string cmdPromptPath = CommandPromptPath;
            proc.StartInfo.FileName = cmdPromptPath;
            proc.StartInfo.Arguments = String.Format(@"/k ""{0}"" x86", VcVarsBatPath);
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardInput = true;
			proc.StartInfo.RedirectStandardError = true;
			proc.ErrorDataReceived += new DataReceivedEventHandler(proc_ErrorDataReceived);
            proc.OutputDataReceived += new DataReceivedEventHandler(proc_OutputDataReceived);

            WriteToTrace("Starting process: '{0}' with args '{1}'", cmdPromptPath, proc.StartInfo.Arguments);

            proc.Start();
            proc.BeginOutputReadLine();

            string viewString = BuildViewString();
            string cmd = @"tf folderdiff ""{0}"" ""{1}"" /r /collection:{2} /view:{3} ";

            cmd = String.Format(cmd, SourcePath, TargetPath, TeamCollectionUrl, viewString);
            WriteToTrace("Executing command: {0}", cmd);
            proc.StandardInput.WriteLine(cmd);

            while(!_completelyFinished)
            {
                Thread.Sleep(SleepTime);
            }
            
        }

		void proc_ErrorDataReceived(object sender, DataReceivedEventArgs e)
		{
			WriteToTrace("Error: ", e.Data);
			_completelyFinished = true;
		}

		/// <summary>
		/// Creates the string necessary for the /view switch based on the View* settings.
		/// </summary>
		/// <returns></returns>
		protected virtual string BuildViewString()
        {
            List<string> results = new List<string>();
            if(ViewSame)
            {
                results.Add("same");
            }
            if(ViewDifferent)
            {
                results.Add("different");
            }
            if(ViewSourceOnly)
            {
                results.Add("sourceOnly");
            }
            if(ViewDifferent)
            {
                results.Add("targetOnly");
            }
            return String.Join(",", results.ToArray());
        }

		/// <summary>
		/// Handler for receiving output data from the command line process.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void proc_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			WriteToTrace(e.Data);
            _output.AppendLine(e.Data);
			
            if(_endOnNext)
            {
                OnCommandLineFinished();
                return;
            }
            if(e.Data.Contains(SummaryText))
            {
                _endOnNext = true;
            }
        }

		/// <summary>
		/// Executed when the command line output has finished. Unbinds the OutputDataReceived, creates the output file, parses, and gets the files.
		/// </summary>
		protected virtual void OnCommandLineFinished()
        {
            proc.OutputDataReceived -= proc_OutputDataReceived;
            CreateFolderDiffOutputFile();
        }

		/// <summary>
		/// Creates the summary output summary file.
		/// </summary>
		protected virtual void CreateFolderDiffOutputFile()
        {
            if(!Directory.Exists(OutputDirectory))
            {
                WriteToTrace("Creating output directory: '{0}'", OutputDirectory);
                Directory.CreateDirectory(OutputDirectory);
            }
            string folderDiffOutputPath = Path.Combine(OutputDirectory, FolderDiffOutputFilePath);
            WriteToTrace("Writing output file: '{0}'", folderDiffOutputPath);
            File.WriteAllText(folderDiffOutputPath, _output.ToString());
        }

		/// <summary>
		/// Parses the results of the process output and gets the files.
		/// </summary>
		public override HashSet<string> GetDifferentFilePaths()
        {
			SetupProjectCollection();
			RunCommandLineFolderDiff();
            string output = _output.ToString();
			TextToParse = output;
			return base.GetDifferentFilePaths();
        }

		/// <summary>
		/// Marks the program as ready to close so when the sleep timer finishes it will exit.
		/// </summary>
		protected virtual void Close()
        {
            _completelyFinished = true;
        }

		/// <summary>
		/// Disposes the process and the Tfs collection connection.
		/// </summary>
		public virtual void Dispose()
		{
			base.Dispose();
            if(proc != null)
            {
                proc.Dispose();
            }
        }
    }
}