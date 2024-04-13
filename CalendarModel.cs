using System.Linq;
using System.Text.Json;

namespace CalendarScheduler
{
    internal class CalendarModel
    {
        private Dictionary<DateOnly, Day> _allDays;
        private Dictionary<DateOnly, Day>? _allDefaultEvents;
        private const string _allDaysJsonFilePath = "allDays.json";
        private const string _allDefaultEventsJsonFilePath = "allDefaultEvents.json";
        public event Action<string> OnEventHappened;
        public CalendarModel()
        {
            FileInfo allDaysFile = new FileInfo(_allDaysJsonFilePath);
            FileInfo allDefaultEventsFile = new FileInfo(_allDefaultEventsJsonFilePath);
            //FileInfo allTimeBoundEventsFile = new FileInfo(_allTimeBoundEventsJsonFilePath);

            if (allDaysFile.Exists && allDefaultEventsFile.Exists )
            {
                _allDays = DeserializeDatesFromJson(_allDaysJsonFilePath);
                _allDefaultEvents = DeserializeDatesFromJson(_allDefaultEventsJsonFilePath);
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
                Day dayToAdd = new Day(currentDayType, currentEvent ?? "");
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

            SerializeDatesToJson(_allDays, _allDaysJsonFilePath);
            SerializeDatesToJson(_allDefaultEvents, _allDefaultEventsJsonFilePath);

        }
        public void PrintAllDays()
        {
            foreach (KeyValuePair<DateOnly, Day> date in _allDays)
            {
                Console.WriteLine(date.Key.ToString("yyyy.mm.dd") + " " + date.Value.NameOfEvent);
            }
        }

        #region interaction with days for user
        public void AddNewEvent(DateOnly date, string newEvent)
        {
            if (_allDays[date].Type[0] == TypeOfDate.Usual)
            {
                var timeBoundEvents = _allDays[date].TimeBoundEvents;
                _allDays[date] = new Day(TypeOfDate.PersonalEvent, newEvent, timeBoundEvents);
            }
            else
            {
                _allDays[date].Type.Add(TypeOfDate.PersonalEvent);
                _allDays[date].NameOfEvent.Add(newEvent);
            }
            SerializeDatesToJson(_allDays, _allDaysJsonFilePath);
        }
        public void AddNewEvent(int month, int day, string newEvent)
        {
            for (int i = 2024; i <= 2028; i++)
            {
                DateOnly date = new DateOnly(i, month, day);
                if (_allDays[date].Type[0] == TypeOfDate.Usual)
                {
                    var timeBoundEvents = _allDays[date].TimeBoundEvents;
                    _allDays[date] = new Day(TypeOfDate.PersonalEvent, newEvent, timeBoundEvents);
                }
                else
                {

                    _allDays[date].Type.Add(TypeOfDate.PersonalEvent);
                    _allDays[date].NameOfEvent.Add(newEvent);
                }
            }
            SerializeDatesToJson(_allDays, _allDaysJsonFilePath);
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
                    _allDays[date].Type.Add(TypeOfDate.Usual);
                }
            }
            else
            {
                throw new ArgumentException("this day don't has to many events");
            }
            SerializeDatesToJson(_allDays, _allDaysJsonFilePath);
        }
        public void ResetDefaultEvents()
        {
            _allDefaultEvents = DeserializeDatesFromJson(_allDefaultEventsJsonFilePath);
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
            SerializeDatesToJson(_allDays, _allDaysJsonFilePath);
        }
        public void EditEventDescription(DateOnly date, byte numberOfEvent, string newDescription)
        {
            if (_allDays[date].NumberOfEvents >= numberOfEvent)
            {
                _allDays[date].NameOfEvent[numberOfEvent - 1] = newDescription;
            }
            else
            {
                throw new ArgumentException("Attempt to change event description with number that don't exist");
            }
        }
        public void EditEventDescription(DateOnly date, string newDescription) => EditEventDescription(date, 1, newDescription);

