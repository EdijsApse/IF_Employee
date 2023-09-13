namespace IF_Employee.Errors
{
    public class NegativeNumberNotAllowedException : Exception
    {
        public NegativeNumberNotAllowedException() : base("Negative numeric values are not allowed") { }
    }
}
