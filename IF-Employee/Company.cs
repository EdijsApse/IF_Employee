using IF_Employee.Errors;
using IF_Employee.Interfaces;

namespace IF_Employee
{
    public class Company : ICompany
    {
        private IHumanResourceService _humanResourceService;

        public string Name { get; }

        public Employee[] Employees => _humanResourceService.GetWorkingEmployees();

        public Company(string name, IHumanResourceService humanResourceService)
        {
            Name = name;
            _humanResourceService = humanResourceService;
        }

        public void AddEmployee(Employee employee, DateTime contractStartDate)
        {
            _humanResourceService.HireEmployee(employee, contractStartDate);
        }

        public EmployeeMonthlyReport[] GetMonthlyReport(DateTime periodStartDate, DateTime periodEndDate)
        {
            return _humanResourceService.GetMonthlyReport(periodStartDate, periodEndDate);
        }

        public void RemoveEmployee(int employeeId, DateTime contractEndDate)
        {
            _humanResourceService.FireEmployee(employeeId, contractEndDate);
        }

        public void ReportHours(int employeeId, DateTime dateAndTime, int hours, int minutes)
        {
            var employee = FindEmployeeById(employeeId);

            if (employee == null) throw new EmployeeNotWorkingForTheCompanyException();

            _humanResourceService.ReportHours(employee, dateAndTime, hours, minutes);
        }

        private Employee FindEmployeeById(int id)
        {
            return Employees.FirstOrDefault(employee => employee.Id == id);
        }
    }
}
