using System;

namespace ModularisTest.Exceptions
{
    public class JobLoggerNotInitializedException : Exception
    {
        public JobLoggerNotInitializedException() : base("You must create an instance of JobLogger to configure it.")
        {

        }
    }
}