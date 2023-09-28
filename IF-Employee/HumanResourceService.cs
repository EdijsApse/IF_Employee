using IF_Employee.Errors;
using IF_Employee.Interfaces;

namespace IF_Employee
{
    public class HumanResourceService : IHumanResourceService
    {
        private List<EmployeeContract> _listOfEmployeeContracts;

        private List<EmployeeShiftRecord> _listOfEmployeeShiftRecords;

        public HumanResourceService(List<EmployeeContract> listOfEmployeeContratcs, List<EmployeeShiftRecord> listOfShiftRecords)
        {
            _listOfEmployeeContracts = listOfEmployeeContratcs;
            _listOfEmployeeShiftRecords = listOfShiftRecords;
        }

        public void HireEmployee(Employee employee, DateTime startDate)
        {
            ValidateEmployee(employee);

            ValidateDateIsNotInFuture(startDate);

            if (HasOngoingContract(employee.Id)) throw new EmployeeAlreadyWorkingForCompanyException();

            _listOfEmployeeContracts.Add(new EmployeeContract(employee, startDate));
        }

        public void ReportHours(Employee employee, DateTime shiftStarted, int workedHours, int workedMinutes)
        {
            ValidateEmployee(employee);

            ValidateHours(workedHours);

            ValidateMinutes(workedMinutes);

            var contract = GetEmployeesContract(employee.Id);

            if (contract == null) throw new EmployeeNotWorkingForTheCompanyException();

            ValidateDateIsNotInFuture(shiftStarted);

            if (contract.StartDate > shiftStarted) throw new InvalidDateTimeIntervalException();

            var ShiftRecord = CreateShiftRecord(contract.Employee, shiftStarted, workedHours, workedMinutes);

            _listOfEmployeeShiftRecords.Add(ShiftRecord);
        }

        public void FireEmployee(int employeeId, DateTime endDate)
        {
            var contract = GetEmployeesContract(employeeId);

            if (contract == null) throw new EmployeeNotWorkingForTheCompanyException();

            ValidateDateIsNotInFuture(endDate);

            ValidateContractsEndDateTime(contract.StartDate, endDate);

            contract.TerminateContract(endDate);
        }

        public Employee[] GetWorkingEmployees()
        {
            return _listOfEmployeeContracts
                .Where(contract => contract.EndDate == null)
                .Select(contract => contract.Employee)
                .ToArray();
        }

        public EmployeeMonthlyReport[] GetMonthlyReport(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate) throw new InvalidDateTimeIntervalException();

            var filteredShifts = _listOfEmployeeShiftRecords
            .Where((shiftRecord) =>
            {
                return shiftRecord.StartDate >= startDate && shiftRecord.EndDate <= endDate;
            })
            .ToList();

            return CreateReportFromShiftRecords(filteredShifts);
        }

        private EmployeeShiftRecord CreateShiftRecord(Employee employee, DateTime shiftStartDateTime, int hours, int minutes)
        {
            var shiftEndDate = shiftStartDateTime.AddHours(hours).AddMinutes(minutes);

            if (ShiftWillExistsInGivenTime(employee, shiftStartDateTime)) throw new ShiftRecordExistsForGivenTimePeriod();

            if (ShiftWillExistsInGivenTime(employee, shiftEndDate)) throw new ShiftRecordExistsForGivenTimePeriod();

            return new EmployeeShiftRecord(employee, shiftStartDateTime, shiftEndDate, hours, minutes);
        }

        private bool ShiftWillExistsInGivenTime(Employee employee, DateTime dateTime)
        {
            return _listOfEmployeeShiftRecords.Any(record =>
            {
                return record.Employee == employee && record.StartDate < dateTime && record.EndDate > dateTime;
            });
        }

        private void ValidateHours(int hours)
        {
            if (hours < 0) throw new NegativeNumberNotAllowedException();
        }

        private void ValidateMinutes(int minutes)
        {
            if (minutes < 0) throw new NegativeNumberNotAllowedException();

            if (minutes >= 60) throw new InvalidMinutesException();
        }

        private void ValidateEmployee(Employee employee)
        {
            if (employee == null) throw new EmployeeCannotBeNullException();
        }

        private void ValidateDateIsNotInFuture(DateTime dateTime)
        {
            if (dateTime > DateTime.Now) throw new InvalidDateTimeIntervalException();
        }

        private void ValidateContractsEndDateTime(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate) throw new ContractStartDatePassesEndDateException();
        }

        private bool HasOngoingContract(int employeeId)
        {
            return _listOfEmployeeContracts.Any(contract => contract.Employee.Id == employeeId && contract.EndDate == null);
        }

        private EmployeeContract GetEmployeesContract(int employeeId)
        {
            return _listOfEmployeeContracts.Find(contract => contract.Employee.Id == employeeId && contract.EndDate == null);
        }

        private EmployeeMonthlyReport[] CreateReportFromShiftRecords(List<EmployeeShiftRecord> records)
        {
            var listOfReports = new List<EmployeeMonthlyReport>();

            var groupedShiftRecords = records.GroupBy(shiftRecord =>
            {
                return shiftRecord.StartDate.Month.ToString() + shiftRecord.StartDate.Year.ToString() + shiftRecord.Employee.Id.ToString();
            });

            foreach (var group in groupedShiftRecords)
            {
                var firstRecord = group.First();
                var totalSalary = group.Aggregate(0m, (sum, shiftRecord) =>
                {
                    var salary = CalculatSalary(shiftRecord.Employee.HourlySalary, shiftRecord.HoursWorked, shiftRecord.MinutesWorked);

                    return sum + salary;
                });

                var report = new EmployeeMonthlyReport();

                report.EmployeeId = firstRecord.Employee.Id;
                report.Year = firstRecord.StartDate.Year;
                report.Month = firstRecord.StartDate.Month;
                report.Salary = totalSalary;

                listOfReports.Add(report);
            }

            return listOfReports.ToArray();
        }

        private decimal CalculatSalary(decimal hourlyRate, int hours, int minutes)
        {
            var totalHours = hours + (minutes / 60);

            return totalHours * hourlyRate;
        }
    }
}
