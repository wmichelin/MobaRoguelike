using System;
using System.Collections.Generic;

namespace MobaRoguelike.Runtime.Pooling
{
    public class ObjectPool<T> where T : class
    {
        private readonly Stack<T> _stack = new Stack<T>();
        private readonly Func<T> _createFunc;
        private readonly Action<T> _onGet;
        private readonly Action<T> _onReturn;

        public ObjectPool(Func<T> createFunc, Action<T> onGet = null, Action<T> onReturn = null)
        {
            _createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
            _onGet = onGet;
            _onReturn = onReturn;
        }

        public T Get()
        {
            T item = _stack.Count > 0 ? _stack.Pop() : _createFunc();
            _onGet?.Invoke(item);
            return item;
        }

        public void Return(T item)
        {
            _onReturn?.Invoke(item);
            _stack.Push(item);
        }

        public int CountInactive => _stack.Count;
    }
}
