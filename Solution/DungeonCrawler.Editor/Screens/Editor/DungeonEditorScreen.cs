namespace CaptainCoder.Dungeoneering.Raylib;

using CaptainCoder.Dungeoneering.DungeonMap.IO;
using CaptainCoder.Dungeoneering.Editor;

using Raylib_cs;

public class DungeonEditorScreen(string projectName) : IScreen
{
    public const int MaxMapSize = 24;
    public const int CellSize = 32;
    private readonly string _dungeonDirectory = Path.Combine(EditorConstants.SaveDir, projectName, Project.DungeonDir);
    public string ProjectName { get; } = projectName;
    public Cursor Cursor { get; private set; } = new(new Position(0, 0), Facing.West);
    public WallType Wall { get; private set; } = WallType.Solid;
    public Dungeon CurrentDungeon { get; private set; } = new(WallMapExtensions.CreateEmpty(MaxMapSize, MaxMapSize));
    public TextureResult SelectedTexture { get; set; } = DefaultTexture.Shared;
    private string? _filename = null;
    private readonly InfoOverLayScreen _overlay = new();
    private IScreen? _editorMenu;
    public IScreen EditorMenu
    {
        get
        {
            if (_editorMenu is null)
            {
                _editorMenu = new ModalMenuScreen(
                    this,
                    new MenuScreen("Menu",
                    [
                        new DynamicEntry(
                            () => "Save " + (Path.GetFileNameWithoutExtension(_filename) ?? string.Empty),
                            Save
                        ),
                        new StaticEntry("Save As", SaveAs),
                        new StaticEntry("New Map", NewMap),
                        new StaticEntry("Randomize Map", RandomizeMap),
                        new StaticEntry("Load", LoadMap),
                        new StaticEntry("Exit Editor", () => Program.Screen = new ProjectScreen(ProjectName)),
                    ]
                ));
            }
            return _editorMenu;
        }
    }

    private void RandomizeMap()
    {
        _filename = null;
        CurrentDungeon = new Dungeon(WallMapExtensions.RandomMap(MaxMapSize, MaxMapSize, .50, .25, .10));
        Program.Screen = this;
    }

    private void NewMap()
    {
        _filename = null;
        CurrentDungeon = new Dungeon(WallMapExtensions.CreateEmpty(MaxMapSize, MaxMapSize));
        Program.Screen = this;
    }

    public void Save()
    {
        if (_filename is null)
        {
            SaveAs();
            return;
        }
        CurrentDungeon.Name = Path.GetFileNameWithoutExtension(_filename);
        Console.WriteLine($"Dungeon name: {CurrentDungeon.Name}");
        File.WriteAllText(_filename, JsonExtensions.ToJson(CurrentDungeon));
        _overlay.AddMessage($"File saved: {Path.GetFileNameWithoutExtension(_filename)}!", Color.Green);
        Program.Screen = this;
    }

    public void SaveAs()
    {
        Program.Screen = new PromptScreen("Save As", this, OnFinished);
        void OnFinished(string filename)
        {
            _filename = Path.Combine(_dungeonDirectory, $"{filename}.json");
            Save();
        }
    }

    public void LoadMap()
    {
        string[] filenames = Directory.GetFiles(_dungeonDirectory);
        Program.Screen = new ModalMenuScreen(
            this,
            new MenuScreen(
                "Load Map",
                filenames.Select((file, ix) => new StaticEntry(Path.GetFileNameWithoutExtension(file), () => LoadMap(file)))
            )
        );

        void LoadMap(string filename)
        {
            if (!File.Exists(filename))
            {
                Console.Error.WriteLine($"File not found: {filename}");
                _overlay.AddMessage($"File not found: {filename}", Color.Red);
                return;
            }
            _overlay.AddMessage($"File loaded!", Color.Green);

            _filename = filename;
            string json = File.ReadAllText(filename);
            CurrentDungeon = JsonExtensions.LoadModel<Dungeon>(json);
            Program.Screen = this;
        }
    }

