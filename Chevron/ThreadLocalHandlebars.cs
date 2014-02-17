using System;
using System.Threading;
using Janitor;

namespace Chevron
{
    [SkipWeaving]
    public class ThreadLocalHandlebars : IDisposable
    {
        ThreadLocal<Handlebars> threadLocal;
        bool valueCreated;

        public ThreadLocalHandlebars()
        {
            threadLocal = new ThreadLocal<Handlebars>(()=>new Handlebars());
        }

        public void Dispose()
        {
        }

        public Handlebars Value
        {
            get
            {
                valueCreated = true;
                return threadLocal.Value;
            }
        }

        // ReSharper disable once UnusedMember.Local
        void DisposeManaged()
        {
            if (threadLocal != null)
            {
                if (valueCreated)
                {
                    threadLocal.Value.Dispose();
                }

                threadLocal.Dispose();
            }
        }
    }
}