using System;
using System.Collections.Generic;
using Core.Entities;
using Core.Models;
using Core.Models.Exceptions;

namespace Core
{
    public class ToDoApp
    {
        private TaskDelegate TaskDelegate { get; set; }
        private bool _working = true;

        public ToDoApp()
        {
            TaskDelegate = new TaskDelegate();
        }

        public void Run()
        {
            while (_working)
            {
                string command = Console.ReadLine() ?? "";
                ConsoleResponse response;
                try
                {
                    response = CommandLineParser.Parse(command);
                }
                catch
                {
                    Console.WriteLine("Invalid argument");
                    if (ConsoleResponse.Help.ContainsKey(command.Split(" ")[0].Substring(1)))
                        Console.WriteLine(ConsoleResponse.Help[command.Split(" ")[0].Substring(1)]);
                    
                    continue;
                }

                try
                {
                    switch (response.Action)
                    {
                        case ConsoleResponse.Actions.Add:
                            TaskDelegate.AddTask(
                                response.Name,
                                response.Info,
                                response.Deadline);
                            break;

                        case ConsoleResponse.Actions.AddSubtask:
                            TaskDelegate.AddSubtask(response.Id, response.Info);
                            break;

                        case ConsoleResponse.Actions.Delete:
                            TaskDelegate.DeleteTask(response.Id);
                            break;


                        case ConsoleResponse.Actions.CreateGroup:
                            TaskDelegate.CreateGroup(response.Name);
                            break;

                        case ConsoleResponse.Actions.DeleteGroup:
                            TaskDelegate.DeleteGroup(response.Name);
                            break;

                        case ConsoleResponse.Actions.AddToGroup:
                            TaskDelegate.AddToGroup(response.Name, response.Id);
                            break;

                        case ConsoleResponse.Actions.DeleteFromGroup:
                            TaskDelegate.DeleteFromGroup(response.Name, response.Id);
                            break;


                        case ConsoleResponse.Actions.All:
                            var filteredResult = TaskDelegate.Filtered(_ => true);

                            Print(filteredResult);

                            break;

                        case ConsoleResponse.Actions.Today:
                            filteredResult = TaskDelegate.Filtered(t =>
                                t.Deadline != null && (t.Deadline ?? DateTime.Now).Date == DateTime.Now.Date);

                            Print(filteredResult);

                            break;


                        case ConsoleResponse.Actions.Save:
                            if (response.Path != "")
                                TaskDelegate.Path = response.Path;
                            
                            TaskDelegate.Save();
                            break;

                        case ConsoleResponse.Actions.Load:
                            TaskDelegate.Path = response.Path;
                            TaskDelegate.Load();

                            break;

                        case ConsoleResponse.Actions.Path:
                            Console.WriteLine(TaskDelegate.Path);
                            break;


                        case ConsoleResponse.Actions.Complete:
                            TaskDelegate.SetComplete(response.Id);
                            break;

                        case ConsoleResponse.Actions.Completed:
                            filteredResult = TaskDelegate.Filtered(t => t.IsCompleted);

                            Print(filteredResult);
                        
                            break;

                        case ConsoleResponse.Actions.CompletedGroup:
                            Group? group = TaskDelegate.Groups.Find(g => g.Name == response.Name);
                        
                            if (group == null)
                                Console.WriteLine(new WrongGroupNameException().Message);

                            var data = new FilteredData()
                            {
                                Groups = new List<(string name, List<Task> tasks)>
                                {
                                    new (group!.Name, group.Tasks.FindAll(t => t.IsCompleted))
                                }
                            };
                            
                            Print(data);

                            break;


                        case ConsoleResponse.Actions.Exit:
                            _working = false;
                            break;

                        case ConsoleResponse.Actions.Help:
                            if (response.HelpWith == "")
                            {
                                foreach (var pair in ConsoleResponse.Help)
                                {
                                    Console.WriteLine($"/{pair.Key} {pair.Value}");
                                }

                                break;
                            }

                            if (ConsoleResponse.Help.ContainsKey(response.HelpWith))
                            {
                                Console.WriteLine($"/{response.HelpWith} {ConsoleResponse.Help[response.HelpWith]}");
                                break;
                            }

                            Console.WriteLine("No such command");

                            break;

                        case ConsoleResponse.Actions.None:
                            Console.WriteLine("This command doesn't exist, try \"/help\" to see the list of commands!");
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private static void Print(FilteredData data)
        {
            foreach (var group in data.Groups)
            {
                Console.WriteLine(group.Item1 + ":");

                foreach (var task in group.Item2)
                {
                    Console.ForegroundColor = task.IsCompleted ? ConsoleColor.Green : ConsoleColor.DarkYellow;
                    if (task.Deadline != null && task.Deadline.Value < DateTime.Now && !task.IsCompleted)
                        Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine("\t" + task.StringRepresentation());

                    foreach (var subtask in task.Subtasks)
                    {
                        Console.WriteLine("\t\t" + subtask.StringRepresentation());
                    }
                }
            }

            foreach (var task in data.Tasks)
            {
                Console.ForegroundColor = task.IsCompleted ? ConsoleColor.Green : ConsoleColor.DarkYellow;
                if (task.Deadline != null && task.Deadline.Value < DateTime.Now)
                    Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine(task.StringRepresentation());

                foreach (var subtask in task.Subtasks)
                {
                    Console.WriteLine("\t" + subtask.StringRepresentation());
                }
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }
    }
}