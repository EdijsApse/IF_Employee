namespace IF_Employee.Errors
{
    public class ContractStartDatePassesEndDateException : Exception
    {
        public ContractStartDatePassesEndDateException() : base("Contract start date cannot be higher than end date!") { }
    }
}
