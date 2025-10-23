using NetworkLibrary;

namespace Dorisoy.Pan.Services;

public class ServiceHub
{
    private static ServiceHub instance;
    public static ServiceHub Instance
    {
        get
        {
            if (instance == null)
                instance = new ServiceHub();
            return instance;
        }
    }


    public FileShare FileShare { get; }

    public Action<int, int> CamSizeFeedbackAvailable;
    public event Action<string, string> LogAvailable;


    private ServiceHub()
    {
 
        FileShare = new FileShare();
        PublishStatistics();
    }

    private void PublishStatistics()
    {
   
    }

    private void HandleMessage(MessageEnvelope message)
    {

    }

    public void ResetBuffers()
    {

    }

    public void Log(string logType, string log)
    {
        LogAvailable?.Invoke(logType, log);
    }
}