    public void Render()
    {
        Raylib.ClearBackground(Color.Black);
        int offset = CellSize * 2;
        RenderTiles(offset, offset, CurrentDungeon.TileTextures);
        CurrentDungeon.RenderWalls(ProjectName, offset, offset);
        Cursor.Render(CellSize, offset, offset);
        RenderScriptedTiles(offset, CurrentDungeon.EventMap.Events.Keys);
        RenderInfoAtCursor(offset + (MaxMapSize + 1) * CellSize, 2 * CellSize);
        _overlay.Render();
    }

    private void RenderScriptedTiles(int offset, IEnumerable<Position> positions)
    {
        foreach (Position position in positions)
        {
            int top = offset + position.Y * CellSize + CellSize / 2;
            int left = offset + position.X * CellSize + CellSize / 2;
            Raylib.DrawCircle(left, top, 4, Color.DarkPurple);
        }
    }

    private void RenderTiles(int left, int top, TileTextureMap map)
    {
        for (int y = 0; y < MaxMapSize; y++)
        {
            for (int x = 0; x < MaxMapSize; x++)
            {
                Position position = new(x, y);
                Texture2D texture = TextureCache.GetTexture(ProjectName, map.GetTileTextureName(position));
                RectInfo tile = new(texture) { Target = new Rectangle(left + x * CellSize, top + y * CellSize, CellSize, CellSize) };
                tile.Render();
            }
        }
    }

    private void RenderInfoAtCursor(int left, int top)
    {
        const int fontSize = 20;
        const int padding = 2;
        var (pos, facing) = Cursor;
        string dungeonName = Path.GetFileNameWithoutExtension(_filename) ?? "Untitled Dungeon";
        DrawText($"{dungeonName}");
        DrawText($"({pos.X}, {pos.Y}) - {facing}");
        WallType wallType = CurrentDungeon.Walls.GetWall(pos, facing);
        DrawText($"WallType: {wallType}");
        DrawText($"Wall Texture: {CurrentDungeon.GetWallTexture(Cursor.Position, Cursor.Facing)}");
        DrawText($"Tile Texture: {CurrentDungeon.TileTextures.GetTileTextureName(Cursor.Position)}");
        DrawText($"Selected Texture: {TextureName(SelectedTexture)}");
        DrawText("Scripts:");
        foreach (TileEvent evt in CurrentDungeon.EventMap.EventsAt(pos))
        {
            DrawText($"  {evt.Trigger}: {evt.ScriptName}");
        }

        void DrawText(string text)
        {
            Raylib.DrawText(text, left, top, fontSize, Color.White);
            top += fontSize + padding;
        }
        static string TextureName(TextureResult texture) => texture switch
        {
            DefaultTexture => "Default",
            TextureReference(string name) => name,
            _ => throw new NotImplementedException($"Unknown texture result: {texture}."),
        };
    }

