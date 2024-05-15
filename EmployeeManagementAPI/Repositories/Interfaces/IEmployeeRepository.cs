namespace EmployeeManagementAPI.DAL.Interfaces
{
    public interface IEmployeeRepository
    {
		Task<IList<Employee>> ListAsync(string? searchName, string? sortColumn, string? sortDirection);
		Task<bool> AddAsync(EmployeeCreateViewModel employee);
		Task<int> UpdateAsync(Employee employee);
		Task<int> DeleteAsync(int id);
    }
}
