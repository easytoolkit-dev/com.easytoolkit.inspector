using System;
using System.Reflection;
using EasyToolKit.Core;
using EasyToolKit.Core.Reflection;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Property operation implementation for member access (fields and properties)
    /// </summary>
    /// <typeparam name="TOwner">Owner type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    public class MemberValueOperation<TOwner, TValue> : ValueOperationBase<TValue>
    {
        private readonly MemberInfo _memberInfo;
        private readonly InstanceGetter<TOwner, TValue> _getter;
        private readonly InstanceSetter<TOwner, TValue> _setter;

        /// <summary>
        /// Initializes a new instance of MemberPropertyOperation
        /// </summary>
        /// <param name="memberInfo">Member information</param>
        public MemberValueOperation(MemberInfo memberInfo) : base(typeof(TOwner))
        {
            var accessor = ReflectionPathFactory.BuildAccessor(memberInfo.Name);

            _getter = accessor.BuildInstanceGetter<TOwner, TValue>();
            try
            {
                _setter = accessor.BuildInstanceSetter<TOwner, TValue>();
            }
            catch (Exception)
            {
                // Member is read-only
            }

            _memberInfo = memberInfo;
        }

        /// <summary>
        /// Whether the value is read-only
        /// </summary>
        public override bool IsReadOnly => _setter == null;

        /// <summary>
        /// Gets the value with type safety
        /// </summary>
        /// <param name="owner">Owner object</param>
        /// <returns>Property value</returns>
        public override TValue GetValue(ref object owner)
        {
            var castedOwner = (TOwner)owner;
            var result = _getter(ref castedOwner);
            owner = castedOwner;
            return result;
        }

        /// <summary>
        /// Sets the value with type safety
        /// </summary>
        /// <param name="owner">Owner object</param>
        /// <param name="value">Value to set</param>
        public override void SetValue(ref object owner, TValue value)
        {
            if (_setter == null)
            {
                throw new NotSupportedException($"Member '{_memberInfo.Name}' is read-only.");
            }
            var castedOwner = (TOwner)owner;
            _setter(ref castedOwner, value);
            owner = castedOwner;
        }
    }
}
