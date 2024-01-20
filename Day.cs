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
        public DateTime Date {  get; set; }
        public TypeOfDate Type { get; set; }
        public string? NameOfEvent { get; set; }

        public Day(DateTime date, TypeOfDate type)
        {
            Date = date;
            Type = type;
        }
    }
}
