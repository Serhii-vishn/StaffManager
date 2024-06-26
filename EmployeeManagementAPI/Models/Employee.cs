﻿namespace EmployeeManagementAPI.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Residence { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public DateOnly DateOfBirth { get; set; }
        public DateOnly HireDate { get; set; }
        public decimal Salary { get; set; }

        public Position Position { get; set; } = null!;
        public Department Department { get; set; } = null!;       
    }
}
