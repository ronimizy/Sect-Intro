using System.Collections.Generic;
using System.IO;
using Core.Entities;

namespace Core.Models
{
    public class FileData
    {
        public List<Group> Groups { get; set; } = new ();
        public List<Task> Tasks { get; set; } = new();
        public int Size { get; set; }

        public FileData()
        {
            
        }

        public FileData(List<Group> groups, List<Task> tasks, int size)
        {
            (Groups, Tasks, Size) = (groups, tasks, size);
        }
    }
}