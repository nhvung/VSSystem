using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VSSystem.ServiceProcess.Workers
{
    public class ScheduleWorkerStartInfo : AWorkerStartInfo
    {

        int _Hour;
        public int Hour { get { return _Hour; } set { _Hour = value; } }

        int _Minute;
        public int Minute { get { return _Minute; } set { _Minute = value; } }

        List<EWorkerScheduleDayOfWeek> _DayOfWeeks;
        public List<EWorkerScheduleDayOfWeek> DayOfWeeks { get { return _DayOfWeeks; } set { _DayOfWeeks = value; } }
        public ScheduleWorkerStartInfo() : base() { }
        public ScheduleWorkerStartInfo(string name, bool enabled, string serviceName, int hour, int minute, List<EWorkerScheduleDayOfWeek> dayOfWeeks) 
            : base(name, enabled, serviceName) 
        {
            _Hour = hour;
            _Minute = minute;
            _DayOfWeeks = dayOfWeeks;
        }
    }
}
