using System.Collections;

using UnityEngine;
using UnityEngine.Events;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class EncounterCamera : MonoBehaviour
    {
        [field: SerializeField] public UnityEvent<Camera> OnCameraRotate { get; private set; }
        private EncounterInputController _inputController;
        private Camera _camera;
        [SerializeField] private float _targetRotation;
        [SerializeField] private float[] _pitches = { 30, 37.5f, 45, 52.5f, 60 };
        [SerializeField] private int _targetPitch = 3;
        [field: SerializeField] public float RotationDuration { get; private set; } = 0.4f;
        [field: SerializeField] public float PitchDuration { get; private set; } = 0.10f;
        [field: SerializeField] public float ZoomDuration { get; private set; } = 0.25f;
        [field: SerializeField] public float ZoomScale { get; private set; } = 0.25f;
        [field: SerializeField] public float MinZoom { get; private set; } = 3;
        [field: SerializeField] public float MaxZoom { get; private set; } = 10;
        [field: SerializeField] public float PanSpeed { get; private set; } = 1f;
        [SerializeField] private float _targetZoom = 5f;
        private Coroutine _rotateCoroutine;
        private Coroutine _pitchCoroutine;
        private Coroutine _zoomCoroutine;
        private Coroutine _panCoroutine;

        void Awake()
        {
            _inputController = GetComponentInParent<EncounterInputController>();
            Debug.Assert(_inputController != null, $"{nameof(_inputController)} was not found", this);
            _camera = GetComponentInChildren<Camera>();
            Debug.Assert(_camera != null, $"{nameof(_camera)} was not found", this);
            _targetZoom = _camera.orthographicSize;
        }

        void OnEnable()
        {
            _inputController.OnInputEvent.AddListener(HandleInputs);
        }

        void OnDisable()
        {
            _inputController.OnInputEvent.RemoveListener(HandleInputs);
        }

        private void HandleInputs(EncounterInputEvent input)
        {
            switch (input)
            {
                case RotateCameraClockwise:
                    RotateClockwise(90);
                    break;
                case RotateCameraCounterClockwise:
                    RotateClockwise(-90);
                    break;
                case CameraPitchUp:
                    AdjustPitch(1);
                    break;
                case CameraPitchDown:
                    AdjustPitch(-1);
                    break;
                case CameraZoom(float delta):
                    ZoomCamera(delta * ZoomScale);
                    break;
                case StartPan(Vector2 pan):
                    CancelCoroutine(_panCoroutine);
                    _panCoroutine = StartCoroutine(PanCamera(pan));
                    break;
                case EndPan:
                    CancelCoroutine(_panCoroutine);
                    break;
            }
        }

        public IEnumerator PanCamera(Vector2 pan)
        {
            Vector3 direction = (_camera.transform.rotation * new Vector3(pan.x, 0, pan.y)).normalized;
            while (true)
            {
                Vector3 position = transform.position + (PanSpeed * Time.deltaTime * direction);
                position.y = 0;
                transform.position = position;
                yield return null;
            }
        }

        public void ZoomCamera(float delta)
        {
            _targetZoom = Mathf.Clamp(_targetZoom - delta, MinZoom, MaxZoom);
            CancelCoroutine(_zoomCoroutine);
            _zoomCoroutine = StartCoroutine(ZoomCamera());
        }

        private IEnumerator ZoomCamera()
        {
            float startSize = _camera.orthographicSize;
            float endSize = _targetZoom;
            float elapsedTime = 0;
            float percent = 0;
            while (percent < 1)
            {
                elapsedTime += Time.deltaTime;
                _camera.orthographicSize = Mathf.Lerp(startSize, endSize, percent);
                yield return null;
                percent = elapsedTime / ZoomDuration;
            }
            _camera.orthographicSize = endSize;
        }

        public void RotateClockwise(float delta)
        {
            _targetRotation += delta;
            CancelCoroutine(_rotateCoroutine);
            _rotateCoroutine = StartCoroutine(RotateCamera());
        }

        public void AdjustPitch(int delta)
        {
            _targetPitch = (((_targetPitch + delta) % _pitches.Length) + _pitches.Length) % _pitches.Length;
            CancelCoroutine(_pitchCoroutine);
            _pitchCoroutine = StartCoroutine(PitchCamera());
        }

        private void CancelCoroutine(Coroutine coroutine)
        {
            if (coroutine == null) { return; }
            StopCoroutine(coroutine);
        }

        private IEnumerator PitchCamera()
        {
            Quaternion startQ = _camera.transform.rotation;
            Vector3 endEuler = _camera.transform.rotation.eulerAngles;
            endEuler.x = _pitches[_targetPitch];
            Quaternion endQ = Quaternion.Euler(endEuler);
            float elapsedTime = 0;
            float percent = 0;
            while (percent < 1)
            {
                elapsedTime += Time.deltaTime;
                _camera.transform.rotation = Quaternion.Lerp(startQ, endQ, percent);
                yield return null;
                percent = elapsedTime / PitchDuration;
            }
            _camera.transform.rotation = endQ;
        }

        private IEnumerator RotateCamera()
        {
            Quaternion startQ = transform.rotation;
            Quaternion endQ = Quaternion.Euler(0, _targetRotation, 0);
            float elapsedTime = 0;
            float percent = 0;
            while (percent < 1)
            {
                elapsedTime += Time.deltaTime;
                transform.rotation = Quaternion.Lerp(startQ, endQ, percent);
                OnCameraRotate?.Invoke(_camera);
                yield return null;
                percent = elapsedTime / RotationDuration;
            }
            transform.rotation = endQ;
            OnCameraRotate?.Invoke(_camera);
        }
    }
}
