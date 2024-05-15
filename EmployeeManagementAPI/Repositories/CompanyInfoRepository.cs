
namespace EmployeeManagementAPI.Repositories
{
    public class CompanyInfoRepository : ICompanyInfoRepository
    {
        private readonly IConnectionProvider _provider;

        public CompanyInfoRepository(IConnectionProvider provider)
        {
            _provider = provider;
        }

        public async Task<CompanyInfoViewModel> GetAsync()
        {
            var company = new CompanyInfoViewModel();
            using (var connection = new SqlConnection(_provider.GetConnectionString()))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "GetCompanyInfo";

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            company.CompanyName = reader["CompanyName"].ToString();
                            company.PhysicalAddress = reader["PhysicalAddress"].ToString();
                            company.ContactPhone = reader["ContactPhone"].ToString();
                            company.Email = reader["Email"].ToString();
                        }
                    }
                }
            }
            return company;
        }
    }
}
