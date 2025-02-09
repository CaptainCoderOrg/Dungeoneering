using System.Collections.Generic;
using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using TMPro;
using UnityEngine;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class MultiWallInfo : MonoBehaviour
    {
        [SerializeField]
        private UndoRedoStackData _undoRedoStack;
        private IEnumerable<DungeonWallController> _selected;
        public IEnumerable<DungeonWallController> Selected
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
Multiple Walls Selected
".Trim();
        }

        private void MultiSetTexture(string newTexture)
        {
            System.Action perform = default;
            System.Action undo = default;
            foreach (DungeonWallController wall in _selected)
            {
                DungeonManifestData manifest = wall.Parent.Manifest;
                Dungeon d = wall.Parent.Dungeon;
                Position p = wall.Parent.Position;
                Facing f = wall.Facing;
                string originalTexture = manifest.GetWallTexture(d, p, f);
                perform += () => manifest.SetWallTexture(d, p, f, newTexture);
                undo += () => manifest.SetWallTexture(d, p, f, originalTexture);
            }
            _undoRedoStack.PerformEdit("Set Multiple Wall Textures", perform, undo);
        }

        private void OpenSelector(DungeonTextureButton button)
        {
            TextureSelector.ShowDialogue(MultiSetTexture, null);
        }

    }
}