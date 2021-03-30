using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using Core.Models.Exceptions;

namespace Core.Entities
{
    public class TaskDelegate
    {
        public string Path { get; set; } = "";

        private int Size { get; set; }

        private List<Task> Tasks { get; set; }
        public List<Group> Groups { get; private set; }

        public TaskDelegate()
        {
            Tasks = new List<Task>();
            Groups = new List<Group>();
        }

        public void Save()
        {
            if (Path == "")
            {
                Console.WriteLine("No path specified, use \"/path\" command");
            }
            else
            {
                ToDoFile.Save(Tasks, Groups, Size, Path);
            }
        }

        public void Load()
        {
            (Tasks, Groups, Size) = ToDoFile.Load(Path);
        }

        public void Add(string name, string info, DateTime? deadline)
        {
            if (Contains(name))
                throw new ExistingTaskException();

            Tasks.Add(new Task(Size++, name, info, false, deadline, new List<Task>()));
        }

        public void AddSubtask(int id, string info)
        {
            var task = FindTask(t => t.Id == id);

            task?.Item2.AddSubtask(new Task(Size++, "", info, false, null, new List<Task>()));
        }

        public void CreateGroup(string name)
        {
            if (Groups.Find(g => g.Name == name) != null)
                throw new ExistingGroupException();

            Groups.Add(new Group(name, new List<Task>()));
        }

        public void DeleteGroup(string name)
        {
            var deleting = Groups.Find(g => g.Name == name);

            if (deleting == null)
                throw new WrongGroupNameException();

            foreach (var task in deleting.Tasks)
            {
                Tasks.Add(task);
                deleting.Tasks.Remove(task);
            }

            Groups.Remove(deleting);
        }

        public void AddToGroup(string name, int id)
        {
            var address = FindTask(t => t.Id == id);
            var destination = Groups.Find(g => g.Name == name);

            if (address == null)
                throw new WrongTaskIdException();

            if (destination == null)
                throw new WrongGroupNameException();

            destination.Tasks.Add(address.Value.Item2);
            address.Value.Item1.Remove(address.Value.Item2);
        }

        public void DeleteFromGroup(string name, int id)
        {
            var source = Groups.Find(g => g.Name == name);

            if (source == null)
                throw new WrongGroupNameException();

            var deleted = source.Tasks.Find(t => t.Id == id);

            if (deleted == null)
                throw new WrongTaskIdException();

            Tasks.Add(deleted);
            source.Tasks.Remove(deleted);
        }


        public bool Contains(int id)
        {
            return FindTask(t => t.Id == id) != null;
        }

        public bool Contains(string name)
        {
            return FindTask(t => t.Name == name) != null;
        }

        public void Delete(int id)
        {
            var address = FindTask(t => t.Id == id);

            if (address == null)
                throw new WrongTaskIdException();

            address.Value.Item1.Remove(address.Value.Item2);
        }

        public void SetComplete(int id)
        {
            var address = FindTask(t => t.Id == id);

            if (address == null)
                throw new WrongTaskIdException();

            address.Value.Item2.IsCompleted = true;

            address.Value.Item1.Sort((task, task1) =>
            {
                if (task.IsCompleted && task1.IsCompleted)
                    return 0;
                if (task.IsCompleted)
                    return 1;
                if (task1.IsCompleted)
                    return -1;

                return 0;
            });
        }


        public (List<Task>, Task)? FindTask(Func<Task, bool> closure)
        {
            var task = Tasks.Find(t => { return closure(t) || t.Subtasks.Find(s => closure(s)) != null; });

            if (task != null)
            {
                if (closure(task))
                    return (Tasks, task);

                return (task.Subtasks, task.Subtasks.Find(s => closure(s)));
            }

            var group = Groups.Find(g =>
            {
                return g.Tasks.Find(t =>
                {
                    return closure(t) || t.Subtasks.Find(s => closure(s)) != null;
                }) != null;
            });

            if (group == null)
                return null;

            task = group.Tasks.Find(t => closure(t) || t.Subtasks.Find(s => closure(s)) != null);
            return closure(task) ? (group.Tasks, task) : (task?.Subtasks, task?.Subtasks.Find(s => closure(s)));
        }

        public (List<(string, List<Task>)>, List<Task>) Filtered(Func<Task, bool> closure)
        {
            var groups = new List<(string, List<Task>)>();
            var tasks = new List<Task>();

            foreach (var group in Groups)
            {
                var list = new List<Task>();
                foreach (var task in group.Tasks)
                {
                    if (!closure(task))
                        continue;

                    var t = new Task(task);
                    t.Subtasks.Clear();

                    foreach (var subtask in task.Subtasks.Where(subtask => closure(subtask)))
                    {
                        t.Subtasks.Add(subtask);
                    }

                    list.Add(t);
                }

                if (list.Count != 0)
                    groups.Add((group.Name, list));
            }

            foreach (var task in Tasks)
            {
                if (!closure(task))
                    continue;

                var t = new Task(task);
                t.Subtasks.Clear();

                foreach (var subtask in task.Subtasks.Where(subtask => closure(subtask)))
                {
                    t.Subtasks.Add(subtask);
                }
                
                tasks.Add(t);
            }

            return (groups, tasks);
        }
    }
}