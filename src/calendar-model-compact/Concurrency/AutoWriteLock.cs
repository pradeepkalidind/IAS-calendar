using System;
using System.Threading;

namespace Calendar.Model.Compact.Concurrency
{
    public class AutoWriteLock : IDisposable
    {
        private readonly ReaderWriterLockSlim slim;

        public AutoWriteLock(ReaderWriterLockSlim slim)
        {
            this.slim = slim;
            slim.EnterWriteLock();
        }

        public void Dispose()
        {
            slim.ExitWriteLock();
        }
    }
}