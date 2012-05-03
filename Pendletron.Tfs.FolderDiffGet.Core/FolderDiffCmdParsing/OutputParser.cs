using System;
using System.Collections.Generic;

namespace Pendletron.Tfs.FolderDiffGet.Core.FolderDiffCmdParsing
{
    public class OutputParser
    {
        public OutputParser(string src, string target)
        {
            SourcePath = src;
            TargetPath = target;

            HeaderOutlineText = "=====";
            ExistOnlyInText = "Items That Exist Only in {0}";
            DifferentText = "Show Items That Have Different Contents";
            IdenticalText = "Show Items That Have Identical Contents";
            SummaryText = "Summary: ";
        }

        public string SourcePath { get; set; }
        public string TargetPath { get; set; }
        public string HeaderOutlineText { get; set; }
        public string ExistOnlyInText { get; set; }
        public string SummaryText { get; set; }
        public string IdenticalText { get; set; }
        public string DifferentText { get; set; }

        public ParseResults Parse(string text)
        {
            List<string> lines = new List<string>();
            lines.AddRange(text.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries));
            var results = new ParseResults();
            string onlyInSourceFormatted = String.Format(ExistOnlyInText, SourcePath);
            string onlyInTargetFormatted = String.Format(ExistOnlyInText, TargetPath);

            results.OnlyInSource = FindSection(lines, onlyInSourceFormatted, true);
            results.OnlyInTarget = FindSection(lines, onlyInTargetFormatted, true);
            results.DifferentContents = FindContentsSection(lines, DifferentText);
            results.IdenticalContents = FindContentsSection(lines, IdenticalText);
            results.Summary = FindSummary(lines);

            return results;
        }

        public HashSet<string> FindContentsSection(List<string> lines, string text)
        {
            var section = FindSection(lines, text, false);
            var results = new HashSet<string>();
            foreach (var s in section)
            {
                string tab = "\t";
                if(s.StartsWith(tab))
                {
                    results.Add(s.Trim());
                }
            }
            return results;
        }

        private string FindSummary(List<string> lines)
        {
            int index = lines.FindLastIndex(s => s.Contains(SummaryText));
            string results = "";
            if(index > -1)
            {
                results = lines[index];
            }
            return results;
        }

        public HashSet<string> FindSection(List<string> lines, string text, bool partial)
        {
            Predicate<string> finder = s => s.Equals(text);
            if(partial)
            {
                finder = s => s.Contains(text);
            }
            return FindSection(lines, finder);
        }

        public HashSet<string> FindSection(List<string> lines, Predicate<string> headerFinder)
        {
            int headerIndex = lines.FindIndex(headerFinder);
            var results = new HashSet<string>();
            if(headerIndex > -1)
            {
                int startAtIndex = headerIndex + 2;
                // Find the next header section so we can get everything in between
                int endOfSectionIndex = lines.FindIndex(startAtIndex, s=>s.Contains(HeaderOutlineText));
                int lineCount = endOfSectionIndex - startAtIndex;
                foreach (var x in lines.GetRange(startAtIndex, lineCount))
                {
                    results.Add(x);
                }
                
            }
            return results;
        }
    }
}