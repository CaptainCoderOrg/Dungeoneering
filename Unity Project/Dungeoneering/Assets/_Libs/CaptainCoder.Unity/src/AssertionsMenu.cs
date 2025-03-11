#if UNITY_EDITOR
using UnityEditor;

using UnityEngine;
namespace CaptainCoder.Unity.Assertions
{
    public static class AssertionsMenu
    {
        public const string EnableAssertionsKey = "EnableAssertNotNull";
        private const string MenuPath = $"Assertions/Enable AssertIsSet Checks";

        [MenuItem(MenuPath)]
        private static void ToggleAssertions()
        {
            bool isEnabled = !IsEnabled;
            PlayerPrefs.SetInt(EnableAssertionsKey, isEnabled ? 1 : 0);
            PlayerPrefs.Save();
            Debug.Log($"[{typeof(AssertIsSetAttribute).Name}] Assertions are now {(isEnabled ? "ENABLED" : "DISABLED")}");
            Menu.SetChecked(MenuPath, isEnabled);
        }

        [MenuItem(MenuPath, true)]
        private static bool ValidateToggleAssertions()
        {
            Menu.SetChecked(MenuPath, IsEnabled);
            return true;
        }

        public static bool IsEnabled => PlayerPrefs.GetInt(EnableAssertionsKey, 1) == 1;
    }
}
#endif