namespace EmployeeManagementAPI.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IConnectionProvider _provider;

        public EmployeeRepository(IConnectionProvider provider)
        {
			_provider = provider;
        }

        public async Task<Employee> GetAsync(int id)
        {
            var employee = new Employee();
            using (var connection = new SqlConnection(_provider.GetConnectionString()))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "GetEmployeeById";
                    command.Parameters.AddWithValue("@Id", id);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            employee = MapEmployee(reader);
                        }
                    }
                }
            }
            return employee;
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
            var validEmployee = new Employee()
            {
                FullName = employee.FullName,
                Residence = employee.Residence,
                PhoneNumber = employee.PhoneNumber,
                DateOfBirth = DateOnly.FromDateTime(employee.DateOfBirth),
                HireDate = DateOnly.FromDateTime(employee.HireDate),
                Salary = employee.Salary
            };
            ValidateEmployee(validEmployee);

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
	
		public async Task<bool> UpdateAsync(Employee employee)
		{
            ValidateEmployee(employee);

            using (var connection = new SqlConnection(_provider.GetConnectionString()))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "UpdateEmployee";

                    command.Parameters.AddWithValue("@Id", employee.Id);
                    command.Parameters.AddWithValue("@FullName", employee.FullName);
                    command.Parameters.AddWithValue("@Residence", employee.Residence);
                    command.Parameters.AddWithValue("@PhoneNumber", employee.PhoneNumber);
                    command.Parameters.AddWithValue("@DateOfBirth", employee.DateOfBirth.ToDateTime(new TimeOnly(12, 0)));
                    command.Parameters.AddWithValue("@HireDate", employee.HireDate.ToDateTime(new TimeOnly(12, 0)));
                    command.Parameters.AddWithValue("@Salary", employee.Salary);

                    await connection.OpenAsync();
                    var id = await command.ExecuteNonQueryAsync();
                    connection.Close();

                    return id > 0;
                }
            }
        }

		public async Task<bool> DeleteAsync(int id)
		{
            using (var connection = new SqlConnection(_provider.GetConnectionString()))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "DeleteEmployee";

                    command.Parameters.AddWithValue("@Id", id);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                    connection.Close();

                    return id > 0;
                }
            }
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
				Salary = Convert.ToDecimal(reader["Salary"]),
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

		private void ValidateEmployee(Employee employee)
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

            if (employee.DateOfBirth < DateOnly.Parse("1950-01-01"))
            {
                throw new ArgumentException("Invalid date of bith", nameof(employee.DateOfBirth));
            }

            if (employee.HireDate < DateOnly.Parse("2000-01-01"))
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

            if (employee.Salary <= 0)
            {
                throw new ArgumentException("Invalid salary", nameof(employee.Salary));
            }
        }
    }
}
