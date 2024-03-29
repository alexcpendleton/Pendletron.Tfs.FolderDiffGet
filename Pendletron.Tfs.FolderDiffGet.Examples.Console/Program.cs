﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Pendletron.Tfs.FolderDiffGet.Core;

namespace Pendletron.Tfs.FolderDiffGet.Examples.Console {
	class Program {
		static void Main(string[] args)
		{
			string sourcePath = args[0]; // The source path (server or local)
			string targetPath = args[1]; // The target path (server or local)
			string collectionUrl = args[2]; // The url to the TFS collection
			string outputDirectory = args[3]; // The directory where to download the files

			//var getter = new CmdParsingFolderDiffGetter(collectionUrl, sourcePath, targetPath, outputDirectory);
			string dllPath = FolderDiffInternalsGetter.DeriveTfsVcControlsDllPathFromRegistry();
			var getter = new FolderDiffInternalsGetter(collectionUrl, sourcePath, targetPath, outputDirectory, dllPath);
			// You may need to change these if you have things installed in other places
			//getter.VcVarsBatPath
			//getter.CommandPromptPath

			Trace.Listeners.Add(new ConsoleTraceListener());
			getter.Go();
		}
	}
}
