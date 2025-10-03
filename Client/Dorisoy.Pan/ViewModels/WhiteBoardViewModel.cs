namespace Dorisoy.PanClient.ViewModels;

using Point = Avalonia.Point;
public class WhiteBoardViewModel : ViewModelBase
{
    private readonly IAppState _appState;
    private readonly MakeRequest _makeRequest;
    public readonly IWhiteBoardService _whiteBoardService;

    public ReactiveCommand<Unit, Unit> CopyRoom { get; set; }
    [Reactive] public ToolsPanelViewModel ToolsPanel { get; set; }
    [Reactive] public ParticipantsPanelViewModel ParticipantsPanel { get; set; }
    [Reactive] public EventsPanelViewModel EventsPanel { get; set; }

    public string Room { get; }
    private readonly DispatcherTimer _pingTimer = new();
    private DateTime lastPing;

    /// <summary>
    /// 是否连接中
    /// </summary>
    [Reactive] public bool Connected { get; set; } = true;
    [Reactive] public int Latency { get; set; }
    [Reactive] public string ReconnectingLabel { get; set; } = "会话中...";

    private readonly SourceCache<Rooms, Guid> _items;
    public IObservable<IChangeSet<Rooms, Guid>> Connect() => _items.Connect();
    private CancellationTokenSource _cts;

    public WhiteBoardViewModel(MakeRequest makeRequest) : base()
    {
        _appState = Locator.Current.GetRequiredService<IAppState>();
        _whiteBoardService = Locator.Current.GetRequiredService<IWhiteBoardService>();
        _makeRequest = makeRequest;
        _items = new SourceCache<Rooms, Guid>(e => e.Id);

        //当前房间
        Room = _appState.Room;

        ToolsPanel = new ToolsPanelViewModel(_appState.BrushSettings);
        ParticipantsPanel = new ParticipantsPanelViewModel(_whiteBoardService);
        EventsPanel = new EventsPanelViewModel(_whiteBoardService);

        //拷贝
        CopyRoom = ReactiveCommand.CreateFromTask(async () =>
        {
            await ClipboardService.SetTextAsync(Room);
        });

        _connection.Reconnecting += Connection_Reconnecting;
        _connection.Reconnected += Connection_Reconnected;
        _connection.Closed += Connection_Closed;

        _connection.On("Pong", () =>
        {
            var diff = DateTime.Now - lastPing;
            Debug.WriteLine($"Pong....... {diff}");
            Latency = diff.Milliseconds;
        });

        _pingTimer.Tick += PingTimer_Tick;
        _pingTimer.Interval = TimeSpan.FromSeconds(2);
        _pingTimer.Start();

        //WhenActivated允许您注册要在ViewModel的视图被激活时调用的Func
        this.WhenActivated(async (CompositeDisposable disposables) =>
        {
            //加入房间 JoinRoom
            await JoinRoomAsync();

            _cts = new CancellationTokenSource();

            LoadDataCommand.Execute(_cts.Token)
            .Subscribe()
            .DisposeWith(disposables);

            System.Reactive.Disposables.Disposable.Create(() => _cts.Cancel()).DisposeWith(disposables);
        });
    }


    public async Task GetRoomsAsync()
    {
        try
        {
            _connection.On<List<Rooms>>("GetRooms", (rooms) =>
            {
                _items.AddOrUpdate(rooms);
            });

            _items.Clear();

            if (_connection.State == HubConnectionState.Disconnected)
                await _connection.StartAsync();

            await _connection.InvokeAsync("GetRoom", Globals.CurrentUser.Id);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }


    public async Task CreateRoomAsync()
    {
        try
        {
            _connection.On<string>("RoomCreated", (room) =>
            {
                _appState.Room = room;
            });

            if (_connection.State == HubConnectionState.Disconnected)
                await _connection.StartAsync();

            await _connection.InvokeAsync("CreateRoom",
                Globals.CurrentUser.Id,
                _appState.Nickname,
                _appState.Room);

            await GetRoomsAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    public async Task LeaveRoomAsync(Guid uid)
    {
        try
        {
            if (_connection.State == HubConnectionState.Disconnected)
                await _connection.StartAsync();

            //LeaveRoom
            await _connection.InvokeAsync("LeaveRoom", Globals.CurrentUser.Id);
            await GetRoomsAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// 加入房间
    /// </summary>
    /// <returns></returns>
    public async Task JoinRoomAsync()
    {
        try
        {
            if (_connection.State == HubConnectionState.Disconnected)
                await _connection.StartAsync();

            //JoinedRoom
            await _connection.InvokeAsync("JoinRoom", Globals.CurrentUser.Id, _appState.Nickname, _appState.Room, Utilities.GetLocalIP());

            await GetRoomsAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    public async Task JoinRandomRoomAsync()
    {
        try
        {
            if (_connection.State == HubConnectionState.Disconnected)
                await _connection.StartAsync();

            await _connection.InvokeAsync("JoinRandomRoom", _appState.Nickname);
            await GetRoomsAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    public async Task DrawPointAsync(double x, double y)
    {
        try
        {
            var data = PayloadConverter.ToBytes(x, y, _appState.BrushSettings.BrushThickness, _appState.BrushSettings.BrushColor);
            await _connection.InvokeAsync("DrawPoint", _appState.Nickname, _appState.Room, data);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    public async Task DrawLineAsync(List<Point> points)
    {
        try
        {
            var data = PayloadConverter.ToBytes(points, _appState.BrushSettings.BrushThickness, _appState.BrushSettings.BrushColor);
            await _connection.InvokeAsync("DrawLine", _appState.Nickname, _appState.Room, data);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }


    private Task Connection_Reconnecting(Exception arg)
    {
        _pingTimer.Stop();
        Connected = false;
        return Task.CompletedTask;
    }

    private Task Connection_Reconnected(string arg)
    {
        _pingTimer.Start();
        Connected = true;
        return Task.CompletedTask;
    }

    private Task Connection_Closed(Exception arg)
    {
        _pingTimer.Stop();
        return Task.CompletedTask;
    }

    private async void PingTimer_Tick(object sender, EventArgs e)
    {
        try
        {
            lastPing = DateTime.Now;
            System.Diagnostics.Debug.WriteLine($"Ping....... {lastPing}");
            await _connection.InvokeAsync("Ping");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// 载入数据
    /// </summary>
    /// <returns></returns>
    protected override void LoadDataAsync(CancellationToken token)
    {
        Task.Run(async () =>
        {
            //获取参与人
            var participants = await _whiteBoardService.GetParticipantsAsync(Room);

            await Dispatcher.UIThread.InvokeAsync( () =>
            {
                if (participants != null && participants.Any())
                {
                    foreach (var user in participants)
                    {
                        ParticipantsPanel.Participants.Add(new ParticipantModel(user.User, user.IPAddress));
                    }
                }
                var initialEventMessage = participants.Count == 1 ? "已创建小组." : "已加入小组.";
                EventsPanel.Events.Add(new EventModel(initialEventMessage));
            });
        }, token);
    }

    public void IndicateDrawing(string nickname)
    {
        var participant = ParticipantsPanel.Participants.FirstOrDefault(x => x.Nickname == nickname);
        if (participant != null)
        {
            participant.Drawing = true;
        }
    }
}

