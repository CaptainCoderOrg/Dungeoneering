namespace Tests;

using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonCrawler.Scripting;
using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.Game;
using CaptainCoder.Dungeoneering.Lua;
using CaptainCoder.Dungeoneering.Lua.Dialogue;
using CaptainCoder.Dungeoneering.Player;

using NSubstitute;

using Shouldly;

public class LuaInterpreter_should
{

    [Fact]
    public void interop_with_LuaAPI()
    {
        Interpreter.EvalRawLua<int>("return LuaAPI:Sum(5, 7)").ShouldBe(12);
    }

    [Fact]
    public void set_player_position()
    {
        IScriptContext context = Substitute.For<IScriptContext>();
        context.View = new PlayerView(0, 0, Facing.North);
        Interpreter.ExecLua("""
        context.SetPlayerPosition(5, 7)
        """, context);
        context.View.ShouldBe(new CaptainCoder.Dungeoneering.Player.PlayerView(new CaptainCoder.Dungeoneering.DungeonMap.Position(5, 7), Facing.North));
    }

    [Theory]
    [InlineData(Facing.North)]
    [InlineData(Facing.East)]
    [InlineData(Facing.South)]
    [InlineData(Facing.West)]
    public void set_player_facing(Facing facing)
    {
        IScriptContext context = Substitute.For<IScriptContext>();
        context.View = new PlayerView(0, 0, Facing.North);
        Interpreter.ExecLua($"""
        context.SetPlayerFacing({facing})
        """, context);
        context.View.ShouldBe(new CaptainCoder.Dungeoneering.Player.PlayerView(new CaptainCoder.Dungeoneering.DungeonMap.Position(0, 0), facing));
    }

    [Fact]
    public void set_player_view()
    {
        IScriptContext context = Substitute.For<IScriptContext>();
        Interpreter.ExecLua("""
        context.SetPlayerView(3, 4, East)
        """, context);
        context.View.ShouldBe(new CaptainCoder.Dungeoneering.Player.PlayerView(new CaptainCoder.Dungeoneering.DungeonMap.Position(3, 4), Facing.East));
    }

    [Theory]
    [InlineData("return context.PlayerView.Position.X", 5)]
    [InlineData("return context.PlayerView.Position.Y", 7)]
    [InlineData("return context.PlayerView.Facing", (int)Facing.East)]
    public void have_access_to_player_view(string script, int expectedResult)
    {
        IScriptContext context = Substitute.For<IScriptContext>();
        context.View = new CaptainCoder.Dungeoneering.Player.PlayerView(new CaptainCoder.Dungeoneering.DungeonMap.Position(5, 7), Facing.East);

        int result = Interpreter.EvalLua<int>(script, context);

        result.ShouldBe(expectedResult);
    }

    public static Dungeon TwoByTwoRoom
    {
        get
        {
            Dungeon dungeon = new();
            dungeon.Walls.SetWall(new Position(0, 0), Facing.North, WallType.Solid);
            dungeon.Walls.SetWall(new Position(0, 0), Facing.West, WallType.Solid);

            dungeon.Walls.SetWall(new Position(1, 0), Facing.North, WallType.Solid);
            dungeon.Walls.SetWall(new Position(1, 0), Facing.East, WallType.Solid);

            dungeon.Walls.SetWall(new Position(1, 1), Facing.South, WallType.Solid);
            dungeon.Walls.SetWall(new Position(1, 1), Facing.East, WallType.Solid);

            dungeon.Walls.SetWall(new Position(0, 1), Facing.South, WallType.Solid);
            dungeon.Walls.SetWall(new Position(0, 1), Facing.West, WallType.Solid);
            return dungeon;
        }
    }

    [Theory]
    [InlineData("return context.GetWall()", WallType.Solid)]
    [InlineData("""
        context.SetPlayerFacing(East)
        return context.GetWall()
        """,
        WallType.None)
    ]
    public void get_wall(string script, WallType expected)
    {
        IScriptContext context = Substitute.For<IScriptContext>();
        context.View = new PlayerView(new Position(0, 0), Facing.North);
        context.CurrentDungeon.Returns(TwoByTwoRoom);

        WallType actual = Interpreter.EvalLua<WallType>(script, context);
        actual.ShouldBe(expected);
    }

