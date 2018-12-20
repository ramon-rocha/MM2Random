using MM2Randomizer.Utilities;
using Xunit;

namespace MM2Randomizer.Tests
{
    public class CsvParserTests
    {
        [Theory]
        [InlineData("line1\nline2", 2)]
        [InlineData("line1\rline2", 2)]
        [InlineData("line1\r\nline2", 2)]
        [InlineData("line1\nline2\nline3", 3)]
        [InlineData("line1\rline2\rline3", 3)]
        [InlineData("line1\r\nline2\r\nline3", 3)]
        public void ReadLines_HandlesAllLineEndings(string text, int lineCount)
        {
            var lines = CsvParser.ReadLines(text);
            Assert.Equal(lineCount, lines.Count);
        }

        [Theory]
        [InlineData("line1\n\nline2", 2)]
        [InlineData("line1\r\rline2", 2)]
        [InlineData("line1\r\n\r\nline2", 2)]
        [InlineData("line1\n\rline2", 2)]
        [InlineData("", 0)]
        [InlineData("\n\n\n", 0)]
        [InlineData("\r\r\r", 0)]
        [InlineData("\r\n\r\n\r\n", 0)]
        [InlineData("\n\r\r", 0)]
        public void ReadLines_IgnoresBlankLines(string text, int lineCount)
        {
            var lines = CsvParser.ReadLines(text);
            Assert.Equal(lineCount, lines.Count);
            foreach (string line in lines)
            {
                Assert.False(string.IsNullOrWhiteSpace(line));
            }
        }

        [Theory]
        [InlineData("#line1\n\nline2", 1)]
        [InlineData("line1\n\n#line2", 1)]
        [InlineData("#line1\n\n#line2", 0)]
        public void ReadLines_IgnoresComments(string text, int lineCount)
        {
            var lines = CsvParser.ReadLines(text);
            Assert.Equal(lineCount, lines.Count);
            foreach (string line in lines)
            {
                Assert.NotEqual('#', line[0]);
            }
        }

        [Theory]
        [InlineData(" line1\nline2")]
        [InlineData("line1\rline2 ")]
        [InlineData(" line1 \r\n line2 ")]
        [InlineData("\tline1\nline2")]
        [InlineData("line1\rline2\t")]
        [InlineData("\tline1\t\r\n\tline2\t")]
        public void ReadLines_TrimsLines(string text)
        {
            var lines = CsvParser.ReadLines(text);
            foreach (string line in lines)
            {
                Assert.Equal(line.Trim(), line);
            }
        }

        [Theory]
        [InlineData("value1,value2,value3\nvalue1,value2,value3", 2, 3)]
        [InlineData("value1\nvalue1", 2, 1)]
        public void ReadValues_DefaultsToCommaDelimiter(string text, int lineCount, int valueCount)
        {
            var data = CsvParser.ReadValues(text);
            Assert.Equal(lineCount, data.Count);
            foreach (string[] values in data)
            {
                Assert.Equal(valueCount, values.Length);
            }
        }

        [Theory]
        [InlineData("value1|value2\nvalue1|value2", '|', 2, 2)]
        [InlineData("value1,,,,|value2,,,,\nvalue1,,,,|value2,,,,", '|', 2, 2)]
        public void ReadValues_CanUseAlternateCharDelimiter(string text, char delimiter, int lineCount, int valueCount)
        {
            var data = CsvParser.ReadValues(text, delimiter);
            Assert.Equal(lineCount, data.Count);
            foreach (string[] values in data)
            {
                Assert.Equal(valueCount, values.Length);
            }
        }
    }
}
