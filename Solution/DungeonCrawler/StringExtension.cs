using System.Text.RegularExpressions;

namespace CaptainCoder.Dungeoneering;

public static class StringExtensions
{
    public static Regex NewLineRegex { get; } = new(@"\r\n?|\n");
    public static string ReplaceNewLines(this string toReplace) => NewLineRegex.Replace(toReplace, Environment.NewLine);

}