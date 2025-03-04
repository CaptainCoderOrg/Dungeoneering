using UnityEngine;

namespace CaptainCoder.Unity
{
    public static class Assertion
    {
        public static void NotNull(Object context, params Object[] objects)
        {
            foreach (Object obj in objects)
            {
                Debug.Assert(obj != null, context);
            }
        }

        public static void NotNull(Object context, params (Object, string message)[] objects)
        {
            foreach ((Object obj, string message) in objects)
            {
                Debug.Assert(obj != null, $"{message}", context);
            }
        }
    }
}