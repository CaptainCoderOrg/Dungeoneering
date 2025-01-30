using UnityEngine;
public static class TransformExtensions
{
    public static void DestroyAllChildren(this Transform transform)
    {
        System.Action<Object> destroy = Object.Destroy;
        if (!Application.isPlaying)
        {
            destroy = Object.DestroyImmediate;
        }
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            child.parent = null;
            destroy.Invoke(child.gameObject);
        }
    }
}