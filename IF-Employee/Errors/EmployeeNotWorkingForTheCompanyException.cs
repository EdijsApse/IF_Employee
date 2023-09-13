namespace IF_Employee.Errors
{
    public class EmployeeNotWorkingForTheCompanyException : Exception
    {
        public EmployeeNotWorkingForTheCompanyException() : base("Employee not working for the company") { }
    }
}
