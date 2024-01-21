using System;
using System.Collections.Generic;
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
    internal struct Day
    {
        public TypeOfDate Type { get; set; }
        public string? NameOfEvents { get; set; }

        public Day(TypeOfDate type)
        {
            Type = type;
        }
        public Day(TypeOfDate type, string nameOfEvent):this(type)
        {
            NameOfEvents = nameOfEvent;
        }

    }
}
