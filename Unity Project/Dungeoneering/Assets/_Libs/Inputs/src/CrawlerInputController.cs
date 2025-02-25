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
        public void HandleMoveForward(CallbackContext ctx)
        {
            if (ctx.performed)
            {
                OnMovement.Invoke(MovementAction.StepForward);
            }
        }
        public void HandleMoveBackward(CallbackContext ctx)
        {
            if (ctx.performed)
            {
                OnMovement.Invoke(MovementAction.StepBackward);
            }
        }
        public void HandleMoveLeft(CallbackContext ctx)
        {
            if (ctx.performed)
            {
                OnMovement.Invoke(MovementAction.StrafeLeft);
            }
        }
        public void HandleMoveRight(CallbackContext ctx)
        {
            if (ctx.performed)
            {
                OnMovement.Invoke(MovementAction.StrafeRight);
            }
        }

        public void HandleRotateCW(CallbackContext ctx)
        {
            if (ctx.performed)
            {
                OnMovement.Invoke(MovementAction.RotateRight);
            }
        }

        public void HandleRotateCounterCW(CallbackContext ctx)
        {
            if (ctx.performed)
            {
                OnMovement.Invoke(MovementAction.RotateLeft);
            }
        }
    }
}