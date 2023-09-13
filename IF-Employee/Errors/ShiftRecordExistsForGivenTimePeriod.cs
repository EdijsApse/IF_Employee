namespace IF_Employee.Errors
{
    public class ShiftRecordExistsForGivenTimePeriod : Exception
    {
        public ShiftRecordExistsForGivenTimePeriod() : base("Shift record exists for given time period!") { }
    }
}
