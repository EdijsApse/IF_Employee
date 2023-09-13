using FluentAssertions;
using IF_Employee.Errors;
using IF_Employee.Interfaces;

namespace IF_Employee.Tests
{
    [TestClass]
    public class HumanResourceServiceTests
    {
        private List<EmployeeContract> _listOfEmployeeContracts;

        private List<EmployeeShiftRecord> _listOfEmployeeShiftRecords;

        private IHumanResourceService _humanResourceService;

        private Employee _defaultEmployee;

        private EmployeeContract _defaultEmployeeContract;

        private const int DEFAULT_EMPLOYEE_ID = 1;

        private const decimal DEFAULT_HOURLY_RATE = 8.50m;

        private const string DEFAULT_FULL_NAME = "Default Surname";

        private readonly DateTime DEFAULT_CONTRACT_STARTDATE = new DateTime(2020, 1, 1);

        [TestInitialize]
        public void Setup()
        {
            _listOfEmployeeContracts = new List<EmployeeContract>();
            _listOfEmployeeShiftRecords = new List<EmployeeShiftRecord>();

            _humanResourceService = new HumanResourceService(_listOfEmployeeContracts, _listOfEmployeeShiftRecords);
            _defaultEmployee = new Employee();

            _defaultEmployee.Id = DEFAULT_EMPLOYEE_ID;
            _defaultEmployee.FullName = DEFAULT_FULL_NAME;
            _defaultEmployee.HourlySalary = DEFAULT_HOURLY_RATE;

            _defaultEmployeeContract = new EmployeeContract(_defaultEmployee, DEFAULT_CONTRACT_STARTDATE);
        }

        [TestMethod]
        public void HireEmployee_WithEmployeeNull_ThrowEmployeeCannotBeNullException()
        {
            var action = () => _humanResourceService.HireEmployee(null, DateTime.Now);

            action.Should().Throw<EmployeeCannotBeNullException>();
        }

        [TestMethod]
        public void HireEmployee_WithStartDateTimeInFuture_ThrowInvalidDateTimeIntervalException()
        {
            var action = () => _humanResourceService.HireEmployee(_defaultEmployee, DateTime.Now.AddMinutes(5));

            action.Should().Throw<InvalidDateTimeIntervalException>();
        }

        [TestMethod]
        public void HireEmployee_WithEmployeeWhoHasOngoingContract_ThrowEmployeeAlreadyWorkingForCompanyException()
        {
            _listOfEmployeeContracts.Add(
                new EmployeeContract(_defaultEmployee, DateTime.Now.AddDays(-2))
            );

            var action = () => _humanResourceService.HireEmployee(_defaultEmployee, DateTime.Now.AddMinutes(-30));

            action.Should().Throw<EmployeeAlreadyWorkingForCompanyException>();
        }

        [TestMethod]
        public void HireEmployee_WithValidEmployeeAndStartDate_ListOfContractsIncreased()
        {
            _humanResourceService.HireEmployee(_defaultEmployee, DateTime.Now.AddDays(-7));

            _listOfEmployeeContracts.Should().HaveCount(1);
        }

        [TestMethod]
        public void HireEmployee_WithValidEmployeeAndStartDate_ValidContractCreated()
        {
            var startDate = DateTime.Now.AddDays(-7);

            _humanResourceService.HireEmployee(_defaultEmployee, startDate);

            var contract = _listOfEmployeeContracts.First();

            contract.Employee.Should().Be(_defaultEmployee);
            contract.StartDate.Should().Be(startDate);
            contract.EndDate.Should().BeNull();
        }

        [TestMethod]
        public void HireEmployee_WithEmployeeWhoWasFired_EmployeeHired()
        {
            var contract = new EmployeeContract(_defaultEmployee, DateTime.Now);
            contract.TerminateContract(DateTime.Now);

            _listOfEmployeeContracts.Add(contract);

            _humanResourceService.HireEmployee(_defaultEmployee, DateTime.Now);

            _listOfEmployeeContracts.Count.Should().Be(2);
        }

        [TestMethod]
        public void GetWorkingEmployees_WithNoWorkingEmployees_EmptyArrayOfEmployees()
        {
            _humanResourceService.GetWorkingEmployees().Length.Should().Be(0);
        }

        [TestMethod]
        public void GetWorkingEmployees_WithOneHiredEmployee_WorkingEmployeeCountShouldBe1()
        {
            _listOfEmployeeContracts.Add(new EmployeeContract(_defaultEmployee, DateTime.Now));

            _humanResourceService.GetWorkingEmployees().Length.Should().Be(1);
        }

