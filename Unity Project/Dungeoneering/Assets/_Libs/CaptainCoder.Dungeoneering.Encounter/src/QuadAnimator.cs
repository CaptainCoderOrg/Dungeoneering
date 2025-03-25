using System;

using CaptainCoder.Unity.Assertions;

using NaughtyAttributes;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class QuadAnimator : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private int _columns;
        [SerializeField] private int _rows;
        [SerializeField] private int _currentFrame;
        [field: SerializeField] public bool IsPlaying { get; private set; } = true;
        [SerializeField] private AnimationData[] _animations;
        [SerializeField] private int _animationIx = 0;
        private AnimationData CurrentAnimation => _animations[_animationIx];
        private float _currentWait;
        private float _xScale;
        private float _yScale;

        void Awake()
        {
            _xScale = 1f / _columns;
            _yScale = 1f / _rows;
        }

        void Update()
        {
            if (!IsPlaying) { return; }
            _currentWait -= Time.deltaTime * CurrentAnimation.FramesPerSecond;
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
        private void Play() => Play(_animationIx);
        private void Play(int ix)
        {
            _animationIx = ix;
            _currentFrame = CurrentAnimation.StartIx;
            _currentWait = Mathf.Sign(CurrentAnimation.FramesPerSecond);
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
            if (_currentFrame < CurrentAnimation.StartIx)
            {
                _currentFrame = CurrentAnimation.Loops ? CurrentAnimation.EndIx : CurrentAnimation.StartIx;
                if (CurrentAnimation.NextAnimation >= 0)
                {
                    Play(CurrentAnimation.NextAnimation);
                }
            }
            else if (_currentFrame > CurrentAnimation.EndIx)
            {
                _currentFrame = CurrentAnimation.Loops ? CurrentAnimation.StartIx : CurrentAnimation.EndIx;
                if (CurrentAnimation.NextAnimation >= 0)
                {
                    Play(CurrentAnimation.NextAnimation);
                }
            }
            UpdateMaterial();
        }
        private void UpdateMaterial()
        {
            (int x, int y) = IndexToOffset(_currentFrame);
            Material.SetTextureOffset("_BaseMap", new(_xScale * x, 1 - (_yScale * (y + 1))));
        }
        private Material Material => _meshRenderer.material;
        private (int X, int Y) IndexToOffset(int ix) => (ix % _columns, ix / _columns);
        private int OffsetToIndex((int x, int y) offset) => offset.y * _columns + offset.x;
    }

    [Serializable]
    public struct AnimationData
    {
        public string Name;
        public int StartIx;
        public int EndIx;
        public int FramesPerSecond;
        public bool Loops;
        public int NextAnimation;
    }
}