namespace Dorisoy.Pan.Models;

public class CameraDevice : ReactiveObject
{
    [Reactive] public int OpenCvId { get; set; }
    [Reactive] public string Name { get; set; }
    [Reactive] public string DeviceId { get; set; }
}
