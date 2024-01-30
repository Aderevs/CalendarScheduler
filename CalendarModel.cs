using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarScheduler
{
    internal class CalendarModel
    {
        //private Day[] dateArray = new Day[365 * 5 + 2];
        private Dictionary<DateOnly, Day> allDays = new Dictionary<DateOnly, Day>();

        public CalendarModel()
        {
            DateOnly currentDay;
            TypeOfDate currentDayType = TypeOfDate.Usual;
            string? currentEvent = null;
            int dayCounterForEvents;


            for (int i = 0; i < 365 * 5 + 2; i++)
            {
                currentDay = new DateOnly(2024, 1, 1).AddDays(i);
                dayCounterForEvents = currentDay.DayOfYear;

                //reduce the day value by 1 if it is a leap year
                if ((currentDay.Year == 2024 || currentDay.Year == 2028) && currentDay.DayOfYear >= 60)
                {
                    dayCounterForEvents--;
                }

                //mark all events with a fixed date
                switch (dayCounterForEvents)
                {
                    case 1:
                        currentEvent = "New Year";
                        currentDayType = TypeOfDate.InternationalEvent;
                        break;
                    case 19:
                        currentEvent = "Baptism";
                        currentDayType = TypeOfDate.HolyEvent;
                        break;
                    case 45:
                        currentEvent = "St. Valentine's Day (for all those who are in love)";
                        currentDayType = TypeOfDate.InternationalEvent;
                        break;
                    case 67:
                        currentEvent = "Women's Rights Day";
                        currentDayType = TypeOfDate.InternationalEvent;
                        break;
                    case 114:
                        currentEvent = "International Chernobyl Remembrance Day";
                        currentDayType = TypeOfDate.TragicEvent;
                        break;
                    case 128:
                        currentEvent = "Day of Remembrance and Victory over Nazism in World War II";
                        currentDayType = TypeOfDate.InternationalEvent;
                        break;
                    case 179:
                        currentEvent = "Day of the Constitution of Ukraine";
                        currentDayType = TypeOfDate.NationalEvent;
                        break;
                    case 236:
                        currentEvent = "Independence Day of Ukraine";
                        currentDayType = TypeOfDate.NationalEvent;
                        break;
                    case 287:
                        currentEvent = "Day of the Defender of Ukraine";
                        currentDayType = TypeOfDate.NationalEvent;
                        break;
                    case 304:
                        currentEvent = "Halloween";
                        currentDayType = TypeOfDate.InternationalEvent;
                        break;
                    case 313:
                        currentEvent = "Day of Ukrainian Writing and Language";
                        currentDayType = TypeOfDate.NationalEvent;
                        break;
                    case 325:
                        currentEvent = "Day of Dignity and Freedome (commemorating the events of Euromaidan)";
                        currentDayType = TypeOfDate.NationalEvent;
                        break;
                    case 330:
                        currentEvent = "Holodomor Remembrance Day";
                        currentDayType = TypeOfDate.TragicEvent;
                        break;
                    case 340:
                        currentEvent = "St. Nicholas Day";
                        currentDayType = TypeOfDate.HolyEvent;
                        break;
                    case 359:
                        currentEvent = "Christmas";
                        currentDayType = TypeOfDate.HolyEvent;
                        break;
                    default:
                        currentDayType = TypeOfDate.Usual;
                        currentEvent = null;
                        break;
                }

                //mark all events with a variable date
                #region mark all events with a variable date
                if (currentDay.Equals(new DateTime(2024, 5, 5)) ||
                    currentDay.Equals(new DateTime(2025, 4, 20)) ||
                    currentDay.Equals(new DateTime(2026, 4, 12)) ||
                    currentDay.Equals(new DateTime(2027, 5, 2)) ||
                    currentDay.Equals(new DateTime(2024, 4, 16)))
                {
                    currentEvent = "Easter";
                    currentDayType = TypeOfDate.HolyEvent;
                }
                else if (currentDay.Equals(new DateTime(2024, 3, 11)) ||
                    currentDay.Equals(new DateTime(2025, 2, 24)) ||
                    currentDay.Equals(new DateTime(2026, 2, 16)) ||
                    currentDay.Equals(new DateTime(2027, 3, 8)) ||
                    currentDay.Equals(new DateTime(2024, 2, 12)))
                {
                    currentEvent = "Shrovetide";
                    currentDayType = TypeOfDate.HolyEvent;
                }
                else if (currentDay.Equals(new DateTime(2024, 5, 5).AddDays(49)) ||
                    currentDay.Equals(new DateTime(2025, 4, 20).AddDays(49)) ||
                    currentDay.Equals(new DateTime(2026, 4, 12).AddDays(49)) ||
                    currentDay.Equals(new DateTime(2027, 5, 2).AddDays(49)) ||
                    currentDay.Equals(new DateTime(2024, 4, 16).AddDays(49)))
                {
                    currentEvent = "Trinity";
                    currentDayType = TypeOfDate.HolyEvent;
                }
                #endregion

                /*dateArray[i] = new Day(currentDay, currentDayType);
                dateArray[i].NameOfEvent = currentEvent;*/
                allDays.Add(currentDay, new Day(currentDayType, currentEvent));
            }

        }
        /*public CalendarModel(int a)
        {
            DateTime currentDay;
            TypeOfDate currentDayType = TypeOfDate.Usual;
            string? currentEvent = null;
            int dayCounterForEvents;
            for (int i = 0; i < dateArray.Length; i++)
            {
                currentDay = new DateTime(2024, 1, 1).AddDays(i);
                dayCounterForEvents = currentDay.DayOfYear;

                //reduce the day value by 1 if it is a leap year
                if ((currentDay.Year == 2024 || currentDay.Year == 2028) && currentDay.DayOfYear >= 60)
                {
                    dayCounterForEvents--;
                }

                //mark all events with a fixed date
                switch (dayCounterForEvents)
                {
                    case 1:
                        currentEvent = "New Year";
                        currentDayType = TypeOfDate.InternationalEvent;
                        break;
                    case 19:
                        currentEvent = "Baptism";
                        currentDayType = TypeOfDate.HolyEvent;
                        break;
                    case 45:
                        currentEvent = "St. Valentine's Day (for all those who are in love)";
                        currentDayType = TypeOfDate.InternationalEvent;
                        break;
                    case 67:
                        currentEvent = "Women's Rights Day";
                        currentDayType = TypeOfDate.InternationalEvent;
                        break;
                    case 114:
                        currentEvent = "International Chernobyl Remembrance Day";
                        currentDayType = TypeOfDate.NationalEvent;
                        break;
                    case 128:
                        currentEvent = "Day of Remembrance and Victory over Nazism in World War II";
                        currentDayType = TypeOfDate.InternationalEvent;
                        break;
                    case 179:
                        currentEvent = "Day of the Constitution of Ukraine";
                        currentDayType = TypeOfDate.NationalEvent;
                        break;
                    case 236:
                        currentEvent = "Independence Day of Ukraine";
                        currentDayType = TypeOfDate.NationalEvent;
                        break;
                    case 287:
                        currentEvent = "Day of the Defender of Ukraine";
                        currentDayType = TypeOfDate.NationalEvent;
                        break;
                    case 304:
                        currentEvent = "Halloween";
                        currentDayType = TypeOfDate.InternationalEvent;
                        break;
                    case 313:
                        currentEvent = "Day of Ukrainian Writing and Language";
                        currentDayType = TypeOfDate.NationalEvent;
                        break;
                    case 325:
                        currentEvent = "Day of Dignity and Freedome (commemorating the events of Euromaidan)";
                        currentDayType = TypeOfDate.NationalEvent;
                        break;
                    case 330:
                        currentEvent = "Holodomor Remembrance Day";
                        currentDayType = TypeOfDate.NationalEvent;
                        break;
                    case 340:
                        currentEvent = "St. Nicholas Day";
                        currentDayType = TypeOfDate.HolyEvent;
                        break;
                    case 359:
                        currentEvent = "Christmas";
                        currentDayType = TypeOfDate.HolyEvent;
                        break;
                    default:
                        currentDayType = TypeOfDate.Usual;
                        break;
                }

                //mark all events with a variable date
                #region mark all events with a variable date
                if (currentDay.Equals(new DateTime(2024, 5, 5)) ||
                    currentDay.Equals(new DateTime(2025, 4, 20)) ||
                    currentDay.Equals(new DateTime(2026, 4, 12)) ||
                    currentDay.Equals(new DateTime(2027, 5, 2)) ||
                    currentDay.Equals(new DateTime(2024, 4, 16)))
                {
                    currentEvent = "Easter";
                    currentDayType = TypeOfDate.HolyEvent;
                }
                else if (currentDay.Equals(new DateTime(2024, 3, 11)) ||
                    currentDay.Equals(new DateTime(2025, 2, 24)) ||
                    currentDay.Equals(new DateTime(2026, 2, 16)) ||
                    currentDay.Equals(new DateTime(2027, 3, 8)) ||
                    currentDay.Equals(new DateTime(2024, 2, 12)))
                {
                    currentEvent = "Shrovetide";
                    currentDayType = TypeOfDate.HolyEvent;
                }
                else if (currentDay.Equals(new DateTime(2024, 5, 5).AddDays(49)) ||
                    currentDay.Equals(new DateTime(2025, 4, 20).AddDays(49)) ||
                    currentDay.Equals(new DateTime(2026, 4, 12).AddDays(49)) ||
                    currentDay.Equals(new DateTime(2027, 5, 2).AddDays(49)) ||
                    currentDay.Equals(new DateTime(2024, 4, 16).AddDays(49)))
                {
                    currentEvent = "Trinity";
                    currentDayType = TypeOfDate.HolyEvent;
                }
                #endregion

                dateArray[i] = new Day(currentDay, currentDayType);
                dateArray[i].NameOfEvent = currentEvent;
            }

        }*/

        public void PrintAllDays()
        {
            foreach (KeyValuePair<DateOnly, Day> date in allDays)
            {
                Console.WriteLine(date.Key.ToString("yyyy.MM.dd") + " " + date.Value.NameOfEvents);
            }
        }

        public void AddNewEvent(DateOnly date, string newEvent)
        {
            if(allDays[date].NameOfEvents == null)
            {
                allDays[date] = new Day(TypeOfDate.PersonalEvent, newEvent);
            }
            else
            {
                Day newDay = allDays[date];
                newDay.NameOfEvents += "; " + newEvent;
                allDays[date] = newDay;
            }

        }
        public void AddNewEvent(int month, int day, string newEvent)
        {
            for(int i = 2024; i <= 2028; i++)
            {
                DateOnly date = new DateOnly(i, month, day);
                if(allDays[date].NameOfEvents == null)
                {
                    allDays[date] = new Day(TypeOfDate.PersonalEvent, newEvent);
                }
                else
                {
                    Day newDay = allDays[date];
                    newDay.NameOfEvents += "; " + newEvent;
                    allDays[date] = newDay;
                }
            }

        }

        public void RemoveEvent(DateOnly date)
        {
            allDays[date] = new Day(TypeOfDate.Usual);
        }
        public void RemoveEvent(int month, int day)
        {
            for (int i = 2024; i <= 2028; i++)
            {
                DateOnly date = new DateOnly(i, month, day);
                allDays[date] = new Day(TypeOfDate.Usual);
            }
        }

        public KeyValuePair<DateOnly, Day>[] GetMonthArray(int year, int month)
        {
            return allDays.Where(day=> day.Key.Year==year &&  day.Key.Month==month).ToArray();
        }

    }
}
