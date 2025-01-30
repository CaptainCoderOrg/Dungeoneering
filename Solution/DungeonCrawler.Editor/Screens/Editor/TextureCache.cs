namespace CaptainCoder.Dungeoneering.Raylib;

using CaptainCoder.Dungeoneering.Editor;

using Raylib_cs;

public static class TextureCache
{
    private readonly static Dictionary<string, Texture2D> Cache = new();
    public static Texture2D GetTexture(string projectName, string textureName)
    {
        string path = Path.Combine(EditorConstants.SaveDir, projectName, Project.TextureDir, textureName);
        if (!Cache.TryGetValue(path, out Texture2D texture))
        {
            texture = Raylib.LoadTexture(path);
            Cache[path] = texture;
        }
        return texture;
    }
}