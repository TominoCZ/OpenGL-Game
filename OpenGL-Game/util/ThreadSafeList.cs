using System.Collections.Generic;

namespace OpenGL_Game
{
    class ThreadSafeList<T>
    {
        private List<T> list;

        public int Count => list.Count;

        public ThreadSafeList()
        {
            list = new List<T>();
        }

        public void Add(T item)
        {
            lock (list)
            {
                list.Add(item);
            }
        }

        public void Remove(T item)
        {
            lock (list)
            {
                list.Remove(item);
            }
        }

        public T this[int index]
        {
            get
            {
                lock (list)
                {
                    return list[index];
                }
            }
            set
            {
                lock (list)
                {
                    list[index] = value;
                }
            }
        }
    }
}
