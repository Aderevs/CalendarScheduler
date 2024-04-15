using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CalendarScheduler
{
    internal class DayBuilder
    {
        private Day _day;
        public DayBuilder()
        {
            _day = new Day();
        }
        public DayBuilder AddType(TypeOfDate type)
        {
            if (_day.Types[0] == TypeOfDate.Usual)
            {
                _day.Types[0] = type;
            }
            else
            {
                _day.Types.Add(type);
            }
            return this;
        }
        public DayBuilder AddEvent(TypeOfDate type, string name)
        {
            if (_day.Types[0] == TypeOfDate.Usual)
            {
                _day.Types[0] = type;
                _day.NameOfEvents[0] = name;
            }
            else
            {
                _day.Types.Add(type);
                _day.NameOfEvents.Add(name);
            }
            return this;
        }
        public DayBuilder AddEventsList(List<TypeOfDate> type, List<string?> nameOfEvents)
        {
            if (_day.Types[0] == TypeOfDate.Usual)
            {
                _day.Types = type;
                _day.NameOfEvents = nameOfEvents;
            }
            else
            {
                _day.Types.AddRange(type);
                _day.NameOfEvents.AddRange(nameOfEvents);
            }
            return this;
        }
        public DayBuilder AddTimeBoundEvent(TimeBoundEvent timeBoundEvent)
        {
            _day.TimeBoundEvents.Add(timeBoundEvent);
            return this;
        }
        public DayBuilder AddSetTimeBoundEvents(SortedSet<TimeBoundEvent> timeBoundEvents)
        {
            if (_day.HasTimeBoundEvents)
            {
                foreach(var timeBoundEvent in timeBoundEvents)
                {
                    _day.TimeBoundEvents.Add(timeBoundEvent);
                }
            }
            else
            {
                _day.TimeBoundEvents = timeBoundEvents;
            }
            return this;
        }
        public Day Build()
        {
            return _day;
        }
    }
}