        [TestMethod]
        public void GetWorkingEmployees_WithOneHiredEmployee_ReturnsSpecificEmployee()
        {
            _listOfEmployeeContracts.Add(new EmployeeContract(_defaultEmployee, DateTime.Now));

            var employee = _humanResourceService.GetWorkingEmployees()[0];

            employee.Should().Be(_defaultEmployee);
        }

        [TestMethod]
        public void GetWorkingEmployees_EmployeeContractEnded_ReturnsNoEmployees()
        {
            var contract = new EmployeeContract(_defaultEmployee, DateTime.Now);

            _listOfEmployeeContracts.Add(contract);

            contract.TerminateContract(DateTime.Now);

            _humanResourceService.GetWorkingEmployees().Length.Should().Be(0);
        }

        [TestMethod]
        public void FireEmployee_WithEmployeeWhoIsNotWorking_ThrowEmployeeNotWorkingForTheCompanyException()
        {
            var action = () => _humanResourceService.FireEmployee(DEFAULT_EMPLOYEE_ID, DateTime.Now);

            action.Should().Throw<EmployeeNotWorkingForTheCompanyException>();
        }

        [TestMethod]
        public void FireEmployee_WithStartDateTimeInFuture_ThrowInvalidDateTimeIntervalException()
        {
            _listOfEmployeeContracts.Add(new EmployeeContract(_defaultEmployee, DateTime.Now));

            var action = () => _humanResourceService.FireEmployee(DEFAULT_EMPLOYEE_ID, DateTime.Now.AddMinutes(5));

            action.Should().Throw<InvalidDateTimeIntervalException>();
        }

        [TestMethod]
        public void FireEmployee_WithWorkingEmployeesId_EmployeeFired()
        {
            var employeeContract = new EmployeeContract(_defaultEmployee, DateTime.Now.AddDays(-2));

            _listOfEmployeeContracts.Add(employeeContract);

            var endDate = DateTime.Now;

            _humanResourceService.FireEmployee(DEFAULT_EMPLOYEE_ID, endDate);

            employeeContract.EndDate.Should().Be(endDate);
        }

        [TestMethod]
        public void FireEmployee_WithEndDateSmallerThanStartDate_ThrowContractStartDatePassesEndDateException()
        {
            var startDate = DateTime.Now;
            var employeeContract = new EmployeeContract(_defaultEmployee, startDate);

            _listOfEmployeeContracts.Add(employeeContract);

            var action = () => _humanResourceService.FireEmployee(DEFAULT_EMPLOYEE_ID, startDate.AddSeconds(-1));

            action.Should().Throw<ContractStartDatePassesEndDateException>();
        }

        [TestMethod]
        public void ReportHours_WithInvalidEmployee_ThrowEmployeeCannotBeNullException()
        {
            var action = () => _humanResourceService.ReportHours(null, DateTime.Now.AddDays(-1), 8, 0);

            action.Should().Throw<EmployeeCannotBeNullException>();
        }

        [TestMethod]
        public void ReportHours_WithNegativeHours_ThrowNegativeNumberNotAllowedException()
        {
            var action = () => _humanResourceService.ReportHours(_defaultEmployee, DateTime.Now.AddDays(-1), -1, 0);

            action.Should().Throw<NegativeNumberNotAllowedException>();
        }

        [TestMethod]
        public void ReportHours_WithNegativeMinutes_ThrowNegativeNumberNotAllowedException()
        {
            var action = () => _humanResourceService.ReportHours(_defaultEmployee, DateTime.Now.AddDays(-1), 10, -10);

            action.Should().Throw<NegativeNumberNotAllowedException>();
        }

        [TestMethod]
        public void ReportHours_WithMinutesOver59_ThrowInvalidMinutesException()
        {
            var action = () => _humanResourceService.ReportHours(_defaultEmployee, DateTime.Now.AddDays(-1), 10, 60);

            action.Should().Throw<InvalidMinutesException>();
        }

        [TestMethod]
        public void ReportHours_WithEmployeeWhoIsNotWorkingForTheCompany_ThrowEmployeeNotWorkingForTheCompanyException()
        {
            var action = () => _humanResourceService.ReportHours(_defaultEmployee, DateTime.Now.AddDays(-1), 8, 0);

            action.Should().Throw<EmployeeNotWorkingForTheCompanyException>();
        }

        [TestMethod]
        public void ReportHours_WithStartDateInFuture_ThrowInvalidDateTimeIntervalException()
        {
            _listOfEmployeeContracts.Add(_defaultEmployeeContract);

            var action = () => _humanResourceService.ReportHours(_defaultEmployee, DateTime.Now.AddMinutes(5), 8, 0);

            action.Should().Throw<InvalidDateTimeIntervalException>();
        }

