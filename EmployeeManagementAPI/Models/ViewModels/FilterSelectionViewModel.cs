namespace EmployeeManagementAPI.Models.ViewModels
{
	public class FilterSelectionViewModel
	{
		public IList<int> DepartmentId { get; set; } = new List<int>();
		public IList<Department> Departments { get; set; } = new List<Department>();

		public IList<int> PositionId { get; set; } = new List<int>();
		public IList<Position> Positions { get; set; } = new List<Position>();

        public decimal? TotalSum { get; set; }
        public IList<SalaryReportViewModel> Report { get; set; } = new List<SalaryReportViewModel>();
    }
}
