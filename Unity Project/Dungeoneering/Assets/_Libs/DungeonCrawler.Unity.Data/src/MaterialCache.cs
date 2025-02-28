using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;

using UnityEngine;
using UnityEngine.Events;

namespace CaptainCoder.Dungeoneering.Unity;

public record struct TileReference(string DungeonName, Position Position);
public record struct WallReference(string DungeonName, Position Position, Facing Facing);
public class MaterialCache
{
    private Dictionary<string, SelectableMaterial> _materialCache = new();
    private Dictionary<int, HashSet<TileReference>> _tileReferences = new();
    private Dictionary<int, string> _idToName = new();
    private readonly UnityEvent<CacheUpdateData> _onCacheChanged = new();
    private DungeonCrawlerManifest _manifest;

    public Dictionary<string, SelectableMaterial> InitializeMaterialCache(DungeonCrawlerManifest manifest)
    {
        Debug.Log("Initializing Cache");
        _manifest = manifest;
        _materialCache.Clear();
        foreach ((string textureName, Texture texture) in manifest.Textures)
        {
            SelectableMaterial material = new(texture.ToMaterial());
            _materialCache[textureName] = material;
            _idToName[material.Id] = textureName;
        }
        BuildReferenceCache();
        _onCacheChanged.Invoke(new CacheUpdateData(_materialCache, true));
        return _materialCache;
    }

    private void BuildReferenceCache()
    {
        _tileReferences.Clear();
        foreach ((string dungeonName, Dungeon dungeon) in _manifest.Dungeons)
        {
            foreach ((Position position, string textureName) in dungeon.TileTextures.Textures)
            {
                if (!_materialCache.TryGetValue(textureName, out SelectableMaterial material))
                {
                    // TODO: Consider defaulting to "default-tile.png" and removing the reference in the dungeon
                    throw new System.Exception($"Dungeon references texture '{textureName}' that was not found in material cache.");
                }
                if (!_tileReferences.TryGetValue(material.Id, out HashSet<TileReference> tileRefs))
                {
                    tileRefs = new HashSet<TileReference>();
                    _tileReferences[material.Id] = tileRefs;
                }
                tileRefs.Add(new TileReference(dungeonName, position));
            }
        }
    }

    public void RemoveTextureReference(int id, DungeonManifestData manifestData, DungeonData dungeonData)
    {
        RemoveTextureReferenceFromDungeonData(id, dungeonData);
        Debug.Log($"Total found: {_tileReferences[id].Count()}");
        foreach (TileReference tileRef in _tileReferences[id])
        {
            manifestData.Manifest.Dungeons[tileRef.DungeonName].TileTextures.Textures.Remove(tileRef.Position);
        }
        _tileReferences[id].Clear();
    }

    private void RemoveTextureReferenceFromDungeonData(int id, DungeonData dungeonData)
    {
        List<TileReference> toRemove = new();
        HashSet<TileReference> trs = _tileReferences[id];
        TileReference[] inData = trs.Where(t => t.DungeonName == dungeonData.Dungeon.Name).ToArray();
        Debug.Log($"Total in dungeon data: {inData.Count()}");
        foreach (TileReference tileRef in inData)
        {
            Debug.Log(tileRef);
            dungeonData.RemoveFloorTexture(tileRef.Position);
        }
        dungeonData.Notify();
    }


    public void UpdateTileReference(Dictionary<int, HashSet<TileReference>> tileReferences, int oldId, int newId)
    {
        foreach (TileReference tileRef in tileReferences[oldId])
        {
            string newTexture = _idToName[newId];
            _manifest.Dungeons[tileRef.DungeonName].TileTextures.Textures[tileRef.Position] = newTexture;
        }
        tileReferences[newId].UnionWith(tileReferences[oldId]);
        tileReferences[oldId].Clear();
    }


    public void RemoveListener(UnityAction<CacheUpdateData> onChange) => _onCacheChanged.RemoveListener(onChange);

    public void AddListener(UnityAction<CacheUpdateData> onChange)
    {
        _onCacheChanged.AddListener(onChange);
        if (_materialCache != null)
        {
            onChange.Invoke(new CacheUpdateData(_materialCache, true));
        }
    }

    public void AddTexture(string name, Texture2D texture)
    {
        if (_manifest.Textures.ContainsKey(name)) { return; }
        Texture dungeonTexture = new(name, ImageConversion.EncodeToPNG(texture));
        _manifest.AddTexture(dungeonTexture);
        _materialCache.Add(name, new SelectableMaterial(dungeonTexture.ToMaterial()));
        _onCacheChanged.Invoke(new CacheUpdateData(_materialCache, false, name));
    }

    public void Clear()
    {
        _materialCache.Clear();
        _onCacheChanged.RemoveAllListeners();
        _manifest = null;
    }

    public SelectableMaterial GetTileMaterial(Dungeon d, Position p) => _materialCache.GetTileMaterial(d, p);
    public TileWallMaterials GetTileWallMaterials(Dungeon d, Position p) => _materialCache.GetTileWallMaterials(d, p);
    public SelectableMaterial GetMaterial(string textureName) => _materialCache[textureName];
}
