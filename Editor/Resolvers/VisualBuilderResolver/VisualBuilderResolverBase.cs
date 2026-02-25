using System;
using EasyToolkit.Core;
using JetBrains.Annotations;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Abstract base class for visual builder resolution in the <see cref="IElement"/> system
    /// </summary>
    public abstract class VisualBuilderResolverBase : ResolverBase, IVisualBuilderResolver
    {
        private bool _isInitialized;

        /// <summary>
        /// Override this method to perform initialization logic
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// Gets the visual builder for the element
        /// </summary>
        /// <returns>The visual builder</returns>
        [MustUseReturnValue]
        protected abstract IVisualBuilder GetVisualBuilder();

        private void EnsureInitialize()
        {
            if (!_isInitialized)
            {
                Initialize();
                _isInitialized = true;
            }
        }

        IVisualBuilder IVisualBuilderResolver.GetVisualBuilder()
        {
            EnsureInitialize();
            return GetVisualBuilder();
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
