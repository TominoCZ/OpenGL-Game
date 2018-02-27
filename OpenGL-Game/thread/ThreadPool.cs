using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace OpenGL_Game
{
    static class ThreadPool
    {
        private static List<Worker> _workers;
        private static List<Worker.Method> _queue;

        private static Thread _queueThread;

        static ThreadPool()
        {
            _workers = new List<Worker>();
            _queue = new List<Worker.Method>();

            for (int i = 0; i < 4; i++)
            {
                _workers.Add(new Worker());
            }

            _queueThread = new Thread(manageTaskQueue);
            _queueThread.Start();
        }

        public static void runTask(bool highPriority, Worker.Method f)
        {
            var worker = getAvailableWorker();

            if (worker != null)
                worker.runTask(f);
            else
            {
                if (highPriority)
                    _queue.Insert(0, f);
                else
                    _queue.Add(f);
            }
        }

        private static void manageTaskQueue()
        {
            while (true)
            {
                var worker = getAvailableWorker();

                if (worker != null && _queue.Count > 0)
                {
                    var func = _queue.First();

                    worker.runTask(func);

                    _queue.Remove(func);
                }

                Thread.Sleep(2);
            }
        }

        private static Worker getAvailableWorker()
        {
            for (int i = 0; i < _workers.Count; i++)
            {
                var worker = _workers[i];

                if (worker.Ready)
                    return worker;
            }

            return null;
        }
    }

    class Worker
    {
        private Thread _thread;
        private Method _task;

        public delegate void Method();

        public bool Ready { get; private set; }

        public Worker()
        {
            _thread = new Thread(run);
            _thread.IsBackground = true;
            _thread.Start();

            Ready = true;
        }

        private void run()
        {
            while (true)
            {
                if (_task != null)
                {
                    _task();

                    _task = null;
                    Ready = true;
                }

                Thread.Sleep(2);
            }
        }

        public void runTask(Method workerFunc)
        {
            Ready = false;

            _task = workerFunc;
        }
    }
}