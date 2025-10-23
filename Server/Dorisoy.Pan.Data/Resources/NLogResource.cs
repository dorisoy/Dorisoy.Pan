namespace Dorisoy.Pan.Data.Resources
{
    public class NLogResource : ResourceParameter
    {
        public NLogResource() : base("Logged")
        {
        }

        public string Message { get; set; }
        public string Level { get; set; }
        public string Source { get; set; }
    }
}
