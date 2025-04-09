namespace CaptainCoder.Dungeoneering.Encounter;

using UnityEngine;

public static class RectTransformExtensions
{
    private static readonly Vector3[] CornerVectors = { default, default, default, default };
    const float Padding = 10;
    public static void EnsureOnScreen(this RectTransform rect)
    {
        Corners corners = rect.FindCorners();
        if (corners.MinX < Padding)
        {
            rect.position += new Vector3(-corners.MinX + Padding, 0, 0);
        }
        if (corners.MinY < Padding)
        {
            rect.position += new Vector3(0, -corners.MinY + Padding, 0);
        }
        if (corners.MaxX > Screen.width - Padding)
        {

            rect.position -= new Vector3(corners.MaxX - Screen.width + Padding, 0, 0);
        }
        if (corners.MaxY > Screen.height - Padding)
        {
            rect.position -= new Vector3(0, corners.MaxY - Screen.height + Padding, 0);
        }
    }

    private static Corners FindCorners(this RectTransform rect)
    {
        rect.GetWorldCorners(CornerVectors);
        return new Corners
        {
            MinX = Mathf.Min(CornerVectors[0].x, CornerVectors[1].x, CornerVectors[2].x, CornerVectors[3].x),
            MaxX = Mathf.Max(CornerVectors[0].x, CornerVectors[1].x, CornerVectors[2].x, CornerVectors[3].x),
            MinY = Mathf.Min(CornerVectors[0].y, CornerVectors[1].y, CornerVectors[2].y, CornerVectors[3].y),
            MaxY = Mathf.Max(CornerVectors[0].y, CornerVectors[1].y, CornerVectors[2].y, CornerVectors[3].y),
        };
    }

}

internal record struct Corners(float MinX, float MinY, float MaxX, float MaxY);