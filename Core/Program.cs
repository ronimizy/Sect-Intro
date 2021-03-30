using System;

namespace Core
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new ToDoApp();
            
            Console.WriteLine("Type \"/help\" to see the list of the commands");
            
            app.Run();
        }
    }
}