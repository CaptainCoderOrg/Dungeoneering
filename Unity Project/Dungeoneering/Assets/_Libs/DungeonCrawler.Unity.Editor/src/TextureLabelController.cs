using TMPro;
using UnityEngine;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class TextureLabelController : MonoBehaviour
    {
        [field: SerializeField]
        public TextMeshProUGUI Label { get; private set; }
        [field: SerializeField]
        public DungeonTextureButton Button { get; private set; }
    }
}