namespace EmployeeManagementAPI.Models.ViewModels
{
    public class EmployeeCreateViewModel
    {
        public string FullName { get; set; } = null!;
        public string Residence { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }

        public int DepartmentId { get; set; }
        public IList<Department> Departments { get; set; } = new List<Department>();

        public int PositionId { get; set; }
        public IList<Position> Positions { get; set; } = new List<Position>();

    }
}
