using System.Collections.Generic;

using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Unity.Data;
public class TextureReferences
{
    // TODO: Consider moving TextureId here
    // TODO: Static bad!
    private static readonly Dictionary<string, TextureReferences> AllReferences = new();
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init()
    {
        AllReferences.Clear();
        // TODO: This is a hack to create a default "No Texture" texture
        new TextureReferences("No Texture", default);
    }
    public static TextureReferences FromName(string name) => AllReferences[name];
    public static bool Remove(string name) => AllReferences.Remove(name);

    public string TextureName { get; private set; }
    public TextureId TextureId => Material.Id;
    public HashSet<TileReference> Tiles { get; private set; } = new();
    public SelectableMaterial Material { get; private set; }
    public TextureReferences(string name, SelectableMaterial material)
    {
        if (AllReferences.ContainsKey(name)) { throw new System.InvalidOperationException($"A reference with the name {name} already exists!"); }
        (TextureName, Material) = (name, material);
        AllReferences[name] = this;
    }
    public int Count => Tiles.Count;
    public void Clear() => Tiles.Clear();

    public static TileWallMaterials GetTileWallMaterials(Dungeon dungeon, Position position)
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

public record struct TileReference(Dungeon Dungeon, Position Position);
public record struct WallReference(string DungeonName, Position Position, Facing Facing);
public record struct TextureId(int Id);