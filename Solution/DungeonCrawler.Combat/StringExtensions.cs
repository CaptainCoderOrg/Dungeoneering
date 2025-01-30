using System.Text.RegularExpressions;

namespace CaptainCoder.DungeonCrawler;

// I hate  this. This is a hack. The source file is in DungeonCrawler.csproj
internal static class StringExtensions
{
    internal static Regex NewLineRegex { get; } = new(@"\r\n?|\n");
    internal static string ReplaceNewLines(this string toReplace) => NewLineRegex.Replace(toReplace, Environment.NewLine);

}