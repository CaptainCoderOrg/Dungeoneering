
using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonMap.IO;

using UnityEngine;
using UnityEngine.Events;

namespace CaptainCoder.Dungeoneering.DungeonMap.Unity
{

    [CreateAssetMenu(fileName = "DungeonManifestData", menuName = "DC/Manifest")]
    public class DungeonManifestData : ObservableSO
    {
        public UnityEvent<TilesChangedData> OnTilesChanged { get; private set; } = new();
        private TilesChangedData _changes = new();
        private DungeonCrawlerManifest _manifest;
        public DungeonCrawlerManifest Manifest => _manifest ??= LoadManifest();
        private Dictionary<string, Material> _materialCache;
        public Dictionary<string, Material> MaterialCache => _materialCache ??= InitializeMaterialCache(Manifest);
        [field: SerializeField]
        public TextAsset ManifestJson { get; private set; }
        public DungeonCrawlerManifest LoadManifest() => JsonExtensions.LoadModel<DungeonCrawlerManifest>(ManifestJson.text);

        public void Notify()
        {
            if (_changes.Tiles.Count == 0) { return; }
            OnTilesChanged.Invoke(_changes);
            _changes = new();
        }

        private Dictionary<string, Material> InitializeMaterialCache(DungeonCrawlerManifest manifest)
        {
            Debug.Log("Initializing Cache");
            _materialCache = manifest.Textures.Values.ToDictionary(t => t.Name, t => t.ToMaterial());
            return _materialCache;
        }

        protected override void AfterEnabled()
        {
            base.AfterEnabled();
            _materialCache = null;
            _manifest = null;
            OnTilesChanged.RemoveAllListeners();
        }

        protected override void OnEnterPlayMode()
        {
            base.OnEnterPlayMode();
            _materialCache = null;
            _manifest = null;
        }

        protected override void OnExitPlayMode()
        {
            base.OnExitPlayMode();
            _materialCache = null;
            _manifest = null;
            OnTilesChanged.RemoveAllListeners();
        }

        public void SetFloorTexture(Dungeon dungeon, Position position, string textureName)
        {
            dungeon.TileTextures.Textures[position] = textureName;
            _changes.AddChange(dungeon, position);
        }

        public void SetWallTexture(Dungeon dungeon, Position position, Facing facing, string textureName)
        {
            dungeon.SetTexture(position, facing, textureName);
            _changes.AddChange(dungeon, position);
        }

        public void SetWallType(Dungeon dungeon, Position position, Facing facing, WallType type)
        {
            if (dungeon.Walls[position, facing] == type) { return; }
            dungeon.WallTextures.Textures.Remove((position, facing));
            dungeon.WallTextures.Textures.Remove((position.Step(facing), facing.Opposite()));
            dungeon.Walls.SetWall(position, facing, type);
            _changes.AddChange(dungeon, position);
        }

        public string GetFloorTexture(Dungeon d, Position p) => d.TileTextures.GetTileTextureName(p);
        public string GetWallTexture(Dungeon d, Position p, Facing f) => d.GetWallTexture(p, f);
    }

    public class TilesChangedData
    {
        public HashSet<(Dungeon, Position)> Tiles { get; private set; } = new();
        public bool AddChange(Dungeon dungeon, Position position) => Tiles.Add((dungeon, position));
    }
}