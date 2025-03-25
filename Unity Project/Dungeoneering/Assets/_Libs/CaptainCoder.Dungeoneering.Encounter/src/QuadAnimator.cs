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
        [AssertIsSet][SerializeField] private AnimationData _currentAnimation;
        private float _currentWait;
        void Awake()
        {
            _meshRenderer.material.mainTexture = _currentAnimation.SpriteSheet.SpriteSheet;
        }
        void Update()
        {
            if (!IsPlaying) { return; }
            _currentWait -= Time.deltaTime * _currentAnimation.FramesPerSecond;
            int frameChange = CalcFrameChange(_currentWait);
            if (frameChange != 0)
            {
                _currentWait += frameChange;
                AdvanceFrames(frameChange);
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
        private void AdvanceFrames(int count)
        {
            _currentFrame += count;
            if (_currentFrame < _currentAnimation.StartIx)
            {
                _currentFrame = _currentAnimation.Loops ? _currentAnimation.EndIx : _currentAnimation.StartIx;
                if (_currentAnimation.NextAnimation != null)
                {
                    Play(_currentAnimation.NextAnimation);
                }
            }
            else if (_currentFrame > _currentAnimation.EndIx)
            {
                _currentFrame = _currentAnimation.Loops ? _currentAnimation.StartIx : _currentAnimation.EndIx;
                if (_currentAnimation.NextAnimation != null)
                {
                    Play(_currentAnimation.NextAnimation);
                }
            }
            UpdateMaterial();
        }
        private void UpdateMaterial() => Material.SetTextureOffset("_BaseMap", _currentAnimation.SpriteSheet.GetTextureOffset(_currentFrame));
        private Material Material => _meshRenderer.material;
    }
}