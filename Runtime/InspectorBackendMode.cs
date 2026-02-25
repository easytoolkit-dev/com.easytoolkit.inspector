using JetBrains.Annotations;

namespace EasyToolkit.Inspector
{
    /// <summary>
    /// Specifies the backend rendering mode for the inspector.
    /// Determines whether the inspector uses IMGUI or UI Toolkit for rendering.
    /// </summary>
    [PublicAPI]
    public enum InspectorBackendMode
    {
        /// <summary>
        /// Uses the Immediate Mode GUI (IMGUI) system for rendering.
        /// This is the traditional Unity inspector rendering approach.
        /// </summary>
        IMGUI,

        /// <summary>
        /// Uses the UI Toolkit system for rendering.
        /// This is the modern retained-mode UI system based on web technologies.
        /// </summary>
        UIToolkit
    }
}
