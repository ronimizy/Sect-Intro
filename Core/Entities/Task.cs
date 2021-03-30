using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models.Exceptions;

namespace Core.Entities
{
    public class Task
    {
        public const int FieldCount = 4;

        public int Id { get; }

        public string Name { get; set; }
        public string Info { get; set; }

        private bool _isCompleted;
        public bool IsCompleted
        {
            get
            {
                return Subtasks.Count == 0 ? _isCompleted : Subtasks.All(s => s.IsCompleted);
            }
            set
            {
                _isCompleted = value;
                Subtasks = Subtasks.Select(s => new Task(s) { IsCompleted = true}).ToList();
            }
        }
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
            int index = Subtasks.FindIndex((task => task.Id == id));
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
            string result = $"Id: {Id}";
            result += Name.Length != 0 ? $", Name: {Name}" : "";
            result += Deadline != null ? $", Deadline: {Deadline}" : "";
            result += Info.Length != 0 ? $", – {Info}" : "";
            result += Subtasks.Count != 0 ? $"({Subtasks.Count(s => s.IsCompleted)}/{Subtasks.Count}:" : "";

            return result;
        }
    }
}