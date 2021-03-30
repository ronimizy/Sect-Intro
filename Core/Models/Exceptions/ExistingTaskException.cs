using System;

namespace Core.Models.Exceptions
{
    public class ExistingTaskException : Exception
    {
        public ExistingTaskException()
            : base("Task with specified name already exists")
        {
            
        }
    }
}