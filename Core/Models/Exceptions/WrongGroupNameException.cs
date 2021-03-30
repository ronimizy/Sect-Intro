using System;

namespace Core.Models.Exceptions
{
    public class WrongGroupNameException : Exception
    {
        public WrongGroupNameException()
            : base("There is no group with specified name")
        {
        }
    }
}