namespace EmployeeManagementAPI.Models
{
    public class Position
    {
        [Key]
        public int PositionId { get; set; }
        public string PositionName { get; set; } = null!;
    }
}