    [Theory]
    [InlineData(0, 0, Facing.North)]
    [InlineData(1, 0, Facing.South)]
    [InlineData(1, 1, Facing.East)]
    [InlineData(0, 0, Facing.West)]
    public void get_wall_at(int x, int y, Facing facing)
    {
        string script = $"""
        return context.GetWallAt({x}, {y}, {facing})
        """;
        IScriptContext context = Substitute.For<IScriptContext>();
        context.View = new PlayerView(new Position(0, 0), Facing.North);
        context.CurrentDungeon.Returns(TwoByTwoRoom);

        WallType actual = Interpreter.EvalLua<WallType>(script, context);
        WallType expected = TwoByTwoRoom.Walls.GetWall(new Position(x, y), facing);
        actual.ShouldBe(expected);
    }

    [Theory]
    [InlineData("context.SetWall(Solid)", 5, 5, Facing.North, WallType.Solid)]
    [InlineData("context.SetWall(None)", 0, 0, Facing.East, WallType.None)]
    [InlineData("context.SetWall(Door)", 2, 2, Facing.West, WallType.Door)]
    [InlineData("context.SetWall(SecretDoor)", 7, 9, Facing.South, WallType.SecretDoor)]
    public void set_wall(string script, int x, int y, Facing facing, WallType expected)
    {
        IScriptContext context = Substitute.For<IScriptContext>();
        context.View = new PlayerView(new Position(x, y), facing);
        Dungeon emptyDungeon = new();
        context.CurrentDungeon.Returns(emptyDungeon);

        Interpreter.ExecLua(script, context);

        WallType actual = emptyDungeon.Walls.GetWall(context.View.Position, context.View.Facing);
        actual.ShouldBe(expected);
        actual = emptyDungeon.Walls.GetWall(context.View.Position.Step(context.View.Facing), context.View.Facing.Opposite());
        actual.ShouldBe(expected);
    }

    [Theory]
    [InlineData(3, 4, Facing.East, WallType.None)]
    [InlineData(5, 5, Facing.North, WallType.Solid)]
    [InlineData(0, 0, Facing.East, WallType.None)]
    [InlineData(2, 2, Facing.West, WallType.Door)]
    [InlineData(7, 9, Facing.South, WallType.SecretDoor)]
    public void set_wall_at(int x, int y, Facing facing, WallType expected)
    {
        string script = $"""
        context.SetWallAt({x}, {y}, {facing}, {expected})
        """;
        IScriptContext context = Substitute.For<IScriptContext>();
        context.View = new PlayerView(new Position(x, y), facing);
        Dungeon emptyDungeon = new();
        context.CurrentDungeon.Returns(emptyDungeon);

        Interpreter.ExecLua(script, context);

        WallType actual = emptyDungeon.Walls.GetWall(new Position(x, y), facing);
        actual.ShouldBe(expected);
        actual = emptyDungeon.Walls.GetWall(new Position(x, y).Step(facing), facing.Opposite());
        actual.ShouldBe(expected);
    }

    [Theory]
    [InlineData("""
    context.WriteInfo("Hello world!")
    """,
    MessageType.Info, "Hello world!")]
    [InlineData("""
    context.Debug("Debug info.")
    """,
    MessageType.Debug, "Debug info.")]
    public void send_messages(string script, MessageType type, string message)
    {
        var context = Substitute.For<IScriptContext>();
        Interpreter.ExecLua(script, context);
        context.Received().SendMessage(new Message(type, message));
    }

