using FluentAssertions;
using IF_Employee.Errors;
using IF_Employee.Interfaces;
using Moq;
using Moq.AutoMock;

namespace IF_Employee.Tests
{
    [TestClass]
    public class CompanyTests
    {
        private AutoMocker _mocker;

        private ICompany _company;

        private const string DEFAULT_COMPANY_NAME = "Default Company";

        private Employee _defaultEmployee;

        private const int DEFAULT_EMPLOYEE_ID = 1;

        private const decimal DEFAULT_HOURLY_RATE = 8.50m;

        private const string DEFAULT_FULL_NAME = "Default Surname";

        [TestInitialize]
        public void Setup()
        {
            _mocker = new AutoMocker();

            var humanResourceService = _mocker.GetMock<IHumanResourceService>();

            _company = new Company(DEFAULT_COMPANY_NAME, humanResourceService.Object);

            _defaultEmployee = new Employee();

            _defaultEmployee.Id = DEFAULT_EMPLOYEE_ID;
            _defaultEmployee.FullName = DEFAULT_FULL_NAME;
            _defaultEmployee.HourlySalary = DEFAULT_HOURLY_RATE;
        }

        [TestMethod]
        public void AddEmployee_WithEmployeeAsNull_ThrowsEmployeeCannotBeNullException()
        {
            _mocker.GetMock<IHumanResourceService>()
                .Setup(service => service.HireEmployee(null, It.IsAny<DateTime>()))
                .Throws<EmployeeCannotBeNullException>();

            var action = () => _company.AddEmployee(null, It.IsAny<DateTime>());

            action.Should().Throw<EmployeeCannotBeNullException>();
        }

        [TestMethod]
        public void AddEmployee_WithDateTimeInFuture_ThrowsInvalidDateTimeIntervalException()
        {
            _mocker.GetMock<IHumanResourceService>()
                .Setup(service => service.HireEmployee(_defaultEmployee, It.IsAny<DateTime>()))
                .Throws<InvalidDateTimeIntervalException>();

            var action = () => _company.AddEmployee(_defaultEmployee, DateTime.Now.AddDays(1));

            action.Should().Throw<InvalidDateTimeIntervalException>();
        }

        [TestMethod]
        public void AddEmployee_WithValidEmployeeAndStartDate_EmployeeAddedAndEmployeeListUpdated()
        {
            var hrMock = _mocker.GetMock<IHumanResourceService>();

            hrMock
                .Setup(service => service.GetWorkingEmployees())
                .Returns(new Employee[] { _defaultEmployee });

            _company.AddEmployee(_defaultEmployee, DateTime.Now);

            _company.Employees.Should().HaveCount(1);
            _company.Employees[0].Should().Be(_defaultEmployee);
        }

        [TestMethod]
        public void AddEmployee_WithAlreadyWorkingEmployee_ThrowsEmployeeAlreadyWorkingForCompanyException()
        {
            _mocker.GetMock<IHumanResourceService>()
                .Setup(service => service.HireEmployee(_defaultEmployee, It.IsAny<DateTime>()))
                .Throws<EmployeeAlreadyWorkingForCompanyException>();

            var action = () => _company.AddEmployee(_defaultEmployee, DateTime.Now);

            action.Should().Throw<EmployeeAlreadyWorkingForCompanyException>();
        }

        [TestMethod]
        public void RemoveEmployee_WithEmployeeWhoIsNotWorking_ThrowsEmployeeNotWorkingForTheCompanyException()
        {
            var hrMock = _mocker.GetMock<IHumanResourceService>();

            hrMock
                .Setup(service => service.FireEmployee(DEFAULT_EMPLOYEE_ID, It.IsAny<DateTime>()))
                .Throws<EmployeeNotWorkingForTheCompanyException>();

            var action = () => _company.RemoveEmployee(DEFAULT_EMPLOYEE_ID, DateTime.Now);

            action.Should().Throw<EmployeeNotWorkingForTheCompanyException>();
        }

        [TestMethod]
        public void RemoveEmployee_WithEndDateInFuture_ThrowsInvalidDateTimeIntervalException()
        {
            var hrMock = _mocker.GetMock<IHumanResourceService>();

            hrMock
                .Setup(service => service.FireEmployee(DEFAULT_EMPLOYEE_ID, It.IsAny<DateTime>()))
                .Throws<InvalidDateTimeIntervalException>();

            var action = () => _company.RemoveEmployee(DEFAULT_EMPLOYEE_ID, DateTime.Now.AddMinutes(10));

            action.Should().Throw<InvalidDateTimeIntervalException>();
        }

        [TestMethod]
        public void RemoveEmployee_WithEmployeeWhoIsWorking_RemoveEmployee()
        {
            var employees = new Employee[] { _defaultEmployee };

            var hrMock = _mocker.GetMock<IHumanResourceService>();

            hrMock
                .Setup(service => service.GetWorkingEmployees())
                .Returns(employees);

            _company.RemoveEmployee(employees[0].Id, DateTime.Now);

            hrMock
                .Setup(service => service.GetWorkingEmployees())
                .Returns(new Employee[] { });

            _company.Employees.Length.Should().Be(0);
        }

        [TestMethod]
        public void Employees_WithWorikingEmployee_GetsEmployeeCount()
        {
            _mocker.GetMock<IHumanResourceService>()
                .Setup(service => service.GetWorkingEmployees())
                .Returns(new Employee[] { _defaultEmployee });

            _company.Employees.Length.Should().Be(1);
        }

        [TestMethod]
        public void ReportHours_WithEmployeeIdWhoIsNotWorking_ThrowEmployeeNotWorkingForTheCompanyException()
        {
            var startDate = new DateTime(2020, 1, 1);
            var hrMock = _mocker.GetMock<IHumanResourceService>();

            hrMock
                .Setup(service => service.GetWorkingEmployees())
                .Returns(new Employee[] { });

            var action = () => _company.ReportHours(1, startDate, 10, 0);

            action.Should().Throw<EmployeeNotWorkingForTheCompanyException>();
        }

        [TestMethod]
        public void ReportHours_WithValidEmployeeAndWorkingHours_WorkingHoursReported()
        {
            var hrMock = _mocker.GetMock<IHumanResourceService>();
            var startDate = new DateTime(2020, 1, 1);

            hrMock
                .Setup(service => service.GetWorkingEmployees())
                .Returns(new Employee[] { _defaultEmployee });

            _company.ReportHours(DEFAULT_EMPLOYEE_ID, startDate, 10, 0);

            hrMock
                .Verify(service => service.ReportHours(_defaultEmployee, startDate, 10, 0), Times.Once);
        }

        [TestMethod]
        public void GetMonthlyReport_WithInvalidDates_ThrowInvalidDateTimeIntervalException()
        {
            var startDate = new DateTime(2020, 1, 1);
            var endDate = startDate.AddMinutes(-5);

            var hrMock = _mocker.GetMock<IHumanResourceService>();

            hrMock
                .Setup(service => service.GetMonthlyReport(startDate, endDate))
                .Throws<InvalidDateTimeIntervalException>();

            var action = () => _company.GetMonthlyReport(startDate, endDate);

            action.Should().Throw<InvalidDateTimeIntervalException>();
        }
    }
}