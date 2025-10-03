namespace Dorisoy.PanClient.Services;

public interface IPatientService
{
    Task<ServiceResult<PatientModel>> AddAsync(PatientModel model);
    IObservable<IChangeSet<PatientModel, Guid>> Connect();
    Task DeleteAsync(PatientModel model);
    Task<List<PatientModel>> GetPatients();
    Task<PatientModel> GetPatient(Guid guid);
    bool PatientnameIsFree(Guid id, string patientname);
    Task<ServiceResult<PatientModel>> UpdateAsync(PatientModel model);
}
