using System.Collections;

using CaptainCoder.Dungeoneering.Player;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Unity
{
    [CreateAssetMenu(menuName = "CameraModes/BirdsEyeMode")]
    public class BirdsEyeMode : CameraMode
    {
        private static readonly Quaternion NorthRotation = Quaternion.Euler(90, -90, 0);
        [field: SerializeField]
        public float Duration { get; private set; } = 0.1f;
        public Vector3 Offset { get; private set; } = new Vector3(0, 10, 0.5f);
        public override IEnumerator HandlePlayerViewChanged(Transform transform, PlayerView exit, PlayerView enter)
        {
            Quaternion startQ = transform.rotation;
            Quaternion endQ = NorthRotation;
            Vector3 start = transform.position;
            Vector3 end = enter.Position.ToVector3() + Offset;
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

        public override (Vector3 Position, Quaternion Rotation) EndPositionAndRotation(PlayerView view)
        {
            return (view.Position.ToVector3() + Offset, NorthRotation);
        }

    }
}