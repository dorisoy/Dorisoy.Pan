using Microsoft.EntityFrameworkCore;
using Dorisoy.Pan.Data.Contexts;


namespace Dorisoy.Pan.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IMapper _mapper;
    private readonly IDbContextFactory<CaptureManagerContext> _contextFactory;
    private readonly SourceCache<DepartmentModel, Guid> _departments;

    public DepartmentService(IDbContextFactory<CaptureManagerContext> contextFactory, IMapper mapper)
    {
        _mapper = mapper;
        _contextFactory = contextFactory;

        _departments = new SourceCache<DepartmentModel, Guid>(e => e.Id);

        using var context = _contextFactory.Create();
        var departments = _mapper.ProjectTo<DepartmentModel>(context.Departments);
        _departments.AddOrUpdate(departments);
    }

    /// <summary>
    /// 获取部门
    /// </summary>
    /// <returns></returns>
    public async Task<List<DepartmentModel>> GetDepartments()
    {
        return await Task.Run(() =>
        {
            var departments = new List<DepartmentModel>();
            try
            {
                using (var context = _contextFactory.Create())
                {
                    var result = _mapper.ProjectTo<DepartmentModel>(context.Departments);
                    departments = result.ToList();
                }
            }
            catch { }
            return departments;
        });
    }

    /// <summary>
    /// 添加部门
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<ServiceResult<DepartmentModel>> AddAsync(DepartmentModel model)
    {
        return await Task.Run(() =>
        {
            using (var context = _contextFactory.Create())
            {
                var entity = new Department();
                _mapper.Map(model, entity);

                context.Departments.Add(entity);
                context.SaveChanges();

                _mapper.Map(entity, model);

                _departments.AddOrUpdate(model);

                return ServiceResult.Ok(model);
            }
        });
    }

    /// <summary>
    /// 更新部门
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<ServiceResult<DepartmentModel>> UpdateAsync(DepartmentModel model)
    {
        return await Task.Run(() =>
        {
            using (var context = _contextFactory.Create())
            {
                var entity = context.Departments.First(e => e.Id == model.Id);
                _mapper.Map(model, entity);

                context.SaveChanges();

                _mapper.Map(entity, model);

                _departments.AddOrUpdate(model);

                return ServiceResult.Ok(model);
            }
        });
    }

    /// <summary>
    /// 删除部门
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task DeleteAsync(DepartmentModel model)
    {
        await Task.Run(() =>
        {
            using (var context = _contextFactory.Create())
            {
                var entity = context.Set<Department>().Find(model.Id);
                if (entity != null)
                {
                    context.Entry(entity).State = EntityState.Deleted;
                    context.SaveChanges();
                }
            }
        });
    }


    /// <summary>
    /// 部门是否存在
    /// </summary>
    /// <param name="departmentname"></param>
    /// <returns></returns>
    public bool DepartmentNameIsFree(string departmentname)
    {
        using (var context = _contextFactory.Create())
        {
            return !context.Departments.Any(e => e.Name == departmentname);
        }
    }
}
