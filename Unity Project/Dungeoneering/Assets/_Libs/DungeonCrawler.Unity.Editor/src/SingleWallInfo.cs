using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using TMPro;
using UnityEngine;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class SingleWallInfo : MonoBehaviour
    {
        [SerializeField]
        private UndoRedoStackData _undoRedoStack;
        private DungeonWallController _selected;
        public DungeonWallController Selected
        {
            get => _selected;
            set
            {
                _selected?.Parent.Manifest.OnTilesChanged.RemoveListener(RefreshInfo);
                _selected = value;
                _selected?.Parent.Manifest.OnTilesChanged.AddListener(RefreshInfo);
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
            TextureButton.Image.texture = _selected.Material?.mainTexture;
            Text.text = $@"
Wall Tile Selected
Position: {Selected.Parent.Position}
Facing: {Selected.Facing}
Type: {Selected.WallType}
".Trim();
        }

        private void RefreshInfo(TilesChangedData _) => Render();
        private void SetTexture(string textureName)
        {
            DungeonManifestData manifest = Selected.Parent.Manifest;
            Dungeon d = Selected.Parent.Dungeon;
            Position p = Selected.Parent.Position;
            Facing f = Selected.Facing;
            string originalTexture = manifest.GetWallTexture(d, p, f);
            System.Action perform = () => manifest.SetWallTexture(d, p, f, textureName);
            System.Action undo = () => manifest.SetWallTexture(d, p, f, originalTexture);
            _undoRedoStack.PerformEdit("Set Wall Texture", perform, undo, manifest);
        }

        private void OpenSelector(DungeonTextureButton button)
        {
            TextureSelector.ShowDialogue(SetTexture, null);
        }
    }
}