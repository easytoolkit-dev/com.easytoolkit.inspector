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
        // class SerializationProcessor : SerializationProcessor<KeyValuePair<string, GlobalPersistentContext>>
        // {
        //     [DependencyProcessor]
        //     private ISerializationProcessor<Type> _typeProcessor;
        //     [DependencyProcessor]
        //     private ISerializationProcessor<string> _keyProcessor;
        //
        //     protected override void Process(ref KeyValuePair<string, GlobalPersistentContext> keyValuePair, IDataFormatter formatter)
        //     {
        //         using var scope = formatter.EnterObject(ValueType);
        //
        //         var key = keyValuePair.Key;
        //         _keyProcessor.Process("Key", ref key, formatter);
        //
        //         var valueType = typeof(GlobalPersistentContext);
        //         if (formatter.Operation == FormatterOperation.Write && keyValuePair.Value != null)
        //         {
        //             valueType = keyValuePair.Value.GetType();
        //         }
        //         _typeProcessor.Process("ValueType", ref valueType, formatter);
        //         if (valueType.IsAbstract)
        //         {
        //             keyValuePair = new KeyValuePair<string, GlobalPersistentContext>(key, null);
        //             return;
        //         }
        //
        //         var valueItemType = valueType.GetGenericArgumentsRelativeTo(typeof(GlobalPersistentContext<>))[0];
        //         if (SerializationProcessorFactory.CreateProcessor(valueItemType, Context) == null)
        //         {
        //             keyValuePair = new KeyValuePair<string, GlobalPersistentContext>(key, null);
        //             return;
        //         }
        //
        //         object value = keyValuePair.Value;
        //         var valueProcessor = SerializationProcessorFactory.CreateProcessor(valueType, Context);
        //         valueProcessor.ProcessUntyped("Value", ref value, formatter);
        //
        //         if (formatter.Operation == FormatterOperation.Read)
        //         {
        //             keyValuePair = new KeyValuePair<string, GlobalPersistentContext>(key, (GlobalPersistentContext)value);
        //         }
        //     }
        // }
    }
}
