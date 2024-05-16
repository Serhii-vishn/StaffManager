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

        public async Task<IList<Employee>> ListAsync(string? sortOrder, string? sortColumn, string? searchName, List<string> positions)
        {
			var employeesList = new List<Employee>();
			using (var connection = new SqlConnection(_provider.GetConnectionString()))
			{
				using (var command = connection.CreateCommand())
				{
					command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "GetAllEmployees";

                    if (!string.IsNullOrEmpty(sortOrder) && !string.IsNullOrEmpty(sortColumn) || positions.Any())
                    {
                        command.CommandText = "GetAllEmployeesSorted";
                        command.Parameters.AddWithValue("@sortedOrder", sortOrder);
                        command.Parameters.AddWithValue("@sortedColumn", sortColumn);

						var positionsParams = string.Join(",", positions);
                        command.Parameters.AddWithValue("@positions", positionsParams);
                    }

                    if(!string.IsNullOrEmpty(searchName))
                    {
						command.CommandText = "SearchEmployeeByName";
						command.Parameters.AddWithValue("@searchString", searchName);
					}

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

        public async Task<IList<SalaryReportViewModel>> SalaryReportAsync(List<string> positionsFilter, List<string> departmentsFilter, string? startYear, string? endYear)
        {
            var employeesList = new List<SalaryReportViewModel>();
            using (var connection = new SqlConnection(_provider.GetConnectionString()))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;

                    if(positionsFilter.Any())
                    {
                        command.CommandText = "GetByPositionEmployeesSalary";

                        var positionsParams = string.Join(",", positionsFilter);
                        command.Parameters.AddWithValue("@positionsIdList", positionsParams);
                    }
                    else if (departmentsFilter.Any())
                    {
                        command.CommandText = "GetByDepartmentEmployeesSalary";

                        var departmentsParams = string.Join(",", departmentsFilter);
                        command.Parameters.AddWithValue("@departmentIdList", departmentsParams);
                    }
                    else if(!string.IsNullOrEmpty(startYear) || !string.IsNullOrEmpty(endYear))
                    {
                        command.CommandText = "GetByHireDateEmployeesSalary";

                        var startDate = new DateTime(int.Parse(startYear), 1, 1);
                        var endDate = new DateTime(int.Parse(endYear), 1, 1);

                        command.Parameters.AddWithValue("@startDate", startDate);
                        command.Parameters.AddWithValue("@endDate", endDate);
                    }
                    else
                    {
                        return employeesList;
                    }

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (command.CommandText.Equals("GetByPositionEmployeesSalary"))
                            {
                                var employee = new SalaryReportViewModel
                                {
                                    FullName = reader["FullName"]?.ToString(),
                                    PhoneNumber = reader["PhoneNumber"]?.ToString(),
                                    PositionName = reader["PositionName"]?.ToString(),
                                    Salary = reader.GetDecimal(reader.GetOrdinal("Salary"))
                                };
                                employeesList.Add(employee);
                            }
                            else if (command.CommandText.Equals("GetByDepartmentEmployeesSalary"))
                            {
                                var employee = new SalaryReportViewModel
                                {
                                    FullName = reader["FullName"]?.ToString(),
                                    PhoneNumber = reader["PhoneNumber"]?.ToString(),
                                    DepartmentName = reader["DepartmentName"]?.ToString(),
                                    Salary = reader.GetDecimal(reader.GetOrdinal("Salary"))
                                };
                                employeesList.Add(employee);
                            }
                            else if (command.CommandText.Equals("GetByHireDateEmployeesSalary"))
                            {
                                var employee = new SalaryReportViewModel
                                {
                                    FullName = reader["FullName"]?.ToString(),
                                    PhoneNumber = reader["PhoneNumber"]?.ToString(),
                                    HireDate = DateOnly.FromDateTime(Convert.ToDateTime(reader["HireDate"])),
                                    Salary = reader.GetDecimal(reader.GetOrdinal("Salary"))
                                };
                                employeesList.Add(employee);
                            }
                        }
                    }
                }
            }
            return employeesList;
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
