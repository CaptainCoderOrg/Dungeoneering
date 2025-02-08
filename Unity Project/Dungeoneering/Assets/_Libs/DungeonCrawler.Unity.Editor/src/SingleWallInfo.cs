using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using TMPro;
using UnityEngine;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class SingleWallInfo : MonoBehaviour
    {
        private DungeonWallController _selected;
        public DungeonWallController Selected
        {
            get => _selected;
            set
            {
                _selected?.Parent.Manifest.OnTileChanged.RemoveListener(RefreshInfo);
                _selected = value;
                _selected?.Parent.Manifest.OnTileChanged.AddListener(RefreshInfo);
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
            TextureButton.Image.texture = _selected.Material.mainTexture;
            Text.text = $@"
Wall Tile Selected
Position: {Selected.Parent.Position}
Facing: {Selected.Facing}
Type: {Selected.WallType}
".Trim();
        }

        private void RefreshInfo(Dungeon dungeon, Position position)
        {
            if (dungeon == _selected.Parent.Dungeon && position == _selected.Parent.Position)
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