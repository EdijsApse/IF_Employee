namespace IF_Employee
{
    public class Employee
    {
        /// <summary>
        /// Unique ID of the employee.
        /// </summary> 
        public int Id { get; set; }

        /// <summary>
        /// Employee full name.
        /// </summary> 
        public string FullName { get; set; }

        /// <summary>
        /// Hourly salary of worked full hour. Use proportion for time smaller than 1 hour.
        /// </summary> 
        public decimal HourlySalary { get; set; }
    }
}
