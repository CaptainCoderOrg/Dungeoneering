using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class DungeonTextureButton : MonoBehaviour
    {
        [field: SerializeField]
        public RawImage Image { get; private set; }
        
    }
}