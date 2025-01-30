namespace CaptainCoder.Dungeoneering.Raylib;
public abstract record MenuEntry(Action OnSelect);
public record StaticEntry(string Text, Action OnSelect) : MenuEntry(OnSelect)
{
    public override string ToString() => Text;
}
public record DynamicEntry(Func<string> GetString, Action OnSelect) : MenuEntry(OnSelect)
{
    public override string ToString() => GetString();
}