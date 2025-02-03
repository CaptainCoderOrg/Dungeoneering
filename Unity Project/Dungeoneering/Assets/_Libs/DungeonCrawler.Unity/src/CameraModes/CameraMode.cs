using System;
using System.Collections;
using CaptainCoder.Dungeoneering.Player;
using UnityEngine;

namespace CaptainCoder.Dungeoneering.Unity
{
    public abstract class CameraMode : ScriptableObject
    {
        public abstract IEnumerator HandlePlayerViewChanged(Transform transform, PlayerView exit, PlayerView enter);

        public virtual (Vector3 Position, Quaternion Rotation) EndPositionAndRotation(PlayerView view) => 
            (view.Position.ToVector3(), view.Facing.ToQuaternion());
    }
}