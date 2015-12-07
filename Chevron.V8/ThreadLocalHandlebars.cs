using System;
using System.Threading;

namespace Chevron
{
    public class ThreadLocalHandlebars : IDisposable
    {
        ThreadLocal<Handlebars> threadLocal;

        public ThreadLocalHandlebars()
            : this(() => new Handlebars())
        {
        }

        public ThreadLocalHandlebars(Func<Handlebars> builder)
        {
            threadLocal = new ThreadLocal<Handlebars>(builder);
        }

        public void Dispose()
        {
        }

        public Handlebars Value => threadLocal.Value;

        // ReSharper disable once UnusedMember.Local
        void DisposeManaged()
        {
            if (threadLocal != null)
            {
                if (threadLocal.IsValueCreated)
                {
                    threadLocal.Value.Dispose();
                }

                threadLocal.Dispose();
            }
        }
    }
}