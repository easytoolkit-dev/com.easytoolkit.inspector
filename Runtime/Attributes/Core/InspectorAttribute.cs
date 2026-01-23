using System;
using System.Collections.Concurrent;
using EasyToolKit.Core.Reflection;
using EasyToolKit.Serialization;

namespace EasyToolKit.Inspector.Attributes
{
    [EasySerializable(AllocInherit = true)]
    public abstract class InspectorAttribute : Attribute
    {
        private string _id;

        private string Id
        {
            get
            {
                if (_id == null)
                {
                    var data = EasySerializer.SerializeToBinary(this);
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
    }
}
