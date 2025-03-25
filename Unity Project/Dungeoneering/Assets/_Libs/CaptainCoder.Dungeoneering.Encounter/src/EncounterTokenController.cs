using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class EncounterTokenController : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private Transform _figureQuad;
        private EncounterController _controller;

        void Awake()
        {
            _controller = GetComponentInParent<EncounterController>();
            Debug.Assert(_controller != null, $"Could not find {nameof(_controller)}", this);
        }

        void OnEnable()
        {
            _controller.EncounterCamera.OnCameraRotate.AddListener(FollowCamera);
        }

        void OnDisable()
        {
            _controller.EncounterCamera.OnCameraRotate.RemoveListener(FollowCamera);
        }

        private void FollowCamera(Camera camera)
        {
            Vector3 eulers = camera.transform.rotation.eulerAngles;
            eulers.x = 0;
            eulers.z = 0;
            _figureQuad.rotation = Quaternion.Euler(eulers);
        }
    }
}