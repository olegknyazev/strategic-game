using System.Collections.Generic;

namespace StrategicGame.Common {
    // Just a queue with mutex, but that's good enough yet.
    public class ConcurrentQueue<T> where T : class {
        Queue<T> _data = new Queue<T>();
        object _mutex = new object();

        public T Dequeue() {
            lock (_mutex)
                return _data.Count > 0 ? _data.Dequeue() : null;
        }

        public void Enqueue(T item) {
            lock (_mutex)
                _data.Enqueue(item);
        }
    }
}
