using System.Numerics;

namespace CaptainCoder.Dungeoneering.Raylib;

using Raylib_cs;

public sealed class RectInfo(Texture2D texture)
{
    public Texture2D Texture { get; init; } = texture;
    public Rectangle Source { get; init; } = new(0, 0, texture.Width, texture.Height);
    public Rectangle Target { get; init; } = new(0, 0, texture.Width, texture.Height);
    public Vector2 Origin { get; init; } = new(0, 0);
    public float Rotation { get; init; } = 0;
    public Color Tint { get; init; } = Color.White;
}

public static class TileRenderer
{
    public static void Render(this RectInfo tile)
    {
        Raylib.DrawTexturePro(tile.Texture, tile.Source, tile.Target, tile.Origin, tile.Rotation, tile.Tint);
        //public static extern void DrawRectangleLinesEx(Rectangle rec, float lineThick, Color color);
        Raylib.DrawRectangleLinesEx(tile.Target, 1, new Color(40, 40, 40, 255));
    }
}