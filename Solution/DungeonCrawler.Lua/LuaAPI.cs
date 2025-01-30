using MoonSharp.Interpreter;

namespace CaptainCoder.Dungeoneering.Lua;

[MoonSharpUserData]
public class LuaAPI
{
    public int Sum(int x, int y) => x + y;
}