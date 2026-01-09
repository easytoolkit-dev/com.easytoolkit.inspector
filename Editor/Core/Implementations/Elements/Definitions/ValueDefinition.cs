using System;
using System.Collections.Generic;

namespace EasyToolKit.Inspector.Attributes.Editor.Implementations
{
    /// <summary>
    /// Base implementation of <see cref="IValueDefinition"/> for all data-containing elements.
    /// </summary>
    public class ValueDefinition : ElementDefinition, IValueDefinition
    {
        /// <summary>
        /// Gets or sets the type of the element.
        /// </summary>
        public Type ValueType { get; set; }
    }
}
