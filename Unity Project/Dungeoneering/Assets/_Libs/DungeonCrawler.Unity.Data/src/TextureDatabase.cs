using System.Collections.Generic;

using CaptainCoder.Dungeoneering.DungeonMap;

namespace CaptainCoder.Dungeoneering.Unity.Data;
internal class TextureDatabase
{
    public readonly TextureId DefaultTextureId = new(0);
    public const string NO_TEXTURE = "No Texture";
    private readonly TextureReference _noTexture = new(NO_TEXTURE, default);
    private readonly Dictionary<string, TextureReference> _textureNames = new();
    public IEnumerable<TextureReference> Textures => _textureNames.Values;
    internal void Clear()
    {
        _textureNames.Clear();
    }
    internal TextureReference FromName(string name) => name == NO_TEXTURE ? _noTexture : _textureNames[name];
    internal bool Remove(string name) => _textureNames.Remove(name);

    internal TextureReference Create(Texture dungeonTexture)
    {
        if (_textureNames.ContainsKey(dungeonTexture.Name)) { throw new System.InvalidOperationException($"A reference with the name {dungeonTexture.Name} already exists!"); }
        TextureReference newRef = new(dungeonTexture.Name, new SelectableMaterial(dungeonTexture.ToMaterial()));
        _textureNames[newRef.TextureName] = newRef;
        return newRef;
    }

    internal TileWallTextures GetTileWallMaterials(Dungeon dungeon, Position position)
    {
        return new TileWallTextures()
        {
            North = FromName(dungeon.GetWallTexture(position, Facing.North)),
            East = FromName(dungeon.GetWallTexture(position, Facing.East)),
            South = FromName(dungeon.GetWallTexture(position, Facing.South)),
            West = FromName(dungeon.GetWallTexture(position, Facing.West)),
        };
    }

    internal void Remove(TextureReference textureRef)
    {
        _textureNames.Remove(textureRef.TextureName);
    }
}