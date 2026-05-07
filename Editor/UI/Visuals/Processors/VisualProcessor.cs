using UnityEngine.UIElements;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Abstract base class for visual element processors that modify and enhance visual elements.
    /// Provides initialization support and chain navigation for sequential processing.
    /// </summary>
    public abstract class VisualProcessor : IVisualProcessor
    {
        private bool _isInitialized;
        private IElement _element;
        private VisualProcessorChain _chain;

        /// <summary>
        /// Gets or sets the inspector element associated with this processor.
        /// </summary>
        IElement IHandler.Element
        {
            get => _element;
            set => _element = value;
        }

        /// <summary>
        /// Gets or sets the visual processor chain that this processor belongs to.
        /// </summary>
        VisualProcessorChain IVisualProcessor.Chain
        {
            get => _chain;
            set => _chain = value;
        }

        /// <summary>
        /// Gets the inspector element associated with this processor.
        /// </summary>
        public IElement Element => _element;

        /// <summary>
        /// Gets the visual processor chain that this processor belongs to.
        /// </summary>
        public VisualProcessorChain Chain => _chain;

        /// <summary>
        /// Determines whether this processor can handle the specified element.
        /// Override this method to provide custom handling logic.
        /// </summary>
        /// <param name="element">The element to test.</param>
        /// <returns>True if this processor can handle the element; otherwise, false.</returns>
        protected virtual bool CanProcess(IElement element)
        {
            return true;
        }

        /// <summary>
        /// Processes the specified visual element to modify its appearance or behavior.
        /// Override this method to implement custom processing logic.
        /// </summary>
        /// <param name="visualElement">The visual element to process.</param>
        protected virtual void Process(VisualElement visualElement)
        {
        }

        /// <summary>
        /// Initializes the processor. Override this method to set up any required state.
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// Calls the next processor in the chain, if available.
        /// </summary>
        /// <param name="visualElement">The visual element to pass to the next processor.</param>
        /// <returns>True if there was a next processor to call; otherwise, false.</returns>
        protected bool CallNextProcessor(VisualElement visualElement)
        {
            if (_chain.MoveNext() && _chain.Current != null)
            {
                _chain.Current.Process(visualElement);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Ensures that the processor has been initialized before processing.
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
            return CanProcess(element);
        }

        /// <summary>
        /// Processes the specified visual element.
        /// Ensures initialization before processing.
        /// </summary>
        /// <param name="visualElement">The visual element to process.</param>
        void IVisualProcessor.Process(VisualElement visualElement)
        {
            EnsureInitialize();
            Process(visualElement);
        }
    }
}
