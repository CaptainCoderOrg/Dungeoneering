using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Dungeoneering.DungeonMap;

namespace CaptainCoder.Dungeoneering.Unity.Data;
internal class TextureReferences
{
    private readonly Dictionary<string, TextureReference> _textureNames = new();
    private readonly Dictionary<TextureId, TextureReference> _references = new();
    public IEnumerable<SelectableMaterial> Materials => _references.Values.Select(r => r.Material);
    internal void Clear()
    {
        _references.Clear();
        _textureNames.Clear();
        _textureNames["No Texture"] = new TextureReference("No Texture", default);
    }
    internal TextureReference FromName(string name) => _textureNames[name];
    internal bool Remove(string name) => _textureNames.Remove(name);

    internal TextureReference Create(string name, SelectableMaterial material)
    {
        if (_textureNames.ContainsKey(name)) { throw new System.InvalidOperationException($"A reference with the name {name} already exists!"); }
        TextureReference newRef = new(name, material);
        _textureNames[name] = newRef;
        _references[newRef.TextureId] = newRef;
        return newRef;
    }

    internal TileWallMaterials GetTileWallMaterials(Dungeon dungeon, Position position)
    {
        return new TileWallMaterials()
        {
            North = _textureNames[dungeon.GetWallTexture(position, Facing.North)].Material,
            East = _textureNames[dungeon.GetWallTexture(position, Facing.East)].Material,
            South = _textureNames[dungeon.GetWallTexture(position, Facing.South)].Material,
            West = _textureNames[dungeon.GetWallTexture(position, Facing.West)].Material,
        };
    }

    internal TextureReference FromId(TextureId tId) => _references[tId];
    internal void Remove(TextureId tId)
    {
        TextureReference textureRef = _references[tId];
        _textureNames.Remove(textureRef.TextureName);
        _references.Remove(tId);
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