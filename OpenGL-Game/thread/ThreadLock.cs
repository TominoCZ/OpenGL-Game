using System.Threading;

namespace OpenGL_Game
{
    class ThreadLock
    {
        private bool locked;

        public void Lock() => locked = true;
        public void Unlock() => locked = false;

        public delegate void Method();

        private Method method;

        public ThreadLock(Method m)
        {
            method = m;
        }

        public void WaitFor()
        {
            while (locked)
            {
                Thread.Sleep(1);
            }
        }

        public void ExecuteCode()
        {
            method();
            Unlock();
        }
    }
}