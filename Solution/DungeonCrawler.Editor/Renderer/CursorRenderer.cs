namespace CaptainCoder.Dungeoneering.Raylib;

using System.Numerics;

using Raylib_cs;

public static class CursorExtensions
{
    public static void Render(this Cursor cursor, int cellSize = DungeonEditorScreen.CellSize, int left = 0, int top = 0)
    {
        float x = (float)(cursor.Position.X * cellSize + left);
        float y = (float)(cursor.Position.Y * cellSize + top);
        Vector2[] points = CursorPoints(cursor.Facing, x, y);
        Raylib.DrawTriangle(points[0], points[1], points[2], Color.Yellow);
    }

    public static Vector2[] CursorPoints(Facing direction, float x, float y, int cellSize = DungeonEditorScreen.CellSize)
    {
        const int padding = 8;
        return direction switch
        {
            Facing.North => [
                new Vector2(x + cellSize * 0.5f, y + padding),
                new Vector2(x + padding, y + cellSize - padding),
                new Vector2(x + cellSize - padding, y + cellSize - padding)
            ],
            Facing.South => [
                new Vector2(x + padding, y + padding),
                new Vector2(x + cellSize * 0.5f, y + cellSize - padding),
                new Vector2(x + cellSize - padding, y + padding),
            ],
            Facing.East => [
                new Vector2(x + padding, y + padding),
                new Vector2(x + padding, y + cellSize - padding),
                new Vector2(x + cellSize - padding, y + cellSize * 0.5f),
            ],
            Facing.West => [
                new Vector2(x + cellSize - padding, y + padding),
                new Vector2(x + padding, y + cellSize * 0.5f),
                new Vector2(x + cellSize - padding, y + cellSize - padding),
            ],
            _ => throw new Exception($"Unexpected facing: {direction}"),
        };
    }
}