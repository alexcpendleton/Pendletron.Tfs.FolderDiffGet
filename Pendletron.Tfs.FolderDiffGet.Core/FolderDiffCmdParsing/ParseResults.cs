using System.Collections.Generic;

namespace Pendletron.Tfs.FolderDiffGet.Core.FolderDiffCmdParsing
{
    public class ParseResults
    {
        public ParseResults()
        {
            OnlyInSource = new HashSet<string>();
            OnlyInTarget = new HashSet<string>();
            DifferentContents = new HashSet<string>();
            IdenticalContents = new HashSet<string>();
            Summary = "";
        }
        public HashSet<string> OnlyInSource { get; set; }
        public HashSet<string> OnlyInTarget { get; set; }
        public HashSet<string> DifferentContents { get; set; }
        public HashSet<string> IdenticalContents { get; set; }

        public HashSet<string> MergeAll()
        {
            var results = new HashSet<string>();
            results.UnionWith(OnlyInSource);
            results.UnionWith(OnlyInTarget);
            results.UnionWith(DifferentContents);
            results.UnionWith(IdenticalContents);
            return results;
        } 
        public string Summary { get; set; }
    }
}