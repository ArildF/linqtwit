using System;

namespace LinqTwit.Linq
{
    public class InvalidQueryException : Exception
    {
        public InvalidQueryException(string message) : base(message)
        {
            
        }
    }
}