    [Theory]
    [InlineData("in-tavern", "true", true)]
    [InlineData("in-tavern", "false", false)]
    [InlineData("gold", "10", 10)]
    [InlineData("name", "'bob'", "bob")]
    public void set_global_variable(string name, string value, object expected)
    {
        IScriptContext context = Substitute.For<IScriptContext>();
        context.State = new GameState();
        string script = $"""
        context.SetVariable("{name}", {value})
        """;

        Interpreter.ExecLua(script, context);

        context.State.GlobalVariables.Count.ShouldBe(1);
        context.State.GlobalVariables[name].ShouldBe(expected);
    }

    [Theory]
    [InlineData("in-tavern", true)]
    [InlineData("in-tavern", false)]
    [InlineData("gold", 10)]
    [InlineData("name", "bob")]
    public void get_global_variable(string name, object expected)
    {
        IScriptContext context = Substitute.For<IScriptContext>();
        context.State = new GameState();
        context.State.GlobalVariables[name] = expected;
        string script = $"""
        return context.GetVariable("{name}")
        """;

        object actual = Interpreter.EvalLua<object>(script, context);
        actual.ShouldBe(expected);
    }

    [Theory]
    [InlineData("no-value")]
    [InlineData("no-value2")]
    [InlineData("tricky")]
    public void get_unset_global_returns_nil(string name)
    {
        IScriptContext context = Substitute.For<IScriptContext>();
        context.State = new GameState();
        string script = $"""
        return context.GetVariable("{name}") == nil
        """;

        bool actual = Interpreter.EvalLua<bool>(script, context);
        actual.ShouldBeTrue();
    }

    [Theory]
    [InlineData("Town", 7, 9, Facing.South)]
    [InlineData("Forest", 2, 3, Facing.East)]
    public void change_dungeon(string dungeonName, int x, int y, Facing facing)
    {
        Dungeon expectedDungeon = Dungeon_should.SimpleSquareDungeon;
        IScriptContext context = Substitute.For<IScriptContext>();
        context.Manifest = new();
        context.Manifest.AddDungeon(dungeonName, expectedDungeon);
        string script = $"""
        context.ChangeDungeon("{dungeonName}", {x}, {y}, {facing})
        """;

        Interpreter.ExecLua(script, context);

        context.CurrentDungeon.ShouldBe(expectedDungeon);
        PlayerView expectedView = new(x, y, facing);
        context.View.ShouldBe(expectedView);
    }

    [Theory]
    [InlineData("some-url")]
    [InlineData("another-url")]
    public void forward_load_dungeon_request_to_delegate(string expected)
    {
        string? actual = default!;
        LuaContext.LoadFromURL = url => actual = url;
        string script = $"""context.LoadCrawlerFromURL("{expected}")""";
        Interpreter.ExecLua(script, Substitute.For<IScriptContext>());
        actual.ShouldBe(expected);
    }

    [Theory]
    [InlineData("wood.png", 5, 7, Facing.North)]
    [InlineData("stone.png", 3, 5, Facing.East)]
    [InlineData("water.png", 2, 4, Facing.South)]
    [InlineData("moss.png", 12, 14, Facing.West)]
    public void set_wall_texture(string textureName, int x, int y, Facing facing)
    {
        IScriptContext context = Substitute.For<IScriptContext>();
        Position position = new(x, y);
        context.View = new PlayerView(position, facing);
        context.CurrentDungeon = new Dungeon()
        {
            Walls = new WallMap()
            {
                Map = new Dictionary<TileEdge, WallType>{
                { new TileEdge(position, facing), WallType.Solid }
            }
            },
        };
        string script = $"""context.SetWallTexture("{textureName}")""";
        Interpreter.ExecLua(script, context);

        string actual = context.CurrentDungeon.GetWallTexture(context.View.Position, context.View.Facing);
        actual.ShouldBe(textureName);
    }

    [Theory]
    [InlineData("script.lua", "return 7", 7)]
    [InlineData("tavern.lua", "return 11 + 4", 15)]
    public void run_script(string scriptName, string targetScript, int expectedResult)
    {
        IScriptContext context = Substitute.For<IScriptContext>();
        context.Manifest = new();
        context.Manifest.AddScript(scriptName, new EventScript(targetScript));
        string script = $"""x = context.RunScript("{scriptName}") return x""";

        int actual = Interpreter.EvalLua<int>(script, context);

        actual.ShouldBe(expectedResult);
    }

