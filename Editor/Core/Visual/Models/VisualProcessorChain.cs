using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Represents a chain of visual processors that can be iterated over during visual element processing.
    /// This class implements both &lt;see cref="IEnumerable{T}"/&gt; and &lt;see cref="IEnumerator{T}"/&gt; interfaces to allow
    /// sequential iteration through visual processors while automatically skipping processors
    /// that should be skipped during the processing.
    /// </summary>
    public class VisualProcessorChain : IEnumerable<IVisualProcessor>, IEnumerator<IVisualProcessor>
    {
        private readonly IVisualProcessor[] _processors;
        private int _index = -1;

        /// <summary>
        /// Gets the current visual processor in the iteration.
        /// Returns null if the iteration is before the first processor or after the last processor.
        /// </summary>
        public IVisualProcessor Current
        {
            get
            {
                if (_index >= 0 && _index < _processors.Length)
                {
                    var result = _processors[_index];
                    result.Element = Element;
                    result.Chain = this;
                    return result;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets all visual processors in this chain, including those that may be skipped during iteration.
        /// </summary>
        public IVisualProcessor[] Processors => _processors;

        /// <summary>
        /// Gets the inspector element associated with this visual processor chain.
        /// </summary>
        public IElement Element { get; private set; }

        /// <summary>
        /// Initializes a new instance of the &lt;see cref="VisualProcessorChain"/&gt; class.
        /// </summary>
        /// <param name="element">The inspector element this visual processor chain is associated with.</param>
        /// <param name="processors">The collection of visual processors to include in the chain.</param>
        public VisualProcessorChain(IElement element, IEnumerable<IVisualProcessor> processors)
        {
            Element = element;
            _processors = processors.ToArray();
        }

        object IEnumerator.Current => Current;

        /// <summary>
        /// Advances the enumerator to the next visual processor in the chain.
        /// </summary>
        /// <returns>true if the enumerator was successfully advanced to the next processor; false if the enumerator has passed the end of the collection.</returns>
        public bool MoveNext()
        {
            _index++;
            return Current != null;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first visual processor in the chain.
        /// </summary>
        public void Reset()
        {
            _index = -1;
        }

        /// <summary>
        /// Returns an enumerator that iterates through all visual processors in the chain.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through all visual processors.</returns>
        public IEnumerator<IVisualProcessor> GetEnumerator()
        {
            return ((IEnumerable<IVisualProcessor>)_processors).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through all visual processors in the chain.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through all visual processors.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// Resets the enumerator position.
        /// </summary>
        void IDisposable.Dispose()
        {
            Reset();
        }
    }
}