    public void HandleUserInput()
    {
        HandleCursorMovement();
        SelectTexture();
        PaintTileTexture();
        PaintWall();

        if (Raylib.IsKeyPressed(KeyboardKey.Minus))
        {
            Program.Screen = new ScriptsScreen(this, ProjectName, Cursor.Position, CurrentDungeon.EventMap);
        }
        if (Raylib.IsKeyPressed(KeyboardKey.Tab))
        {
            Wall = Wall.Next();
            _overlay.AddMessage($"WallType: {Wall}", Color.Green, .5f);
        }
        if (Raylib.IsKeyPressed(KeyboardKey.Escape))
        {
            Program.Screen = EditorMenu;
        }
    }
    private void PaintWall()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Space))
        {
            bool shouldRemoveWall = CurrentDungeon.Walls.TryGetWall(Cursor.Position, Cursor.Facing, out WallType wall) &&
                                    wall == Wall &&
                                    SelectedTextureName() == CurrentDungeon.GetWallTexture(Cursor.Position, Cursor.Facing);
            if (shouldRemoveWall)
            {
                CurrentDungeon.Walls.RemoveWall(Cursor.Position, Cursor.Facing);
                CurrentDungeon.WallTextures.Textures.Remove((Cursor.Position, Cursor.Facing));
            }
            else
            {
                CurrentDungeon.Walls.SetWall(Cursor.Position, Cursor.Facing, Wall);
                Action action = SelectedTexture switch
                {
                    DefaultTexture => () => CurrentDungeon.WallTextures.Textures.Remove((Cursor.Position, Cursor.Facing)),
                    TextureReference(string textureName) => () => CurrentDungeon.SetTexture(Cursor.Position, Cursor.Facing, textureName),
                    _ => throw new NotImplementedException($"Unknown TextureResult: {SelectedTexture}"),
                };
                action.Invoke();
            }
        }
    }

    private string SelectedTextureName() => (SelectedTexture, Wall) switch
    {
        (DefaultTexture, WallType.Solid) => CurrentDungeon.WallTextures.DefaultSolid,
        (DefaultTexture, WallType.Door) => CurrentDungeon.WallTextures.DefaultDoor,
        (DefaultTexture, WallType.SecretDoor) => CurrentDungeon.WallTextures.DefaultSecretDoor,
        (TextureReference(string textureName), _) => textureName,
        _ => string.Empty,
    };
    private void PaintTileTexture()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.J))
        {
            Action action = SelectedTexture switch
            {
                DefaultTexture => () => CurrentDungeon.TileTextures.Textures.Remove(Cursor.Position),
                TextureReference(string textureName) => () => CurrentDungeon.TileTextures.Textures[Cursor.Position] = textureName,
                _ => throw new NotImplementedException($"Unknown TextureResult: {SelectedTexture}"),
            };
            action.Invoke();
        }
    }

    private void SelectTexture()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.T))
        {
            Program.Screen = new SelectTextureScreen
            {
                ProjectName = ProjectName,
                PreviousScreen = this,
                OnFinished = (TextureResult result) => SelectedTexture = result
            };
        }
    }

    private bool IsShiftDown => Raylib.IsKeyDown(KeyboardKey.LeftShift) || Raylib.IsKeyDown(KeyboardKey.RightShift);
    private bool IsWestKeyPressed => Raylib.IsKeyPressed(KeyboardKey.A) || Raylib.IsKeyPressed(KeyboardKey.Left);
    private bool IsNorthKeyPressed => Raylib.IsKeyPressed(KeyboardKey.W) || Raylib.IsKeyPressed(KeyboardKey.Up);
    private bool IsSouthKeyPressed => Raylib.IsKeyPressed(KeyboardKey.S) || Raylib.IsKeyPressed(KeyboardKey.Down);
    private bool IsEastKeyPressed => Raylib.IsKeyPressed(KeyboardKey.D) || Raylib.IsKeyPressed(KeyboardKey.Right);

    private void HandleCursorMovement()
    {
        if (IsShiftDown && IsNorthKeyPressed)
        {
            Cursor = Cursor.Move(Facing.North);
        }
        else if (IsShiftDown && IsSouthKeyPressed)
        {
            Cursor = Cursor.Move(Facing.South);
        }
        else if (IsShiftDown && IsEastKeyPressed)
        {
            Cursor = Cursor.Move(Facing.East);
        }
        else if (IsShiftDown && IsWestKeyPressed)
        {
            Cursor = Cursor.Move(Facing.West);
        }
        else if (IsNorthKeyPressed)
        {
            Cursor = MoveOrRotate(Facing.North);
        }
        else if (IsEastKeyPressed)
        {
            Cursor = MoveOrRotate(Facing.East);
        }
        else if (IsSouthKeyPressed)
        {
            Cursor = MoveOrRotate(Facing.South);
        }
        else if (IsWestKeyPressed)
        {
            Cursor = MoveOrRotate(Facing.West);
        }

        Cursor = Cursor with { Position = Cursor.Position.Clamp(MaxMapSize, MaxMapSize) };
    }

    private Cursor MoveOrRotate(Facing facing)
    {
        if (Cursor.Facing == facing)
        {
            return Cursor.MoveAndRotate(facing);
        }
        else
        {
            return Cursor.Turn(facing);
        }
    }
}