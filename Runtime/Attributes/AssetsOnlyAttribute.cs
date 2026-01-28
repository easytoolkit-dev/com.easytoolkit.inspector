using System;
using System.Diagnostics;
using UnityEngine;

namespace EasyToolkit.Inspector.Attributes
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AssetsOnlyAttribute : InspectorAttribute
    {

    }
}
