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
        public List<TypeOfDate> Type { get; set; }
        public List<string?> NameOfEvent { get; set; }
        public SortedSet<TimeBoundEvent> TimeBoundEvents { get; set; }
        public int NumberOfEvents
        {
            get
            {
                return Type.Count;
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
            Type = new List<TypeOfDate>();
            NameOfEvent = new();
            TimeBoundEvents = [];
            Type.Add(TypeOfDate.Usual);
            NameOfEvent.Add(null);
        }
        public Day(TypeOfDate type)
        {
            Type = new List<TypeOfDate>();
            NameOfEvent = new();
            TimeBoundEvents = [];
            Type.Add(type);
            NameOfEvent.Add(null);
        }
        public Day(TypeOfDate type, string nameOfEvent) : this(type)
        {
            NameOfEvent = new();
            NameOfEvent.Add(nameOfEvent);
        }
        public Day(TypeOfDate type, string nameOfEvent, SortedSet<TimeBoundEvent> timeBoundEvents) : this(type, nameOfEvent)
        {
            TimeBoundEvents = timeBoundEvents;
        }
        public Day(List<TypeOfDate> type, List<string?> nameOfEvent)
        {
            Type = type;
            NameOfEvent = nameOfEvent;
            TimeBoundEvents = [];
        }
    }
}
