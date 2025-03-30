namespace CaptainCoder.Dungeoneering.Encounter;

using UnityEngine;

public static class RectTransformExtensions
{
    private static readonly Vector3[] Corners = { default, default, default, default };
    public static void EnsureOnScreen(this RectTransform rect)
    {
        rect.GetWorldCorners(Corners);
        if (Corners[0].x < 10)
        {
            rect.position += new Vector3(-Corners[0].x + 10, 0, 0);
        }
    }
}