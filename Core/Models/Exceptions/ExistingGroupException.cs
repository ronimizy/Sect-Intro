using System;

namespace Core.Models.Exceptions
{
    public class ExistingGroupException : Exception
    {
        public ExistingGroupException()
            : base("Group with specified name already exists")
        {
            
        }
    }
}