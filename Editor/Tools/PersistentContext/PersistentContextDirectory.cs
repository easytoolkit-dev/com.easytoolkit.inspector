using System;
using System.Collections.Generic;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Serialization.Formatters;
using EasyToolkit.Serialization.Processors;

namespace EasyToolkit.Inspector.Editor
{
    [Serializable]
    internal class PersistentContextDirectory : Dictionary<string, GlobalPersistentContext>
    {
        class SerializationProcessor : SerializationProcessor<KeyValuePair<string, GlobalPersistentContext>>
        {
            [DependencyProcessor]
            private ISerializationProcessor<Type> _typeProcessor;
            [DependencyProcessor]
            private ISerializationProcessor<string> _keyProcessor;

            protected override void Process(string name, ref KeyValuePair<string, GlobalPersistentContext> keyValuePair, IDataFormatter formatter)
            {
                formatter.BeginMember(name);
                using var scope = formatter.EnterObject(ValueType);

                var key = keyValuePair.Key;
                _keyProcessor.Process("Key", ref key, formatter);

                Type valueType = null;
                if (formatter.Operation == FormatterOperation.Write)
                {
                    valueType = keyValuePair.Value.GetType();
                }
                _typeProcessor.Process("ValueType", ref valueType, formatter);

                var valueItemType = valueType.GetGenericArgumentsRelativeTo(typeof(GlobalPersistentContext<>))[0];
                if (SerializationProcessorFactory.GetProcessor(valueItemType) == null)
                {
                    keyValuePair = new KeyValuePair<string, GlobalPersistentContext>(key, null);
                    return;
                }

                object value = keyValuePair.Value;
                var valueProcessor = SerializationProcessorFactory.GetProcessor(valueType);
                valueProcessor.ProcessUntyped("Value", ref value, formatter);

                if (formatter.Operation == FormatterOperation.Read)
                {
                    keyValuePair = new KeyValuePair<string, GlobalPersistentContext>(key, (GlobalPersistentContext)value);
                }
            }
        }
    }
}
