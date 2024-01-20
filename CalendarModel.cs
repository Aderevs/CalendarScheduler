using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarScheduler
{
    internal class CalendarModel
    {
        private Day[] dateArray = new Day[365 * 5 + 2];
        public CalendarModel()
        {
            DateTime currentDay;
            TypeOfDate currentDayType;
            for (int i = 0; i < dateArray.Length; i++)
            {
                currentDay = new DateTime(2024, 1, 1).AddDays(i);
                switch (currentDay.DayOfYear)
                {
                    case 1: 
                        currentDayType = TypeOfDate.InternationalEvent;
                        break;

                    default:
                        currentDayType = TypeOfDate.Usual;
                        break;

                }
                dateArray[i] = new Day(currentDay, currentDayType);
            }

        }

        public void PrintAllDays()
        {
            foreach (Day date in dateArray)
            {
                Console.WriteLine(date.Date.ToString("yyyy-MM-dd"));
            }
        }
    }
}
