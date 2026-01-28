using System;
using System.Diagnostics;
using UnityEngine;

namespace EasyToolkit.Inspector.Attributes
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class MetroListDrawerSettingsAttribute : ListDrawerSettingsAttribute
    {
        public string IconTextureGetter { get; set; }

        public MetroListDrawerSettingsAttribute()
        {
        }
    }
}
