
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonMap.IO;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;

namespace CaptainCoder.Dungeoneering.DungeonMap.Unity
{

    [CreateAssetMenu(fileName = "DungeonManifestData", menuName = "DC/Manifest")]
    public class DungeonManifestData : ObservableSO
    {
        public UnityEvent<Dungeon, Position> OnTileChanged { get; private set; } = new();
        private DungeonCrawlerManifest _manifest;
        public DungeonCrawlerManifest Manifest => _manifest ??= LoadManifest();
        private Dictionary<string, Material> _materialCache;
        public Dictionary<string, Material> MaterialCache => _materialCache ??= InitializeMaterialCache(Manifest);
        
        [field: SerializeField] 
        public TextAsset ManifestJson { get; private set; }
        public DungeonCrawlerManifest LoadManifest() => JsonExtensions.LoadModel<DungeonCrawlerManifest>(ManifestJson.text);

        // public DungeonCrawlerManifest LoadFromFile(string path) => JsonExtensions.LoadModel<DungeonCrawlerManifest>(File.ReadAllText(path));

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
            OnTileChanged.RemoveAllListeners();
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
            OnTileChanged.RemoveAllListeners();            
        }

        public void SetFloorTexture(Dungeon dungeon, Position position, string textureName)
        {
            dungeon.TileTextures.Textures[position] = textureName;
            OnTileChanged.Invoke(dungeon, position);
            // File.WriteAllText("Assets/_Project/Data/Ikea/Test.json", Manifest.ToJson());
        }

        public void SetWallTexture(Dungeon dungeon, Position position, Facing facing, string textureName)
        {
            // dungeon.TileTextures.Textures[position] = textureName;
            dungeon.SetTexture(position, facing, textureName);
            OnTileChanged.Invoke(dungeon, position);
            // File.WriteAllText("Assets/_Project/Data/Ikea/Test.json", Manifest.ToJson());
        }

        public void SetWallType(Dungeon dungeon, Position position, Facing facing, WallType type)
        {
            if (dungeon.Walls[position, facing] == type) { return; }
            dungeon.Walls[position, facing] = type;
            OnTileChanged.Invoke(dungeon, position);
        }

        public string GetFloorTexture(Dungeon d, Position p) => d.TileTextures.GetTileTextureName(p);
        public string GetWallTexture(Dungeon d, Position p, Facing f) => d.GetWallTexture(p, f);
    }
}