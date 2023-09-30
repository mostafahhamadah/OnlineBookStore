using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class CompanyRepository :Repository<Company> , ICompanyRepository
    {

        private ApplicationDBContext _db;
        public CompanyRepository(ApplicationDBContext db): base(db) 
        {
            _db = db;
        }


     

        public void update(Company company)
        {
            _db.Companies.Update(company);
        }
    }
}
