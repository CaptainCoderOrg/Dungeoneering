using MoonSharp.Interpreter;

using DM = CaptainCoder.Dungeoneering.DungeonMap;

namespace CaptainCoder.Dungeoneering.Lua.Proxy;


[MoonSharpUserData]
public class Position(DM.Position target)
{
    private readonly DM.Position _target = target;
    public int X => _target.X;
    public int Y => _target.Y;
    public override string ToString() => _target.ToString();
}