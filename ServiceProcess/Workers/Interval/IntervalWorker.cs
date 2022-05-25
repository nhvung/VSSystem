using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VSSystem.Logger;

namespace VSSystem.ServiceProcess.Workers
{
    public abstract class IntervalWorker : AWorker
    {
        protected const int _WorkerWaitTimeout = 3600000;

        #region Properties        

        protected int _Interval;
        public int Interval { get { return _Interval; } set { _Interval = value; } }

        protected int _NumberOfThreads;
        public int NumberOfThreads { get { return _NumberOfThreads; } set { _NumberOfThreads = value; } }
        
        protected EWorkerIntervalUnit _intervalUnit;
        protected TimeSpan _tsInterval;

        #endregion

        #region Contructor
        public IntervalWorker(IntervalWorkerStartInfo startInfo, ALogger logger = null)
            : base(startInfo, logger)
        {           
            _Interval = startInfo.Interval;
            _NumberOfThreads = startInfo.NumberOfThreads;                        
            _intervalUnit = startInfo.IntervalUnit;
            _type = EWorkerType.Interval;
            _CorrectInterval();
        }

        void _CorrectInterval()
        {
            try
            {
                switch (_intervalUnit)
                {
                    case EWorkerIntervalUnit.Second:
                        _tsInterval = new TimeSpan(0, 0, _Interval);
                        break;
                    case EWorkerIntervalUnit.Minute:
                        _tsInterval = new TimeSpan(0, _Interval, 0);
                        break;
                    case EWorkerIntervalUnit.Hour:
                        _tsInterval = new TimeSpan(_Interval, 0, 0);
                        break;
                    case EWorkerIntervalUnit.Day:
                        _tsInterval = new TimeSpan(_Interval, 0, 0, 0);
                        break;
                }
            }
            catch { }
        }
        #endregion
        protected override async Task _RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (_ValuesInitialized())
                    {
                        
                        DateTime bTime = DateTime.Now;
                        await _RunInternalTasksAsync(cancellationToken);

                        DateTime eTime = DateTime.Now;
                        TimeSpan ts = eTime - bTime;
                        if (ts < _tsInterval)
                        {
                            ts = _tsInterval - ts;
                            Thread.Sleep(ts);
                        }
                    }
                    else
                    {
                        //this.LogWarning(_name + " cannot start because values has not been initialized.");
                        Thread.Sleep(5000);
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// For implement worker
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected abstract Task _RunInternalTasksAsync(CancellationToken cancellationToken);
    }
}
