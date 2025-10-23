namespace Dorisoy.Pan.Models
{
    public class PageModel : BaseModel
    {
        public Guid Id { get; set; }
        [Reactive] public string Name { get; set; }
        [Reactive] public string Title { get; set; }
        [Reactive] public int Order { get; set; }
    }
}
