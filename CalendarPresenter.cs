using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarScheduler
{
    internal class CalendarPresenter
    {
        private CalendarModel _model;
        private readonly CalendarUserInterface _view;

        public CalendarPresenter()
        {
            _model = new CalendarModel();
            _view = new CalendarUserInterface(_model);

            _view.OnEventAdded += AddEvent;
            _view.OnEventRemoved += RemoveEvent;
            _view.OnDefaultEventsReturned += _model.ResetDefaultEvents;
            _view.OnCalendarReseted += ResetCalendar;
            _view.OnEventEdited += EditDescription;
            _view.MonthInterface(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        }

        public void AddEvent(int year, int month, int day, string nameOfEvent)
        {
            Console.WriteLine("Do you want to add this event in each year (else add event only in current year)? Y/N");
            string? choice = Console.ReadLine();
            choice = choice.Trim().ToLowerInvariant();
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
        public void RemoveEvent(KeyValuePair<DateOnly, Day> date)
        {
            byte numberOfEvent = 1;
            if (date.Value.NumberOfEvents > 1)
            {
                Console.WriteLine("Enter number of event you want to remove:");
                while(!byte.TryParse(Console.ReadLine(), out numberOfEvent) || numberOfEvent > date.Value.NumberOfEvents)
                {
                    Console.WriteLine("incorrect input, value must be bigger than 0 and smaller or equal the quantity of events in this day.\n" +
                        "Please try again: ");
                }
            }
            Console.WriteLine("Are you sure that you want to delete this event (if it is personal event you won't be able to recover it)? Y/N");
            string? choice = Console.ReadLine();
            choice = choice.Trim().ToLowerInvariant();
            if (choice == "y")
            {
                _model.RemoveEvent(date.Key, numberOfEvent);
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
        public void EditDescription(KeyValuePair<DateOnly, Day> date)
        {
            if (date.Value.NumberOfEvents > 1)
            {
                byte numberOfEvent;
                Console.WriteLine("Enter number of event you want to edit the description:");
                while (!byte.TryParse(Console.ReadLine(), out numberOfEvent) || numberOfEvent > date.Value.NumberOfEvents)
                {
                    Console.WriteLine("incorrect input, value must be bigger than 0 and smaller or equal the quantity of events in this day.\n" +
                        "Please try again: ");
                }
                Console.WriteLine("Enter new description (or press enter to cencel operation):");
                string newDesc = Console.ReadLine();
                if (newDesc != "")
                {
                    _model.EditEventDescription(date.Key, numberOfEvent, newDesc);
                }
            }
            else
            {
                Console.WriteLine("Enter new description (or press enter to cencel operation):");
                string newDesc = Console.ReadLine();
                if (newDesc != "")
                {
                    _model.EditEventDescription(date.Key, newDesc);
                }
            }
        }
    }
}
