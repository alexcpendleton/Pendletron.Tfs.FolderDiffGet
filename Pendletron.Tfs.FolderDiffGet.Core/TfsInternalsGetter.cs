using System.Collections.Generic;
using Pendletron.Tfs.FolderDiffGet.Core.Wrappers;
using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.IO;
using Microsoft.Win32;
namespace Pendletron.Tfs.FolderDiffGet.Core {
	public class TfsInternalsGetter : BaseFolderDiffGetter {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="collectionUri">The URL to the TFS collection</param>
		/// <param name="srcPath">The path to the source of the compare</param>
		/// <param name="targetPath">The path to the target of the compare</param>
		/// <param name="outputDir">The directory into which the files will be downloaded</param>
		/// <param name="tfsVcControlsDllPath">The path to the Microsoft.TeamFoundation.VersionControl.Controls.dll file. You can probably use DeriveTfsVcControlsDllPathFromRegistry for this.</param>
		public TfsInternalsGetter(string collectionUri, string srcPath, string targetPath, string outputDir, string tfsVcControlsDllPath)
			: base(collectionUri, srcPath, targetPath, outputDir) {
			PathToTfsVcControlsDll = tfsVcControlsDllPath;
		}

		/// <summary>
		/// Derives the path to the Microsoft.TeamFoundation.VersionControl.Controls.dll file based on the Visual Studio installation path (in PrivateAssemblies).
		/// </summary>
		/// <param name="version">The visual studio version.</param>
		/// <returns></returns>
		public static string DeriveTfsVcControlsDllPathFromRegistry(string version = "10.0")
		{
			string installDir = GetVisualStudioInstallationPath(version);
			string result = Path.GetFullPath(Path.Combine(installDir, @"PrivateAssemblies/Microsoft.TeamFoundation.VersionControl.Controls.dll"));
			return result;
		}

		/// <summary>
		/// Finds the VS installation path from the registry. Does not work for Express editions. http://stackoverflow.com/a/7363241/21201
		/// </summary>
		/// <param name="version">The visual studio version.</param>
		/// <returns></returns>
		private static string GetVisualStudioInstallationPath(string version) {
			string installationPath = null;
			if (Environment.Is64BitOperatingSystem) {
				installationPath = (string)Registry.GetValue(
				   string.Format("HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Microsoft\\VisualStudio\\{0}\\", version),
					"InstallDir",
					null);
			}
			else {
				installationPath = (string)Registry.GetValue(
		   string.Format("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\VisualStudio\\{0}\\", version),
				  "InstallDir",
				  null);
			}
			return installationPath;

		}

		/// <summary>
		/// Gets or sets the path to the Microsoft.TeamFoundation.VersionControl.Controls.dll file. 
		/// </summary>
		public string PathToTfsVcControlsDll { get; set; }
		public override void Go() {
			SetupProjectCollection();
			var srcSpec = VersionSpec.Latest;
			var targetSpec = VersionSpec.Latest;
			var recursion = RecursionType.Full;
			string assemblyPath = PathToTfsVcControlsDll; // TODO: Make this loaded from config or something
			_folderDiff = new FolderDiffWrapper(assemblyPath, SourcePath, srcSpec, TargetPath, targetSpec, GetVcs(), recursion);
			_folderDiff.ViewSame = ViewSame;
			_folderDiff.ViewDifferent = ViewDifferent;
			_folderDiff.ViewSourceOnly = ViewSourceOnly;
			_folderDiff.ViewTargetOnly = ViewTargetOnly;
			base.Go();
		}

		/// <summary>
		/// Gets the version control server from the TfsCollection
		/// </summary>
		/// <returns></returns>
		public VersionControlServer GetVcs() {
			return _collection.GetService<VersionControlServer>();
		}

		/// <summary>
		/// The wrapper for the FolderDiff object.
		/// </summary>
		private FolderDiffWrapper _folderDiff;

		/// <summary>
		/// Gets the folder differences from the FolderDiff.
		/// </summary>
		/// <returns></returns>
		public override System.Collections.Generic.HashSet<string> GetDifferentFilePaths() {
			var results = new HashSet<string>();
			var o = new List<object>();
			foreach (var x in _folderDiff) {
				dynamic folder = new AccessPrivateWrapper(x);
				dynamic val = new AccessPrivateWrapper(folder.Value);
				dynamic files = new AccessPrivateWrapper(val.Files);
				dynamic wrappedObject = files._wrapped;
				foreach (var f in wrappedObject) {
					dynamic f2 = new AccessPrivateWrapper(f);
					if (f2 != null) {
						results.Add(f2.Path2 as string);
					}

				}
			}
			return results;
		}

		/// <summary>
		/// Sets up the TFS project collection from the collection url
		/// </summary>
		public override void SetupProjectCollection() {
			base.SetupProjectCollection();
		}
	}
}