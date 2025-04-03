using System;

using CaptainCoder.Unity.Assertions;

using UnityEngine;
using UnityEngine.Events;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class EncounterFigureController : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private MeshRenderer _baseMeshRenderer;
        [AssertIsSet][SerializeField] private float _flickerSpeed = 0.5f;
        [SerializeField] private bool _isSelected = false;
        [AssertIsSet][SerializeField] private Transform _figureQuad;
        [SerializeField] private FigureData _figureData;
        public FigureData Figure
        {
            get => _figureData;
            set
            {
                _figureData = value;
                Initialize();
            }
        }
        [AssertIsSet][SerializeField] private QuadAnimator _animator;
        private EncounterController _controller;
        [field: SerializeField] public UnityEvent<FigureData> OnSelected { get; private set; }

        public void Select()
        {
            OnSelected.Invoke(_figureData);
            _isSelected = true;
        }

        void Awake()
        {
            _controller = GetComponentInParent<EncounterController>();
            Debug.Assert(_controller != null, $"Could not find {nameof(_controller)}", this);
        }

        private void Initialize()
        {
            _animator.Play(_figureData.EntityData.SpawnAnimation);
            transform.localPosition = _figureData.LocalPosition;
        }

        void OnEnable()
        {
            _controller.EncounterCamera.ObserveCamera(FollowCamera);
        }

        void OnDisable()
        {
            _controller.EncounterCamera.RemoveObserver(FollowCamera);
        }

        private void FollowCamera(Camera camera)
        {
            Vector3 eulers = camera.transform.rotation.eulerAngles;
            eulers.x *= 0.25f;
            eulers.z = 0;
            _figureQuad.rotation = Quaternion.Euler(eulers);
            Vector3 position = _figureQuad.localPosition;
            position.y = 0.8f + (eulers.x * .01f);
            _figureQuad.localPosition = position;
        }

        public void Update()
        {
            if (!_isSelected)
            {
                _baseMeshRenderer.enabled = false;
                return;
            }
            _baseMeshRenderer.enabled = true;
            Color c = _baseMeshRenderer.material.color;
            c.a = Mathf.Abs(Mathf.Sin(Time.time * Mathf.PI * _flickerSpeed)) * 0.25f + 0.20f;
            _baseMeshRenderer.material.color = c;
        }

        internal void Deselect() => _isSelected = false;
    }
}