using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace CalendarScheduler
{
    internal class CalendarUserInterface
    {
        public int?[,] GetMatrixForMonth(DateOnly firstDayOfMonth)
        {
            int daysInMonth = DateTime.DaysInMonth(firstDayOfMonth.Year, firstDayOfMonth.Month);

            int?[,] daysOfMonth = new int?[6, 7];
            int currentDayCounter = 0;
            for (int i = (int)firstDayOfMonth.DayOfWeek; i < 7; i++)
            {
                daysOfMonth[0, i] = ++currentDayCounter;
            }
            for (int i = 1; i < 6; i++)
            {
                for (int j = 0; j < 7 && currentDayCounter < daysInMonth; j++)
                {
                    daysOfMonth[i, j] = ++currentDayCounter;
                }
            }
            return daysOfMonth;
        }
        public KeyValuePair<DateOnly, Day>[][] GetMatrixForMonth(KeyValuePair<DateOnly, Day>[] Month)
        {
            KeyValuePair<DateOnly, Day> firstDayOfMonth = Month[0];
            int daysInMonth = DateTime.DaysInMonth(firstDayOfMonth.Key.Year, firstDayOfMonth.Key.Month);

            var daysOfMonth = new KeyValuePair<DateOnly, Day>[6][];
            int currentDayCounter = 0;
            daysOfMonth[0] = new KeyValuePair<DateOnly, Day>[7 - (int)firstDayOfMonth.Key.DayOfWeek];
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

        public void PrintMonth(DateOnly chosenDay)
        {
            int?[,] daysMatrix = GetMatrixForMonth(new DateOnly(chosenDay.Year, chosenDay.Month, 1));
            Console.Write(chosenDay.Year + " ");
            switch (chosenDay.Month)
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

            Console.WriteLine("Sun" + "\t" + "Mon" + "\t" + "Tus" + "\t" + "Wed" + "\t" + "Thi" + "\t" + "Fri" + "\t" + "Sat");
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (daysMatrix[i, j] == chosenDay.Day)
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    Console.Write(daysMatrix[i, j]);
                    if (daysMatrix[i, j] == chosenDay.Day)
                    {
                        Console.ResetColor();
                    }
                    Console.Write("\t");
                }
                Console.WriteLine();
            }
        }

        public void PrintMonth(KeyValuePair<DateOnly, Day>[][] daysMatrix, int chosenDay)
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

            Console.WriteLine("Sun" + "\t" + "Mon" + "\t" + "Tus" + "\t" + "Wed" + "\t" + "Thi" + "\t" + "Fri" + "\t" + "Sat");
            if (daysMatrix[0].Length > 0)
            {
                for (int i = 0; i < 7 - daysMatrix[0].Length; i++)
                {
                    Console.Write("\t");
                }
            }
            for (int i = 0; i < daysMatrix.Length; i++)
            {
                for (int j = 0; j < daysMatrix[i].Length; j++)
                {
                    if (daysMatrix[i][j].Key.Day == chosenDay)
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    else if (daysMatrix[i][j].Value.Type == TypeOfDate.HolyEvent)
                    {
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    else if (daysMatrix[i][j].Value.Type == TypeOfDate.NationalEvent)
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (daysMatrix[i][j].Value.Type == TypeOfDate.InternationalEvent)
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (daysMatrix[i][j].Value.Type == TypeOfDate.PersonalEvent)
                    {
                        Console.BackgroundColor = ConsoleColor.Magenta;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (daysMatrix[i][j].Value.Type == TypeOfDate.TragicEvent)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    Console.Write(daysMatrix[i][j].Key.Day);
                    Console.ResetColor();
                    Console.Write("\t");
                }
                Console.WriteLine();
            }
        }

        public void MonthInterface(DateOnly highlightedDay)
        {

            PrintMonth(highlightedDay);
            int currerntDay = highlightedDay.Day;

            int daysInMonth = DateTime.DaysInMonth(highlightedDay.Year, highlightedDay.Month);
            ConsoleKeyInfo keyInfo;
            do
            {
                keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.D || keyInfo.Key == ConsoleKey.RightArrow)
                {
                    if (currerntDay + 1 <= daysInMonth)
                    {
                        Console.Clear();
                        PrintMonth(new DateOnly(highlightedDay.Year, highlightedDay.Month, ++currerntDay));
                    }
                }
                else if (keyInfo.Key == ConsoleKey.A || keyInfo.Key == ConsoleKey.LeftArrow)
                {
                    if (currerntDay - 1 >= 1)
                    {
                        Console.Clear();
                        PrintMonth(new DateOnly(highlightedDay.Year, highlightedDay.Month, --currerntDay));
                    }
                }
                else if (keyInfo.Key == ConsoleKey.S || keyInfo.Key == ConsoleKey.DownArrow)
                {
                    if (currerntDay + 7 <= daysInMonth)
                    {
                        currerntDay += 7;
                        Console.Clear();
                        PrintMonth(new DateOnly(highlightedDay.Year, highlightedDay.Month, currerntDay));
                    }
                }
                else if (keyInfo.Key == ConsoleKey.W || keyInfo.Key == ConsoleKey.UpArrow)
                {
                    if (currerntDay - 7 >= 1)
                    {
                        currerntDay -= 7;
                        Console.Clear();
                        PrintMonth(new DateOnly(highlightedDay.Year, highlightedDay.Month, currerntDay));
                    }
                }
                else
                {
                    Console.Clear();
                    PrintMonth(new DateOnly(highlightedDay.Year, highlightedDay.Month, currerntDay));
                }
            } while (keyInfo.Key != ConsoleKey.Escape);
        }
        public void MonthInterface(int year, int month)
        {
            var model = new CalendarModel();
            var monthMatrix = GetMatrixForMonth(model.GetMonthArray(year, month));
            PrintMonth(monthMatrix, 1);
            int currerntDay = 1;

            int daysInMonth = DateTime.DaysInMonth(year, month);
            ConsoleKeyInfo keyInfo;
            do
            {
                keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.D || keyInfo.Key == ConsoleKey.RightArrow)
                {
                    if (currerntDay + 1 <= daysInMonth)
                    {
                        Console.Clear();
                        PrintMonth(monthMatrix, ++currerntDay);
                    }
                    else
                    {
                        if(month + 1 <= 12)
                        {
                            daysInMonth = DateTime.DaysInMonth(year, ++month);
                        }
                        else
                        {
                            month = 1;
                            daysInMonth = DateTime.DaysInMonth(++year, month);

                        }
                        
                        monthMatrix = GetMatrixForMonth(model.GetMonthArray(year, month));
                        currerntDay = 1;
                        Console.Clear();
                        PrintMonth(monthMatrix, currerntDay);
                    }
                }
                else if (keyInfo.Key == ConsoleKey.A || keyInfo.Key == ConsoleKey.LeftArrow)
                {
                    if (currerntDay - 1 >= 1)
                    {
                        Console.Clear();
                        PrintMonth(monthMatrix, --currerntDay);
                    }
                    else
                    {
                        if (month - 1 >= 1)
                        {
                            daysInMonth = DateTime.DaysInMonth(year, --month);
                        }
                        else
                        {
                            month = 12;
                            daysInMonth = DateTime.DaysInMonth(--year, month);
                        }
                        monthMatrix = GetMatrixForMonth(model.GetMonthArray(year, month));
                        currerntDay = daysInMonth;
                        Console.Clear();
                        PrintMonth(monthMatrix, currerntDay);
                    }
                }
                else if (keyInfo.Key == ConsoleKey.S || keyInfo.Key == ConsoleKey.DownArrow)
                {
                    if (currerntDay + 7 <= daysInMonth)
                    {
                        currerntDay += 7;
                        Console.Clear();
                        PrintMonth(monthMatrix, currerntDay);
                    }
                    else
                    {
                        currerntDay = 7 - (daysInMonth - currerntDay);
                        if (month + 1 <= 12)
                        {
                            daysInMonth = DateTime.DaysInMonth(year, ++month);
                        }
                        else
                        {
                            month = 1;
                            daysInMonth = DateTime.DaysInMonth(++year, month);

                        }
                        monthMatrix = GetMatrixForMonth(model.GetMonthArray(year, month));
                        Console.Clear();
                        PrintMonth(monthMatrix, currerntDay);
                    }
                }
                else if (keyInfo.Key == ConsoleKey.W || keyInfo.Key == ConsoleKey.UpArrow)
                {
                    if (currerntDay - 7 >= 1)
                    {
                        currerntDay -= 7;
                        Console.Clear();
                        PrintMonth(monthMatrix, currerntDay);
                    }
                    else
                    {
                        if (month - 1 >= 1)
                        {
                            daysInMonth = DateTime.DaysInMonth(year, --month);
                        }
                        else
                        {
                            month = 12;
                            daysInMonth = DateTime.DaysInMonth(--year, month);
                        }
                        monthMatrix = GetMatrixForMonth(model.GetMonthArray(year, month));
                        currerntDay = daysInMonth-(7 - currerntDay);
                        Console.Clear();
                        PrintMonth(monthMatrix, currerntDay);
                    }
                }
                else
                {
                    Console.Clear();
                    PrintMonth(monthMatrix, currerntDay);
                }
            } while (keyInfo.Key != ConsoleKey.Escape);
        }
    }
}
