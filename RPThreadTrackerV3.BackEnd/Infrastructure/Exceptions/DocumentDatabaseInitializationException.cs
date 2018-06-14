
namespace RPThreadTrackerV3.BackEnd.Infrastructure.Exceptions
{
    using System;

    public class DocumentDatabaseInitializationException : Exception
    {
        public DocumentDatabaseInitializationException(string message, Exception e) 
            : base(message, e)
        {
        }
    }
}
