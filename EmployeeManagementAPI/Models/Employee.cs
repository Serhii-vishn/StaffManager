namespace EmployeeManagementAPI.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Residence { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }

        public int PositionId { get; set; }
        public Position Position { get; set; } = null!;
        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;       
    }
}
