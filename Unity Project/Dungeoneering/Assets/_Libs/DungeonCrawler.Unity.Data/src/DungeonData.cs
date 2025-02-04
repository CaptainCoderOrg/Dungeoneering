using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.Player;
using UnityEngine;
using UnityEngine.Events;
namespace CaptainCoder.Dungeoneering.DungeonMap.Unity
{
    [CreateAssetMenu(menuName = "DC/DungeonData")]   
    public class DungeonData : ObservableSO
    {
        public UnityEvent<Dungeon> OnChange { get; private set; } = new();
        private Dungeon _dungeon;
        public Dungeon Dungeon 
        { 
            get => _dungeon; 
            set
            {
                _dungeon = value;
                OnChange.Invoke(_dungeon);
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
        }

        public void OnValidate()
        {
            
        }
        
    }
}