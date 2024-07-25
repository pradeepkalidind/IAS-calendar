using System;
using System.Threading;

namespace Calendar.Model.Compact.Concurrency
{
    public class AutoReadLock : IDisposable
    {
        private readonly ReaderWriterLockSlim slim;

        public AutoReadLock(ReaderWriterLockSlim slim)
        {
            this.slim = slim;
            slim.EnterReadLock();
        }

        public void Dispose()
        {
            slim.ExitReadLock();
        }
    }
}