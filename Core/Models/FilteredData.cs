using System.Collections.Generic;
using Core.Entities;

namespace Core.Models
{
    public class FilteredData
    {
        public List<(string name, List<Task> tasks)> Groups { get; set; } = new();
        public List<Task> Tasks { get; set; } = new();
    }
}