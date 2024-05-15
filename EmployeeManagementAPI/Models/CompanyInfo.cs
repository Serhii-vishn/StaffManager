namespace EmployeeManagementAPI.Models
{
    public class CompanyInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string PhysicalAddress { get; set; } = null!;
        public string ContactPhone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string AdditionalInfo { get; set; } = null!;
    }
}
