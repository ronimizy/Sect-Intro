using System;
using System.Collections.Generic;
using Core.Models.Exceptions;

namespace Core.Entities
{
    public class Task
    {
        public const int FieldCount = 4;

        public int Id { get; }

        public string Name { get; set; }
        public string Info { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? Deadline { get; set; }
        public List<Task> Subtasks { get; set; }
        
        public bool IsSubtask { get; set; }

        public Task(int id, string name, string info, bool isCompleted, DateTime? deadline, List<Task> subtasks)
        {
            Id = id;
            Name = name;
            Info = info;
            IsCompleted = isCompleted;
            Deadline = deadline;
            Subtasks = subtasks;
        }

        public Task(Task task)
        {
            Id = task.Id;
            Name = task.Name;
            Info = task.Info;
            IsCompleted = task.IsCompleted;
            Deadline = task.Deadline;
            Subtasks = task.Subtasks;
        }

        public void CompleteSubTask(int id)
        {
            var index = Subtasks.FindIndex((task => task.Id == id));
            if (index != -1)
                Subtasks[index].IsCompleted = true;
        }

        public void AddSubtask(Task subtask)
        {
            if (IsSubtask)
                throw new SubtaskNestingException();

            subtask.IsSubtask = true;
            Subtasks.Add(subtask);
        }
        
        public string StringRepresentation()
        {
            var result = $"Id: {Id}";
            result += Name.Length != 0 ? $", Name: {Name}" : "";
            result += Deadline != null ? $", Deadline: {Deadline}" : "";
            result += Info.Length != 0 ? $", – {Info}" : "";
            result += Subtasks.Count != 0 ? ":" : "";

            return result;
        }
    }
}