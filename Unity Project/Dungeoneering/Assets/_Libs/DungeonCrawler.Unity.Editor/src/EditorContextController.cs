using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using UnityEngine;
namespace CaptainCoder.Dungeoneering.Unity
{
    public class EditorContextController : MonoBehaviour
    {
        [field: SerializeField]
        public DungeonEditorSelectionData SelectionData { get; private set; }
        

        public void Click(DungeonTile clicked)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
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
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
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