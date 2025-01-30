namespace CaptainCoder.Dungeoneering.Raylib;

using Raylib_cs;

public static class ScreenExtension
{

    public static void DrawCentered(this string text, int top, int fontSize, Color color)
    {
        int width = Raylib.MeasureText(text, fontSize);
        int center = (Raylib.GetScreenWidth() / 2) - (width / 2);
        Raylib.DrawText(text, center + 2, top + 2, fontSize, Color.Black);
        Raylib.DrawText(text, center, top, fontSize, color);
    }
}