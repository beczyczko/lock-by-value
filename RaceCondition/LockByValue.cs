using System;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;

namespace RaceCondition
{
    public class LockByValue : IDisposable
    {
        public bool WasHandled;
        private Mutex _mutex;

        public LockByValue(string mutexId, int timeOut = -1)
        {
            InitMutex(mutexId);
            try
            {
                if (timeOut < 0)
                {
                    WasHandled = _mutex.WaitOne(Timeout.Infinite, false);
                }
                else
                {
                    WasHandled = _mutex.WaitOne(timeOut, false);
                }

                if (WasHandled == false)
                {
                    throw new TimeoutException("Timeout waiting for exclusive access on SingleInstance");
                }
            }
            catch (AbandonedMutexException)
            {
                WasHandled = true;
            }
        }

        public void Dispose()
        {
            if (_mutex != null)
            {
                if (WasHandled)
                {
                    _mutex.ReleaseMutex();
                }

                _mutex.Close();
            }
        }

        private void InitMutex(string mutexId)
        {
            _mutex = new Mutex(false, mutexId);

            var allowEveryoneRule = new MutexAccessRule(
                new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                MutexRights.FullControl,
                AccessControlType.Allow);
            var securitySettings = new MutexSecurity();
            securitySettings.AddAccessRule(allowEveryoneRule);
            _mutex.SetAccessControl(securitySettings);
        }
    }
}