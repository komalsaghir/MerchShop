using MerchShop.Models.DataAccessLayer;
using Microsoft.EntityFrameworkCore;

namespace MerchShop.Models
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected MerchShopContext _context { get; set; }
        private DbSet<T> _dbset { get; set; }

        public Repository(MerchShopContext context)
        {
            _context = context;
            _dbset = context.Set<T>();
        }
        //new
        public int Count => _dbset.Count();

        public virtual IEnumerable<T> List(QueryOptions<T> options) => BuildQuery(options).ToList();

		public virtual List<T>? Get() => _dbset.ToList();

		public virtual T? Get(int id) => _dbset.Find(id);
        public virtual T? Get(string id) => _dbset.Find(id);
        public virtual T? Get(QueryOptions<T> options) => BuildQuery(options).FirstOrDefault();

        public virtual void Insert(T entity) => _dbset.Add(entity);
        public virtual void Update(T entity) => _dbset.Update(entity);
        public virtual void Delete(T entity) => _dbset.Remove(entity);
        public virtual void Save() => _context.SaveChanges();

        private IQueryable<T> BuildQuery(QueryOptions<T> options)
        {
            IQueryable<T> query = _dbset;
            foreach (string include in options.GetIncludes())
            {
                query = query.Include(include);
            }

            if (options.HasWhere)
                query = query.Where(options.Where);

            if (options.HasOrderBy)
                if (options.OrderByDirection == "asc")
                {
                    query = query.OrderBy(options.OrderBy);
                }
                else
                {
                    query = query.OrderByDescending(options.OrderBy);
                }

            if (options.HasPaging)
                query = query.PageBy(options.PageNumber, options.PageSize);

            return query;

        }

    }
}
