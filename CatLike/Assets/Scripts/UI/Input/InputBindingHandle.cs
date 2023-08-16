using System;

namespace UI.Input.Binding
{
    public class InputBindingHandle : IInputBindingHandle
    {
        private readonly Action m_removeFunc;

        public InputBindingHandle(Action removeFunc)
        {
            m_removeFunc = removeFunc;
        }

        public void RemoveBinding()
        {
            m_removeFunc();
        }
    }

    public interface IInputBindingHandle
    {
        void RemoveBinding();
    }
}
