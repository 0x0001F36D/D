namespace D.Core
{
    using System;

    public sealed class DException : Exception
    {
        public DException(string message) : base(message)
        {
        }
    }
}