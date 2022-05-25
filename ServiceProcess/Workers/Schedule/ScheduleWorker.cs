using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VSSystem.Logger;
using VSSystem.ServiceProcess.Extensions;

namespace VSSystem.ServiceProcess.Workers
{
    public abstract class ScheduleWorker : AWorker
    {
        protected int _sleepTime;
        protected int _hour, _minute;
        protected List<EWorkerScheduleDayOfWeek> _dayOfWeeks;

        public ScheduleWorker(ScheduleWorkerStartInfo startInfo, ALogger logger = null) 
            : base(startInfo, logger)
        {
            _hour = startInfo.Hour;
            _minute = startInfo.Minute;
            _sleepTime = 31000;
            _type = EWorkerType.Schedule;
            _dayOfWeeks = startInfo.DayOfWeeks;
        }        

        protected override async Task _RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (_ValuesInitialized())
                    {
                        DateTime now = DateTime.Now;
                        if(_OnTime(now))
                        {
                            await Task.Run(() => _RunScheduleTasksAsync(cancellationToken));
                        }
                        Thread.Sleep(_sleepTime);
                    }
                    else
                    {
                        _ = this.LogWarningAsync(_name + " cannot start because values has not been initialized.");
                        Thread.Sleep(5000);
                    }
                }
            }
            catch { }
        }        

        protected virtual bool _OnTime(DateTime dt)
        {
            return _IsOnDay(dt) && ((_hour == -1 || dt.Hour == _hour) && (_minute == - 1 || dt.Minute == _minute));
        }

        protected bool _IsOnDay(DateTime dt)
        {
            if(_dayOfWeeks?.Count > 0)
            {
                EWorkerScheduleDayOfWeek dayOfWeek = EWorkerScheduleDayOfWeek.NoDay;
                Enum.TryParse(dt.DayOfWeek.ToString(), true, out dayOfWeek);
                if(_dayOfWeeks.Contains(dayOfWeek))
                {
                    return true;
                }
            }
            return false;
        }

        protected abstract Task _RunScheduleTasksAsync(CancellationToken cancellationToken);
    }
}
