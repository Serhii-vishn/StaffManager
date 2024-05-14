namespace EmployeeManagementAPI.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }
        public string FullName { get; set; } = null!;
        public string Residence { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }

        public string Position { get; set; } = null!;
        public string DepartmentName { get; set; } = null!;
    }
}
