using System;
using EasyToolKit.Core;
using EasyToolKit.Serialization;

namespace EasyToolKit.Inspector.Attributes
{
    public abstract class InspectorAttribute : Attribute
    {
        private string _id;

        private string Id
        {
            get
            {
                if (_id == null)
                {
                    var serializationData = new EasySerializationData();
                    var value = this;
                    EasySerializer.Serialize(ref value, ref serializationData);
                    _id = GetType().FullName + "+" + Convert.ToBase64String(serializationData.BinaryData);
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
