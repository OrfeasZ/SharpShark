using System;
using System.Collections.Generic;

namespace GS.Lib.Network.Sockets
{
    /// <summary>
    /// Represents a pool of objects with a size limit.
    /// </summary>
    /// <typeparam name="T">The type of object in the pool.</typeparam>
    public sealed class ObjectPool<T> : IDisposable
        where T : new()
    {
        private readonly int m_Size;
        private readonly object m_Locker;
        private readonly Queue<T> m_Queue;
        private int m_Count;


        /// <summary>
        /// Initializes a new instance of the ObjectPool class.
        /// </summary>
        /// <param name="p_Size">The size of the object pool.</param>
        public ObjectPool(int p_Size)
        {
            if (p_Size <= 0)
            {
                const string c_Message = "The size of the pool must be greater than zero.";
                throw new ArgumentOutOfRangeException("p_Size", p_Size, c_Message);
            }

            m_Size = p_Size;
            m_Locker = new object();
            m_Queue = new Queue<T>();
        }


        /// <summary>
        /// Retrieves an item from the pool. 
        /// </summary>
        /// <returns>The item retrieved from the pool.</returns>
        public T Get()
        {
            lock (m_Locker)
            {
                if (m_Queue.Count > 0)
                    return m_Queue.Dequeue();

                ++m_Count;
                return new T();
            }
        }

        /// <summary>
        /// Places an item in the pool.
        /// </summary>
        /// <param name="p_Item">The item to place to the pool.</param>
        public void Put(T p_Item)
        {
            lock (m_Locker)
            {
                if (m_Count < m_Size)
                {
                    m_Queue.Enqueue(p_Item);
                }
                else
                {
                    using (p_Item as IDisposable)
                        --m_Count;
                }
            }
        }

        /// <summary>
        /// Disposes of items in the pool that implement IDisposable.
        /// </summary>
        public void Dispose()
        {
            lock (m_Locker)
            {
                m_Count = 0;
                while (m_Queue.Count > 0)
                {
                    using (m_Queue.Dequeue() as IDisposable)
                    {

                    }
                }
            }
        }
    }
}