using System.Collections.Generic;
using Core.Entities;

namespace Core.Models
{
    public class FoundTask
    {
        public Task Task { get; set; }
        public List<Task> Container { get; set; }

        public FoundTask(List<Task> container, Task task)
        {
            Task = task;
            Container = container;
        }
    }
}