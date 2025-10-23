namespace Dorisoy.Pan.ViewModels;

public class EventModel : ReactiveObject
{
    [Reactive] public bool HasParticipant { get; set; }
    [Reactive] public string Participant { get; set; }
    [Reactive] public string EventMessage { get; set; }

    public EventModel(string eventMessage)
    {
        EventMessage = eventMessage;
    }

    public EventModel(string participant, string eventMessage)
    {
        HasParticipant = true;
        Participant = participant;
        EventMessage = eventMessage;
    }
}


public class EventsPanelViewModel : ViewModelBase
{
    [Reactive] public ObservableCollection<EventModel> Events { get; set; } = new();
    public EventsPanelViewModel(IWhiteBoardService signalRService) : base()
    {
        try
        {
            _connection.Reconnecting += Connection_Reconnecting;
            _connection.Reconnected += Connection_Reconnected;
            _connection.On<Guid, string, string>("JoinedRoom", Connection_JoinedRoom);
            _connection.On<string>("LeftRoom", Connection_LeftRoom);
        }
        catch (Exception) { }

        this.WhenActivated((CompositeDisposable disposables) =>
        {
        });
    }


    private Task Connection_Reconnecting(Exception arg)
    {
        Events.Add(new EventModel("正在连接..."));
        return Task.CompletedTask;
    }

    /// <summary>
    /// 已连接
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    private Task Connection_Reconnected(string arg)
    {
        Events.Add(new EventModel("已连接."));
        return Task.CompletedTask;
    }

    /// <summary>
    /// 加入
    /// </summary>
    /// <param name="participant"></param>
    private void Connection_JoinedRoom(Guid uid, string nickname, string ip)
    {
        Events.Add(new EventModel(nickname, "已加入."));
    }

    /// <summary>
    /// 离开
    /// </summary>
    /// <param name="participant"></param>
    private void Connection_LeftRoom(string participant)
    {
        Events.Add(new EventModel(participant, "已离开."));
    }
}

