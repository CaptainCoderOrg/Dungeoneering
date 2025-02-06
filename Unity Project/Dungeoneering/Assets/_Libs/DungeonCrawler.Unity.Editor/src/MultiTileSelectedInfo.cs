using System.Collections;
using System.Collections.Generic;
using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using TMPro;
using UnityEngine;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class MultiTileSelectedInfo : MonoBehaviour
    {
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
            // TextureButton.Image.texture = _selected.FloorTile.material.mainTexture;
            Text.text = $@"
Multiple Tiles Selected
".Trim();
        }

        // private void RefreshInfo(IEnumerable<DungeonTile>)
        // {
        //     if (dungeon == _selected.Dungeon && position == _selected.Position)
        //     {
        //         Render();
        //     }
        // }

        private void MultiSetTexture(string newTexture)
        {
            foreach (DungeonTile tile in _selected)
            {
                tile.SetTexture(newTexture);
            }
        }

        private void OpenSelector(DungeonTextureButton button)
        {
            TextureSelector.ShowDialogue(MultiSetTexture, null);
        }



    }
}