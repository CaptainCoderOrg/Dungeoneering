using System.Collections.Generic;

namespace CaptainCoder.Dungeoneering.Unity.Data;

public class TextureReference
{
    private static int s_nextID = 1;
    public string TextureName { get; private set; }
    public readonly TextureId TextureId;
    public SelectableMaterial Material { get; private set; }
    public int Count => Tiles.Count + Walls.Count;
    internal TextureReference(string name, SelectableMaterial material)
    {
        TextureId = new(s_nextID++);
        (TextureName, Material) = (name, material);
    }
    internal HashSet<TileReference> Tiles { get; private set; } = new();
    internal HashSet<WallReference> Walls { get; private set; } = new();
    internal void Clear() => Tiles.Clear();
}