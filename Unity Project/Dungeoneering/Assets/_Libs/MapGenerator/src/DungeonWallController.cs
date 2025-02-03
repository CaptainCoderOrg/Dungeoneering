using UnityEngine;

namespace CaptainCoder.Dungeoneering.DungeonMap.Unity
{
    public class DungeonWallController : MonoBehaviour
    {
        [field: SerializeField]
        public MeshRenderer[] Renderers { get; private set; } = default!;
        
        private Material _material;
        public Material Material 
        { 
            get => _material; 
            set
            {
                foreach (var renderer in Renderers)
                {
                    renderer.material = value;
                }
                _material = value;
            }
        }
    }
}