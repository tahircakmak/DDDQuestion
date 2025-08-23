using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FrameworkX.Common
{
    public class EFRepositoryBase<TDbContext, TModel> : IRepository<TModel>
         where TDbContext : DbContext
        where TModel : class
    {
        protected readonly TDbContext db;
        public EFRepositoryBase(TDbContext db)
        {
            this.db = db;
        }

        public virtual TModel FindById(int primaryFieldValue)
        {
            return db.Set<TModel>().Find(primaryFieldValue);
        }

        public virtual IQueryable<TModel> Find(Expression<Func<TModel, bool>> filterExpression)
        {
            return db.Set<TModel>().Where(filterExpression);
        }

        public virtual void Insert(TModel model)
        {
            db.Set<TModel>().Add(model);
        }

        public virtual void Delete(TModel model)
        {
            if (db.Entry(model).State == EntityState.Detached)
            {
                db.Set<TModel>().Attach(model);
                db.Entry(model).State = EntityState.Unchanged;
            }

            db.Set<TModel>().Remove(model);
        }
    }
}