    [Theory]
    [InlineData("Some dialogue.", "Close", "another option", "some.lua")]
    [InlineData("Goodbye", "...", "second option", "gainxp.lua")]
    public void construct_dialog(string message, string label, string label2, string scriptName)
    {
        IScriptContext context = Substitute.For<IScriptContext>();
        string script = $"""
        dialogue = Dialogue("{message}")
        dialogue.AddOption(CloseDialogueOption("{label}"))
        dialogue.AddOption(RunScriptDialogueOption("{label2}", "{scriptName}"))
        return dialogue
        """;
        Dialogue dialogue = Interpreter.EvalLua<Dialogue>(script, context);

        Dialogue expected = new(message, [new CloseDialogueOption(label), new RunScriptDialogueOption(label2, scriptName)]);
        dialogue.ShouldBe(expected);
    }

    [Theory]
    [InlineData("some label")]
    [InlineData("another label")]
    public void construct_close_dialogue_option(string label)
    {
        IScriptContext context = Substitute.For<IScriptContext>();
        string script = $"""
        return CloseDialogueOption("{label}")
        """;

        CloseDialogueOption option = Interpreter.EvalLua<CloseDialogueOption>(script, context);

        option.ShouldBe(new CloseDialogueOption(label));
    }

    [Theory]
    [InlineData("some label", "script.lua")]
    [InlineData("another label", "tavern.lua")]
    public void construct_run_script_dialogue(string label, string scriptname)
    {
        IScriptContext context = Substitute.For<IScriptContext>();
        string script = $"""
        return RunScriptDialogueOption("{label}", "{scriptname}")
        """;

        RunScriptDialogueOption option = Interpreter.EvalLua<RunScriptDialogueOption>(script, context);

        option.ShouldBe(new RunScriptDialogueOption(label, scriptname));
    }

    [Theory]
    [InlineData("some label", "some dialogue", "done")]
    [InlineData("another label", "some other dialogue", "okay")]
    public void construct_continue_dialogue(string label, string nextDialogue, string closeLabel)
    {
        IScriptContext context = Substitute.For<IScriptContext>();
        string script = $"""
        dialogue = Dialogue("{nextDialogue}")
        dialogue.AddOption(CloseDialogueOption("{closeLabel}"))
        return ContinueDialogueOption("{label}", dialogue)
        """;

        ContinueDialogueOption option = Interpreter.EvalLua<ContinueDialogueOption>(script, context);

        Dialogue dialogue = new(nextDialogue, [new CloseDialogueOption(closeLabel)]);
        option.ShouldBe(new ContinueDialogueOption(label, dialogue));
    }

    [Fact]
    public void show_dialogue()
    {
        IScriptContext context = Substitute.For<IScriptContext>();
        Dialogue simple = new("Test");

        Interpreter.ExecLua("""context.ShowDialogue(Dialogue("Test"))""", context);

        context.ReceivedWithAnyArgs().ShowDialogue(simple);
    }

    [Fact]
    public void allow_nested_dialogue()
    {
        string script = """
        local parent = Dialogue("some dialog")

        local child = Dialogue("More dialogue")
        parent.AddOption(ContinueDialogueOption("Continue", child));

        return priestDialogue
        """;
        Interpreter.EvalLua<Dialogue>(script, Substitute.For<IScriptContext>());
    }

    [Fact]
    public void start_combat()
    {
        string script = """
        map = [[
        ####
        ####
        ####
        ]]
        context.StartCombat(map, "onfinish.lua", "onfail.lua")
        """;
        IScriptContext context = Substitute.For<IScriptContext>();
        Interpreter.ExecLua(script, context);
        string expectedString = $"####{Environment.NewLine}####{Environment.NewLine}####";
        context.Received().StartCombat(expectedString, "onfinish.lua", "onfail.lua");
    }
}