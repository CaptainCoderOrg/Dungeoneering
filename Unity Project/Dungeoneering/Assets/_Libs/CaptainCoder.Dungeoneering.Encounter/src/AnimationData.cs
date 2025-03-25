using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/AnimationData")]
    public class AnimationData : ScriptableObject
    {
        public string Name;
        public SpriteSheetData SpriteSheet;
        public int StartIx;
        public int EndIx;
        public int FramesPerSecond = 6;
        public bool Loops = true;
        public AnimationData NextAnimation;
        public int FirstFrame => FramesPerSecond >= 0 ? StartIx : EndIx;
        public int FinalFrame => FramesPerSecond >= 0 ? EndIx : StartIx;
    }
}