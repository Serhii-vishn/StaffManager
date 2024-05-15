namespace EmployeeManagementAPI.Repositories
{
    public class PositionRepository : IPositionRepository
    {
        private readonly IConnectionProvider _provider;

        public PositionRepository(IConnectionProvider provider)
        {
            _provider = provider;
        }

        public async Task<IList<Position>> ListAsync()
        {
            var positions = new List<Position>();
            using (var connection = new SqlConnection(_provider.GetConnectionString()))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "GetPositions";

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            positions.Add(MapPosition(reader));
                        }
                    }
                }
            }
            return positions;
        }

        private Position MapPosition(SqlDataReader reader)
        {
            return new Position()
            {
                Id = Convert.ToInt32(reader["Id"]),
                Name = reader["PositionName"].ToString()
            };
        }
    }
}
