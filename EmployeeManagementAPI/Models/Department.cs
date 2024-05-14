namespace EmployeeManagementAPI.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }
        public string DepartmentName { get; set; } = null!;
    }
}
