namespace Dorisoy.Pan.ViewModels;

public class ParticipantsPanelViewModel : ViewModelBase
{
    public ParticipantsPanelViewModel(IWhiteBoardService whiteBoardService) : base()
    {
        try
        {
            _connection.On<Guid, string, string>("JoinedRoom", Connection_JoinedRoom);
            _connection.On<string>("LeftRoom", Connection_LeftRoom);

        }
        catch (Exception) { }

        this.WhenActivated((CompositeDisposable disposables) =>
        {

        });
    }

    [Reactive] public ObservableCollection<ParticipantModel> Participants { get; set; } = new();

    private void Connection_JoinedRoom(Guid uid, string nickname, string ip)
    {
        Participants.Add(new ParticipantModel(nickname, ip));
    }

    /// <summary>
    /// 用户离开时清理列表
    /// </summary>
    /// <param name="nickname"></param>
    private void Connection_LeftRoom(string nickname)
    {
        var participant = Participants.FirstOrDefault(x => x.Nickname == nickname);
        Participants.Remove(participant);
    }

}


public class ParticipantModel : ReactiveObject
{
    private readonly DispatcherTimer _drawingIndicatorTimer = new();

    [Reactive] public string Nickname { get; set; }
    [Reactive] public string IPAddress { get; set; }

    public ParticipantModel(string nickname, string ip)
    {
        Nickname = nickname;
        IPAddress = ip;

        _drawingIndicatorTimer.Tick += DrawingIndicatorTimer_Tick;
        _drawingIndicatorTimer.Interval = TimeSpan.FromSeconds(1);

    }


    private bool drawing;
    public bool Drawing
    {
        get => drawing;
        set
        {
            this.RaiseAndSetIfChanged(ref drawing, value);
            if (!_drawingIndicatorTimer.IsEnabled)
            {
                _drawingIndicatorTimer.Start();
            }
        }
    }

    private void DrawingIndicatorTimer_Tick(object sender, EventArgs e)
    {
        Drawing = false;
        _drawingIndicatorTimer.Stop();
    }
}
