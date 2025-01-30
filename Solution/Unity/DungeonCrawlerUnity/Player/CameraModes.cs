using System.Collections;

using CaptainCoder.Dungeoneering.DungeonMap;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Player.Unity;

public static class CameraModes
{
    public static readonly YieldInstruction WaitForEndOfFrame = new WaitForEndOfFrame();
    public static void InstantTransitionToPlayerView(this Transform transform, PlayerView view)
    {
        transform.position = view.Position.ToVector();
        transform.rotation = view.Facing.ToQuaternion();
    }

    public static IEnumerator LerpTransitionToPlayerView(this Transform transform, PlayerView exit, PlayerView enter, float duration = 0.1f)
    {
        Quaternion startQ = exit.Facing.ToQuaternion();
        Quaternion endQ = enter.Facing.ToQuaternion();
        Vector3 start = exit.Position.ToVector();
        Vector3 end = enter.Position.ToVector();
        float elapsedTime = 0;
        while (Percent() < 1)
        {
            transform.position = Vector3.Lerp(start, end, Percent());
            transform.rotation = Quaternion.Lerp(startQ, endQ, Percent());
            elapsedTime += Time.deltaTime;
            yield return WaitForEndOfFrame;
        }
        transform.position = Vector3.Lerp(start, end, 1);
        transform.rotation = Quaternion.Lerp(startQ, endQ, 1);
        float Percent() => elapsedTime / duration;
    }

    public static Quaternion ToQuaternion(this Facing facing) => facing switch
    {
        Facing.North => Quaternion.Euler(0, -90, 0),
        Facing.East => Quaternion.Euler(0, 0, 0),
        Facing.South => Quaternion.Euler(0, 90, 0),
        Facing.West => Quaternion.Euler(0, 180, 0),
        _ => throw new NotImplementedException($"Unexpected Facing: {facing}"),
    };
}

public static class PositionExtensions
{
    public static Vector3 ToVector(this Position position) => new(position.Y, 0, position.X);
}