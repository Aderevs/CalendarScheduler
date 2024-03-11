using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarScheduler
{
    public enum TypeOfDate
    {
        Usual,
        InternationalEvent,
        NationalEvent,
        HolyEvent,
        PersonalEvent,
        TragicEvent
    }
    internal class Day
    {
        public List<TypeOfDate> Type { get; set; }
        public List<string?> NameOfEvents { get; set; }
        public int NumberOfEvents
        {
            get
            {
                return Type.Count;
            }
        }

        public Day()
        {
            Type = new List<TypeOfDate>();
            NameOfEvents = new List<string>();
            Type.Add(TypeOfDate.Usual);
            NameOfEvents.Add(null);
        }
        public Day(TypeOfDate type)
        {
            Type = new List<TypeOfDate>();
            NameOfEvents = new List<string>();
            Type.Add(type);
            NameOfEvents.Add(null);
        }
        public Day(TypeOfDate type, string nameOfEvent) : this(type)
        {
            NameOfEvents = new List<string>();
            NameOfEvents.Add(nameOfEvent);
        }
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is Day)
            {
                Day other = (Day)obj;
                return other.Type.Equals(Type) && other.NameOfEvents.Equals(NameOfEvents);
            }
            return false;
        }
    }
}
