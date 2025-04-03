using System;
using System.Collections;

using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class EncounterCamera : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private EncounterSettingsData _encounterSettingsData;
        private System.Action<Camera> _onCameraRotate;
        private EncounterInputController _inputController;
        [AssertIsSet][SerializeField] private Camera _camera;
        [SerializeField] private float[] _pitches = { 30, 37.5f, 45, 52.5f, 60 };
        [field: SerializeField] public float RotationDuration { get; private set; } = 0.4f;
        [field: SerializeField] public float PitchDuration { get; private set; } = 0.10f;
        [field: SerializeField] public float ZoomDuration { get; private set; } = 0.25f;
        [field: SerializeField] public float ZoomScale { get; private set; } = 0.25f;
        [field: SerializeField] public float MinZoom { get; private set; } = 3;
        [field: SerializeField] public float MaxZoom { get; private set; } = 10;
        [field: SerializeField] public float PanSpeed { get; private set; } = 1f;
        [field: SerializeField] public float PanTime { get; private set; } = .5f;
        private Coroutine _rotateCoroutine;
        private Coroutine _pitchCoroutine;
        private Coroutine _zoomCoroutine;
        private Coroutine _panCoroutine;

        public void ObserveCamera(System.Action<Camera> observer)
        {
            _onCameraRotate += observer;
            observer.Invoke(_camera);
        }

        public void RemoveObserver(System.Action<Camera> observer)
        {
            _onCameraRotate -= observer;
        }

        void Awake()
        {
            _inputController = GetComponentInParent<EncounterInputController>();
            Debug.Assert(_inputController != null, $"{nameof(_inputController)} was not found", this);
            _camera.orthographicSize = _encounterSettingsData.TargetZoom;
            transform.rotation = Quaternion.Euler(0, _encounterSettingsData.TargetRotation, 0);
            Vector3 endEuler = _camera.transform.rotation.eulerAngles;
            endEuler.x = _pitches[_encounterSettingsData.TargetPitch];
            Quaternion endQ = Quaternion.Euler(endEuler);
            _camera.transform.rotation = endQ;
            _onCameraRotate?.Invoke(_camera);
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
            _encounterSettingsData.TargetZoom = Mathf.Clamp(_encounterSettingsData.TargetZoom - delta, MinZoom, MaxZoom);
            CancelCoroutine(_zoomCoroutine);
            _zoomCoroutine = StartCoroutine(ZoomCamera());
        }

        private IEnumerator ZoomCamera()
        {
            float startSize = _camera.orthographicSize;
            float endSize = _encounterSettingsData.TargetZoom;
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
            _encounterSettingsData.TargetRotation += delta;
            CancelCoroutine(_rotateCoroutine);
            _rotateCoroutine = StartCoroutine(RotateCamera());
        }

        public void AdjustPitch(int delta)
        {
            _encounterSettingsData.TargetPitch = (((_encounterSettingsData.TargetPitch + delta) % _pitches.Length) + _pitches.Length) % _pitches.Length;
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
            float startPitch = _camera.transform.rotation.eulerAngles.x;
            float endPitch = _pitches[_encounterSettingsData.TargetPitch];
            float elapsedTime = 0;
            float percent = 0;
            while (percent < 1)
            {
                elapsedTime += Time.deltaTime;
                float pitch = Mathf.Lerp(startPitch, endPitch, percent);
                Vector3 euler = _camera.transform.rotation.eulerAngles;
                euler.x = pitch;
                _camera.transform.rotation = Quaternion.Euler(euler);
                _onCameraRotate?.Invoke(_camera);
                yield return null;
                percent = elapsedTime / PitchDuration;
            }
            Vector3 final = _camera.transform.rotation.eulerAngles;
            final.x = endPitch;
            _camera.transform.rotation = Quaternion.Euler(final);
            _onCameraRotate?.Invoke(_camera);
        }

        private IEnumerator RotateCamera()
        {
            Quaternion startQ = transform.rotation;
            Quaternion endQ = Quaternion.Euler(0, _encounterSettingsData.TargetRotation, 0);
            float elapsedTime = 0;
            float percent = 0;
            while (percent < 1)
            {
                elapsedTime += Time.deltaTime;
                transform.rotation = Quaternion.Lerp(startQ, endQ, percent);
                _onCameraRotate?.Invoke(_camera);
                yield return null;
                percent = elapsedTime / RotationDuration;
            }
            transform.rotation = endQ;
            _onCameraRotate?.Invoke(_camera);
        }

        internal void PanTo(EncounterFigureController selected)
        {
            CancelCoroutine(_panCoroutine);
            _panCoroutine = StartCoroutine(PanTo(transform.position, selected.transform.position, PanTime));
        }

        private IEnumerator PanTo(Vector3 start, Vector3 target, float duration)
        {
            target.y = start.y;
            float elapsedTime = 0;
            float percent = 0;
            while (percent < 1)
            {
                elapsedTime += Time.deltaTime;
                transform.position = Vector3.Lerp(start, target, percent);
                yield return null;
                percent = elapsedTime / duration;
            }
            transform.position = target;
        }
    }
}
