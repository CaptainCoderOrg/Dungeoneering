using System;
using System.IO;

using CaptainCoder.Dungeoneering.Lua;

using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Serialization;
public class Program
{
    public static void Main()
    {
        Console.WriteLine("Generating hardwired Lua output");
        File.WriteAllText("hardwired.moonlua", Hardwired.GenerateHardwiredLua());
    }
}


public static class Hardwired
{
    public static string GenerateHardwiredLua()
    {
        UserData.RegisterAssembly(typeof(LuaAPI).Assembly);
        return UserData.GetDescriptionOfRegisteredTypes(true).Serialize();
    }
}