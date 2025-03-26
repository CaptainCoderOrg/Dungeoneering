using CaptainCoder.Unity.Assertions;

using NaughtyAttributes;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class QuadAnimator : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private int _frameIx;
        [field: SerializeField] public bool IsPlaying { get; private set; } = true;
        [field: SerializeField] public float PlaybackSpeed { get; private set; } = 1;
        [AssertIsSet][SerializeField] private AnimationData _currentAnimation;
        private float _currentWait;
        void Awake()
        {
            InitializeAnimation(_currentAnimation);
            UpdateMaterial();
        }
        void Update()
        {
            if (!IsPlaying) { return; }
            _currentWait -= Time.deltaTime * _currentAnimation.FramesPerSecond * PlaybackSpeed;
            int frameChange = CalcFrameChange(_currentWait);
            if (frameChange != 0)
            {
                _currentWait += frameChange;
                UpdateFrameBy(frameChange);
            }
        }

        private int CalcFrameChange(float wait) => wait switch
        {
            _ when wait < 0 => Mathf.CeilToInt(Mathf.Abs(wait)),
            _ when wait > 0 => -Mathf.CeilToInt(_currentWait - 1),
            _ => 0
        };

        private void InitializeAnimation(AnimationData animationData)
        {
            _currentAnimation = animationData;
            _meshRenderer.material.mainTexture = _currentAnimation.SpriteSheet.SpriteSheet;
            _meshRenderer.material.mainTextureScale = new Vector2(_currentAnimation.SpriteSheet.XScale, _currentAnimation.SpriteSheet.YScale);
            _frameIx = 0;
            _currentWait = _currentAnimation.FramesPerSecond >= 0 ? 1 : 0;
        }
        [Button("Play")]
        private void Play() => Play(_currentAnimation);
        private void Play(AnimationData animationData)
        {
            InitializeAnimation(animationData);
            IsPlaying = true;
            UpdateMaterial();
        }

        [Button("Pause")]
        private void Pause() => IsPlaying = false;

        [Button("Advance Frame")]
        private void AdvanceFrame() => AdvanceFrames(1);
        private void UpdateFrameBy(int count)
        {
            if (count >= 0) { AdvanceFrames(count); }
            else { RevertFrames(count); }
        }

        private void AdvanceFrames(int count)
        {
            _frameIx += count;
            if (_frameIx >= _currentAnimation.Frames.Length)
            {
                if (_currentAnimation.Loops)
                {
                    _frameIx = 0;
                }
                else if (_currentAnimation.NextAnimation != null)
                {
                    InitializeAnimation(_currentAnimation.NextAnimation);
                }
                else
                {
                    _frameIx = _currentAnimation.Frames.Length - 1;
                }
            }
            UpdateMaterial();
        }
        private void RevertFrames(int count)
        {
            _frameIx += count;
            if (_frameIx < 0)
            {
                if (_currentAnimation.Loops)
                {
                    _frameIx = _currentAnimation.Frames.Length - 1;
                }
                else if (_currentAnimation.NextAnimation != null)
                {
                    InitializeAnimation(_currentAnimation.NextAnimation);
                }
                else
                {
                    _frameIx = 0;
                }
            }
            UpdateMaterial();
        }
        private void UpdateMaterial() => _meshRenderer.material.SetTextureOffset("_BaseMap", _currentAnimation.GetTextureOffset(_frameIx));
    }
}