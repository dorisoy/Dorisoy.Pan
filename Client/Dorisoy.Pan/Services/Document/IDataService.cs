namespace Dorisoy.PanClient.Services
{
    public interface IDataService<TModel, TKey>
    {
        IObservable<IChangeSet<TModel, TKey>> Connect();

        Task<ServiceResult<TModel>> AddAsync(TModel model);

        Task<ServiceResult<TModel>> UpdateAsync(TModel model);

        Task DeleteAsync(TModel model);
    }
}
