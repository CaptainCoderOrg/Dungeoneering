using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/FigureData")]
    public class FigureData : ObservableSO
    {
        [field: SerializeField] public LivingEntityData EntityData { get; private set; }
        [field: SerializeField] public Vector2Int Position { get; set; }
        public Vector3 LocalPosition => new(Position.x, 0, Position.y);
    }
}