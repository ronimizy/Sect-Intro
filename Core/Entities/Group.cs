using System.Collections.Generic;

namespace Core.Entities
{
    public class Group
    {
        public string Name { get; }
        public List<Task> Tasks { get; }

        public Group(string name, List<Task> tasks)
        {
            Name = name;
            Tasks = tasks;
        }
    }
}