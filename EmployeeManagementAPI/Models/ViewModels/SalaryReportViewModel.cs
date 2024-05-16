namespace EmployeeManagementAPI.Models.ViewModels
{
	public class SalaryReportViewModel
	{
		public string? FullName { get; set; }
		public string? PhoneNumber { get; set; }
		public string? PositionName { get; set; }
		public string? DepartmentName { get; set; }
		public DateOnly? HireDate { get; set; }
		public decimal Salary { get; set; }
	}
}
