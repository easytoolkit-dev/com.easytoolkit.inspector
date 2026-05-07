using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace EasyToolkit.Inspector.Editor
{
    public sealed class LocalPersistentContext<T>
    {
        private readonly GlobalPersistentContext<T> _context;
        [CanBeNull] private T _localValue;

        [CanBeNull] public T Value
        {
            get => _localValue;
            set
            {
                if (!EqualityComparer<T>.Default.Equals(_localValue, value))
                {
                    _context.Value = value;
                    _localValue = value;
                }
            }
        }

        private LocalPersistentContext(GlobalPersistentContext<T> global)
        {
            if (global == null)
                throw new ArgumentNullException(nameof(global));

            _context = global;
            _localValue = _context.Value;
        }

        /// <summary>
        /// Creates a local context object for the provided global context.
        /// </summary>
        /// <param name="global">The global context object.</param>
        public static LocalPersistentContext<T> Create(GlobalPersistentContext<T> global)
        {
            return new LocalPersistentContext<T>(global);
        }

        /// <summary>
        /// Updates the local value to the current global value.
        /// </summary>
        public void UpdateLocalValue()
        {
            _localValue = _context.Value;
        }
    }
}
