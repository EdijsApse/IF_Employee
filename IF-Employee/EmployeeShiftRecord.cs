namespace IF_Employee
{
    public class EmployeeShiftRecord
    {
        public Employee Employee;

        public DateTime StartDate;

        public DateTime EndDate;

        public int HoursWorked;

        public int MinutesWorked;

        public EmployeeShiftRecord(Employee employee, DateTime shiftStartDate, DateTime shiftEndDate, int hoursWorked, int minutesWorked)
        {
            Employee = employee;
            StartDate = shiftStartDate;
            EndDate = shiftEndDate;
            HoursWorked = hoursWorked;
            MinutesWorked = minutesWorked;
        }
    }
}
