namespace EmployeeManagementAPI.DAL.Interfaces
{
    public interface IEmployeeDAL
    {
        Task<IList<Employee>> GetAllAsync();
    }
}
