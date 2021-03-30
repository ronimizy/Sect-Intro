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
                ToDoFile.Save(new FileData(Groups, Tasks, Size), Path);
            }
        }

        public void Load()
        {
            FileData data = ToDoFile.Load(Path);
            (Tasks, Groups, Size) = (data.Tasks, data.Groups, data.Size);
        }

        public void AddTask(string name, string info, DateTime? deadline)
        {
            if (Contains(name))
                throw new ExistingTaskException();

            Tasks.Add(new Task(Size++, name, info, false, deadline, new List<Task>()));
        }

        public void DeleteTask(int id)
        {
            FoundTask? address = FindTask(t => t.Id == id);

            if (address == null)
                throw new WrongTaskIdException();

            address.Container.Remove(address.Task);
        }
        
        public void SetComplete(int id)
        {
            FoundTask? address = FindTask(t => t.Id == id);

            if (address == null)
                throw new WrongTaskIdException();

            address.Task.IsCompleted = true;

            address.Container.Sort((task, task1) =>
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
        
        public void AddSubtask(int id, string info)
        {
            FoundTask? task = FindTask(t => t.Id == id);

            if (task == null)
                throw new WrongTaskIdException();

            task.Task.AddSubtask(new Task(Size++, "", info, false, null, new List<Task>()));
        }

        
        public void CreateGroup(string name)
        {
            if (Groups.Find(g => g.Name == name) != null)
                throw new ExistingGroupException();

            Groups.Add(new Group(name, new List<Task>()));
        }

        public void DeleteGroup(string name)
        {
            Group? deleting = Groups.Find(g => g.Name == name);

            if (deleting == null)
                throw new WrongGroupNameException();

            Tasks.AddRange(deleting.Tasks);
            deleting.Tasks.Clear();

            Groups.Remove(deleting);
        }

        public void AddToGroup(string name, int id)
        {
            FoundTask? taskData = FindTask(t => t.Id == id);
            Group? destination = Groups.Find(g => g.Name == name);

            if (taskData == null)
                throw new WrongTaskIdException();

            if (destination == null)
                throw new WrongGroupNameException();

            destination.Tasks.Add(taskData.Task);
            taskData.Container.Remove(taskData.Task);
        }

        public void DeleteFromGroup(string name, int id)
        {
            Group? source = Groups.Find(g => g.Name == name);

            if (source == null)
                throw new WrongGroupNameException();

            Task? deleted = source.Tasks.Find(t => t.Id == id);

            if (deleted == null)
                throw new WrongTaskIdException();

            Tasks.Add(deleted);
            source.Tasks.Remove(deleted);
        }


        public bool Contains(int id)
        {
            return FindTask(t => t.Id == id) != null;
        }

        private bool Contains(string name)
        {
            return FindTask(t => t.Name == name) != null;
        }
        

        private FoundTask? FindTask(Func<Task, bool> closure)
        {
            if (Groups.Any(g => g.Tasks.Any(closure)))
            {
                Group group = Groups.First(g => g.Tasks.Any(closure));

                return new FoundTask(group.Tasks, group.Tasks.First(closure));
            }

            if (Groups.Any(g => g.Tasks.Any(t => t.Subtasks.Any(closure))))
            {
                Group group = Groups.First(g => g.Tasks.Any(t => t.Subtasks.Any(closure)));
                Task task = group.Tasks.First(t => t.Subtasks.Any(closure));
                Task subtask = task.Subtasks.First(closure);

                return new FoundTask(task.Subtasks, subtask);
            }

            if (Tasks.Any(closure))
            {
                return new FoundTask(Tasks, Tasks.First(closure));
            }

            if (Tasks.Any(t => t.Subtasks.Any(closure)))
            {
                Task task = Tasks.First(t => t.Subtasks.Any(closure));
                Task subtask = task.Subtasks.First(closure);

                return new FoundTask(task.Subtasks, subtask);
            }

            return null;
        }

        public FilteredData Filtered(Func<Task, bool> closure)
        {
            var data = new FilteredData
            {
                Groups = Groups
                    .Where(g => g.Tasks.Any(closure))
                    .Select(g => (g.Name, g.Tasks.Where(closure).ToList())).ToList(),
                Tasks = Tasks
                    .Where(closure).ToList()
            };


            return data;
        }
    }
}