using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class EncounterFigureController : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private Transform _figureQuad;
        [AssertIsSet][SerializeField] private FigureData _figureData;
        [AssertIsSet][SerializeField] private QuadAnimator _animator;
        private EncounterController _controller;

        void Awake()
        {
            _controller = GetComponentInParent<EncounterController>();
            Debug.Assert(_controller != null, $"Could not find {nameof(_controller)}", this);
            _animator.Play(_figureData.SpawnAnimation);
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
    }
}