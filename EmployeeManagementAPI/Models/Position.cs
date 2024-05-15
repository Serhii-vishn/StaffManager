namespace EmployeeManagementAPI.Models
{
    public class Position
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