        public void AddTimeBoundEvent(DateOnly date, TimeOnly start, string description)
        {
            var eventToAdd = new TimeBoundEvent(start, description);
            _allDays[date].TimeBoundEvents.Add(eventToAdd);
            SerializeDatesToJson(_allDays, _allDaysJsonFilePath);
        }
        public void EditTimeBoundEventDescription(DateOnly date, byte numberOfEvent, string newDescription)
        {
            if (_allDays[date].TimeBoundEvents.Count >= numberOfEvent)
            {
                _allDays[date].TimeBoundEvents.ElementAt(numberOfEvent - 1).Description = newDescription;
                SerializeDatesToJson(_allDays, _allDaysJsonFilePath);
            }
            else
            {
                throw new ArgumentException("Attempt to change event description with number that don't exist");
            }
        }
        public void EditTimeBoundEventDescription(DateOnly date, string newDescription) => EditTimeBoundEventDescription(date, 1, newDescription);
        public void EditTimeBoundEventTime(DateOnly date, byte numberOfEvent, TimeOnly newStart, TimeOnly newEnd)
        {
            if (_allDays[date].TimeBoundEvents.Count <= numberOfEvent)
            {
                _allDays[date].TimeBoundEvents.ElementAt(numberOfEvent - 1).Start = newStart;
                _allDays[date].TimeBoundEvents.ElementAt(numberOfEvent - 1).End = newEnd;
            }
            else
            {
                throw new ArgumentException("Attempt to change event description with number that don't exist");
            }

        }
        public void EditTimeBoundEventTime(DateOnly date, byte numberOfEvent, TimeOnly newStart)
        {
            if (_allDays[date].TimeBoundEvents.Count >= numberOfEvent)
            {
                _allDays[date].TimeBoundEvents.ElementAt(numberOfEvent - 1).Start = newStart;
                SerializeDatesToJson(_allDays, _allDaysJsonFilePath);
            }
            else
            {
                throw new ArgumentException("Attempt to change event with number that don't exist");
            }
        }
        public void RemoveTimeBoundEvent(DateOnly date, byte numberOfEvent)
        {
            if (_allDays[date].TimeBoundEvents.Count >= numberOfEvent)
            {
                var eventToRemove = _allDays[date].TimeBoundEvents.ElementAt(numberOfEvent - 1);
                _allDays[date].TimeBoundEvents.Remove(eventToRemove);
                SerializeDatesToJson(_allDays, _allDaysJsonFilePath);
            }
            else
            {
                throw new ArgumentException("Attempt to change event with number that don't exist");
            }
        }
        #endregion

        public KeyValuePair<DateOnly, Day>[] GetMonthArray(int year, int month)
        {
            return _allDays.Where(day => day.Key.Year == year && day.Key.Month == month).ToArray();
        }
        private void SerializeDatesToJson(Dictionary<DateOnly, Day> dates, string filePath)
        {
            var jsonString = JsonSerializer.Serialize(dates);

            using (var writer = new StreamWriter(filePath))
            {
                writer.Write(jsonString);
            }
        }
        private Dictionary<DateOnly, Day> DeserializeDatesFromJson(string filePath)
        {
            Dictionary<DateOnly, Day>? dates;
            string json = File.ReadAllText(filePath);
            dates = JsonSerializer.Deserialize<Dictionary<DateOnly, Day>>(json);
            return dates;
        }

        public void CheckForEvents()
        {
            var today = new DateOnly(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
            var todaysDay = _allDays.First(d => d.Key == today);
            if (todaysDay.Value.HasTimeBoundEvents)
            {
                while (true)
                {
                    var scheduledEvent = todaysDay.Value.TimeBoundEvents.FirstOrDefault(tbe => tbe.Start == new TimeOnly(DateTime.Now.Hour, DateTime.Now.Minute));
                    if (scheduledEvent!=null)
                    {
                        OnEventHappened(scheduledEvent.Description);
                    }
                }
            }
        }
       
    }
}
