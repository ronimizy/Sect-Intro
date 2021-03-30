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
        private bool Working = true;

        public ToDoApp()
        {
            TaskDelegate = new TaskDelegate();
        }

        public void Run()
        {
            while (Working)
            {
                var command = Console.ReadLine();
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

                switch (response.Action)
                {
                    case ConsoleResponse.Actions.Add:
                        try
                        {
                            TaskDelegate.Add(
                                response.Name,
                                response.Info,
                                response.Deadline);
                        }
                        catch (ExistingTaskException e)
                        {
                            Console.WriteLine(e.Message);
                        }

                        break;

                    case ConsoleResponse.Actions.AddSubtask:
                        try
                        {
                            TaskDelegate.AddSubtask(response.Id, response.Info);
                        }
                        catch (SubtaskNestingException e)
                        {
                            Console.WriteLine(e.Message);
                        }

                        break;

                    case ConsoleResponse.Actions.Delete:
                        try
                        {
                            TaskDelegate.Delete(response.Id);
                        }
                        catch (WrongTaskIdException e)
                        {
                            Console.WriteLine(e.Message);
                        }

                        break;


                    case ConsoleResponse.Actions.CreateGroup:
                        try
                        {
                            TaskDelegate.CreateGroup(response.Name);
                        }
                        catch (ExistingGroupException e)
                        {
                            Console.WriteLine(e);
                        }

                        break;

                    case ConsoleResponse.Actions.DeleteGroup:
                        try
                        {
                            TaskDelegate.DeleteGroup(response.Name);
                        }
                        catch (WrongGroupNameException e)
                        {
                            Console.WriteLine(e.Message);
                        }

                        break;

                    case ConsoleResponse.Actions.AddToGroup:
                        try
                        {
                            TaskDelegate.AddToGroup(response.Name, response.Id);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                        break;

                    case ConsoleResponse.Actions.DeleteFromGroup:
                        try
                        {
                            TaskDelegate.DeleteFromGroup(response.Name, response.Id);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                        break;


                    case ConsoleResponse.Actions.All:
                        var filteredResult = TaskDelegate.Filtered(t => true);

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

                        try
                        {
                            TaskDelegate.Save();
                        }
                        catch
                        {
                            Console.WriteLine($"Saving at {TaskDelegate.Path} failed");
                        }

                        break;

                    case ConsoleResponse.Actions.Load:
                        TaskDelegate.Path = response.Path;

                        try
                        {
                            TaskDelegate.Load();
                        }
                        catch
                        {
                            Console.WriteLine($"Loading from {TaskDelegate.Path} failed");
                        }

                        break;

                    case ConsoleResponse.Actions.Path:
                        if (response.Path == "")
                        {
                            Console.WriteLine(TaskDelegate.Path);
                        }
                        else
                        {
                            TaskDelegate.Path = response.Path;
                        }

                        break;


                    case ConsoleResponse.Actions.Complete:
                        try
                        {
                            TaskDelegate.SetComplete(response.Id);
                        }
                        catch (WrongTaskIdException e)
                        {
                            Console.WriteLine(e.Message);
                            throw;
                        }
                        break;

                    case ConsoleResponse.Actions.Completed:
                        filteredResult = TaskDelegate.Filtered(t => t.IsCompleted);

                        Print(filteredResult);
                        
                        break;

                    case ConsoleResponse.Actions.CompletedGroup:
                        var group = TaskDelegate.Groups.Find(g => g.Name == response.Name);
                        
                        if (group == null)
                            Console.WriteLine(new WrongGroupNameException().Message);

                        Print((new List<(string, List<Task>)>
                        {
                            new (group.Name, group.Tasks.FindAll(t => t.IsCompleted))
                        }, new List<Task>()));

                        break;


                    case ConsoleResponse.Actions.Exit:
                        Working = false;
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
        }

        private static void Print((List<(string, List<Task>)>, List<Task>) result)
        {
            foreach (var group in result.Item1)
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

            foreach (var task in result.Item2)
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