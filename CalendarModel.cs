using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CalendarScheduler
{
    internal class CalendarModel
    {
        private readonly Dictionary<DateOnly, Day> _allDays;
        private readonly Dictionary<DateOnly, Day> _allDefaultEvents;
        private string _allDaysJsonFilePath = "allDays.json";
        private string _allDefaultEventsJsonFilePath = "allDefaultEvents.json";
        public CalendarModel()
        {
            FileInfo allDaysFile = new FileInfo(_allDaysJsonFilePath);
            FileInfo allDefaultEventsFile = new FileInfo(_allDefaultEventsJsonFilePath);

            if(allDaysFile.Exists && allDefaultEventsFile.Exists)
            {
                _allDays = DeserializationFromJson(_allDaysJsonFilePath);
                _allDefaultEvents = DeserializationFromJson(_allDefaultEventsJsonFilePath);
            }

            if(!allDaysFile.Exists || !allDefaultEventsFile.Exists ||
                _allDays == null || _allDefaultEvents == null)
            {
                _allDays = new Dictionary<DateOnly, Day>();
                _allDefaultEvents = new Dictionary<DateOnly, Day>();

                DateOnly currentDay;
                string? currentEvent;
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

                    TypeOfDate currentDayType;
                
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
                    if (currentDay.Equals(new DateOnly(2024, 5, 5)) ||
                        currentDay.Equals(new DateOnly(2025, 4, 20)) ||
                        currentDay.Equals(new DateOnly(2026, 4, 12)) ||
                        currentDay.Equals(new DateOnly(2027, 5, 2)) ||
                        currentDay.Equals(new DateOnly(2028, 4, 16)))
                    {
                        currentEvent = "Easter";
                        currentDayType = TypeOfDate.HolyEvent;
                    }
                    else if (currentDay.Equals(new DateOnly(2024, 3, 11)) ||
                        currentDay.Equals(new DateOnly(2025, 2, 24)) ||
                        currentDay.Equals(new DateOnly(2026, 2, 16)) ||
                        currentDay.Equals(new DateOnly(2027, 3, 8)) ||
                        currentDay.Equals(new DateOnly(2028, 2, 12)))
                    {
                        currentEvent = "Shrovetide";
                        currentDayType = TypeOfDate.HolyEvent;
                    }
                    else if (currentDay.Equals(new DateOnly(2024, 5, 5).AddDays(49)) ||
                        currentDay.Equals(new DateOnly(2025, 4, 20).AddDays(49)) ||
                        currentDay.Equals(new DateOnly(2026, 4, 12).AddDays(49)) ||
                        currentDay.Equals(new DateOnly(2027, 5, 2).AddDays(49)) ||
                        currentDay.Equals(new DateOnly(2028, 4, 16).AddDays(49)))
                    {
                        currentEvent = "Trinity";
                        currentDayType = TypeOfDate.HolyEvent;
                    }
                    #endregion

                    _allDays.Add(currentDay, new Day(currentDayType, currentEvent));
                    if(currentDayType != TypeOfDate.Usual)
                    {
                        _allDefaultEvents.Add(currentDay, new Day(currentDayType, currentEvent));
                    }
                }

                SerializationDatesToJson(_allDays, _allDaysJsonFilePath);
                SerializationDatesToJson(_allDefaultEvents, _allDefaultEventsJsonFilePath);
            }
        }
        public void PrintAllDays()
        {
            foreach (KeyValuePair<DateOnly, Day> date in _allDays)
            {
                Console.WriteLine(date.Key.ToString("yyyy.MM.dd") + " " + date.Value.NameOfEvents);
            }
        }

        public void AddNewEvent(DateOnly date, string newEvent)
        {
            if(_allDays[date].NameOfEvents == null)
            {
                _allDays[date] = new Day(TypeOfDate.PersonalEvent, newEvent);
            }
            else
            {
                Day newDay = _allDays[date];
                newDay.NameOfEvents += ";\n" + newEvent;
                newDay.Type = TypeOfDate.PersonalEvent;
                _allDays[date] = newDay;
            }
            SerializationDatesToJson(_allDays, _allDaysJsonFilePath);
        }
        public void AddNewEvent(int month, int day, string newEvent)
        {
            for(int i = 2024; i <= 2028; i++)
            {
                DateOnly date = new DateOnly(i, month, day);
                if(_allDays[date].NameOfEvents == null)
                {
                    _allDays[date] = new Day(TypeOfDate.PersonalEvent, newEvent);
                }
                else
                {
                    Day newDay = _allDays[date];
                    newDay.NameOfEvents += ";\n" + newEvent;
                    newDay.Type = TypeOfDate.PersonalEvent;
                    _allDays[date] = newDay;
                }
            }
            SerializationDatesToJson(_allDays, _allDaysJsonFilePath);
        }

        public void RemoveEvent(DateOnly date)
        {
            _allDays[date] = new Day(TypeOfDate.Usual);
            SerializationDatesToJson(_allDays, _allDaysJsonFilePath);
        }
        /*public void RemoveEvent(int month, int day)
        {
            Day currentDay = new Day();
            for (int i = 2024; i <= 2028; i++)
            {
                DateOnly date = new DateOnly(i, month, day);
                if (allDays[date].Equals(currentDay))
                {
                    currentDay = allDays[date];
                    allDays[date] = new Day(TypeOfDate.Usual);
                }
            }
        }*/

        public void ResetDefaultEvents()
        {
            foreach(var day in _allDays)
            {
                if (_allDefaultEvents.ContainsKey(day.Key))
                {
                    _allDays[day.Key] = _allDefaultEvents[day.Key];
                }
            }
            SerializationDatesToJson(_allDays, _allDaysJsonFilePath);
        }
        public KeyValuePair<DateOnly, Day>[] GetMonthArray(int year, int month)
        {
            return _allDays.Where(day=> day.Key.Year==year &&  day.Key.Month==month).ToArray();
        }

        private void SerializationDatesToJson(Dictionary<DateOnly, Day> dates, string filePath)
        {
            var jsonString = JsonSerializer.Serialize(dates);

            using (var writer =  new StreamWriter(filePath))
            {
                writer.Write(jsonString);
            }
        }

        private Dictionary<DateOnly, Day> DeserializationFromJson(string filePath)
        {
            Dictionary<DateOnly, Day> dates;
            string json = File.ReadAllText(filePath);
            dates = JsonSerializer.Deserialize<Dictionary<DateOnly, Day>>(json);
            return dates;
        }

    }
}
