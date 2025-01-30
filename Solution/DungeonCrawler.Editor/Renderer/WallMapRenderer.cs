namespace CaptainCoder.Dungeoneering.Raylib;

using System.Numerics;

using Raylib_cs;

public static class WallMapRenderer
{
    public static void RenderWalls(this Dungeon dungeon, string projectName, int left = 0, int top = 0)
    {
        foreach (TileEdge edge in dungeon.Walls.Map.Keys)
        {
            Texture2D texture = TextureCache.GetTexture(projectName, dungeon.GetWallTexture(edge.Position, edge.Facing));
            RectInfo side0 = edge.ToRectInfo(texture, DungeonEditorScreen.CellSize, left, top);
            TileEdge opposite = edge with { Position = edge.Position.Step(edge.Facing), Facing = edge.Facing.Opposite() };
            Texture2D texture2 = TextureCache.GetTexture(projectName, dungeon.GetWallTexture(opposite.Position, opposite.Facing));
            RectInfo side1 = opposite.ToRectInfo(texture2, DungeonEditorScreen.CellSize, left, top);
            side0.Render();
            side1.Render();
        }
    }

    private static Color GetWallColor(WallType wall) => wall switch
    {
        WallType.Solid => Color.DarkGray,
        WallType.Door => Color.Beige,
        WallType.SecretDoor => Color.Blue,
        _ => throw new Exception($"Unknown wall type: {wall}"),
    };

    public static RectInfo ToRectInfo(this TileEdge edge, Texture2D texture, int cellSize, int left = 0, int top = 0)
    {
        const int halfWallWidth = 8;
        float baseX = edge.Position.X * cellSize + left;
        float baseY = edge.Position.Y * cellSize + top;
        var (x, y) = edge.Facing switch
        {
            Facing.North => (baseX, baseY),
            Facing.South => (baseX, baseY + cellSize - halfWallWidth),
            Facing.East => (baseX + cellSize - halfWallWidth, baseY),
            Facing.West => (baseX, baseY),
            _ => throw new NotImplementedException($"Unknown facing: {edge.Facing}"),
        };
        var (width, height) = edge.Facing switch
        {
            Facing.North or Facing.South => (cellSize, halfWallWidth),
            Facing.East or Facing.West => (halfWallWidth, cellSize),
            _ => throw new NotImplementedException($"Unknown facing: {edge.Facing}"),
        };
        return new RectInfo(texture) { Target = new Rectangle(x, y, width, height) };
    }

    public static Line ToScreenCoords(this TileEdge edge, int cellSize, int left = 0, int top = 0)
    {
        float baseX = edge.Position.X * cellSize + left;
        float baseY = edge.Position.Y * cellSize + top;
        var (startX, startY) = edge.Facing switch
        {
            Facing.North or Facing.West => (baseX, baseY),
            Facing.South => (baseX, baseY + cellSize),
            Facing.East => (baseX + cellSize, baseY),
            _ => throw new NotImplementedException($"Unknown facing: {edge.Facing}"),
        };
        var (endX, endY) = edge.Facing switch
        {
            Facing.North or Facing.South => (startX + cellSize, startY),
            Facing.East or Facing.West => (startX, startY + cellSize),
            _ => throw new NotImplementedException($"Unknown facing: {edge.Facing}"),
        };
        return new Line(startX, startY, endX, endY);
    }

    public static void Render(this Line line, float thickness, Color color)
    {
        Raylib.DrawLineEx(
            new Vector2(line.StartX, line.StartY),
            new Vector2(line.EndX, line.EndY),
            thickness,
            color
        );
    }
}

public record struct Line(float StartX, float StartY, float EndX, float EndY);