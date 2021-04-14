using System.Collections.Generic;
using System.Threading;

namespace networking.utils
{
    public class BlockingQueue<T>
    {
        private readonly Queue<T> _queue = new();

        private bool _closing;
        
        public void Enqueue(T item)
        {
            lock (_queue)
            {
                _queue.Enqueue(item);
                if (_queue.Count == 1)
                    Monitor.PulseAll(_queue);
            }
        }

        public T? Dequeue()
        {
            lock (_queue)
            {
                while (_queue.Count == 0)
                {
                    if (_closing)
                        return default;
                    Monitor.Wait(_queue);
                }
                
                return _queue.Dequeue();
            }
        }

        public void Close()
        {
            lock (_queue)
            {
                _closing = true;
                Monitor.PulseAll(_queue);
            }
        }
    }
}