using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class 
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter=null, string? includeproperties=null);
  
        //this is an expression where you want to pass a linq operator or
        //an expression that takes an input T and the output is bool
        // like (u=> u.id==id) for example 
        T Get(Expression<Func<T, bool>> filter, string? includeproperties = null,bool tracked = false);
        void add(T entity);
        void remove(T entity);
        void removeRange(IEnumerable<T> entity);
    }
    
}
