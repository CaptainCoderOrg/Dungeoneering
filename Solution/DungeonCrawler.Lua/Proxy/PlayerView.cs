using MoonSharp.Interpreter;

using DMP = CaptainCoder.Dungeoneering.Player;

namespace CaptainCoder.Dungeoneering.Lua.Proxy;

[MoonSharpUserData]
public class PlayerView(DMP.PlayerView target)
{
    private readonly DMP.PlayerView _target = target;
    public DynValue Position => UserData.Create(_target.Position);
    public int Facing => (int)_target.Facing;
    public override string ToString() => _target.ToString();
}