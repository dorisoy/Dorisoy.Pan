namespace Dorisoy.Pan.Helper
{
    public abstract class ResourceParameters
    {
        public ResourceParameters(string orderBy)
        {
            this.OrderBy = orderBy;
        }
        const int maxPageSize = 100;
        public int Skip { get; set; } =0;

        private int _pageSize = 10;
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {

                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }

        public string SearchQuery { get; set; }

        public string OrderBy { get; set; }

        public string Fields { get; set; }
    }
}
