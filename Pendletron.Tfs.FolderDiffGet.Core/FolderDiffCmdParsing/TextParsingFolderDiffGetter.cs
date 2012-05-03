using System.Collections.Generic;

namespace Pendletron.Tfs.FolderDiffGet.Core.FolderDiffCmdParsing
{
	public class TextParsingFolderDiffGetter : BaseFolderDiffGetter
	{
		public TextParsingFolderDiffGetter(string collectionUri, string srcPath, string targetPath, string outputDir, string textToParse):base(collectionUri, srcPath, targetPath, outputDir)
		{
			TextToParse = textToParse;
		}

		public string TextToParse { get; set; }
		public override HashSet<string> GetDifferentFilePaths() {
			var parser = new OutputParser(SourcePath, TargetPath);
			var results = parser.Parse(TextToParse);
			return results.MergeAll();
		}
	}
}