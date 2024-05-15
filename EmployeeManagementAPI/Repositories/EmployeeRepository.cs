namespace EmployeeManagementAPI.DAL
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IConnectionProvider _provider;

        public EmployeeRepository(IConnectionProvider provider)
        {
			_provider = provider;
        }

		public async Task<IList<Employee>> ListAsync(string? searchName, string? sortColumn, string? sortDirection)
        {
			var employeesList = new List<Employee>();
			using (var connection = new SqlConnection(_provider.GetConnectionString()))
			{
				using (var command = connection.CreateCommand())
				{
					command.CommandType = CommandType.StoredProcedure;
					command.CommandText = "GetAllEmployees";

					await connection.OpenAsync();

					using (var reader = await command.ExecuteReaderAsync())
					{
						while (await reader.ReadAsync())
						{
							employeesList.Add(MapEmployee(reader));
						}
					}
				}
			}
			return employeesList;
		}

		public async Task<bool> AddAsync(EmployeeCreateViewModel employee)
		{ 
			ValidateEmployee(employee);
			using (var connection = new SqlConnection(_provider.GetConnectionString()))
			{
				using (var command = connection.CreateCommand())
				{
					command.CommandType = CommandType.StoredProcedure;
					command.CommandText = "InsertEmployee";

					command.Parameters.AddWithValue("@FullName", employee.FullName);
					command.Parameters.AddWithValue("@Residence", employee.Residence);
					command.Parameters.AddWithValue("@PhoneNumber", employee.PhoneNumber);
					command.Parameters.AddWithValue("@DateOfBirth", employee.DateOfBirth);
					command.Parameters.AddWithValue("@HireDate", employee.HireDate);
					command.Parameters.AddWithValue("@Salary", employee.Salary);
					command.Parameters.AddWithValue("@DepartmentId", employee.DepartmentId);
					command.Parameters.AddWithValue("@PositionId", employee.PositionId);

					await connection.OpenAsync();
					var id = await command.ExecuteNonQueryAsync();
					connection.Close();

					return id > 0;
				}
			}
		}
	
		public Task<int> UpdateAsync(Employee employee)
		{
			throw new NotImplementedException();
		}

		public Task<int> DeleteAsync(int id)
		{
			throw new NotImplementedException();
		}

		private Employee MapEmployee(SqlDataReader reader)
		{
			return new Employee()
			{
				Id = Convert.ToInt32(reader["Id"]),
				FullName = reader["FullName"].ToString(),
				Residence = reader["Residence"].ToString(),
				PhoneNumber = reader["PhoneNumber"].ToString(),
				DateOfBirth = DateOnly.FromDateTime(Convert.ToDateTime(reader["DateOfBirth"])),
				HireDate = DateOnly.FromDateTime(Convert.ToDateTime(reader["HireDate"])),
				Salary = Convert.ToDouble(reader["Salary"]),
				Position = new Position()
				{
					Id = Convert.ToInt32(reader["PositionId"]),
					Name = reader["PositionName"].ToString()
				},
				Department = new Department()
				{
					Id = Convert.ToInt32(reader["DepartmentId"]),
					Name = reader["DepartmentName"].ToString()
				},
			};
		}

		private void ValidateEmployee(EmployeeCreateViewModel employee)
		{
            if (employee is null)
            {
                throw new ArgumentNullException(nameof(employee), "Employee is empty");
            }

            if (string.IsNullOrEmpty(employee.FullName) || employee.FullName.Length > 55)
            {
                throw new ArgumentException("FullName is required and should be maximum 55 characters long", nameof(employee.FullName));
            }

            if (string.IsNullOrEmpty(employee.Residence) || employee.Residence.Length > 75)
            {
                throw new ArgumentException("Residence is required and should be maximum 75 characters long", nameof(employee.FullName));
            }

            if (employee.DateOfBirth < DateTime.Parse("1950-01-01"))
            {
                throw new ArgumentException("Invalid date of bith", nameof(employee.DateOfBirth));
            }
            if (employee.HireDate < DateTime.Parse("2000-01-01"))
            {
                throw new ArgumentException("Invalid date of hire", nameof(employee.HireDate));
            }

            if (string.IsNullOrWhiteSpace(employee.PhoneNumber))
            {
                throw new ArgumentNullException(nameof(employee.PhoneNumber), "Phone number is empty");
            }
            else
            {
                employee.PhoneNumber = employee.PhoneNumber.Trim();

                const string ukrainianPhoneNumberPattern = @"^380\d{9}$";

                if (!Regex.IsMatch(employee.PhoneNumber, ukrainianPhoneNumberPattern))
                {
                    throw new ArgumentException(nameof(employee.PhoneNumber), "Phone number is invalid");
                }
            }
        }
	}
}
