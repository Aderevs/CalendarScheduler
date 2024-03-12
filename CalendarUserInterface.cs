using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CalendarScheduler
{
    //delegate void AddEvent(int year, int month, int day, string nameOfEvent);
    //delegate void RemoveEvent(DateOnly date);

    internal class CalendarUserInterface
    {
        public CalendarModel CalenderModel { get; set; }
        public CalendarUserInterface(CalendarModel calenderModel)
        {
            CalenderModel = calenderModel;
        }

        public event Action<int, int, int, string> OnEventAdded;
        public event Action<DateOnly> OnEventRemoved;
        public event Action OnCalendarReseted;
        public event Action OnDefaultEventsReturned;
        public event Action OnEventEdited;
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
                daysOfMonth[0][i] = Month.Where(day => day.Key.Day == currentDayCounter).First();
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
                    daysOfMonth[i][j] = Month.Where(day => day.Key.Day == currentDayCounter).First();
                }
            }
            return daysOfMonth;
        }
        private void PrintMonth(KeyValuePair<DateOnly, Day>[][] daysMatrix, int chosenDay)
        {
            Console.Write(daysMatrix[0][0].Key.Year + " ");
            switch (daysMatrix[0][0].Key.Month)
            {
                case 1:
                    Console.WriteLine("January");
                    break;
                case 2:
                    Console.WriteLine("February");
                    break;
                case 3:
                    Console.WriteLine("March");
                    break;
                case 4:
                    Console.WriteLine("April");
                    break;
                case 5:
                    Console.WriteLine("May");
                    break;
                case 6:
                    Console.WriteLine("June");
                    break;
                case 7:
                    Console.WriteLine("July");
                    break;
                case 8:
                    Console.WriteLine("August");
                    break;
                case 9:
                    Console.WriteLine("September");
                    break;
                case 10:
                    Console.WriteLine("October");
                    break;
                case 11:
                    Console.WriteLine("November");
                    break;
                case 12:
                    Console.WriteLine("December");
                    break;
            }

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
                    int lastIndex = daysMatrix[i][j].Value.Type.Count - 1;
                    if (daysMatrix[i][j].Key.Day == chosenDay)
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                        dayToWrite = daysMatrix[i][j].Value;
                    }
                    else if (daysMatrix[i][j].Value.Type[lastIndex] == TypeOfDate.HolyEvent)
                    {
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    else if (daysMatrix[i][j].Value.Type[lastIndex] == TypeOfDate.NationalEvent)
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (daysMatrix[i][j].Value.Type[lastIndex] == TypeOfDate.InternationalEvent)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (daysMatrix[i][j].Value.Type[lastIndex] == TypeOfDate.PersonalEvent)
                    {
                        Console.BackgroundColor = ConsoleColor.Magenta;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (daysMatrix[i][j].Value.Type[lastIndex] == TypeOfDate.TragicEvent)
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

            if (dayToWrite.Type[0] != TypeOfDate.Usual)
            {
                for (int i = 0; i < dayToWrite.NumberOfEvents; i++)
                {
                    if (dayToWrite.Type[i] == TypeOfDate.HolyEvent)
                    {
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    else if (dayToWrite.Type[i] == TypeOfDate.NationalEvent)
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (dayToWrite.Type[i] == TypeOfDate.InternationalEvent)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (dayToWrite.Type[i] == TypeOfDate.PersonalEvent)
                    {
                        Console.BackgroundColor = ConsoleColor.Magenta;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (dayToWrite.Type[i] == TypeOfDate.TragicEvent)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    Console.WriteLine(dayToWrite.NameOfEvents[i]);
                }
                Console.ResetColor();
            }
        }
        public void MonthInterface(int year, int month, int day)
        {
            var currentMonthArray = CalenderModel.GetMonthArray(year, month);
            var monthMatrix = GetMatrixForMonth(currentMonthArray);
            int daysInMonth = DateTime.DaysInMonth(year, month);

            ConsoleKeyInfo keyInfo;
            do
            {
                Console.Clear();
                PrintMonth(monthMatrix, day);
                keyInfo = Console.ReadKey();

                if (keyInfo.Key == ConsoleKey.D || keyInfo.Key == ConsoleKey.RightArrow)
                {
                    if (day + 1 <= daysInMonth)
                    {
                        day++;
                    }
                    else
                    {

                        if (month + 1 <= 12)
                        {
                            daysInMonth = DateTime.DaysInMonth(year, ++month);
                            day = 1;
                            currentMonthArray = CalenderModel.GetMonthArray(year, month);
                            monthMatrix = GetMatrixForMonth(currentMonthArray);
                        }
                        else
                        {
                            if (year + 1 <= 2028)
                            {
                                month = 1;
                                daysInMonth = DateTime.DaysInMonth(++year, month);
                                day = 1;
                                currentMonthArray = CalenderModel.GetMonthArray(year, month);
                                monthMatrix = GetMatrixForMonth(currentMonthArray);
                            }
                        }
                    }
                }
                else if (keyInfo.Key == ConsoleKey.A || keyInfo.Key == ConsoleKey.LeftArrow)
                {
                    if (day - 1 >= 1)
                    {
                        day--;
                    }
                    else
                    {
                        if (month - 1 >= 1)
                        {
                            daysInMonth = DateTime.DaysInMonth(year, --month);
                            day = daysInMonth;
                            currentMonthArray = CalenderModel.GetMonthArray(year, month);
                            monthMatrix = GetMatrixForMonth(currentMonthArray);
                        }
                        else
                        {
                            if (year - 1 >= 2024)
                            {
                                month = 12;
                                daysInMonth = DateTime.DaysInMonth(--year, month);
                                currentMonthArray = CalenderModel.GetMonthArray(year, month);
                                monthMatrix = GetMatrixForMonth(currentMonthArray);
                                day = daysInMonth;
                            }
                        }
                    }
                }
                else if (keyInfo.Key == ConsoleKey.S || keyInfo.Key == ConsoleKey.DownArrow)
                {
                    if (day + 7 <= daysInMonth)
                    {
                        day += 7;
                    }
                    else
                    {
                        if (month + 1 <= 12)
                        {
                            day = 7 - (daysInMonth - day);
                            daysInMonth = DateTime.DaysInMonth(year, ++month);
                            currentMonthArray = CalenderModel.GetMonthArray(year, month);
                            monthMatrix = GetMatrixForMonth(currentMonthArray);
                        }
                        else
                        {
                            if (year + 1 <= 2028)
                            {
                                day = 7 - (daysInMonth - day);
                                month = 1;
                                daysInMonth = DateTime.DaysInMonth(++year, month);
                                currentMonthArray = CalenderModel.GetMonthArray(year, month);
                                monthMatrix = GetMatrixForMonth(currentMonthArray);
                            }
                        }
                    }
                }
                else if (keyInfo.Key == ConsoleKey.W || keyInfo.Key == ConsoleKey.UpArrow)
                {
                    if (day - 7 >= 1)
                    {
                        day -= 7;
                    }
                    else
                    {
                        if (month - 1 >= 1)
                        {
                            daysInMonth = DateTime.DaysInMonth(year, --month);
                            currentMonthArray = CalenderModel.GetMonthArray(year, month);
                            monthMatrix = GetMatrixForMonth(currentMonthArray);
                            day = daysInMonth - (7 - day);
                        }
                        else
                        {
                            if (year - 1 >= 2024)
                            {
                                month = 12;
                                daysInMonth = DateTime.DaysInMonth(--year, month);
                                currentMonthArray = CalenderModel.GetMonthArray(year, month);
                                monthMatrix = GetMatrixForMonth(currentMonthArray);
                                day = daysInMonth - (7 - day);
                            }
                        }
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    DateOnly currentDate = ShowMenuToDo(currentMonthArray[day - 1]);
                    day = currentDate.Day;
                    month = currentDate.Month;
                    year = currentDate.Year;
                    currentMonthArray = CalenderModel.GetMonthArray(year, month);
                    monthMatrix = GetMatrixForMonth(currentMonthArray);
                }
            } while (keyInfo.Key != ConsoleKey.Escape);
        }
        public void MonthInterface(int year, int month) => MonthInterface(year, month, 1);

        private void PrintMenu(string[] menuString, int choosenString)
        {
            for (int i = 0; i < menuString.Length; i++)
            {
                if (i == choosenString)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Gray;
                }
                Console.WriteLine(menuString[i]);
                if (i == choosenString)
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

            var monthMatrix = GetMatrixForMonth(CalenderModel.GetMonthArray(date.Key.Year, date.Key.Month));

            bool exit = false;
            int currentOprtion = 0;
            ConsoleKeyInfo keyInfo;
            int choice = 0;
            while (!exit)
            {
                Console.Clear();
                PrintMonth(monthMatrix, date.Key.Day);
                PrintMenu(menuStrings, currentOprtion);


                keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.S || keyInfo.Key == ConsoleKey.DownArrow)
                {
                    currentOprtion = currentOprtion + 1 <= menuStrings.Length - 1 ? currentOprtion + 1 : 0;
                }
                else if (keyInfo.Key == ConsoleKey.W || keyInfo.Key == ConsoleKey.UpArrow)
                {
                    currentOprtion = currentOprtion - 1 >= 0 ? currentOprtion - 1 : menuStrings.Length - 1;
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    choice = currentOprtion;
                    break;
                }
            }
            switch (choice)
            {
                case 0:
                    Console.WriteLine("Enter date you want to move (Format: dd.mm.yyyy)");
                    string dateToMove = Console.ReadLine();
                    var splitedDateToMove = dateToMove.Split('.');
                    if (splitedDateToMove.Length == 3)
                    {
                        int year, month, day;
                        if (int.TryParse(splitedDateToMove[2], out year) &&
                            int.TryParse(splitedDateToMove[1], out month) &&
                            int.TryParse(splitedDateToMove[0], out day) &&
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

        private void EditMenu(KeyValuePair<DateOnly, Day> date)
        {
            string[] menuStrings =
            {
                "1. Add event;",
                "1. Remove event;",
                "2. Edit event;",
                "Exit"
            };
            bool exit = false;
            int currentOprtion = 0;
            ConsoleKeyInfo keyInfo;
            int choice = 0;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine(date.Key);
                PrintMenu(menuStrings, currentOprtion);


                keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.S || keyInfo.Key == ConsoleKey.DownArrow)
                {
                    currentOprtion = currentOprtion + 1 <= menuStrings.Length - 1 ? currentOprtion + 1 : 0;
                }
                else if (keyInfo.Key == ConsoleKey.W || keyInfo.Key == ConsoleKey.UpArrow)
                {
                    currentOprtion = currentOprtion - 1 >= 0 ? currentOprtion - 1 : menuStrings.Length - 1;
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    choice = currentOprtion;
                    break;
                }
            }
            switch (choice)
            {
                case 0:
                    Console.WriteLine("Enter name of your event:");
                    string? nameOfEvent = Console.ReadLine();
                    OnEventAdded(date.Key.Year, date.Key.Month, date.Key.Day, nameOfEvent);
                    break;
                case 1:
                    OnEventRemoved(date.Key);
                    break;
                case 2:
                    OnEventEdited();
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
            bool exit = false;
            int currentOprtion = 0;
            ConsoleKeyInfo keyInfo;
            int choice = 0;
            while (!exit)
            {
                Console.Clear();
                PrintMenu(menuStrings, currentOprtion);

                keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.S || keyInfo.Key == ConsoleKey.DownArrow)
                {
                    currentOprtion = currentOprtion + 1 <= menuStrings.Length - 1 ? currentOprtion + 1 : 0;
                }
                else if (keyInfo.Key == ConsoleKey.W || keyInfo.Key == ConsoleKey.UpArrow)
                {
                    currentOprtion = currentOprtion - 1 >= 0 ? currentOprtion - 1 : menuStrings.Length - 1;
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    choice = currentOprtion;
                    break;
                }
            }
            switch (choice)
            {
                case 0:
                    OnDefaultEventsReturned();
                    break;
                case 1:
                    OnCalendarReseted();
                    break;
                default:
                    break;
            }
        }
    }
}
