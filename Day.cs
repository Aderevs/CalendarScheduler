using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace CalendarScheduler
{
    internal class Day
    {
        public List<TypeOfDate> Types { get; set; }
        public List<string?> NameOfEvents { get; set; }
        public SortedSet<TimeBoundEvent> TimeBoundEvents { get; set; }
        public int NumberOfEvents
        {
            get
            {
                return Types.Count;
            }
        }
        public bool HasTimeBoundEvents
        {
            get
            {
                return TimeBoundEvents.Any();
            }
        }
        public Day()
        {
            Types = new List<TypeOfDate>();
            NameOfEvents = new();
            TimeBoundEvents = [];
            Types.Add(TypeOfDate.Usual);
            NameOfEvents.Add(null);
        }
        public Day(TypeOfDate type)
        {
            Types = new List<TypeOfDate>();
            NameOfEvents = new();
            TimeBoundEvents = [];
            Types.Add(type);
            NameOfEvents.Add(null);
        }
        public Day(TypeOfDate type, string nameOfEvent) : this(type)
        {
            NameOfEvents = new();
            NameOfEvents.Add(nameOfEvent);
        }
        public Day(TypeOfDate type, string nameOfEvent, SortedSet<TimeBoundEvent> timeBoundEvents) : this(type, nameOfEvent)
        {
            TimeBoundEvents = timeBoundEvents;
        }
        public Day(List<TypeOfDate> type, List<string?> nameOfEvents)
        {
            Types = type;
            NameOfEvents = nameOfEvents;
            TimeBoundEvents = [];
        }
    }
}
