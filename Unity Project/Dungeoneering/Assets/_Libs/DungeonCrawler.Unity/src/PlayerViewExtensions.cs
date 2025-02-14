using System;

using CaptainCoder.Dungeoneering.DungeonMap;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Unity
{
    public static class PlayerViewExtensions
    {
        public static Vector3 ToVector3(this Position position) => new(position.Y, 0, position.X);
        public static Quaternion ToQuaternion(this Facing facing) => facing switch
        {
            Facing.North => Quaternion.Euler(0, 270, 0),
            Facing.East => Quaternion.Euler(0, 0, 0),
            Facing.South => Quaternion.Euler(0, 90, 0),
            Facing.West => Quaternion.Euler(0, 180, 0),
            _ => throw new ArgumentException($"Unknown facing {facing}"),
        };
    }
}