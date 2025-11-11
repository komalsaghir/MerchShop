using System.Linq.Expressions;

namespace MerchShop.Models
{
    public class QueryOptions<T>
    {
        public Expression<Func<T, Object>> OrderBy { get; set; } = null!;
        public Expression<Func<T, bool>> Where { get; set; } = null!;
        public string OrderByDirection { get; set; } = "asc'"; //default value
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        private string[] includes = Array.Empty<string>();

        public string Includes
        {
            set => includes = value.Replace(" ", "").Split(',');
        }

        public string[] GetIncludes() => includes;

        public bool HasOrderBy => OrderBy != null;
        public bool HasWhere => Where != null;
        public bool HasPaging => PageNumber > 0 && PageSize > 0;

    }
}