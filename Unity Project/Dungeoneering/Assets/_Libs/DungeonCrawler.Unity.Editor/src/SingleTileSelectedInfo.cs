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
                _selected = value;
                Text.text = $@"
Floor Tile Selected
{value.Position}
Floor Texture: {value.FloorTextureName}
".Trim();
            }
        }

        [field: SerializeField]
        public TextMeshProUGUI Text { get; private set; }        
        
    }
}