using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/FigureData")]
    public class FigureData : ObservableSO
    {
        [field: SerializeField] public LivingEntityData EntityData { get; private set; }
        [field: SerializeField] public Vector2Int Position { get; set; }
        public Vector3 LocalPosition => new(Position.x, 0, Position.y);
        [field: SerializeField] public bool HasTakenTurn { get; set; } = false;
        [field: SerializeField] public int Movement { get; set; } = 0;
        [field: SerializeField] public int Attacks { get; set; } = 0;

        public static FigureData Create(LivingEntityData entity, Vector2Int position)
        {
            FigureData data = CreateInstance<FigureData>();
            data.EntityData = entity;
            data.Position = position;
            return data;
        }

        public static FigureData CopyEnemyAndCreate(EnemyEntityData entity, Vector2Int position)
        {
            FigureData data = CreateInstance<FigureData>();
            data.EntityData = entity.Copy();
            data.Position = position;
            return data;
        }
    }
}