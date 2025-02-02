using System.Collections;
using CaptainCoder.Dungeoneering.Player;
using UnityEngine;

namespace CaptainCoder.Dungeoneering.Unity
{
    [CreateAssetMenu(menuName = "CameraModes/LerpStep")]
    public class LerpStepMode : CameraMode
    {
        [field: SerializeField]
        public float Duration { get; private set; } = 0.1f;
        public override IEnumerator HandlePlayerViewChanged(Transform transform, PlayerView exit, PlayerView enter)
        {
            Quaternion startQ = exit.Facing.ToQuaternion();
            Quaternion endQ = enter.Facing.ToQuaternion();
            Vector3 start = exit.Position.ToVector3();
            Vector3 end = enter.Position.ToVector3();
            float elapsedTime = 0;
            while (Percent() < 1)
            {
                transform.position = Vector3.Lerp(start, end, Percent());
                transform.rotation = Quaternion.Lerp(startQ, endQ, Percent());
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            transform.position = Vector3.Lerp(start, end, 1);
            transform.rotation = Quaternion.Lerp(startQ, endQ, 1);
            float Percent() => elapsedTime / Duration;
        }

    }
}