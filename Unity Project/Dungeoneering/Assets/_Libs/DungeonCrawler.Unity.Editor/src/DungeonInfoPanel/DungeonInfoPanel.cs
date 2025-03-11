using System.Linq;

using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.Unity.Data;
using CaptainCoder.Unity;

using TMPro;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class DungeonInfoPanel : MonoBehaviour
    {
        [SerializeField]
        private DungeonCrawlerData _dungeonCrawlerData;
        [SerializeField]
        private TextureSelectorPanel _dungeonTextureSelector;
        [SerializeField]
        private TMP_InputField _dungeonNameInput;
        [SerializeField]
        private DungeonTextureButton _tileTexture;
        [SerializeField]
        private DungeonTextureButton _doorTexture;
        [SerializeField]
        private DungeonTextureButton _wallTexture;
        [SerializeField]
        private DungeonTextureButton _secretTexture;

        private Dungeon _dungeon;

        void Awake()
        {
            Assertion.NotNull(this,
                (_dungeonCrawlerData, nameof(_dungeonCrawlerData)),
                (_dungeonTextureSelector, nameof(_dungeonTextureSelector)),
                (_dungeonNameInput, nameof(_dungeonNameInput)),
                (_tileTexture, nameof(_tileTexture)),
                (_doorTexture, nameof(_doorTexture)),
                (_wallTexture, nameof(_wallTexture)),
                (_secretTexture, nameof(_secretTexture))
            );
            Show(_dungeonCrawlerData.ManifestData.Manifest.Dungeons.Values.First());
        }

        public void Show(Dungeon dungeon)
        {
            _dungeon = dungeon;
            _dungeonNameInput.text = dungeon.Name;
            _tileTexture.Texture = _dungeonCrawlerData.MaterialCache.GetTexture(dungeon.TileTextures.Default);
            _doorTexture.Texture = _dungeonCrawlerData.MaterialCache.GetTexture(dungeon.WallTextures.DefaultDoor);
            _wallTexture.Texture = _dungeonCrawlerData.MaterialCache.GetTexture(dungeon.WallTextures.DefaultSolid);
            _secretTexture.Texture = _dungeonCrawlerData.MaterialCache.GetTexture(dungeon.WallTextures.DefaultSecretDoor);
            gameObject.SetActive(true);
        }

        public void PromptTileTexture() => _dungeonTextureSelector.ShowDialogue(UpdateTileTexture);
        public void PromptWallTexture() => _dungeonTextureSelector.ShowDialogue(UpdateWallTexture);
        public void PromptDoorTexture() => _dungeonTextureSelector.ShowDialogue(UpdateDoorTexture);
        public void PromptSecretTexture() => _dungeonTextureSelector.ShowDialogue(UpdateSecretTexture);

        private void UpdateWallTexture(TextureReference newTexture)
        {
            _dungeonCrawlerData.SetDefaultWallTexture(_dungeon, newTexture, WallType.Solid);
            _wallTexture.Texture = newTexture;
        }
        private void UpdateDoorTexture(TextureReference newTexture)
        {
            _dungeonCrawlerData.SetDefaultWallTexture(_dungeon, newTexture, WallType.Door);
            _doorTexture.Texture = newTexture;
        }

        private void UpdateSecretTexture(TextureReference newTexture)
        {
            _dungeonCrawlerData.SetDefaultWallTexture(_dungeon, newTexture, WallType.SecretDoor);
            _secretTexture.Texture = newTexture;
        }

        private void UpdateTileTexture(TextureReference newTexture)
        {
            _dungeonCrawlerData.SetDefaultTileTexture(_dungeon, newTexture);
            _tileTexture.Texture = newTexture;
        }
    }
}