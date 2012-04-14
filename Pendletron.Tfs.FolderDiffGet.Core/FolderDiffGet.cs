using System;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Pendletron.Tfs.FolderDiffGet.Core
{
    public class FolderDiffGet : IDisposable
    {
        public FolderDiffGet(string collectionUri, string srcPath, string targetPath, string outputDir)
        {
            TeamCollectionUrl = collectionUri;
            SourcePath = srcPath;
            TargetPath = targetPath;
            OutputDirectory = outputDir;
            FolderDiffOutputFilePath = "FolderDiffOutput.txt";
            CommandPromptPath = @"c:\windows\system32\cmd.exe";
            VcVarsBatPath = @"C:\Program Files (x86)\Microsoft Visual Studio 10.0\VC\vcvarsall.bat";
        	SummaryText = "Summary: ";
            ViewSame = false;
            ViewDifferent = true;
            ViewSourceOnly = false;
            ViewTargetOnly = true;
            SleepTime = 5000;
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
		/// Gets or sets the time (in milliseconds) to wait between checks to see if the command prompt has finished executing. Default is 5000 (5 seconds)
		/// </summary>
        public int SleepTime { get; set; }
		/// <summary>
		/// Gets or sets the directory where the different files will be downloaded.
		/// </summary>
        public string OutputDirectory { get; set; }
		/// <summary>
		/// Gets or sets the URL of the TFS Team Project Collection 
		/// </summary>
        public string TeamCollectionUrl { get; set; }
		/// <summary>
		/// Gets or sets the file name of the folder diff summary file.
		/// </summary>
        public string FolderDiffOutputFilePath { get; set; }
		/// <summary>
		/// Gets or sets the path to the vcvars.bat file for the location to tf.exe
		/// </summary>
        public string VcVarsBatPath { get; set; }
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
		/// <summary>
		/// Gets or sets the path to the command prompt. 
		/// </summary>
        public string CommandPromptPath { get; set; }
		/// <summary>
		/// Gets or sets a TraceListener for logging messages. Null by default.
		/// </summary>
		public TraceListener Trace { get; set; }
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
		/// Writes a message to the TraceListener (if available.)
		/// </summary>
		/// <param name="message">The message to write.</param>
        protected virtual void WriteToTrace(string message)
        {
            if(Trace != null)
            {
                Trace.WriteLine(message);
            }
        }
		/// <summary>
		/// Formats and writes a message to the TraceListener (if available.)
		/// </summary>
		/// <param name="message">The message to write, with formatting placeholders.</param>
		/// <param name="args">The formatting arguments.</param>
		/// <remarks>Does a String.Format(message, args) and calls WriteToTrace with the result.</remarks>
		protected virtual void WriteToTrace(string message, params object[] args)
        {
            if(Trace != null)
            {
                WriteToTrace(String.Format(message, args));
            }
        }
		/// <summary>
		/// Setups up and connects to the TfsTeamProjectCollection, uses UICredentialsProvider
		/// </summary>
		protected virtual void SetupProjectCollection()
        {
            _collection = new TfsTeamProjectCollection(new Uri(TeamCollectionUrl), new UICredentialsProvider());
            _collection.EnsureAuthenticated();
        }

		/// <summary>
		/// Runs the diff and get process.
		/// </summary>
		public virtual void Go()
        {
            SetupProjectCollection();
            RunCommandLineFolderDiff();
        }

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
            proc.OutputDataReceived += new DataReceivedEventHandler(proc_OutputDataReceived);
            proc.Exited += new EventHandler(proc_Exited);

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
            ParseAndGet();
            Close();
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
		protected virtual void ParseAndGet()
        {
            string output = _output.ToString();
            
            var parser = new OutputParser(SourcePath, TargetPath);
            var results = parser.Parse(output);
            GetAll(results);
        }

		/// <summary>
		/// Marks the program as ready to close so when the sleep timer finishes it will exit.
		/// </summary>
		protected virtual void Close()
        {
            _completelyFinished = true;
        }

		/// <summary>
		/// Gets all files in the parsed results.
		/// </summary>
		/// <param name="parsed">The parsed results.</param>
		protected virtual void GetAll(ParseResults parsed)
        {
            SetupProjectCollection();
            var filesToGet = parsed.MergeAll();
            
            var vcs = _collection.GetService<VersionControlServer>();
            foreach (var serverPath in filesToGet)
            {
                string outputFilePath = serverPath.Replace(TargetPath, "");
                if(outputFilePath.StartsWith("/"))
                {
                    outputFilePath = outputFilePath.Remove(0, 1);
                }
                outputFilePath = Path.Combine(OutputDirectory, outputFilePath);
                outputFilePath = Path.GetFullPath(outputFilePath);
                var item = vcs.GetItem(serverPath);
                if(item.ItemType.HasFlag(ItemType.Folder))
                {
                    if(!Directory.Exists(outputFilePath))
                    {
                        WriteToTrace("Creating directory: '{0}'", outputFilePath);
                        Directory.CreateDirectory(outputFilePath);
                    }
                }
                else
                {
                    WriteToTrace("Downloading file from TFS: '{0}'", outputFilePath);
                    item.DownloadFile(outputFilePath);
                }
            }
        }

		/// <summary>
		/// Disposes the process and the Tfs collection connection.
		/// </summary>
		public virtual void Dispose()
        {
            if(_collection != null)
            {
                _collection.Dispose();
            }
            if(proc != null)
            {
                proc.Dispose();
            }
        }
    }
}