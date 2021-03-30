using System;

namespace Core.Models.Exceptions
{
    public class WrongTaskIdException : Exception
    {
        public WrongTaskIdException()
            : base("There is no task with specified id")
        {
            
        }
    }
}