        [TestMethod]
        public void ReportHours_WithShiftStartDateSmallerThanContractStartDate_ThrowInvalidDateTimeIntervalException()
        {
            _listOfEmployeeContracts.Add(_defaultEmployeeContract);

            var action = () => _humanResourceService.ReportHours(_defaultEmployee, DEFAULT_CONTRACT_STARTDATE.AddMinutes(-5), 8, 0);

            action.Should().Throw<InvalidDateTimeIntervalException>();
        }

        [TestMethod]
        public void ReportHours_WithValidParams_ShiftRecordCreated()
        {
            var shiftStartDate = DEFAULT_CONTRACT_STARTDATE.AddDays(1);
            _listOfEmployeeContracts.Add(_defaultEmployeeContract);

            _humanResourceService.ReportHours(_defaultEmployee, shiftStartDate, 8, 0);

            _listOfEmployeeShiftRecords.Count.Should().Be(1);
        }

        [TestMethod]
        public void ReportHours_AtMultipleDays_ShiftRecordsCreated()
        {
            var shiftStartDate = DEFAULT_CONTRACT_STARTDATE.AddDays(1);
            var workedHoursPerShift = 8;

            _listOfEmployeeContracts.Add(_defaultEmployeeContract);

            _humanResourceService.ReportHours(_defaultEmployee, shiftStartDate, workedHoursPerShift, 0);

            var secondShiftStartDate = shiftStartDate.AddHours(workedHoursPerShift);
            _humanResourceService.ReportHours(_defaultEmployee, secondShiftStartDate, workedHoursPerShift, 0);

            _listOfEmployeeShiftRecords.Count.Should().Be(2);
        }

        [TestMethod]
        public void ReportHours_WithValidParams_ValidShiftRecordCreated()
        {
            var shiftStartDate = DEFAULT_CONTRACT_STARTDATE.AddDays(1);
            var hoursWorked = 10;
            var minutesWorked = 0;

            _listOfEmployeeContracts.Add(_defaultEmployeeContract);

            _humanResourceService.ReportHours(_defaultEmployee, shiftStartDate, hoursWorked, minutesWorked);

            var record = _listOfEmployeeShiftRecords.First();

            record.Employee.Should().Be(_defaultEmployee);
            record.StartDate.Should().Be(shiftStartDate);
            record.HoursWorked.Should().Be(hoursWorked);
            record.MinutesWorked.Should().Be(minutesWorked);
            record.EndDate.Should().Be(shiftStartDate.AddHours(hoursWorked).AddMinutes(minutesWorked));
        }

        [TestMethod]
        public void ReportHours_WithStartDateWhichIsOverLapping_ThrowShiftRecordExistsForGivenTimePeriod()
        {
            _listOfEmployeeContracts.Add(_defaultEmployeeContract);

            var shiftStartDate = new DateTime(2022, 1, 1);
            var shiftEndDate = shiftStartDate.AddHours(10);
            var firstShift = new EmployeeShiftRecord(_defaultEmployee, shiftStartDate, shiftEndDate, 10, 0);

            _listOfEmployeeShiftRecords.Add(firstShift);

            var action = () => _humanResourceService.ReportHours(_defaultEmployee, shiftEndDate.AddHours(-1), 10, 0);

            action.Should().Throw<ShiftRecordExistsForGivenTimePeriod>();
        }

        [TestMethod]
        public void ReportHours_WithEndDateWhichIsOverLapping_ThrowShiftRecordExistsForGivenTimePeriod()
        {
            _listOfEmployeeContracts.Add(_defaultEmployeeContract);

            var shiftStartDate = new DateTime(2022, 1, 1);
            var shiftEndDate = shiftStartDate.AddHours(10);
            var firstShift = new EmployeeShiftRecord(_defaultEmployee, shiftStartDate, shiftEndDate, 10, 0);

            _listOfEmployeeShiftRecords.Add(firstShift);

            var action = () => _humanResourceService.ReportHours(_defaultEmployee, shiftEndDate.AddHours(-12), 10, 0);

            action.Should().Throw<ShiftRecordExistsForGivenTimePeriod>();
        }

        [TestMethod]
        public void GetMonthlyReport_WithStartDateGreaterThanEndDate_ThrowInvalidDateTimeIntervalException()
        {
            var startDate = new DateTime(2020, 1, 1);
            var action = () => _humanResourceService.GetMonthlyReport(startDate, startDate.AddMinutes(-5));

            action.Should().Throw<InvalidDateTimeIntervalException>();
        }

