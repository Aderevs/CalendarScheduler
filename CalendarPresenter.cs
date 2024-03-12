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
        public void RemoveEvent(DateOnly date)
        {
            Console.WriteLine("Are you sure that you want to delete this event (if it is personal event you won't be able to recover it)? Y/N");
            string? choice = Console.ReadLine();
            choice = choice.Trim().ToLowerInvariant();
            if (choice == "y")
            {
                _model.RemoveEvent(date);
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
    }
}
