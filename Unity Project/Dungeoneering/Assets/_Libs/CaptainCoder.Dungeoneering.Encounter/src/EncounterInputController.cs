using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace CaptainCoder.Dungeoneering.Encounter
{
    public class EncounterInputController : MonoBehaviour
    {
        public const string CAMERA_CLOCKWISE = "CameraClockwise";
        public const string CAMERA_COUNTER_CLOCKWISE = "CameraCounterClockwise";
        public const string CAMERA_PAN = "CameraPan";
        public const string CAMERA_PITCH_DOWN = "CameraPitchDown";
        public const string CAMERA_PITCH_UP = "CameraPitchUp";
        public const string CAMERA_ZOOM = "CameraZoom";
        private PlayerInput _playerInput;
        [SerializeField] private bool _logInputs;
        [field: SerializeField] public UnityEvent<EncounterInputEvent> OnInputEvent { get; private set; }

        public void OnEnable()
        {
            _playerInput ??= GetComponent<PlayerInput>();
            if (_logInputs) { OnInputEvent.AddListener(LogEvents); }
            _playerInput.onActionTriggered += HandleInput;
        }

        public void OnDisable()
        {
            OnInputEvent.RemoveListener(LogEvents);
            _playerInput.onActionTriggered -= HandleInput;
        }


        public void HandleInput(InputAction.CallbackContext context)
        {
            EncounterInputEvent @event = context.action.name switch
            {
                CAMERA_CLOCKWISE when context.performed => RotateCameraClockwise.Instance,
                CAMERA_COUNTER_CLOCKWISE when context.performed => RotateCameraCounterClockwise.Instance,
                CAMERA_PAN when context.performed => new StartPan(context.action.ReadValue<Vector2>()),
                CAMERA_PAN when context.canceled => EndPan.Instance,
                CAMERA_PITCH_UP when context.performed => CameraPitchUp.Instance,
                CAMERA_PITCH_DOWN when context.performed => CameraPitchDown.Instance,
                CAMERA_ZOOM when context.performed => new CameraZoom(context.action.ReadValue<Vector2>().y),
                _ => null
            };

            if (_logInputs && @event == null)
            {
                Debug.Log($"Ignored {context.action.name}");
                return;
            }
            OnInputEvent?.Invoke(@event);
        }

        private void LogEvents(EncounterInputEvent @event)
        {
            Debug.Log(@event);
        }
    }
}