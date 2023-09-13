namespace IF_Employee.Errors
{
    public class InvalidMinutesException : Exception
    {
        public InvalidMinutesException() : base("Invalid minutes given!") {}
    }
}
