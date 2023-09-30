using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {


        private ApplicationDBContext _db;
        public ProductRepository(ApplicationDBContext db) : base(db)
        {
            _db=db;
        }
       

        public void update(Product product)
        {

            _db.products.Update(product);
        }
        public IEnumerable<Product> Get(Expression<Func<Product, bool>> filter = null,
                                    Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = null,
                                    string includeProperties = null)
        {
            IQueryable<Product> query = _db.products; // Use the actual DbSet, in this case _db.products

            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Eager loading of related entities
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            // Ordering and other query operations
            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return query.ToList();
        }


    }
}
