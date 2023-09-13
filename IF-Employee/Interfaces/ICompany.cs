namespace IF_Employee.Interfaces
{
    public interface ICompany
    {
        /// <summary>
        /// Name of the company.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// List of the employees that are working for the company.
        /// </summary>
        Employee[] Employees { get; }

        /// <summary>
        /// Adds new employee from the given date. Employee Id must be unique.
        /// </summary>
        /// <param name="employee">Employee to add.</param>
        /// <param name="contractStartDate">Employee work start date and time.</param>
        void AddEmployee(Employee employee, DateTime contractStartDate);

        /// <summary>
        /// Remove employee from the company at the given date.
        /// </summary>
        /// <param name="employeeId">Id of the employee.</param>
        /// <param name="contractEndDate">Employee work end date and time.</param>
        void RemoveEmployee(int employeeId, DateTime contractEndDate);

        /// <summary> 
        /// Report worked time at given day and time. 
        /// If an employee reports 1 hour and 30 minutes at 13:00, it means that the employee was working from 13:00 to 14:30.
        /// </summary>
        /// <param name="employeeId">Id of the employee.</param>
        /// <param name="dateAndTime">Date when work was started.</param>
        /// <param name="hours">Full hours.</param>
        /// <param name="minutes">Full minutes.</param>
        void ReportHours(int employeeId, DateTime dateAndTime, int hours, int minutes);

        /// <summary> 
        /// Get a full report for each employee where data is available for each month. Assume that there is no overtime.
        /// </summary>
        /// <param name="periodStartDate">Report start date.</param>
        /// <param name="periodEndDate">Report end date.</param>
        EmployeeMonthlyReport[] GetMonthlyReport(DateTime periodStartDate, DateTime periodEndDate);
    }
}
