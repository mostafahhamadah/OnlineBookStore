using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bulky.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {

        private readonly ApplicationDBContext _db;
        internal DbSet<T> dbSet;
        public Repository(ApplicationDBContext db)
        {
                _db= db;
            this.dbSet = _db.Set<T>();
            //that's how you refer to the entity of the foriegn key. which is category 
            //we can add multiple include statements 
            _db.products.Include(u => u.category).Include(u=>u.CategoryId);
        }
        public void add(T entity)
        {
            //we can't say _db.categories.add() as we did before in the regular crud operations but 
            // have to implement the dbSet class first to the genric class we created
            // then we set this dbset class to
            // the set method of that genric class this we can use _db.categories.Add() by saying this.dbset 
             dbSet.Add(entity);


        }

        public T Get(Expression<Func<T, bool>> filter,string? includeproperties=null, bool tracked = false)
        {
            IQueryable<T> query;
            if (tracked)
            {
                query = dbSet;
             
            }
            else
            {
                 query = dbSet.AsNoTracking();
            }
            query = query.Where(filter);
            if (!String.IsNullOrEmpty(includeproperties))
            {
                foreach (var includeprop in includeproperties.Split(new char[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeprop);

                }
            }


            return query.FirstOrDefault();


        }
        //Category,CoverType >> must match the include statement ,, case sensitive! 
        //you can pass many props and separete between them by ','. 
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter , string? includeproperties = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!String.IsNullOrEmpty(includeproperties))
            {
                foreach(var includeprop in includeproperties.Split(new char[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries))
                    {
                    query = query.Include(includeprop); 

                    }
            }


            return query.ToList();
        }

       
        public void remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void removeRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }
    }
}
