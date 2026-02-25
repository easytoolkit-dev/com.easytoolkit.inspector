using UnityEngine.UIElements;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Defines a contract for visual element processors that can modify and enhance visual elements.
    /// </summary>
    public interface IVisualProcessor : IHandler
    {
        /// <summary>
        /// Gets or sets the processor chain that this processor belongs to.
        /// </summary>
        VisualProcessorChain Chain { get; set; }

        /// <summary>
        /// Processes the specified visual element to modify its appearance or behavior.
        /// </summary>
        /// <param name="visualElement">The visual element to process.</param>
        void Process(VisualElement visualElement);
    }
}
