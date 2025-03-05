using System.Collections.Generic;

using CaptainCoder.Dungeoneering.DungeonMap;

namespace CaptainCoder.Dungeoneering.Unity.Data;
internal class TextureDatabase
{
    public readonly TextureId DefaultTextureId = new(0);
    private readonly Dictionary<string, TextureReference> _textureNames = new();
    private readonly Dictionary<TextureId, TextureReference> _references = new();
    public IEnumerable<TextureReference> Textures => _references.Values;
    internal void Clear()
    {
        _references.Clear();
        _textureNames.Clear();
        _textureNames["No Texture"] = new TextureReference("No Texture", default);
    }
    internal TextureReference FromName(string name) => _textureNames[name];
    internal bool Remove(string name) => _textureNames.Remove(name);

    internal TextureReference Create(Texture dungeonTexture)
    {
        if (_textureNames.ContainsKey(dungeonTexture.Name)) { throw new System.InvalidOperationException($"A reference with the name {dungeonTexture.Name} already exists!"); }
        TextureReference newRef = new(dungeonTexture.Name, new SelectableMaterial(dungeonTexture.ToMaterial()));
        _textureNames[newRef.TextureName] = newRef;
        _references[newRef.TextureId] = newRef;
        return newRef;
    }

    internal TileWallTextures GetTileWallMaterials(Dungeon dungeon, Position position)
    {
        return new TileWallTextures()
        {
            North = _textureNames[dungeon.GetWallTexture(position, Facing.North)],
            East = _textureNames[dungeon.GetWallTexture(position, Facing.East)],
            South = _textureNames[dungeon.GetWallTexture(position, Facing.South)],
            West = _textureNames[dungeon.GetWallTexture(position, Facing.West)],
        };
    }

    internal TextureReference FromId(TextureId tId) => _references[tId];

    internal void Remove(TextureReference textureRef)
    {
        _textureNames.Remove(textureRef.TextureName);
        _references.Remove(textureRef.TextureId);
    }
}