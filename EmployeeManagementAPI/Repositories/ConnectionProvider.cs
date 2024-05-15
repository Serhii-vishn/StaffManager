namespace EmployeeManagementAPI.Repository
{
	public class ConnectionProvider : IConnectionProvider
	{
		private readonly IConfiguration _configuration;

		public ConnectionProvider(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public string GetConnectionString() => _configuration.GetConnectionString("DefaultConnection");
	}
}
