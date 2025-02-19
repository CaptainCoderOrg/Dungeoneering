using System.Linq;

using UnityEngine;
public static class TransformExtensions
{
    public static void DestroyAllChildren(this Transform transform, params Transform[] skip)
    {
        System.Action<Object> destroy = Object.Destroy;
        if (!Application.isPlaying)
        {
            destroy = Object.DestroyImmediate;
        }
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (skip.Contains(child)) { continue; }
            child.SetParent(null, true);
            destroy.Invoke(child.gameObject);
        }
    }
}