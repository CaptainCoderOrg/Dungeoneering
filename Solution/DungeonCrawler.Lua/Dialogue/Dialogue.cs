using MoonSharp.Interpreter;

namespace CaptainCoder.Dungeoneering.Lua.Dialogue;

[MoonSharpUserData]
public class Dialogue(string text, IEnumerable<DialogueOption>? options = null) : IEquatable<Dialogue>
{
    public string Text { get; set; } = text;
    public List<DialogueOption> Options { get; set; } = options?.ToList() ?? [];
    public void AddOption(DialogueOption option) => Options.Add(option);
    public void AddOption(DynValue option) => Options.Add(option.ToObject<DialogueOption>());

    public bool Equals(Dialogue other)
    {
        return Text == other.Text &&
               Options.SequenceEqual(other.Options);
    }
}

// [MoonSharpUserData]
public abstract class DialogueOption(string label)
{
    public string Label { get; set; } = label;
}

[MoonSharpUserData]
public sealed class CloseDialogueOption(string label) : DialogueOption(label)
{
    public override bool Equals(object? obj)
    {
        return obj is CloseDialogueOption option &&
               Label == option.Label;
    }

    public override int GetHashCode() => HashCode.Combine(Label);

    public override string ToString() => $"{nameof(CloseDialogueOption)}(\"{Label}\")";
}

[MoonSharpUserData]
public sealed class RunScriptDialogueOption(string label, string scriptName) : DialogueOption(label)
{
    public string ScriptName { get; set; } = scriptName;

    public override bool Equals(object? obj)
    {
        return obj is RunScriptDialogueOption option &&
               Label == option.Label &&
               ScriptName == option.ScriptName;
    }

    public override int GetHashCode() => HashCode.Combine(Label, ScriptName);

    public override string ToString() => $"{nameof(RunScriptDialogueOption)} {{ {nameof(Label)}: \"{Label}\", {nameof(ScriptName)}: {ScriptName} }}";
}

[MoonSharpUserData]
public sealed class ContinueDialogueOption(string label, Dialogue nextDialog) : DialogueOption(label), IEquatable<ContinueDialogueOption>
{
    public Dialogue NextDialogue { get; set; } = nextDialog;
    public bool Equals(ContinueDialogueOption other)
    {
        return Label == other.Label &&
               NextDialogue.Equals(other.NextDialogue);
    }
}

public static class DialogueExtensions
{
    public static void RegisterDialogueConstructors(this Script script)
    {
        script.Globals[nameof(Dialogue)] = (string text) => UserData.Create(new Dialogue(text));
        script.Globals[nameof(CloseDialogueOption)] = (string text) => UserData.Create(new CloseDialogueOption(text));
        script.Globals[nameof(RunScriptDialogueOption)] = (string text, string scriptName) => UserData.Create(new RunScriptDialogueOption(text, scriptName));
        script.Globals[nameof(ContinueDialogueOption)] = (string text, DynValue nextDialog) => UserData.Create(new ContinueDialogueOption(text, nextDialog.ToObject<Dialogue>()));
    }
}