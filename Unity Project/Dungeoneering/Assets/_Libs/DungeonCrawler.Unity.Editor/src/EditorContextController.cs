using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using UnityEngine;
using UnityEngine.InputSystem;
namespace CaptainCoder.Dungeoneering.Unity
{
    public class EditorContextController : MonoBehaviour
    {
        [field: SerializeField]
        public DungeonEditorSelectionData SelectionData { get; private set; }
        

        public void Click(DungeonTile clicked)
        {
            if (Keyboard.current.shiftKey.isPressed)
            {
                SelectionData.AddTileSelection(clicked);
            }
            else
            {
                SelectionData.SetWallSelection();
                SelectionData.SetTileSelection(clicked);                
            }
        }

        public void Click(DungeonWallController clicked)
        {
            if (Keyboard.current.shiftKey.isPressed)
            {
                SelectionData.AddWallSelection(clicked);
            }
            else
            {
                SelectionData.SetTileSelection();
                SelectionData.SetWallSelection(clicked);
            }
            
        }

        
    }
}