        [TestMethod]
        public void GetMonthlyReport_WithEmployeesAlreadyReportedHours_MonthlyReportOfSingleRecord()
        {
            // Arrange
            var startDate = new DateTime(2020, 1, 1);
            var shiftEndDate = startDate.AddHours(10);
            _listOfEmployeeShiftRecords.Add(new EmployeeShiftRecord(_defaultEmployee, startDate, shiftEndDate, 10, 0));

            var newShiftStartDate = new DateTime(2020, 1, 2);
            var newShiftEndDate = newShiftStartDate.AddHours(8);
            _listOfEmployeeShiftRecords.Add(new EmployeeShiftRecord(_defaultEmployee, newShiftStartDate, newShiftEndDate, 8, 0));

            var reportEndDate = new DateTime(2020, 1, 3);

            // Act
            var reports = _humanResourceService.GetMonthlyReport(startDate, reportEndDate);
            
            var report = reports.First();

            // Assert
            reports.Count().Should().Be(1);

            report.EmployeeId.Should().Be(_defaultEmployee.Id);
            report.Month.Should().Be(startDate.Month);
            report.Year.Should().Be(startDate.Year);
            report.Salary.Should().Be((10 + 8) * _defaultEmployee.HourlySalary);
        }

        [TestMethod]
        public void GetMonthlyReport_WithOneEmployeeShiftsOfMultipleMonths_MonthlyReportOfSingleEmployee()
        {
            var startDate = new DateTime(2020, 1, 1);
            var shiftEndDate = startDate.AddHours(10);
            _listOfEmployeeShiftRecords.Add(new EmployeeShiftRecord(_defaultEmployee, startDate, shiftEndDate, 10, 0));

            var secondMonthStartDate = new DateTime(2020, 2, 1);
            var secondMonthEndDate = secondMonthStartDate.AddHours(8);
            _listOfEmployeeShiftRecords.Add(new EmployeeShiftRecord(_defaultEmployee, secondMonthStartDate, secondMonthEndDate, 8, 0));

            var reportEndDate = new DateTime(2020, 2, 2);

            var reports = _humanResourceService.GetMonthlyReport(startDate, reportEndDate);

            reports.Count().Should().Be(2);

            var report = reports.First();
            var lastReport = reports.Last();

            report.EmployeeId.Should().Be(_defaultEmployee.Id);
            report.Month.Should().Be(startDate.Month);
            report.Year.Should().Be(startDate.Year);
            report.Salary.Should().Be(10 * _defaultEmployee.HourlySalary);

            lastReport.EmployeeId.Should().Be(_defaultEmployee.Id);
            lastReport.Month.Should().Be(secondMonthStartDate.Month);
            lastReport.Year.Should().Be(secondMonthStartDate.Year);
            lastReport.Salary.Should().Be(8 * _defaultEmployee.HourlySalary);
        }

        [TestMethod]
        public void GetMonthlyReport_WithMultipleEmployees_MonthlyReportOfMultipleEmployees()
        {
            var startDate = new DateTime(2020, 1, 1);
            var shiftEndDate = startDate.AddHours(10);
            _listOfEmployeeShiftRecords.Add(new EmployeeShiftRecord(_defaultEmployee, startDate, shiftEndDate, 10, 0));

            var secondEmployee = new Employee();
            secondEmployee.Id= 2;
            secondEmployee.FullName = "Second Employee";
            secondEmployee.HourlySalary = 9;

            var secondMonthStartDate = new DateTime(2020, 1, 1);
            var secondMonthEndDate = secondMonthStartDate.AddHours(8);
            _listOfEmployeeShiftRecords.Add(new EmployeeShiftRecord(secondEmployee, secondMonthStartDate, secondMonthEndDate, 8, 0));

            var reportEndDate = new DateTime(2020, 1, 2);

            var reports = _humanResourceService.GetMonthlyReport(startDate, reportEndDate);

            reports.Count().Should().Be(2);

            var report = reports.First();
            var lastReport = reports.Last();

            report.EmployeeId.Should().Be(_defaultEmployee.Id);
            report.Month.Should().Be(startDate.Month);
            report.Year.Should().Be(startDate.Year);
            report.Salary.Should().Be(10 * _defaultEmployee.HourlySalary);

            lastReport.EmployeeId.Should().Be(secondEmployee.Id);
            lastReport.Month.Should().Be(secondMonthStartDate.Month);
            lastReport.Year.Should().Be(secondMonthStartDate.Year);
            lastReport.Salary.Should().Be(8 * secondEmployee.HourlySalary);
        }
    }
}