using System.Collections;

using CaptainCoder.Dungeoneering.Player;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Unity
{
    [CreateAssetMenu(menuName = "CameraModes/InstantStep")]
    public class InstantStepMode : CameraMode
    {
        public override IEnumerator HandlePlayerViewChanged(Transform transform, PlayerView exit, PlayerView enter)
        {
            transform.position = enter.Position.ToVector3();
            transform.rotation = enter.Facing.ToQuaternion();
            yield return null;
        }
    }
}