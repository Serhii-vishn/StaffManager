namespace EmployeeManagementAPI.Repositories.Interfaces
{
	public interface IDepartmentRepository
	{
		Task<IList<Department>> ListAsync();
	}
}
