namespace POS.Data.Resources
{

    public abstract class ResourceParameter
    {
        public ResourceParameter(string orderBy)
        {
            this.OrderBy = orderBy;
        }
        const int maxPageSize = 100;
        public int Skip { get; set; } = 0;

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

    public class QueryResource : ResourceParameter
    {
        public QueryResource() : base("Email")
        {
        }

        public string Name { get; set; }
    }
}
