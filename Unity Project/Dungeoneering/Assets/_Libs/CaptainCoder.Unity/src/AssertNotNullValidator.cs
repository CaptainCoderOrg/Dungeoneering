#if UNITY_EDITOR
using System.Reflection;

using UnityEngine;

namespace CaptainCoder.Unity.Assertions
{
    public static class AssertionValidator
    {

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnSceneLoaded()
        {
            if (AssertionsMenu.IsEnabled) { ValidateAllMonoBehaviours(); }
        }

        private static void ValidateAllMonoBehaviours()
        {
            foreach (GameObject obj in Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                foreach (MonoBehaviour mono in obj.GetComponents<MonoBehaviour>())
                {
                    if (mono != null)
                    {
                        ValidateMonoBehaviour(mono);
                    }
                }
            }
        }

        private static void ValidateMonoBehaviour(MonoBehaviour mono)
        {
            System.Type type = mono.GetType();

            foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (System.Attribute.IsDefined(field, typeof(AssertIsSetAttribute)))
                {
                    object value = field.GetValue(mono);
                    if (value == null)
                    {
                        ReportNullError(mono, type, field.Name);
                    }
                }
            }

            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (System.Attribute.IsDefined(property, typeof(AssertIsSetAttribute)) && property.CanRead)
                {
                    object value = property.GetValue(mono);
                    if (value == null)
                    {
                        ReportNullError(mono, type, property.Name);
                    }
                }
            }
        }

        private static void ReportNullError(MonoBehaviour mono, System.Type type, string memberName)
        {
            Debug.LogError($"[{typeof(AssertIsSetAttribute).Name}] {type.Name} on '{mono.gameObject.name}': '{memberName}' is null!", mono);
        }
    }
}
#endif