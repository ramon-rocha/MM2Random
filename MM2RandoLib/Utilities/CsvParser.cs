using System;
using System.Collections.Generic;
using System.Linq;

namespace MM2Randomizer.Utilities
{
    public static class CsvParser
    {
        private static readonly char[] _lineEndings = new[] { '\r', '\n' };

        public static IReadOnlyList<string> ReadLines(string text)
        {
            string[] allLines = text.Split(_lineEndings, StringSplitOptions.RemoveEmptyEntries);
            var dataLines = new List<string>(allLines.Length);
            for (int i = 0; i < allLines.Length; i++)
            {
                string line = allLines[i].Trim();
                if (line.StartsWith("#") || line.Length == 0)
                {
                    continue;
                }
                dataLines.Add(line);
            }
            return dataLines;
        }

        public static IReadOnlyList<string[]> ReadValues(string text) => ReadValues(text, ',');

        public static IReadOnlyList<string[]> ReadValues(string text, char delimiter)
        {
            IReadOnlyList<string> lines = ReadLines(text);
            var data = new List<string[]>(lines.Count);
            foreach (string line in lines)
            {
                string[] values = line
                    .Split(delimiter)
                    .Select(p => p.Trim())
                    .ToArray();
                data.Add(values);
            }
            return data;
        }
    }
}
