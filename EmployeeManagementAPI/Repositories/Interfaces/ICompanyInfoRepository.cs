namespace EmployeeManagementAPI.Repositories.Interfaces
{
    public interface ICompanyInfoRepository
    {
        Task<CompanyInfoViewModel> GetAsync();
    }
}
