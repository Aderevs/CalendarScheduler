using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace CalendarScheduler
{
    internal class CalendarPresenter
    {
        private readonly CalendarModel _model;

        public CalendarPresenter()
        {
            _model = new CalendarModel();
            CalendarUserInterface _view = new CalendarUserInterface(_model);

            _view.OnEventAdded += AddEvent;
            _view.OnEventRemoved += RemoveEvent;
            _view.OnDefaultEventsReturned += _model.ResetDefaultEvents;
            _view.OnCalendarReset += ResetCalendar;
            _view.OnEventEdited += EditEvent;
            _model.OnEventHappened += _view.Remind;
            Task checking = new Task(_model.CheckForEvents);
            checking.Start();
            Task monthInterface = new Task(() => _view.MonthInterface(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
            monthInterface.Start();
            checking.Wait();
            monthInterface.Wait();
        }

        public void AddEvent(int year, int month, int day, string nameOfEvent)
        {
            Console.WriteLine("Do you want to add time constrains to this event? Y/N");
            string? choice = Console.ReadLine();
            choice = choice?.Trim().ToLowerInvariant();
            if (choice == "y")
            {
                Console.WriteLine("Enter time your event will start (format: hh:mm)");
                string? inputTime = Console.ReadLine();
                string[]? splitInput = inputTime?.Split(':');
                if (splitInput != null &&
                    splitInput.Length == 2 &&
                    byte.TryParse(splitInput[0], out byte hours) &&
                    byte.TryParse(splitInput[1], out byte minutes))
                {
                    if (hours >= 0 && hours < 24 &&
                        minutes >= 0 && minutes < 60)
                    {
                        _model.AddTimeBoundEvent(new DateOnly(year, month, day), new TimeOnly(hours, minutes), nameOfEvent);
                    }
                    else
                    {
                        Console.WriteLine("invalid format of input time is out of range\nPress any key to continue...");
                        Console.ReadKey();
                    }
                }
                else
                {
                    Console.WriteLine("invalid format of input\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
            else if (choice == "n")
            {
                Console.WriteLine("Do you want to add this event in each year (else add event only in current year)? Y/N");
                choice = Console.ReadLine();
                choice = choice?.Trim().ToLowerInvariant();
                if (choice == "y")
                {
                    _model.AddNewEvent(month, day, nameOfEvent);
                }
                else if (choice == "n")
                {
                    _model.AddNewEvent(new DateOnly(year, month, day), nameOfEvent);
                }
                else
                {
                    Console.WriteLine("invalid input\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine("invalid input\nPress any key to continue...");
                Console.ReadKey();
            }

        }
        public void RemoveEvent(KeyValuePair<DateOnly, Day> date)
        {
            byte numberOfEvent = 1;
            if (date.Value.NumberOfEvents > 1 ||
                date.Value.Type[0] != TypeOfDate.Usual && date.Value.HasTimeBoundEvents ||
                date.Value.TimeBoundEvents.Count > 1)
            {
                Console.WriteLine("Enter number of event you want to remove:");
                while (!byte.TryParse(Console.ReadLine(), out numberOfEvent) || numberOfEvent > date.Value.NumberOfEvents + date.Value.TimeBoundEvents.Count)
                {
                    Console.WriteLine("incorrect input, value must be bigger than 0 and smaller or equal the quantity of events in this day.\n" +
                        "Please try again: ");
                }
            }
            Console.WriteLine("Are you sure that you want to delete this event (if it is personal event you won't be able to recover it)? Y/N");
            string? choice = Console.ReadLine();
            choice = choice?.Trim().ToLowerInvariant();
            if (choice == "y")
            {
                if (date.Value.HasTimeBoundEvents)
                {
                    if (date.Value.TimeBoundEvents.Count >= numberOfEvent)
                    {
                        _model.RemoveTimeBoundEvent(date.Key, numberOfEvent);
                    }
                    else
                    {
                        numberOfEvent -= (byte)date.Value.TimeBoundEvents.Count;
                        _model.RemoveEvent(date.Key, numberOfEvent);
                    }
                }
                else
                {
                    _model.RemoveEvent(date.Key, numberOfEvent);
                }
            }
        }
        public void EditEvent(KeyValuePair<DateOnly, Day> date)
        {
            if (date.Value.NumberOfEvents > 1 ||
                date.Value.Type[0] != TypeOfDate.Usual && date.Value.HasTimeBoundEvents)
            {
                byte numberOfEvent;
                Console.WriteLine("Enter number of event you want to edit:");
                while (!byte.TryParse(Console.ReadLine(), out numberOfEvent) || numberOfEvent > date.Value.NumberOfEvents + date.Value.TimeBoundEvents.Count)
                {
                    Console.WriteLine("incorrect input, value must be bigger than 0 and smaller or equal the quantity of events in this day.\n" +
                        "Please try again: ");
                }
                if (date.Value.HasTimeBoundEvents && numberOfEvent <= date.Value.TimeBoundEvents.Count)
                {

                    EditTimeboundEvent(date, numberOfEvent);
                }
                else
                {
                    if (date.Value.HasTimeBoundEvents)
                    {
                        numberOfEvent -= (byte)date.Value.TimeBoundEvents.Count;
                    }
                    Console.WriteLine("Enter new description (or press enter to cancel operation):");
                    string newDesc = Console.ReadLine();
                    if (newDesc != "")
                    {
                        _model.EditEventDescription(date.Key, numberOfEvent, newDesc ?? "");
                    }
                }
            }
            else
            {
                if (date.Value.TimeBoundEvents.Count == 1)
                {
                    EditTimeboundEvent(date, 1);
                }
                else
                {
                    Console.WriteLine("Enter new description (or press enter to cancel operation):");
                    string newDesc = Console.ReadLine();
                    if (newDesc != "")
                    {

                        if (date.Value.Type[0] == TypeOfDate.Usual)
                        {
                            _model.AddNewEvent(date.Key, newDesc);
                        }
                        else
                        {
                            _model.EditEventDescription(date.Key, newDesc);
                        }
                    }
                }

            }
        }
        public void ResetCalendar()
        {
            Console.WriteLine("Are you sure that you want to reset calendar (All personal events will deleted and return all default events)? Y/N");
            string? choice = Console.ReadLine();
            choice = choice.Trim().ToLowerInvariant();
            if (choice == "y")
            {
                _model.ResetOrGenerate();
            }
        }

        private void EditTimeboundEvent(KeyValuePair<DateOnly, Day> date, byte numberOfEvent)
        {
            Console.WriteLine("Do you want to edit description (enter 0), or time of event (enter 1)");
            if (byte.TryParse(Console.ReadLine(), out byte choice) && choice <= 1)
            {
                if (choice == 1)
                {
                    Console.WriteLine("Enter time your event will start (format — hh:mm)");
                    string? inputTime = Console.ReadLine();
                    string[] splitInput = inputTime.Split(':');
                    if (splitInput.Length == 2 &&
                        byte.TryParse(splitInput[0], out byte hours) &&
                        byte.TryParse(splitInput[1], out byte minutes))
                    {
                        if (hours >= 0 && hours < 24 &&
                            minutes >= 0 && minutes < 60)
                        {
                            _model.EditTimeBoundEventTime(date.Key, numberOfEvent, new TimeOnly(hours, minutes));
                        }
                        else
                        {
                            Console.WriteLine("invalid format of input time is out of range\nPress any key to continue...");
                            Console.ReadKey();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Enter new description (or press enter to cancel operation):");
                    string? newDescForTbEvent = Console.ReadLine();
                    if (newDescForTbEvent != "")
                    {
                        _model.EditTimeBoundEventDescription(date.Key, numberOfEvent, newDescForTbEvent ?? "");
                    }
                }
            }
        }

    }
}
