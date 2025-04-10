using System.Collections;

using CaptainCoder.Unity.Assertions;

using NaughtyAttributes;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    public class DieController : MonoBehaviour
    {
        private static Quaternion[] s_faceRotation = {
            Quaternion.Euler(0, 90, 90), // 0
            Quaternion.Euler(180, 180, 0), // 1
            Quaternion.Euler(90, 180, -180), // 2
            Quaternion.Euler(0, -90, -90), // 3
            Quaternion.Euler(0, 0, 0), // 4
            Quaternion.Euler(-90, 180, 0), // 5
            
        };
        [SerializeField] private Transform[] _faces;
        [AssertIsSet][SerializeField] private DieData _dieData;
        public DieData Die
        {
            get => _dieData;
            set
            {
                _dieData = value;
                InitializeDie();
            }
        }
        [AssertIsSet][SerializeField] private MeshRenderer _dieRenderer;
        [AssertIsSet][SerializeField] private MeshRenderer _dieAlbedoRenderer;
        [AssertIsSet][SerializeField] private Transform _pivot;
        [AssertIsSet][SerializeField] private Rigidbody _rigidbody;
        private Vector3 _startPosition;
        private Quaternion _targetQuaternion;
        private Coroutine _rollRoutine;
        [SerializeField] private float _rollDelay = 1f;
        [SerializeField] private float _rollTime = 4f;
        public event System.Action<DieResult> OnResult;

        void Awake() => InitializeDie();

        [Button]
        public void InitializeDie()
        {
            _startPosition = _pivot.transform.position;
            Texture2D texture = _dieData.Texture;
            _dieRenderer.material.SetTexture("_BaseMap", texture);
            _dieAlbedoRenderer.material.color = _dieData.Albedo;
        }

        [field: SerializeField] public int Face { get; private set; }
        [Button]
        private void SetFace()
        {
            _targetQuaternion = s_faceRotation[Face];
            RollToTarget();
        }

        private void RollToTarget()
        {
            if (_rollRoutine != null) { StopCoroutine(_rollRoutine); }
            _rollRoutine = StartCoroutine(RollTo(_targetQuaternion));
        }

        private IEnumerator RollTo(Quaternion endQ)
        {
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            Vector3 startP = _pivot.transform.position;
            Quaternion startQ = _pivot.transform.rotation;
            float elapsedTime = 0;
            float percent = 0;
            while (percent < 1)
            {
                elapsedTime += Time.deltaTime;
                _pivot.transform.rotation = Quaternion.Lerp(startQ, endQ, percent);
                _pivot.transform.position = Vector3.Lerp(startP, _startPosition, percent);
                yield return null;
                percent = elapsedTime / _rollDelay;
            }
            _pivot.transform.rotation = endQ;
            _pivot.transform.position = _startPosition;
            OnResult?.Invoke(new DieResult(Die, Face));
        }
        [SerializeField] private float _rollVelocity = 20f;
        [SerializeField] private float _upForce = 20f;

        [Button]
        public void Roll()
        {
            StopAllCoroutines();
            StartCoroutine(DoRoll());
        }
        private static readonly WaitForFixedUpdate WaitForFixedUpdate = new();

        private IEnumerator DoRoll()
        {
            _rigidbody.constraints = RigidbodyConstraints.None;
            yield return WaitForFixedUpdate;
            _rigidbody.AddForce(Vector3.up * _upForce, ForceMode.Impulse);
            yield return WaitForFixedUpdate;
            _rigidbody.angularVelocity = new(RandomValue(), RandomValue(), RandomValue());
            yield return new WaitForSeconds(_rollTime);
            RollToNearest();
        }

        private float RandomValue()
        {
            float value = Random.Range(-_rollVelocity, _rollVelocity);
            if (Mathf.Abs(value) < 5)
            {
                return Mathf.Sign(value) * 5;
            }
            return value;
        }

        private void RollToNearest()
        {
            Vector3 above = _pivot.transform.position + Vector3.up;
            float distance = float.MaxValue;
            int target = 0;
            for (int ix = 0; ix < _faces.Length; ix++)
            {
                var face = _faces[ix];
                float value = Vector3.Distance(face.position, above);
                if (value < distance)
                {
                    target = ix;
                    distance = value;
                }
            }
            Face = target;
            _rollRoutine = StartCoroutine(RollTo(s_faceRotation[Face]));
        }

    }
}