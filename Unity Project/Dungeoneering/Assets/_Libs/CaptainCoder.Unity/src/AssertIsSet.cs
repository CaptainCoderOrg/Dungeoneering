using System;

using UnityEngine;

namespace CaptainCoder.Unity.Assertions
{

    /// <summary>
    /// Attribute to ensure that a field or property was set in the inspector
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class AssertIsSetAttribute : PropertyAttribute { }
}