namespace Dorisoy.Pan;

/// <summary>
/// 一个执行器对象，尤其是音频和MIDI设备需要此功能。
/// 制造它们的线索一定是破坏它们的人。
/// </summary>
internal class SingleThreadDispatcher
{
    /// <summary>
    /// 一个执行器线程
    /// </summary>
    private readonly Thread executor;
    private readonly AutoResetEvent execute = new(false);
    private readonly ConcurrentQueue<Action> actions = new();

    private int disposed = 0;
    private bool noThrow;

    public SingleThreadDispatcher(bool noThrow = true)
    {
        this.noThrow = noThrow;

        executor = new Thread(ExecutionLoop)
        {
            Name = "CustomDispatcher"
        };

        executor.Start();
    }

    /// <summary>
    /// 执行循环
    /// </summary>
    private void ExecutionLoop()
    {
        while (true)
        {
            try
            {
                execute.WaitOne();
            }
            catch { }

            if (Interlocked.CompareExchange(ref disposed, 0, 0) == 1)
                return;

            while (actions.TryDequeue(out var todo))
            {
                try
                {
                    todo?.Invoke();
                }
                catch
                {
                    if (!noThrow)
                        throw;
                }
            }
        }
    }


    /// <summary>
    /// 排队
    /// </summary>
    /// <param name="todo"></param>
    public void Enqueue(Action todo)
    {
        actions.Enqueue(todo);
        execute.Set();
    }

    /// <summary>
    /// 排队阻塞
    /// </summary>
    /// <param name="todo"></param>
    /// <param name="timeout"></param>
    public void EnqueueBlocking(Action todo, int timeout=0)
    {
        ManualResetEvent mre = new ManualResetEvent(false); 
        actions.Enqueue(() => 
        {
            try
            {
                todo?.Invoke();

            }
            finally
            {
                try
                {
                    mre.Set();
                }
                catch{ }
            }
        });
        execute.Set();

        //if(Thread.CurrentThread.ManagedThreadId == executor.ManagedThreadId)
        if (Environment.CurrentManagedThreadId == executor.ManagedThreadId)
        {
            return;
        }
        _ = timeout == 0 ? mre.WaitOne() : mre.WaitOne(timeout);

    }

    /// <summary>
    /// 释放
    /// </summary>
    internal void Dispose()
    {
       Interlocked.Exchange(ref disposed, 1);
    }
}
