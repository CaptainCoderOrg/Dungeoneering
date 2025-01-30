using System;
using CaptainCoder.Dungeoneering.Player;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.InputSystem.InputAction;

namespace CaptainCoder.Dungeoneering.DungeonCrawler.Input
{

    public class CrawlerInputController : MonoBehaviour
    {
        [field: SerializeField]
        public UnityEvent<MovementAction> OnMovement { get; private set; }
        public void HandleMovement(CallbackContext ctx)
        {
            if (ctx.performed)
            {
                Debug.Log($"Input: {ctx.ReadValue<Vector2>()}");
                Vector2 raw = ctx.ReadValue<Vector2>();
                (int x, int y) = (System.Math.Sign(raw.x), System.Math.Sign(raw.y));
                Debug.Log($"Signed: {x}, {y}");
                MovementAction? action = (x, y) switch
                {
                    (_, 1) => MovementAction.StepForward,
                    (_, -1) => MovementAction.StepBackward,
                    (1, _) => MovementAction.StrafeRight,
                    (-1, _) => MovementAction.StrafeLeft,
                    _ => null
                };
                if (action.HasValue) { OnMovement.Invoke(action.Value); }
            }
        }
    }


}