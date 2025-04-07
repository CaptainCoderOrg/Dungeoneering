using System.Collections.Generic;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class HeroTurnController : MonoBehaviour
    {
        private EncounterController _controller;
        private EncounterController Controller => _controller = (_controller == null ? GetComponentInParent<EncounterController>() : _controller);
        private EncounterState State => Controller.State;
        [SerializeField] private EncounterFigureController _figureController;

        internal void BeginTurn(EncounterFigureController figureController, IEnumerable<TacticData> tactics)
        {
            _figureController = figureController;
            foreach (var tactic in tactics)
            {
                tactic.Effect.OnTurnStart(_figureController.Figure);
            }
        }
    }
}