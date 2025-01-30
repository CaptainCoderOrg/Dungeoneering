using UnityEngine;

namespace CaptainCoder.Unity;

public static class TransformExtensions
{
    public static void DestroyAllChildren(this Transform parent, Action<GameObject> destroy)
    {
        while (parent.childCount > 0)
        {
            Transform child = parent.GetChild(0);
            child.SetParent(null);
            destroy.Invoke(child.gameObject);
        }
    }
}