using UnityEngine.UIElements;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Abstract base class for building visual elements in the inspector.
    /// Provides a foundation for creating custom visual element builders with initialization support.
    /// </summary>
    public abstract class VisualBuilder : IVisualBuilder
    {
        private bool _isInitialized;
        private IElement _element;

        /// <summary>
        /// Gets or sets the inspector element associated with this builder.
        /// </summary>
        IElement IHandler.Element
        {
            get => _element;
            set => _element = value;
        }

        /// <summary>
        /// Gets the inspector element associated with this builder.
        /// </summary>
        public IElement Element => _element;

        /// <summary>
        /// Determines whether this builder can handle the specified element.
        /// Override this method to provide custom handling logic.
        /// </summary>
        /// <param name="element">The element to test.</param>
        /// <returns>True if this builder can handle the element; otherwise, false.</returns>
        protected virtual bool CanBuild(IElement element)
        {
            return true;
        }

        /// <summary>
        /// Initializes the builder. Override this method to set up any required state.
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// Creates and returns a visual element for the inspector.
        /// </summary>
        /// <returns>A new visual element instance.</returns>
        public abstract VisualElement CreateVisualElement();

        /// <summary>
        /// Ensures that the builder has been initialized before processing.
        /// </summary>
        private void EnsureInitialize()
        {
            if (!_isInitialized)
            {
                Initialize();
                _isInitialized = true;
            }
        }

        /// <summary>
        /// Determines whether this handler can process the specified element.
        /// </summary>
        bool IHandler.CanHandle(IElement element)
        {
            return CanBuild(element);
        }

        /// <summary>
        /// Creates a visual element for the specified inspector element.
        /// Ensures initialization before creating the visual element.
        /// </summary>
        /// <returns>A new visual element instance.</returns>
        VisualElement IVisualBuilder.CreateVisualElement()
        {
            EnsureInitialize();
            return CreateVisualElement();
        }

        public virtual void Dispose()
        {
        }
    }
}
