
using System;
using System.Linq;

using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonMap;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Unity.Data
{
    [CreateAssetMenu(menuName = "DC/DungeonCrawlerData")]
    public class DungeonCrawlerData : ObservableSO
    {
        [field: SerializeField]
        public TextAsset DefaultManifestJson { get; private set; }
        public DungeonManifestData ManifestData { get; private set; }
        public DungeonData CurrentDungeon { get; private set; }
        public MaterialCache MaterialCache { get; private set; }

        private void Init()
        {
            ManifestData.RemoveObserver(HandleManifestChanged);
            ManifestData.AddObserver(HandleManifestChanged);
        }

        private void HandleManifestChanged(DungeonManifestChanged changeEvent)
        {
            switch (changeEvent)
            {
                case ManifestLoadedEvent(DungeonCrawlerManifest manifest):
                    HandleManifestLoaded(manifest);
                    break;
                case DungeonAddedEvent(Dungeon added):
                    HandleDungeonAdded(added);
                    break;
                case DungeonRemovedEvent(Dungeon removed):
                    HandleDungeonRemoved(removed);
                    break;
            }
        }

        private void HandleDungeonRemoved(Dungeon dungeon)
        {
            MaterialCache.RemoveDungeonReferences(dungeon);
        }

        private void HandleDungeonAdded(Dungeon dungeon)
        {
            CurrentDungeon.Dungeon = dungeon;
            MaterialCache.AddDungeonReferences(dungeon);
        }

        private void HandleManifestLoaded(DungeonCrawlerManifest manifest)
        {
            CurrentDungeon.Dungeon = manifest.Dungeons.First().Value.Copy();
        }

        public override void OnBeforeEnterPlayMode()
        {
            base.OnBeforeEnterPlayMode();
            ForceInitialize();
        }

#if UNITY_EDITOR
        public override void OnAfterEnterPlayMode()
        {
            base.OnAfterEnterPlayMode();
            // Required to correctly load SelectableMaterial in PlayMode
            if (!ManifestData.TryLoadManifest(DefaultManifestJson.text, out _))
            {
                Debug.Log("Manifest could not be loaded");
            }
        }
#endif

        protected override void OnExitPlayMode()
        {
            base.OnExitPlayMode();
            ManifestData.RemoveObserver(HandleManifestChanged);
        }

        /// <summary>
        /// Forces all DungeonCrawlerData to be reinitialized. This is an expensive operation and is designed for testing.
        /// Note: This clears all listeners on data objects.
        /// </summary>
        public void ForceInitialize()
        {
            MaterialCache = new();
            CurrentDungeon = new(MaterialCache);
            ManifestData = new(MaterialCache);
            CurrentDungeon.OnChange += HandleDungeonChanged;
            if (!ManifestData.TryLoadManifest(DefaultManifestJson.text, out _))
            {
                Debug.Log("Manifest could not be loaded");
            }
            Init();
        }

        private void HandleDungeonChanged(DungeonChangedData changes)
        {
            MaterialCache.RemoveDungeonReferences(changes.Previous);
            MaterialCache.AddDungeonReferences(changes.New);
        }

        public void SetDefaultWallTexture(Dungeon targetDungeon, TextureReference newTexture, WallType wallType)
        {
            if (!ManifestData.Manifest.Dungeons.TryGetValue(targetDungeon.Name, out Dungeon dungeon))
            {
                throw new InvalidOperationException($"The specified dungeon {targetDungeon.Name} does not exist in the manifest.");
            }
            TextureReference previousTexture = MaterialCache.GetTexture(dungeon.WallTextures.DefaultSolid);
            SetWallTexture(dungeon.WallTextures, wallType, newTexture.TextureName);
            previousTexture.RemoveDefaultWall(wallType, dungeon);
            newTexture.AddDefaultWall(wallType, dungeon);
            if (targetDungeon.Name == CurrentDungeon.Dungeon.Name)
            {
                previousTexture.RemoveDefaultWall(wallType, CurrentDungeon.Dungeon);
                newTexture.AddDefaultWall(wallType, CurrentDungeon.Dungeon);
                CurrentDungeon.SetDefaultWallTexture(newTexture, wallType);
            }
        }

        private void SetWallTexture(WallTextureMap wallTextures, WallType wallType, string textureName)
        {
            Action<string> setter = wallType switch
            {
                WallType.Solid => s => wallTextures.DefaultSolid = s,
                WallType.Door => s => wallTextures.DefaultDoor = s,
                WallType.SecretDoor => s => wallTextures.DefaultSecretDoor = s,
                _ => throw new Exception($"Cannot set texture for wall type None"),
            };
            setter.Invoke(textureName);
        }

        public void SetDefaultTileTexture(Dungeon targetDungeon, TextureReference newTexture)
        {
            if (!ManifestData.Manifest.Dungeons.TryGetValue(targetDungeon.Name, out Dungeon dungeon))
            {
                throw new InvalidOperationException($"The specified dungeon {targetDungeon.Name} does not exist in the manifest.");
            }
            TextureReference previousTexture = MaterialCache.GetTexture(dungeon.TileTextures.Default);
            dungeon.TileTextures.Default = newTexture.TextureName;
            previousTexture.DefaultTileDungeons.Remove(dungeon);
            newTexture.DefaultTileDungeons.Add(dungeon);
            if (targetDungeon.Name == CurrentDungeon.Dungeon.Name)
            {
                previousTexture.DefaultTileDungeons.Remove(CurrentDungeon.Dungeon);
                newTexture.DefaultTileDungeons.Add(CurrentDungeon.Dungeon);
                CurrentDungeon.SetDefaultTileTexture(newTexture);
            }
        }
    }
}