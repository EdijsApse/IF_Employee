namespace IF_Employee.Errors
{
    public class EmployeeCannotBeNullException : Exception
    {
        public EmployeeCannotBeNullException() : base("Employee cannot be null!") { }
    }
}
