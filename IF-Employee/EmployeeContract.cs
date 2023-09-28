namespace IF_Employee
{
    public class EmployeeContract
    {
        public Employee Employee;

        public DateTime StartDate;

        public DateTime? EndDate;

        public EmployeeContract(Employee employee, DateTime startDate)
        {
            Employee = employee;
            StartDate = startDate;
        }

        public void TerminateContract(DateTime endDate)
        {
            EndDate = endDate;
        }
    }
}
