namespace IF_Employee.Interfaces
{
    public interface IHumanResourceService
    {
        public void HireEmployee(Employee employee, DateTime startDate);

        public void FireEmployee(int employeeId, DateTime endDate);

        public void ReportHours(Employee employee, DateTime shiftStarted, int workedHours, int workedMinutes);

        public Employee[] GetWorkingEmployees();

        EmployeeMonthlyReport[] GetMonthlyReport(DateTime startDate, DateTime endDate);
    }
}
