namespace Tests;

using CaptainCoder.Dungeoneering;
using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonCrawler.Scripting;
using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.IO;

using Shouldly;

public class DungeonCrawlerManifest_should
{
    public static DungeonCrawlerManifest Empty => new();
    public static DungeonCrawlerManifest Simple
    {
        get
        {
            DungeonCrawlerManifest manifest = new();
            manifest.AddDungeon("simple", Dungeon_should.SimpleSquareDungeon);
            manifest.AddScript("simpleScript", new EventScript("simple"));
            manifest.AddTexture(new Texture("wall", [0, 2, 5]));
            return manifest;
        }
    }

    public static IEnumerable<object[]> BeJsonableData => [
        [Empty],
        [Simple],
    ];
    [Theory]
    [MemberData(nameof(BeJsonableData))]
    public void be_jsonable(DungeonCrawlerManifest underTest)
    {
        string json = underTest.ToJson();
        DungeonCrawlerManifest actual = JsonExtensions.LoadModel<DungeonCrawlerManifest>(json);
        actual.ShouldBe(underTest);
    }

    public static IEnumerable<object[]> AddDungeonsData => [

        ["simple", Dungeon_should.SimpleSquareDungeon],
        ["twobytwo", Dungeon_should.TwoByTwoRoom],
    ];
    [Theory]
    [MemberData(nameof(AddDungeonsData))]
    public void add_dungeons(string name, Dungeon toAdd)
    {
        DungeonCrawlerManifest underTest = new();

        underTest.AddDungeon(name, toAdd);

        underTest.Dungeons.Count.ShouldBe(1);
        underTest.Dungeons[name].ShouldBe(toAdd);
    }

    [Theory]
    [MemberData(nameof(AddDungeonsData))]
    public void not_allow_adding_dungeon_with_same_name(string name, Dungeon toAdd)
    {
        DungeonCrawlerManifest underTest = new();

        underTest.AddDungeon(name, toAdd);
        Should.Throw<InvalidOperationException>(() =>
        {
            underTest.AddDungeon(name, toAdd);
        });
    }

    public static IEnumerable<object[]> AddScriptData => [

        ["simple", new EventScript("simple")],
        ["twobytwo", new EventScript("twobytwo")],
    ];
    [Theory]
    [MemberData(nameof(AddScriptData))]
    public void add_script(string name, EventScript toAdd)
    {
        DungeonCrawlerManifest underTest = new();

        underTest.AddScript(name, toAdd);

        underTest.Scripts.Count.ShouldBe(1);
        underTest.Scripts[name].ShouldBe(toAdd);
    }

    public static IEnumerable<object[]> AddTextureData => [

        [new Texture("simple", [])],
        [new Texture("twobytwo", [])],
    ];
    [Theory]
    [MemberData(nameof(AddTextureData))]
    public void add_texture(Texture toAdd)
    {
        DungeonCrawlerManifest underTest = new();

        underTest.AddTexture(toAdd);

        underTest.Textures.Count.ShouldBe(1);
        underTest.Textures[toAdd.Name].ShouldBe(toAdd);
    }

    [Theory]
    [MemberData(nameof(AddScriptData))]
    public void not_allow_adding_script_with_same_name(string name, EventScript toAdd)
    {
        DungeonCrawlerManifest underTest = new();

        underTest.AddScript(name, toAdd);
        Should.Throw<InvalidOperationException>(() =>
        {
            underTest.AddScript(name, toAdd);
        });
    }

    [Fact]
    public void should_be_equal()
    {
        DungeonCrawlerManifest first = MakeManifest();
        first.ShouldBe(MakeManifest());
        static DungeonCrawlerManifest MakeManifest()
        {
            DungeonCrawlerManifest manifest = new();
            manifest.AddDungeon("simple", new Dungeon());
            manifest.AddScript("scriptname", new EventScript("Simple Script"));
            manifest.AddTexture(new Texture("wall", [1, 2, 3]));
            return manifest;
        }
    }
}