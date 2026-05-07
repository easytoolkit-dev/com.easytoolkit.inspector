using System;
using EasyToolkit.Core;
using JetBrains.Annotations;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Abstract base class for visual processor chain resolution in the <see cref="IElement"/> system
    /// </summary>
    public abstract class VisualProcessorChainResolverBase : ResolverBase, IVisualProcessorChainResolver
    {
        private bool _isInitialized;

        /// <summary>
        /// Override this method to perform initialization logic
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// Gets the visual processor chain for the element
        /// </summary>
        /// <returns>The visual processor chain</returns>
        [MustUseReturnValue]
        protected abstract VisualProcessorChain GetVisualProcessorChain();

        private void EnsureInitialize()
        {
            if (!_isInitialized)
            {
                Initialize();
                _isInitialized = true;
            }
        }

        VisualProcessorChain IVisualProcessorChainResolver.GetVisualProcessorChain()
        {
            EnsureInitialize();
            return GetVisualProcessorChain();
        }

        /// <summary>
        /// Resets the initialization flag when the resolver is returned to the pool.
        /// </summary>
        protected override void OnRelease()
        {
            base.OnRelease();
            _isInitialized = false;
        }
    }
}
