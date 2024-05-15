namespace EmployeeManagementAPI.Repositories
{
	public class DepartmentRepository : IDepartmentRepository
	{
		private readonly IConnectionProvider _provider;

		public DepartmentRepository(IConnectionProvider provider)
		{
			_provider = provider;
		}

		public async Task<IList<Department>> ListAsync()
		{ 
			var departmants = new List<Department>();
			using (var connection = new SqlConnection(_provider.GetConnectionString()))
			{
				using (var command = connection.CreateCommand())
				{
					command.CommandType = CommandType.StoredProcedure;
					command.CommandText = "GetDepartments";

					await connection.OpenAsync();

					using (var reader = await command.ExecuteReaderAsync())
					{
						while (await reader.ReadAsync())
						{
							departmants.Add(MapDepartmant(reader));
						}
					}
				}
			}
			return departmants;
		}

		private Department MapDepartmant(SqlDataReader reader)
		{
			return new Department()
			{
				Id = Convert.ToInt32(reader["Id"]),
				Name = reader["DepartmentName"].ToString()
			};
		}
	}
}
