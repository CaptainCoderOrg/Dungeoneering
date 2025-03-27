using NaughtyAttributes;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    public class DiceBoxController : MonoBehaviour
    {
        [SerializeField] private DieController[] _dice;

        void Awake()
        {
            _dice = GetComponentsInChildren<DieController>(true);
        }

        [Button]
        private void Roll()
        {
            foreach (var die in _dice) { die.Roll(); }
        }
    }
}