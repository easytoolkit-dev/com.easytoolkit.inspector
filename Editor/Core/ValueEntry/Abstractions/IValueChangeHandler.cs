using System;

namespace EasyToolkit.Inspector.Editor
{
    public interface IValueChangeHandler
    {
        event EventHandler<ValueChangedEventArgs> BeforeValueChanged;
        event EventHandler<ValueChangedEventArgs> AfterValueChanged;
        void ApplyChanges();
        void EnqueueChange(Action action);
    }
}
