using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CalendarScheduler
{
    internal class CalendarUserInterface
    {
        public CalendarModel CalendarModel { get; set; }
        public CalendarUserInterface(CalendarModel calendarModel)
        {
            CalendarModel = calendarModel;
        }

        public event Action<int, int, int, string>? OnEventAdded;
        public event Action<KeyValuePair<DateOnly, Day>>? OnEventRemoved;
        public event Action? OnCalendarReset;
        public event Action? OnDefaultEventsReturned;
        public event Action<KeyValuePair<DateOnly, Day>>? OnEventEdited;
        public static readonly object _consoleLock = new object();
        private int _year, _month, _day;
        private bool _isReminderRun = false;
        private KeyValuePair<DateOnly, Day>[][] GetMatrixForMonth(KeyValuePair<DateOnly, Day>[] Month)
        {
            KeyValuePair<DateOnly, Day> firstDayOfMonth = Month[0];
            int daysInMonth = DateTime.DaysInMonth(firstDayOfMonth.Key.Year, firstDayOfMonth.Key.Month);

            var daysOfMonth = new KeyValuePair<DateOnly, Day>[6][];
            int currentDayCounter = 0;

            //DayOfWeek returning values starts on Sunday, in Europe the week starts on Monday, so we fix it
            int numberOfFirstDayInMonthInWeek = 7 - (int)firstDayOfMonth.Key.DayOfWeek + 1;
            if (numberOfFirstDayInMonthInWeek <= 7)
            {
                daysOfMonth[0] = new KeyValuePair<DateOnly, Day>[numberOfFirstDayInMonthInWeek];
            }
            else
            {
                daysOfMonth[0] = new KeyValuePair<DateOnly, Day>[1];
            }

            for (int i = 0; i < daysOfMonth[0].Length; i++)
            {
                currentDayCounter++;
                daysOfMonth[0][i] = Month.First(day => day.Key.Day == currentDayCounter);
            }
            for (int i = 1; i < 6; i++)
            {
                if (daysInMonth - currentDayCounter >= 7)
                {
                    daysOfMonth[i] = new KeyValuePair<DateOnly, Day>[7];
                }
                else
                {
                    daysOfMonth[i] = new KeyValuePair<DateOnly, Day>[daysInMonth - currentDayCounter];
                }
                for (int j = 0; j < 7 && currentDayCounter < daysInMonth; j++)
                {
                    currentDayCounter++;
                    daysOfMonth[i][j] = Month.First(day => day.Key.Day == currentDayCounter);
                }
            }
            return daysOfMonth;
        }
        private void PrintMonth(KeyValuePair<DateOnly, Day>[][] daysMatrix, int chosenDay)
        {
            Console.Write(daysMatrix[0][0].Key.Year + " ");
            Console.WriteLine
                (
                daysMatrix[0][0].Key.Month
                switch
                {
                    1 => "January",
                    2 => "February",
                    3 => "March",
                    4 => "April",
                    5 => "May",
                    6 => "June",
                    7 => "July",
                    8 => "August",
                    9 => "September",
                    10 => "October",
                    11 => "November",
                    12 => "December",
                    _ => ""
                });
            Console.WriteLine("Mon" + "\t" + "Tus" + "\t" + "Wed" + "\t" + "Thi" + "\t" + "Fri" + "\t" + "Sat" + "\t" + "Sun");
            if (daysMatrix[0].Length > 0)
            {
                for (int i = 0; i < 7 - daysMatrix[0].Length; i++)
                {
                    Console.Write("\t");
                }
            }
            Day dayToWrite = new Day();
            for (int i = 0; i < daysMatrix.Length; i++)
            {
                for (int j = 0; j < daysMatrix[i].Length; j++)
                {
                    int lastIndex = daysMatrix[i][j].Value.Types.Count - 1;
                    if (daysMatrix[i][j].Key.DayOfWeek == DayOfWeek.Saturday || daysMatrix[i][j].Key.DayOfWeek == DayOfWeek.Sunday)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    if (daysMatrix[i][j].Key.Day == chosenDay)
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                        dayToWrite = daysMatrix[i][j].Value;
                    }
                    else if (daysMatrix[i][j].Value.HasTimeBoundEvents)
                    {
                        Console.BackgroundColor = ConsoleColor.Magenta;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (daysMatrix[i][j].Value.Types[lastIndex] == TypeOfDate.PersonalEvent)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkMagenta;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (daysMatrix[i][j].Value.Types[lastIndex] == TypeOfDate.HolyEvent)
                    {
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    else if (daysMatrix[i][j].Value.Types[lastIndex] == TypeOfDate.NationalEvent)
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (daysMatrix[i][j].Value.Types[lastIndex] == TypeOfDate.InternationalEvent)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (daysMatrix[i][j].Value.Types[lastIndex] == TypeOfDate.TragicEvent)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    Console.Write(daysMatrix[i][j].Key.Day);
                    Console.ResetColor();
                    Console.Write("\t");
                }
                Console.WriteLine();
            }

            if (dayToWrite.HasTimeBoundEvents)
            {
                Console.BackgroundColor = ConsoleColor.Magenta;
                Console.ForegroundColor = ConsoleColor.White;
                foreach (var tbEvent in dayToWrite.TimeBoundEvents)
                {
                    Console.WriteLine($"{tbEvent.Description} ({tbEvent.Start})");
                }
                Console.ResetColor();
            }
            if (dayToWrite.Types[0] != TypeOfDate.Usual)
            {
                for (int i = 0; i < dayToWrite.NumberOfEvents; i++)
                {
                    if (dayToWrite.Types[i] == TypeOfDate.PersonalEvent)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkMagenta;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Personal Event:");
                    }
                    else if (dayToWrite.Types[i] == TypeOfDate.HolyEvent)
                    {
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine("Holy Event:");
                    }
                    else if (dayToWrite.Types[i] == TypeOfDate.NationalEvent)
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("National Event:");
                    }
                    else if (dayToWrite.Types[i] == TypeOfDate.InternationalEvent)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("International Event:");
                    }
                    else if (dayToWrite.Types[i] == TypeOfDate.TragicEvent)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine("Tragic Event:");
                    }
                    Console.WriteLine(dayToWrite.NameOfEvents[i]);
                }
                Console.ResetColor();
            }
        }

        public void MonthInterface(int year, int month, int day)
        {
            lock (_consoleLock)
            {
                _day = day;
                _month = month;
                _year = year;
                var currentMonthArray = CalendarModel.GetMonthArray(_year, _month);
                var monthMatrix = GetMatrixForMonth(currentMonthArray);
                int daysInMonth = DateTime.DaysInMonth(_year, _month);

                ConsoleKeyInfo keyInfo;
                do
                {
                    Console.Clear();
                    PrintMonth(monthMatrix, _day);
                    keyInfo = Console.ReadKey();

                    if (keyInfo.Key == ConsoleKey.D || keyInfo.Key == ConsoleKey.RightArrow)
                    {
                        if (_day + 1 <= daysInMonth)
                        {
                            _day++;
                        }
                        else
                        {

                            if (_month + 1 <= 12)
                            {
                                daysInMonth = DateTime.DaysInMonth(_year, ++_month);
                                _day = 1;
                                currentMonthArray = CalendarModel.GetMonthArray(_year, _month);
                                monthMatrix = GetMatrixForMonth(currentMonthArray);
                            }
                            else
                            {
                                if (_year + 1 <= 2028)
                                {
                                    _month = 1;
                                    daysInMonth = DateTime.DaysInMonth(++_year, _month);
                                    _day = 1;
                                    currentMonthArray = CalendarModel.GetMonthArray(_year, _month);
                                    monthMatrix = GetMatrixForMonth(currentMonthArray);
                                }
                            }
                        }
                    }
                    else if (keyInfo.Key == ConsoleKey.A || keyInfo.Key == ConsoleKey.LeftArrow)
                    {
                        if (_day - 1 >= 1)
                        {
                            _day--;
                        }
                        else
                        {
                            if (_month - 1 >= 1)
                            {
                                daysInMonth = DateTime.DaysInMonth(_year, --_month);
                                _day = daysInMonth;
                                currentMonthArray = CalendarModel.GetMonthArray(_year, _month);
                                monthMatrix = GetMatrixForMonth(currentMonthArray);
                            }
                            else
                            {
                                if (_year - 1 >= 2024)
                                {
                                    _month = 12;
                                    daysInMonth = DateTime.DaysInMonth(--_year, _month);
                                    currentMonthArray = CalendarModel.GetMonthArray(_year, _month);
                                    monthMatrix = GetMatrixForMonth(currentMonthArray);
                                    _day = daysInMonth;
                                }
                            }
                        }
                    }
                    else if (keyInfo.Key == ConsoleKey.S || keyInfo.Key == ConsoleKey.DownArrow)
                    {
                        if (_day + 7 <= daysInMonth)
                        {
                            _day += 7;
                        }
                        else
                        {
                            if (_month + 1 <= 12)
                            {
                                _day = 7 - (daysInMonth - _day);
                                daysInMonth = DateTime.DaysInMonth(_year, ++_month);
                                currentMonthArray = CalendarModel.GetMonthArray(_year, _month);
                                monthMatrix = GetMatrixForMonth(currentMonthArray);
                            }
                            else
                            {
                                if (_year + 1 <= 2028)
                                {
                                    _day = 7 - (daysInMonth - _day);
                                    _month = 1;
                                    daysInMonth = DateTime.DaysInMonth(++_year, _month);
                                    currentMonthArray = CalendarModel.GetMonthArray(_year, _month);
                                    monthMatrix = GetMatrixForMonth(currentMonthArray);
                                }
                            }
                        }
                    }
                    else if (keyInfo.Key == ConsoleKey.W || keyInfo.Key == ConsoleKey.UpArrow)
                    {
                        if (_day - 7 >= 1)
                        {
                            _day -= 7;
                        }
                        else
                        {
                            if (_month - 1 >= 1)
                            {
                                daysInMonth = DateTime.DaysInMonth(_year, --_month);
                                currentMonthArray = CalendarModel.GetMonthArray(_year, _month);
                                monthMatrix = GetMatrixForMonth(currentMonthArray);
                                _day = daysInMonth - (7 - _day);
                            }
                            else
                            {
                                if (_year - 1 >= 2024)
                                {
                                    _month = 12;
                                    daysInMonth = DateTime.DaysInMonth(--_year, _month);
                                    currentMonthArray = CalendarModel.GetMonthArray(_year, _month);
                                    monthMatrix = GetMatrixForMonth(currentMonthArray);
                                    _day = daysInMonth - (7 - _day);
                                }
                            }
                        }
                    }
                    else if (keyInfo.Key == ConsoleKey.Enter)
                    {
                        DateOnly currentDate = ShowMenuToDo(currentMonthArray[_day - 1]);
                        _day = currentDate.Day;
                        _month = currentDate.Month;
                        _year = currentDate.Year;
                        currentMonthArray = CalendarModel.GetMonthArray(_year, _month);
                        monthMatrix = GetMatrixForMonth(currentMonthArray);
                    }
                } while (keyInfo.Key != ConsoleKey.Escape && !_isReminderRun);
            }
        }
        public void MonthInterface(int year, int month) => MonthInterface(year, month, 1);

        private void PrintMenu(string[] menuString, int chosenString)
        {
            for (int i = 0; i < menuString.Length; i++)
            {
                if (i == chosenString)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Gray;
                }
                Console.WriteLine(menuString[i]);
                if (i == chosenString)
                {
                    Console.ResetColor();
                }
            }
        }

        public DateOnly ShowMenuToDo(KeyValuePair<DateOnly, Day> date)
        {

            string[] menuStrings =
            {
                "0. Move to date:",
                "1. Edit;",
                "2. Settings;",
                "Exit"
            };

            var monthMatrix = GetMatrixForMonth(CalendarModel.GetMonthArray(date.Key.Year, date.Key.Month));

            int currentOption = 0;
            ConsoleKeyInfo keyInfo;
            int choice = 0;
            while (true)
            {
                Console.Clear();
                PrintMonth(monthMatrix, date.Key.Day);
                PrintMenu(menuStrings, currentOption);


                keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.S || keyInfo.Key == ConsoleKey.DownArrow)
                {
                    currentOption = currentOption + 1 <= menuStrings.Length - 1 ? currentOption + 1 : 0;
                }
                else if (keyInfo.Key == ConsoleKey.W || keyInfo.Key == ConsoleKey.UpArrow)
                {
                    currentOption = currentOption - 1 >= 0 ? currentOption - 1 : menuStrings.Length - 1;
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    choice = currentOption;
                    break;
                }
            }
            switch (choice)
            {
                case 0:
                    Console.WriteLine("Enter date you want to move (Format: dd.mm.yyyy)");
                    string dateToMove = Console.ReadLine();
                    var splitDateToMove = dateToMove.Split('.');
                    if (splitDateToMove.Length == 3)
                    {
                        int year, month, day;
                        if (int.TryParse(splitDateToMove[2], out year) &&
                            int.TryParse(splitDateToMove[1], out month) &&
                            int.TryParse(splitDateToMove[0], out day) &&
                            year >= 2024 && year <= 2028 &&
                            month >= 1 && month <= 12)
                        {

                            int daysInCurrentMonth = DateTime.DaysInMonth(year, month);
                            if (day <= daysInCurrentMonth)
                                return new DateOnly(year, month, day);

                        }
                    }
                    Console.WriteLine("invalid format of input \nPress any key to continue...");
                    Console.ReadKey();
                    return date.Key;

                case 1:
                    EditMenu(date);
                    return date.Key;
                case 2:
                    SettingsMenu();
                    return date.Key;
                default:
                    return date.Key;
            }
        }

        public void Remind(string message)
        {
            _isReminderRun = true;
            lock (_consoleLock)
            {
                for (int i = 0; i < 3; i++)
                {
                    Console.Clear();
                    string notification =
                        "---------------------------\n" +
                        "|                         |\n" +
                        "|                         |\n" +
                        $" {message} \n" +
                        "|                         |\n" +
                        "|                         |\n" +
                        "---------------------------";
                    Console.WriteLine(notification);
                    Thread.Sleep(200);
                    Console.Clear();
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine(notification);
                    Console.ResetColor();
                    Thread.Sleep(200);

                }

                Console.ReadKey();
                _isReminderRun = false;
                Task monthInterface = new Task(() => MonthInterface(_year, _month, _day));
                monthInterface.Start();

            }
        }

        private void EditMenu(KeyValuePair<DateOnly, Day> date)
        {
            string[] menuStrings =
            {
                "1. Add event;",
                "2. Remove event;",
                "3. Edit event;",
                "Exit"
            };
            bool exit = false;
            int currentOption = 0;
            ConsoleKeyInfo keyInfo;
            int choice = 0;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine(date.Key);

                byte numberEvent = 0;
                if (date.Value.HasTimeBoundEvents)
                {
                    Console.BackgroundColor = ConsoleColor.Magenta;
                    Console.ForegroundColor = ConsoleColor.White;
                    foreach (var tbEvent in date.Value.TimeBoundEvents)
                    {
                        Console.WriteLine($"{++numberEvent}) {tbEvent.Description} ({tbEvent.Start})");
                    }
                    Console.ResetColor();
                }
                if (date.Value.Types[0] != TypeOfDate.Usual)
                {
                    for (int i = 0; i < date.Value.NumberOfEvents; i++)
                    {
                        if (date.Value.Types[i] == TypeOfDate.PersonalEvent)
                        {
                            Console.BackgroundColor = ConsoleColor.DarkMagenta;
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else if (date.Value.Types[i] == TypeOfDate.HolyEvent)
                        {
                            Console.BackgroundColor = ConsoleColor.Yellow;
                            Console.ForegroundColor = ConsoleColor.Black;
                        }
                        else if (date.Value.Types[i] == TypeOfDate.NationalEvent)
                        {
                            Console.BackgroundColor = ConsoleColor.Blue;
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else if (date.Value.Types[i] == TypeOfDate.InternationalEvent)
                        {
                            Console.BackgroundColor = ConsoleColor.DarkGreen;
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else if (date.Value.Types[i] == TypeOfDate.TragicEvent)
                        {
                            Console.BackgroundColor = ConsoleColor.DarkRed;
                            Console.ForegroundColor = ConsoleColor.Black;
                        }
                        Console.WriteLine($"{++numberEvent}) {date.Value.NameOfEvents[i]}");
                    }
                    Console.ResetColor();
                }
                Console.WriteLine();

                PrintMenu(menuStrings, currentOption);


                keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.S || keyInfo.Key == ConsoleKey.DownArrow)
                {
                    currentOption = currentOption + 1 <= menuStrings.Length - 1 ? currentOption + 1 : 0;
                }
                else if (keyInfo.Key == ConsoleKey.W || keyInfo.Key == ConsoleKey.UpArrow)
                {
                    currentOption = currentOption - 1 >= 0 ? currentOption - 1 : menuStrings.Length - 1;
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    choice = currentOption;
                    break;
                }
            }
            switch (choice)
            {
                case 0:
                    Console.WriteLine("Enter name or description for your event:");
                    string? nameOfEvent = Console.ReadLine();
                    OnEventAdded(date.Key.Year, date.Key.Month, date.Key.Day, nameOfEvent);
                    break;
                case 1:
                    OnEventRemoved(date);
                    break;
                case 2:
                    OnEventEdited(date);
                    break;
                default:
                    break;
            }
        }
        private void SettingsMenu()
        {
            string[] menuStrings =
            {
                "1. Return all default events;",
                "2. Reset calendar;",
                "Exit"
            };
            int currentOption = 0;
            ConsoleKeyInfo keyInfo;
            int choice = 0;
            while (true)
            {
                Console.Clear();
                PrintMenu(menuStrings, currentOption);

                keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.S || keyInfo.Key == ConsoleKey.DownArrow)
                {
                    currentOption = currentOption + 1 <= menuStrings.Length - 1 ? currentOption + 1 : 0;
                }
                else if (keyInfo.Key == ConsoleKey.W || keyInfo.Key == ConsoleKey.UpArrow)
                {
                    currentOption = currentOption - 1 >= 0 ? currentOption - 1 : menuStrings.Length - 1;
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    choice = currentOption;
                    break;
                }
            }
            switch (choice)
            {
                case 0:
                    OnDefaultEventsReturned();
                    break;
                case 1:
                    OnCalendarReset();
                    break;
                default:
                    break;
            }
        }
    }
}
