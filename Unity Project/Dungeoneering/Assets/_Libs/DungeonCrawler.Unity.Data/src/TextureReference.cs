using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Dungeoneering.DungeonMap;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Unity.Data;

public class TextureReference
{
    private static int s_nextID = 1;
    public string TextureName { get; private set; }
    public readonly TextureId TextureId;
    public SelectableMaterial Material { get; private set; }
    public UnityEngine.Texture Texture => Material.Unselected.mainTexture;
    public event System.Action<TextureReference> OnTextureChange;
    public int Count => Tiles.Count + Walls.Count;
    public bool IsDefaultTexture => DefaultTileDungeons.Any() || DefaultDoorDungeons.Any() || DefaultSolidDungeons.Any() || DefaultSecretDungeons.Any();
    internal TextureReference(string name, SelectableMaterial material)
    {
        TextureId = new(s_nextID++);
        (TextureName, Material) = (name, material);
    }
    internal void SetTexture(Texture2D newTexture)
    {
        Material.Unselected.mainTexture = newTexture;
        Material.Selected.mainTexture = newTexture;
        OnTextureChange?.Invoke(this);
    }
    internal HashSet<TileReference> Tiles { get; private set; } = new();
    internal HashSet<WallReference> Walls { get; private set; } = new();
    internal HashSet<Dungeon> DefaultTileDungeons { get; private set; } = new();
    internal HashSet<Dungeon> DefaultDoorDungeons { get; private set; } = new();
    internal HashSet<Dungeon> DefaultSolidDungeons { get; private set; } = new();
    internal HashSet<Dungeon> DefaultSecretDungeons { get; private set; } = new();
    internal void Clear() => Tiles.Clear();
}