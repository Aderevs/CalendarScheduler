namespace CalendarScheduler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /*CalendarModel model = new CalendarModel();
            model.AddNewEvent(11, 18, "MY BIRTHDAY!!!!");
            model.PrintAllDays();*/
            CalendarUserInterface userInterface = new CalendarUserInterface();
            userInterface.MonthInterface(2024, 12);
        }
    }
}

