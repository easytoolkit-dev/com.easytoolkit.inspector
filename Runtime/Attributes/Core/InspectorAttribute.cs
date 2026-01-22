using System;
using System.Collections.Concurrent;
using EasyToolKit.Core.Reflection;
using EasyToolKit.Serialization;

namespace EasyToolKit.Inspector.Attributes
{
    [EasySerializable(AllocInherit = true)]
    public abstract class InspectorAttribute : Attribute
    {
        private static readonly ConcurrentDictionary<Type, StaticInvoker<InspectorAttribute, byte[]>> SerializeInvokerByAttributeType =
            new ConcurrentDictionary<Type, StaticInvoker<InspectorAttribute, byte[]>>();

        private string _id;

        private string Id
        {
            get
            {
                if (_id == null)
                {
                    var invoker = SerializeInvokerByAttributeType.GetOrAdd(GetType(), type =>
                    {
                        var method = typeof(InspectorAttribute)
                            .GetMethod(nameof(SerializeWrapper), MemberAccessFlags.AllStatic)!
                            .MakeGenericMethod(type);
                        return ReflectionCompiler.CreateStaticMethodInvoker<InspectorAttribute, byte[]>(method);
                    });
                    var data = invoker(this);
                    _id = GetType().FullName + "+" + Convert.ToBase64String(data);
                }
                return _id;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Id == ((InspectorAttribute)obj).Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        private static byte[] SerializeWrapper<T>(T attribute)
        {
            return EasySerializer.SerializeToBinary(ref attribute);
        }
    }
}
