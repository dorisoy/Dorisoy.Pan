using Microsoft.EntityFrameworkCore;
using Dorisoy.Pan.Data.Contexts;

namespace Dorisoy.Pan.Services;

public class PatientService : IPatientService
{
    private readonly IMapper _mapper;
    private readonly IDbContextFactory<CaptureManagerContext> _contextFactory;

    private readonly SourceCache<PatientModel, Guid> _employees;

    public IObservable<IChangeSet<PatientModel, Guid>> Connect() => _employees.Connect();

    public PatientService(IDbContextFactory<CaptureManagerContext> contextFactory, IMapper mapper)
    {
        _mapper = mapper;
        _contextFactory = contextFactory;

        _employees = new SourceCache<PatientModel, Guid>(e => e.Id);

        using (var context = _contextFactory.Create())
        {
            var patients = _mapper.ProjectTo<PatientModel>(context.Patients);
            _employees.AddOrUpdate(patients);
        }
    }


    public async Task<List<PatientModel>> GetPatients()
    {
        return await Task.Run(() =>
        {
            var patients = new List<PatientModel>();
            using (var context = _contextFactory.Create())
            {
                var result = _mapper.ProjectTo<PatientModel>(context.Patients);
                patients = result.ToList();
            }
            return patients;
        });
    }

    public async Task<PatientModel> GetPatient(Guid guid)
    {
        return await Task.Run(async () =>
        {
            var patient = new PatientModel();
            using (var context = _contextFactory.Create())
            {
                var p = await context.Patients.Where(s => s.Id == guid).FirstOrDefaultAsync();
                patient = _mapper.Map<PatientModel>(p);
            }
            return patient;
        });
    }

    public async Task<ServiceResult<PatientModel>> AddAsync(PatientModel model)
    {
        return await Task.Run(() =>
        {
            using (var context = _contextFactory.Create())
            {
                var user = Globals.CurrentUser;
                var exits = context.Patients
              .Where(p => p.Code == model.Code)
              .Count() > 0;

                if (!exits)
                {
                    var entity = new Patient();

                    entity.CreatedBy = user.Id;
                    entity.CreatedDate = DateTime.Now;
                    entity.ModifiedBy = user.Id;
                    entity.ModifiedDate = DateTime.Now;

                    _mapper.Map(model, entity);

                    context.Patients.Add(entity);
                    context.SaveChanges();

                    _mapper.Map(entity, model);

                    _employees.AddOrUpdate(model);
                }

                return ServiceResult.Ok(model);
            }
        });
    }

    public async Task<ServiceResult<PatientModel>> UpdateAsync(PatientModel model)
    {
        return await Task.Run(() =>
        {
            var user = Globals.CurrentUser;
            using (var context = _contextFactory.Create())
            {
                var entity = context.Patients.First(e => e.Id == model.Id);
                _mapper.Map(model, entity);
                entity.ModifiedBy = user.Id;
                entity.ModifiedDate = DateTime.Now;
                context.SaveChanges();

                _mapper.Map(entity, model);

                _employees.AddOrUpdate(model);

                return ServiceResult.Ok(model);
            }
        });
    }

    public async Task DeleteAsync(PatientModel model)
    {
        await Task.Run(() =>
        {
            using (var context = _contextFactory.Create())
            {
                var entity = context.Set<Patient>().Find(model.Id);
                if (entity != null)
                {
                    context.Entry(entity).State = EntityState.Deleted;
                    context.SaveChanges();
                }
            }
        });
    }

    public bool PatientnameIsFree(Guid id, string patientname)
    {
        using (var context = _contextFactory.Create())
        {
            var count = context.Patients.Count(e => e.Id == id && e.RaleName == patientname) > 1;
            return !count;
        }
    }
}
