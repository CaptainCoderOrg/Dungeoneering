using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.Player;
using UnityEngine;
using UnityEngine.Events;
namespace CaptainCoder.Dungeoneering.Unity
{
    [CreateAssetMenu(menuName = "DC/PlayerView")]   
    public class PlayerViewData : ObservableSO
    {
        public UnityEvent<PlayerView, PlayerView> OnChange { get; private set; } = new();

        [field: SerializeField]
        public int X { get; set; }
        [field: SerializeField]
        public int Y { get; set; }
        [field: SerializeField]
        public Facing Facing { get; set; }
        
        private PlayerView _view;
        public PlayerView View 
        { 
            get => _view;
            private set
            {
                if (_view == value) { return; }
                PlayerView exit = _view;
                _view = value;
                X = _view.Position.X;
                Y = _view.Position.Y;
                Facing = _view.Facing;
                OnChange.Invoke(exit, _view);
            }
        }

        protected override void OnEnterPlayMode()
        {
            base.OnEnterPlayMode();
        }

        protected override void OnExitPlayMode()
        {
            base.OnExitPlayMode();
            OnChange.RemoveAllListeners();
        }

        public override void OnEnable()
        {
            base.OnEnable();
            View = new PlayerView(new (X, Y), Facing);
        }

        public void OnValidate()
        {
            View = new PlayerView(new (X, Y), Facing);
        }
        
    }
}