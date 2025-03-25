using System.Linq;

using NaughtyAttributes;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/AnimationData")]
    public class AnimationData : ScriptableObject
    {
        public string Name;
        public SpriteSheetData SpriteSheet;
        public int[] Frames;
        public int FramesPerSecond = 6;
        public bool Loops = true;
        public AnimationData NextAnimation;
        public int StartIx;
        public int EndIx;
        public Vector2 GetTextureOffset(int frameIx) => SpriteSheet.GetTextureOffset(Frames[frameIx]);
        [Button]
        private void GenerateFrames() => Frames = Enumerable.Range(StartIx, EndIx - StartIx + 1).ToArray();
        [Button]
        private void GenerateReverseFrames() => Frames = Enumerable.Range(StartIx, EndIx - StartIx + 1).Reverse().ToArray();
    }
}