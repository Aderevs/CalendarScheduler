using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarScheduler
{
    internal class TimeBoundEvent : IComparable<TimeBoundEvent>
    {
        public TimeOnly Start { get; set; }
        public TimeOnly End { get; set; }
        public string Description { get; set; }
        public TimeBoundEvent(TimeOnly start, string description)
        {
            Start = start;
            Description = description;
            TimeOnly end = new TimeOnly(23, 59, 59, 999);
            End = end;
        }

        public int CompareTo(TimeBoundEvent? other)
        {
            if (this == other) return 0;
            if (other == null) return 1;
            var resultOfCompareStarts = this.Start.CompareTo(other.Start);
            if (resultOfCompareStarts == 0)
            {
                return this.End.CompareTo(other.End);
            }
            return resultOfCompareStarts;
        }
    }
}
