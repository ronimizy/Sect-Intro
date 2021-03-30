using System;

namespace Core.Models.Exceptions
{
    public class SubtaskNestingException : Exception
    {
        public SubtaskNestingException()
            : base("Subtask nesting is not supported")
        {
            
        }
    }
}