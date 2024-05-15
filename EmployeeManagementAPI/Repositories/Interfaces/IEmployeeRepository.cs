namespace EmployeeManagementAPI.Repository.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<Employee> GetAsync(int id);
        Task<IList<Employee>> ListAsync(string? sortOrder, string? sortColumn, string? sortDirection);
		Task<bool> AddAsync(EmployeeCreateViewModel employee);
		Task<bool> UpdateAsync(Employee employee);
		Task<bool> DeleteAsync(int id);
    }
}
