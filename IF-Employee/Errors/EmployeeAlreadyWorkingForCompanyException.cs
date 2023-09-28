namespace IF_Employee.Errors
{
    public class EmployeeAlreadyWorkingForCompanyException : Exception
    {
        public EmployeeAlreadyWorkingForCompanyException() : base("Employee is already working for company!") { }
    }
}
