using System.Collections.Generic;

using CaptainCoder.Dungeoneering.DungeonMap;

namespace CaptainCoder.Dungeoneering.Unity.Data;
public class TextureReferences
{
    private readonly Dictionary<string, TextureReference> AllReferences = new();
    public void Clear()
    {
        AllReferences.Clear();
        Create("No Texture", default);
    }
    public TextureReference FromName(string name) => AllReferences[name];
    public bool Remove(string name) => AllReferences.Remove(name);

    public TextureReference Create(string name, SelectableMaterial material)
    {
        if (AllReferences.ContainsKey(name)) { throw new System.InvalidOperationException($"A reference with the name {name} already exists!"); }
        TextureReference newRef = new(name, material);
        AllReferences[name] = newRef;
        return newRef;
    }


    public TileWallMaterials GetTileWallMaterials(Dungeon dungeon, Position position)
    {
        return new TileWallMaterials()
        {
            North = AllReferences[dungeon.GetWallTexture(position, Facing.North)].Material,
            East = AllReferences[dungeon.GetWallTexture(position, Facing.East)].Material,
            South = AllReferences[dungeon.GetWallTexture(position, Facing.South)].Material,
            West = AllReferences[dungeon.GetWallTexture(position, Facing.West)].Material,
        };
    }
}

public class TextureReference
{
    public string TextureName { get; private set; }
    public TextureId TextureId => Material.Id;
    public SelectableMaterial Material { get; private set; }
    public int Count => Tiles.Count;
    internal TextureReference(string name, SelectableMaterial material) => (TextureName, Material) = (name, material);
    internal HashSet<TileReference> Tiles { get; private set; } = new();
    internal HashSet<WallReference> Walls { get; private set; } = new();
    internal void Clear() => Tiles.Clear();
}


public record struct TileReference(Dungeon Dungeon, Position Position);
public record struct WallReference(Dungeon Dungeon, Position Position, Facing Facing);
public record struct TextureId(int Id);