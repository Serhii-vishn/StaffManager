namespace EmployeeManagementAPI.Repositories.Interfaces
{
    public interface IPositionRepository
    {
        Task<IList<Position>> ListAsync();
    }
}
