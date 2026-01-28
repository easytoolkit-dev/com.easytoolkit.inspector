using System;
using EasyToolkit.Core;
using EasyToolkit.Core.Editor;
using EasyToolkit.Core.Pooling;
using JetBrains.Annotations;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Provides data for the value dirty event.
    /// </summary>
    [MustDisposeResource]
    public class ValueDirtyEventArgs : EventArgs, IPoolItem, IDisposable
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ValueDirtyEventArgs"/> class from the object pool.
        /// </summary>
        /// <returns>A new or reused instance of <see cref="ValueDirtyEventArgs"/>.</returns>
        public static ValueDirtyEventArgs Create()
        {
            return EditorPoolUtility.Rent<ValueDirtyEventArgs>();
        }

        /// <summary>
        /// Releases the instance back to the object pool.
        /// </summary>
        public void Dispose()
        {
            EditorPoolUtility.Release(this);
        }

        void IPoolItem.Rent()
        {
        }

        void IPoolItem.Release()
        {
        }
    }
}
