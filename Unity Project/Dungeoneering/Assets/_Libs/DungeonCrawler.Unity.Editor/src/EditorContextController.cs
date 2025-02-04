using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using UnityEngine;
namespace CaptainCoder.Dungeoneering.Unity
{
    public class EditorContextController : MonoBehaviour
    {
        [field: SerializeField]
        public DungeonEditorSelectionData SelectionData { get; private set; }
        

        public void Click(DungeonTile clicked) => SelectionData.SetSelection(clicked);
    }
}