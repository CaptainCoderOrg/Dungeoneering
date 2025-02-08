using System.Collections.Generic;
using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using TMPro;
using UnityEngine;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class MultiWallInfo : MonoBehaviour
    {
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
            foreach (DungeonWallController wall in _selected)
            {
                wall.SetTexture(newTexture);
            }
        }

        private void OpenSelector(DungeonTextureButton button)
        {
            TextureSelector.ShowDialogue(MultiSetTexture, null);
        }



    }
}