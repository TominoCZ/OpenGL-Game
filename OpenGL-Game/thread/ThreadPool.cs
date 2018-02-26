using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace OpenGL_Game.thread
{
    delegate Delegate WorkerFunc();

    static class ThreadPool
    {
        private static List<Worker> _workers;
        private static List<WorkerFunc> _queue;

        private static Thread _queueThread;

        static ThreadPool()
        {
            _workers = new List<Worker>();
            _queue = new List<WorkerFunc>();
            
            for (int i = 0; i < 16; i++)
            {
                _workers.Add(new Worker());
            }

            _queueThread = new Thread(manageTaskQueue);
            _queueThread.IsBackground = true;
            _queueThread.Start();
        }

        public static void runTask(WorkerFunc f)
        {
            var worker = getAvailableWorker();

            if (worker != null)
                worker.runTask(f);
            else
            {
                _queue.Add(f);

                if (_queue.Count == 0)
                    _queueThread.Resume();
            }
        }

        private static void manageTaskQueue()
        {
            while (true)
            {
                var worker = getAvailableWorker();

                if (worker != null)
                {
                    var func = _queue.First();

                    worker.runTask(func);

                    _queue.Remove(func);
                }

                if (_queue.Count == 0)
                    _queueThread.Suspend();
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
        private WorkerFunc _task;

        public bool Ready { get; private set; }

        public Worker()
        {
            _thread = new Thread(run);
            _thread.IsBackground = true;
            _thread.Start();
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

                _thread.Suspend();
            }
        }

        public void runTask(WorkerFunc workerFunc)
        {
            Ready = false;

            _task = workerFunc;

            _thread.Resume();
        }
    }
}