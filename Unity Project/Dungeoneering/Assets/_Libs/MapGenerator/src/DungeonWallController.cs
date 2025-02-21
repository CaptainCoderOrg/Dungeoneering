using UnityEngine;

namespace CaptainCoder.Dungeoneering.DungeonMap.Unity
{
    public class DungeonWallController : MonoBehaviour
    {

        [field: SerializeField]
        public MeshRenderer[] Renderers { get; private set; } = default!;
        private MouseEvents _mouseEvents;
        public DungeonTile Parent { get; private set; }
        [field: SerializeField]
        public Facing Facing { get; private set; }
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

        public WallType WallType => Parent.Dungeon.Walls.GetWall(Parent.Position, Facing);

        public void SetTexture(string textureName)
        {
            // Manifest.SetFloorTexture(Dungeon, Position, textureName);
            Parent.Manifest.SetWallTexture(Parent.Dungeon, Parent.Position, Facing, textureName);
        }

        void Awake()
        {
            _mouseEvents = GetComponentInChildren<MouseEvents>();
            Debug.Assert(_mouseEvents != null, this);
            Parent = GetComponentInParent<DungeonTile>();
            Debug.Assert(Parent != null, this);
        }

        void OnEnable()
        {
            _mouseEvents.OnClick.AddListener(OnClick);
        }

        void OnDisable()
        {
            _mouseEvents.OnClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            Parent.OnWallClicked.Invoke(this);
        }
    }
}