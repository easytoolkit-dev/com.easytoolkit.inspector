using EasyToolkit.Core.Reflection;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Default implementation of visual builder resolver for <see cref="IElement"/>
    /// </summary>
    public class DefaultVisualBuilderResolver : VisualBuilderResolverBase
    {
        private IVisualBuilder _builder;

        /// <summary>
        /// Initializes the resolver by discovering and creating the visual builder instance
        /// </summary>
        protected override void Initialize()
        {
            // Get visual builder type for the element
            var builderType = VisualBuilderUtility.GetVisualBuilderType(Element);
            if (builderType != null)
            {
                _builder = builderType.CreateInstance<IVisualBuilder>();
            }
        }

        /// <summary>
        /// Gets the visual builder for the element
        /// </summary>
        /// <returns>The visual builder, or null if no builder is found</returns>
        protected override IVisualBuilder GetVisualBuilder()
        {
            return _builder;
        }

        /// <summary>
        /// Clears the cached visual builder when the resolver is returned to the pool.
        /// </summary>
        protected override void OnRelease()
        {
            base.OnRelease();
            _builder = null;
        }
    }
}
