using CaptainCoder.Dungeoneering.Unity.Data;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.DungeonMap.Unity
{
    public class DungeonWallController : MonoBehaviour, ISelectable
    {
        [field: SerializeField]
        public MeshRenderer[] Renderers { get; private set; } = default!;
        private MouseEvents _mouseEvents;
        public DungeonTile Parent { get; private set; }
        public WallReference WallReference => new(Parent.Dungeon, Parent.Position, Facing);
        [field: SerializeField]
        public Facing Facing { get; private set; }
        private SelectableMaterial _material;
        public TextureReference Texture => Parent.DungeonController.DungeonCrawlerData.GetTexture(WallReference);
        public SelectableMaterial Material
        {
            get => _material;
            set
            {
                Material mat = value.GetMaterial(IsSelected);
                foreach (var renderer in Renderers)
                {
                    renderer.material = mat;
                }
                _material = value;
            }
        }
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value)
                    return;
                _isSelected = value;
                IsSelectedChanged(_isSelected);
            }
        }

        private void IsSelectedChanged(bool _)
        {
            // The property will pull out the correct version for selected/unselected
            Material = _material;
        }

        public WallType WallType => Parent.Dungeon.Walls.GetWall(Parent.Position, Facing);

        public void SetTexture(TextureReference newTexture) => Parent.DungeonController.DungeonCrawlerData.SetTexture(WallReference, newTexture);

        void Awake()
        {
            _mouseEvents = GetComponentInChildren<MouseEvents>();
            Debug.Assert(_mouseEvents != null, this);
            Parent = GetComponentInParent<DungeonTile>();
            Debug.Assert(Parent != null, this);
        }

        void OnEnable() => _mouseEvents.OnClick.AddListener(OnClick);
        void OnDisable() => _mouseEvents.OnClick.RemoveListener(OnClick);
        private void OnClick() => Parent.OnWallClicked.Invoke(this);
    }
}