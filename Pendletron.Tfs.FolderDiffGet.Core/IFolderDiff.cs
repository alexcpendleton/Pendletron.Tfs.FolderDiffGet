using Microsoft.TeamFoundation.VersionControl.Client;

namespace Pendletron.Tfs.FolderDiffGet.Core
{
	public interface IFolderDiffEntry
	{
		bool IsPath2Local { get; set; }
		VersionSpec Path2VersionSpec { get; set; }
		string Path1 { get; set; }
		string Path2 { get; set; }
		ItemType ItemType { get; }
	}
}