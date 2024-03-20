using System.Text.Json;

namespace CalendarScheduler
{
    internal class CalendarModel
    {
        private Dictionary<DateOnly, Day> _allDays;
        private Dictionary<DateOnly, Day> _allDefaultEvents;
        private const string _allDaysJsonFilePath = "allDays.json";
        private const string _allDefaultEventsJsonFilePath = "allDefaultEvents.json";
        public CalendarModel()
        {
            FileInfo allDaysFile = new FileInfo(_allDaysJsonFilePath);
            FileInfo allDefaultEventsFile = new FileInfo(_allDefaultEventsJsonFilePath);

            if (allDaysFile.Exists && allDefaultEventsFile.Exists)
            {
                _allDays = DeserializationFromJson(_allDaysJsonFilePath);
                _allDefaultEvents = DeserializationFromJson(_allDefaultEventsJsonFilePath);
            }

            if (!allDaysFile.Exists || !allDefaultEventsFile.Exists ||
                _allDays == null || _allDefaultEvents == null)
            {
                ResetOrGenerate();
            }
        }
        public void ResetOrGenerate()
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
                Day dayToAdd = new Day(currentDayType, currentEvent);
                _allDays.Add(currentDay, dayToAdd);
                if (currentDayType != TypeOfDate.Usual)
                {
                    if (_allDefaultEvents.ContainsKey(currentDay))
                    {
                        _allDefaultEvents[currentDay].Type.Add(currentDayType);
                        _allDefaultEvents[currentDay].NameOfEvent.Add(currentEvent);
                    }
                    _allDefaultEvents.Add(currentDay, dayToAdd);
                }
            }

            SerializationDatesToJson(_allDays, _allDaysJsonFilePath);
            SerializationDatesToJson(_allDefaultEvents, _allDefaultEventsJsonFilePath);

        }
        public void PrintAllDays()
        {
            foreach (KeyValuePair<DateOnly, Day> date in _allDays)
            {
                Console.WriteLine(date.Key.ToString("yyyy.MM.dd") + " " + date.Value.NameOfEvent);
            }
        }

        public void AddNewEvent(DateOnly date, string newEvent)
        {
            if (_allDays[date].Type[0] == TypeOfDate.Usual)
            {
                _allDays[date] = new Day(TypeOfDate.PersonalEvent, newEvent);
            }
            else
            {
                _allDays[date].Type.Add(TypeOfDate.PersonalEvent);
                _allDays[date].NameOfEvent.Add(newEvent);
            }
            SerializationDatesToJson(_allDays, _allDaysJsonFilePath);
        }
        public void AddNewEvent(int month, int day, string newEvent)
        {
            for (int i = 2024; i <= 2028; i++)
            {
                DateOnly date = new DateOnly(i, month, day);
                if (_allDays[date].Type[0] == TypeOfDate.Usual)
                {
                    _allDays[date] = new Day(TypeOfDate.PersonalEvent, newEvent);
                }
                else
                {
                   
                    _allDays[date].Type.Add(TypeOfDate.PersonalEvent);
                    _allDays[date].NameOfEvent.Add(newEvent);
                }
            }
            SerializationDatesToJson(_allDays, _allDaysJsonFilePath);
        } 
        public void RemoveEvent(DateOnly date) => RemoveEvent(date, (byte)(_allDays[date].NumberOfEvents - 1));
        public void RemoveEvent(DateOnly date, byte numberOfEvent)
        {
            if (numberOfEvent <= _allDays[date].NumberOfEvents)
            {
                _allDays[date].Type.RemoveAt(numberOfEvent - 1);
                _allDays[date].NameOfEvent.RemoveAt(numberOfEvent - 1);
                if (_allDays[date].NumberOfEvents < 1)
                {
                    _allDays[date] = new Day(TypeOfDate.Usual);
                }
            }
            else
            {
                throw new ArgumentException("this day don't has to many events");
            }
            SerializationDatesToJson(_allDays, _allDaysJsonFilePath);
        }
        public void ResetDefaultEvents()
        {
            _allDefaultEvents = DeserializationFromJson(_allDefaultEventsJsonFilePath);
            foreach (var day in _allDays)
            {
                if (_allDefaultEvents.ContainsKey(day.Key))
                {
                    var thisDayEvents = new List<string>();
                    if (_allDays[day.Key].NumberOfEvents > 1 || _allDays[day.Key].Type[0] == TypeOfDate.PersonalEvent)
                    {
                        for (int i = 0; i < day.Value.NumberOfEvents; i++)
                        {
                            Console.WriteLine();
                            if (day.Value.Type[i] == TypeOfDate.PersonalEvent)
                            {
                                thisDayEvents.Add(day.Value.NameOfEvent[i]);
                            }
                        }
                    }
                    _allDays[day.Key] = new Day(_allDefaultEvents[day.Key].Type, _allDefaultEvents[day.Key].NameOfEvent);
                    foreach (var eventInfo in thisDayEvents)
                    {
                        AddNewEvent(day.Key, eventInfo);
                    }

                }
            }
            SerializationDatesToJson(_allDays, _allDaysJsonFilePath);
        }
        public void EditEventDescription(DateOnly date, byte numberOfEvent, string newDescription)
        {
            if (_allDays[date].NumberOfEvents <= numberOfEvent)
            {
                _allDays[date].NameOfEvent[numberOfEvent - 1] = newDescription;
            }
            else
            {
                throw new ArgumentException("Attempt to change event description with number that don't exist");
            }
        }
        public void EditEventDescription(DateOnly date, string newDescription) => EditEventDescription(date, 1, newDescription);
        public KeyValuePair<DateOnly, Day>[] GetMonthArray(int year, int month)
        {
            return _allDays.Where(day => day.Key.Year == year && day.Key.Month == month).ToArray();
        }

        private void SerializationDatesToJson(Dictionary<DateOnly, Day> dates, string filePath)
        {
            var jsonString = JsonSerializer.Serialize(dates);

            using (var writer = new StreamWriter(filePath))
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
