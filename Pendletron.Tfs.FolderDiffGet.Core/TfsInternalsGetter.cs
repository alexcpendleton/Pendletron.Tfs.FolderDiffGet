using System.Collections.Generic;
using Pendletron.Tfs.FolderDiffGet.Core.Wrappers;
using Microsoft.TeamFoundation.VersionControl.Client;
namespace Pendletron.Tfs.FolderDiffGet.Core {
	public class TfsInternalsGetter : BaseFolderDiffGetter {
		public TfsInternalsGetter(string collectionUri, string srcPath, string targetPath, string outputDir)
			: base(collectionUri,srcPath,targetPath,outputDir)
		{
			
		}

		public override void Go()
		{
			SetupProjectCollection();
			var srcSpec = VersionSpec.Latest;
			var targetSpec = VersionSpec.Latest;
			var recursion = RecursionType.Full;
			string assemblyPath =
				@"D:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\PrivateAssemblies\Microsoft.TeamFoundation.VersionControl.Controls.dll"; // TODO: Make this loaded from config or something
			_folderDiff = new FolderDiffWrapper(assemblyPath, SourcePath, srcSpec, TargetPath, targetSpec, GetVcs(), recursion);
			_folderDiff.ViewSame = ViewSame;
			_folderDiff.ViewDifferent = ViewDifferent;
			_folderDiff.ViewSourceOnly = ViewSourceOnly;
			_folderDiff.ViewTargetOnly = ViewTargetOnly;
			base.Go();
		}

		public VersionControlServer GetVcs()
		{
			return _collection.GetService<VersionControlServer>();
		}

		private FolderDiffWrapper _folderDiff;

		public override System.Collections.Generic.HashSet<string> GetDifferentFilePaths() {
			var results = new HashSet<string>();
			var o = new List<object>();
			foreach (var x in _folderDiff)
			{
				dynamic folder = new AccessPrivateWrapper(x);
				dynamic val = new AccessPrivateWrapper(folder.Value);
				AccessPrivateWrapper files = new AccessPrivateWrapper(val.Files);
				dynamic fuck = files._wrapped;
				foreach (var f in fuck)
				{
					dynamic f2 = new AccessPrivateWrapper(f);
					if (f2 != null)
					{
						results.Add(f2.Path2 as string);
					}

				}
			}
			return results;
		}

		public override void SetupProjectCollection() {
			base.SetupProjectCollection();
		}
	}
}