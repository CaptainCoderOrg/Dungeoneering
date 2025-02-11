using UnityEngine;

namespace CaptainCoder.Unity
{
    public static class Assertion
    {
        public static void NotNull(object context, params object[] objects)
        {
            foreach (object obj in objects)
            {
                Debug.Assert(obj != null, context);
            }
        }
    }
}