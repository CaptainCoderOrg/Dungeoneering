using CaptainCoder.Dungeoneering.DungeonCrawler.Scripting;
using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.Lua.Dialogue;
using CaptainCoder.Dungeoneering.Player;

using MoonSharp.Interpreter;

namespace CaptainCoder.Dungeoneering.Lua;

public static class Interpreter
{
    private static bool s_initialized = false;
    public static T EvalLua<T>(string script, IScriptContext context) => EvalLua(script, context).ToObject<T>();
    public static void ExecLua(string script, IScriptContext context) => EvalLua(script, context);
    public static DynValue EvalLua(string script, IScriptContext context) => Initialize(context).DoString(script);
    public static T EvalRawLua<T>(string script) => Initialize().DoString(script).ToObject<T>();
    private static Script Initialize(IScriptContext? context = null)
    {
        if (!s_initialized)
        {
            LuaInitializer.Initialize();
            UserData.RegisterProxyType<Proxy.PlayerView, PlayerView>(data => new Proxy.PlayerView(data));
            UserData.RegisterProxyType<Proxy.Position, Position>(data => new Proxy.Position(data));
            s_initialized = true;
        }
        Script lua = new();
        lua.RegisterDialogueConstructors();
        if (context is not null)
        {
            lua.Globals.Set("context", UserData.Create(new LuaContext(context)));
        }
        lua.Globals.Set("LuaAPI", UserData.Create(new LuaAPI()));
        lua.RegisterEnum<Facing>();
        lua.RegisterEnum<WallType>();
        lua.RegisterEnum<ScriptSound>();
        return lua;
    }


}

public static class LuaExtensions
{
    public static void RegisterEnum<TEnum>(this Script lua) where TEnum : struct
    {
        foreach (string name in Enum.GetNames(typeof(TEnum)))
        {
            lua.Globals[name] = Enum.Parse<TEnum>(name);
        }
    }
}

public static class EventScriptExtensions
{
    public static void Invoke(this EventScript script, IScriptContext context) => Interpreter.ExecLua(script.Script, context);
}