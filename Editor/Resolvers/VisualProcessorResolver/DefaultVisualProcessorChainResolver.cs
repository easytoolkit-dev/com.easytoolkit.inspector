using System.Collections.Generic;
using System.Linq;
using EasyToolkit.Core;
using EasyToolkit.Core.Reflection;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Default implementation of visual processor chain resolver for <see cref="IElement"/>
    /// </summary>
    public class DefaultVisualProcessorChainResolver : VisualProcessorChainResolverBase
    {
        private VisualProcessorChain _chain;

        /// <summary>
        /// Initializes the resolver by discovering and creating visual processor instances
        /// </summary>
        protected override void Initialize()
        {
            // Get visual processor types for the element
            var processorTypes = VisualProcessorUtility.GetVisualProcessorTypes(Element);
            var processors = new List<IVisualProcessor>();

            // Create and initialize visual processor instances
            foreach (var processorType in processorTypes)
            {
                var processor = processorType.CreateInstance<IVisualProcessor>();
                processors.Add(processor);
            }

            // Create and cache the visual processor chain
            _chain = new VisualProcessorChain(Element, processors);
        }

        /// <summary>
        /// Gets the visual processor chain for the element
        /// </summary>
        /// <returns>The visual processor chain</returns>
        protected override VisualProcessorChain GetVisualProcessorChain()
        {
            return _chain;
        }

        /// <summary>
        /// Clears the cached visual processor chain when the resolver is returned to the pool.
        /// </summary>
        protected override void OnRelease()
        {
            base.OnRelease();
            _chain = null;
        }
    }
}
