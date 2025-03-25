using CaptainCoder.Unity.Assertions;

using NaughtyAttributes;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class QuadAnimator : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private int _currentFrame;
        [field: SerializeField] public bool IsPlaying { get; private set; } = true;
        [field: SerializeField] public float PlaybackSpeed { get; private set; } = 1;
        [AssertIsSet][SerializeField] private AnimationData _currentAnimation;
        private float _currentWait;
        void Awake()
        {
            _meshRenderer.material.mainTexture = _currentAnimation.SpriteSheet.SpriteSheet;
            _currentFrame = _currentAnimation.StartIx;
            _currentWait = Mathf.Sign(_currentAnimation.FramesPerSecond);
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

        [Button("Play")]
        private void Play() => Play(_currentAnimation);
        private void Play(AnimationData animationData)
        {
            _currentAnimation = animationData;
            _meshRenderer.material.mainTexture = _currentAnimation.SpriteSheet.SpriteSheet;
            _currentFrame = _currentAnimation.StartIx;
            _currentWait = Mathf.Sign(_currentAnimation.FramesPerSecond);
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
            _currentFrame += count;
            if (_currentFrame > _currentAnimation.EndIx)
            {
                if (_currentAnimation.Loops)
                {
                    _currentFrame = _currentAnimation.StartIx;
                }
                else if (_currentAnimation.NextAnimation != null)
                {
                    Play(_currentAnimation.NextAnimation);
                }
                else
                {
                    _currentFrame = _currentAnimation.EndIx;
                }
            }
            UpdateMaterial();
        }
        private void RevertFrames(int count)
        {
            _currentFrame += count;
            if (_currentFrame < _currentAnimation.StartIx && _currentAnimation.Loops)
            {
                if (_currentAnimation.Loops)
                {
                    _currentFrame = _currentAnimation.EndIx;
                }
                else if (_currentAnimation.NextAnimation != null)
                {
                    Play(_currentAnimation.NextAnimation);
                }
                else
                {
                    _currentFrame = _currentAnimation.StartIx;
                }
            }
            UpdateMaterial();
        }
        private void UpdateMaterial() => _meshRenderer.material.SetTextureOffset("_BaseMap", _currentAnimation.SpriteSheet.GetTextureOffset(_currentFrame));
    }
}