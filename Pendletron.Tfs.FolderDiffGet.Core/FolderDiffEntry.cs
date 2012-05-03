using Microsoft.TeamFoundation.VersionControl.Client;

namespace Pendletron.Tfs.FolderDiffGet.Core
{
	public class FolderDiffEntry : IFolderDiffEntry
	{
		public FolderDiffEntry() {
			IsPath2Local = false;
			
		}

		public FolderDiffEntry(dynamic dynamicSource):this()
		{
			Path1 = dynamicSource.Path1;
			Path2 = dynamicSource.Path2;
			ItemType = dynamicSource.ItemType;
		}

		public bool IsPath2Local { get; set; }
		public VersionSpec Path2VersionSpec { get; set; }
		public string Path1 { get; set; }
		public string Path2 { get; set; }
		public ItemType ItemType { get; set; }
	}
}