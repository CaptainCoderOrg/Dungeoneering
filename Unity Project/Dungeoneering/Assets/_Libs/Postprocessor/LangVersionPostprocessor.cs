using System.Text.RegularExpressions;

using UnityEditor;

/// <remarks>
/// This class need to be placed in Assets/Standard Assets/Editor folder to be compiled before other scripts.
/// See <see href="https://docs.unity3d.com/Manual/ScriptCompileOrderFolders.html"/>.
/// </remarks>
internal sealed class LangVersionPostprocessor : AssetPostprocessor
{
    private static string OnGeneratedCSProject(string path, string content)
    {
        var pattern = @"<LangVersion>(.*?)<\/LangVersion>";
        var replacement = "<LangVersion>10.0</LangVersion>";
        content = Regex.Replace(content, pattern, replacement);
        return content;
    }
}