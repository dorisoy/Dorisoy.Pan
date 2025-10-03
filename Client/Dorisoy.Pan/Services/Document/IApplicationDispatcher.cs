namespace Dorisoy.PanClient.Services
{
    public interface IApplicationDispatcher
    {
        void Dispatch(Action action);

        Task DispatchAsync(Func<Task> task);
    }
}