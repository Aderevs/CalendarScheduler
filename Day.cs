﻿using System;
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
        public List<string?> NameOfEvent { get; set; }
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
            NameOfEvent = new List<string>();
            Type.Add(TypeOfDate.Usual);
            NameOfEvent.Add(null);
        }
        public Day(TypeOfDate type)
        {
            Type = new List<TypeOfDate>();
            NameOfEvent = new List<string>();
            Type.Add(type);
            NameOfEvent.Add(null);
        }
        public Day(TypeOfDate type, string nameOfEvent) : this(type)
        {
            NameOfEvent = new List<string>();
            NameOfEvent.Add(nameOfEvent);
        }
        public Day(List<TypeOfDate> type, List<string?> nameOfEvent)
        {
            Type = type;
            NameOfEvent = nameOfEvent;
        }

        /*public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is Day)
            {
                Day other = (Day)obj;
                return other.Type.Equals(Type) && other.NameOfEvent.Equals(NameOfEvent);
            }
            return false;
        }*/
    }
}
