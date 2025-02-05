using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using TMPro;
using UnityEngine;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class SingleTileSelectedInfo : MonoBehaviour
    {
        private DungeonTile _selected;
        public DungeonTile Selected
        {
            get => _selected;
            set
            {
                _selected?.Manifest.OnTileChanged.RemoveListener(RefreshInfo);
                _selected = value;
                _selected?.Manifest.OnTileChanged.AddListener(RefreshInfo);
                Render();

            }
        }

        [field: SerializeField]
        public TextMeshProUGUI Text { get; private set; }
        [field: SerializeField]
        public DungeonTextureButton TextureButton { get; private set; }
        [field: SerializeField]
        public DungeonTextureSelectorController TextureSelector { get; private set; }

        void Awake()
        {
            Debug.Assert(Text != null, this);
            Debug.Assert(TextureButton != null, this);
            TextureButton.OnClick.AddListener(OpenSelector);
        }

        private void Render()
        {
            TextureButton.Image.texture = _selected.FloorTile.material.mainTexture;
            Text.text = $@"
Floor Tile Selected
{Selected.Position}
Floor Texture: {Selected.FloorTextureName}
".Trim();
        }

        private void RefreshInfo(Dungeon dungeon, Position position)
        {
            if (dungeon == _selected.Dungeon && position == _selected.Position)
            {
                Render();
            }
        }

        private void OpenSelector(DungeonTextureButton button)
        {
            TextureSelector.ShowDialogue(Selected.SetTexture, null);
        }



    }
}