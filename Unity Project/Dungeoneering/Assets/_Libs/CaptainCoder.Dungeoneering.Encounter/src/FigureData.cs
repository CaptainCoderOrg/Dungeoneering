using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/FigureData")]
    public class FigureData : ObservableSO
    {
        [field: SerializeField] public LivingEntityData EntityData { get; private set; }
        [field: SerializeField] public Vector2Int Position { get; set; }
        public Vector3 LocalPosition => new(Position.y, 0, Position.x);
        [field: SerializeField] private bool _hasTakenTurn = false;
        public bool HasTakenTurn
        {
            get => _hasTakenTurn;
            set
            {
                _hasTakenTurn = value;
                OnChanged?.Invoke(ChangedEvent);
            }
        }
        [field: SerializeField] private int _movment = 0;
        public int Movement
        {
            get => _movment;
            set
            {
                _movment = value;
                OnChanged?.Invoke(ChangedEvent);
            }
        }
        [field: SerializeField] private int _attacks = 0;
        public int Attacks
        {
            get => _attacks;
            set
            {
                _attacks = value;
                OnChanged?.Invoke(ChangedEvent);
            }
        }
        private FigureDataChangedEvent _changedEvent;
        private FigureDataChangedEvent ChangedEvent => _changedEvent ??= new(this);

        public event System.Action<FigureDataChangedEvent> OnChanged;

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

        public override void OnBeforeEnterPlayMode()
        {
            base.OnAfterEnterPlayMode();
            OnChanged = null;
        }

        protected override void OnExitPlayMode()
        {
            base.OnExitPlayMode();
            OnChanged = null;
        }
    }

    public sealed record class FigureDataChangedEvent(FigureData Figure);
}