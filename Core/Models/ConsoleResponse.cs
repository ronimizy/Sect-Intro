using System;
using System.Collections.Generic;

namespace Core.Models
{
    public class ConsoleResponse
    {
        public enum Actions
        {
            Add,
            AddSubtask,
            Delete,

            CreateGroup,
            DeleteGroup,
            AddToGroup,
            DeleteFromGroup,

            All,
            Today,

            Save,
            Load,
            Path,

            Complete,
            Completed,
            CompletedGroup,

            Exit,
            Help,
            None,
        }

        public static readonly Dictionary<string, string> Help = new ()
        {
            {
                "add",
                "[task name] -i [task info(optional)] -f [date-time format string(a must if deadline specified)] -d [deadline date-time string(optional)]\n\t" +
                "Creates task with unique specified name\n\t" +
                "example: \"/add name -i info -f ddmmyyyy -d 20202021"
            },
            {
                "add-subtask", "-i [subtask info(optional)]\n\t" +
                               "Adds a subtask with given info to the task with specified id"
            },
            {
                "delete", "[id] \n\t" +
                          "Removes a task with specified Id, try \"/all\" to see the full list of tasks"
            },

            {
                "create-group", "[group name] \n\t" +
                                "Creates group with unique specified name"
            },
            {
                "delete-group", "[group name] \n\t" +
                                "Deletes group with specified name, all its nested tasks will be moved into main tasks list"
            },
            {
                "add-to-group", "[task id] [group name] \n\t" +
                                "Adds an existing task to the group with specified name"
            },
            {
                "delete-from-group", "[task id] [group name]\n\t" +
                                     "Deletes task with specified id from group with specified name"
            },

            {
                "all", "[no args]\n\t" +
                       "Prints all current tasks"
            },
            {
                "today", "[no args] \n\t" +
                         "Prints all tasks which deadline is today"
            },


            {
                "save", "[path(optional)] \n\t" +
                        $"Saves current ToDo list at given directory (with filename.{ToDoFile.Extension}, \n\t" +
                        "if no path was given, file will be saved to the last entered directory"
            },
            {
                "load", $"[path] \n\t" +
                        $"Loads ToDo list from \".{ToDoFile.Extension}\" file"
            },
            {
                "path", $"[path(optional)] \n\t" +
                        $"Specifies a new path (must end with .{ToDoFile.Extension} file, if no path was given prints a current path"
            },
            
            
            {
                "complete", "[id] \n\t" +
                            "Marks a task with given Id as completed, try \"/all\" to see the full list of tasks"
            },
            {
                "completed", "[group name(optional)] \n\t" +
                             "If group name specified, prints all completed tasks from it, if not, \n\t" +
                             "prints all completed tasks, try \"/all\" to see the full list of tasks"
            },

            {
                "help", "[no args] \n\t" +
                        "Specify a command to get description ex:\"/help path\""
            },
            {
                "exit", "[no args] \n\t" +
                        "Closes the program"
            },
        };

        public Actions Action { get; }

        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Info { get; set; } = "";
        public DateTime? Deadline;

        public string Path { get; set; } = "";

        public string HelpWith { get; set; } = "";

        public ConsoleResponse(Actions action)
        {
            Action = action;
        }
    }
}