using EmployeeManagementAPI.Models;
using System.Data.SqlClient;
using System.Data;

namespace EmployeeManagementAPI.DAL
{
    public class EmployeeDAL : IEmployeeDAL
    {
        SqlConnection _connection = null;
        SqlCommand _command = null!;
        public  static IConfiguration Configuration { get; set; }

        private string GetConnectionString()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
            return Configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IList<Employee>> GetAllAsync()
        {
            var employeesList = new List<Employee>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "GetAllEmployees";

                await _connection.OpenAsync();

                var dr = await _command.ExecuteReaderAsync();

                while (await dr.ReadAsync())
                {
                    employeesList.Add(new Employee()
                    {
                        EmployeeId = Convert.ToInt32(dr["EmployeeId"]),
                        FullName = dr["FullName"].ToString(),
                        Residence = dr["Residence"].ToString(),
                        PhoneNumber = dr["PhoneNumber"].ToString(),
                        DateOfBirth = DateTime.Parse(dr["DateOfBirth"].ToString()),
                        HireDate = DateTime.Parse(dr["HireDate"].ToString()),
                        Salary = Convert.ToDecimal(dr["Salary"]),
                        Position = dr["PositionName"].ToString(),
                        DepartmentName = dr["DepartmentName"].ToString()
                    });
                }
            }
            return employeesList;
        }
    }
}
