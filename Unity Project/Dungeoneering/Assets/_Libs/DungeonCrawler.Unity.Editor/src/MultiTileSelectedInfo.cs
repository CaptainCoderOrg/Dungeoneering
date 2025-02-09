using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using TMPro;
using UnityEngine;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class MultiTileSelectedInfo : MonoBehaviour
    {
        [SerializeField]
        private UndoRedoStackData _undoRedoStack;
        private IEnumerable<DungeonTile> _selected;
        public IEnumerable<DungeonTile> Selected
        {
            get => _selected;
            set
            {
                _selected = value;
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
            Text.text = $@"
Multiple Tiles Selected
".Trim();
        }

        private void MultiSetTexture(string newTexture)
        {
            System.Action perform = default;
            System.Action undo = default;
            foreach (DungeonTile tile in _selected)
            {
                DungeonManifestData manifest = tile.Manifest;
                Dungeon d = tile.Dungeon;
                Position p = tile.Position;
                string originalTexture = manifest.GetFloorTexture(d, p);
                perform += () => manifest.SetFloorTexture(d, p, newTexture);
                undo += () => manifest.SetFloorTexture(d, p, originalTexture);
            }
            _undoRedoStack.PerformEdit("Set Multiple Textures", perform, undo);
        }

        private void OpenSelector(DungeonTextureButton button)
        {
            TextureSelector.ShowDialogue(MultiSetTexture, null);
        }
    }
}