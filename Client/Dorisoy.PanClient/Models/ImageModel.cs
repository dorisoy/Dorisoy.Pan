using Avalonia.Media.Imaging;


namespace Dorisoy.PanClient.Models
{
    public class ImageModel : ReactiveObject
    {
        [Reactive] public Guid Id { get; set; } = Guid.NewGuid();
        [Reactive] public DateTime CeateTime { get; set; } = DateTime.Now;
        [Reactive] public string SourcePath { get; set; }
        [Reactive] public string Name { get; set; }
        [Reactive] public string Auther { get; set; } = "Sinol";
        [Reactive] public Bitmap Cover { get; set; }
        [Reactive] public string Icon { get; set; }
    }
}
