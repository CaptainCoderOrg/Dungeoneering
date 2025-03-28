#if UNITY_EDITOR


using System.Linq;

using NaughtyAttributes;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class EncounterMovementCreator : MonoBehaviour
    {

        private EncounterController _controller;
        public Vector2Int[] Path;

        [Button]
        public void SimulateMovement()
        {
            _controller.HandleMovementEvent(new MoveFigureEvent(_controller.State.Figures[Path.First()], Path));
        }

        void Awake()
        {
            _controller = GetComponentInParent<EncounterController>();
        }
    }
}
#endif