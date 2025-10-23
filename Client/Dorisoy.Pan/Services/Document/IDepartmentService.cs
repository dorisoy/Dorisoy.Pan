namespace Dorisoy.Pan.Services
{
    public interface IDepartmentService
    {
        Task<ServiceResult<DepartmentModel>> AddAsync(DepartmentModel model);
        Task DeleteAsync(DepartmentModel model);
        bool DepartmentNameIsFree(string departmentname);
        Task<List<DepartmentModel>> GetDepartments();
        Task<ServiceResult<DepartmentModel>> UpdateAsync(DepartmentModel model);
    }
}