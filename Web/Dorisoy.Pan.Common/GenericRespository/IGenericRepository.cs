using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Dorisoy.Pan.Common.GenericRespository
{
    public interface IGenericRepository<TC>
        where TC : class
    {
        IQueryable<TC> All { get; }
        IQueryable<TC> AllIncluding(params Expression<Func<TC, object>>[] includeProperties);
        //Task<IEnumerable<TC>> AllIncludingAsync(params Expression<Func<TC, object>>[] includeProperties);
        IQueryable<TC> FindByInclude(Expression<Func<TC, bool>> predicate, params Expression<Func<TC, object>>[] includeProperties);
        //Task<IEnumerable<TC>> FindByIncludeAsync(Expression<Func<TC, bool>> predicate, params Expression<Func<TC, object>>[] includeProperties);
        IQueryable<TC> FindBy(Expression<Func<TC, bool>> predicate);
        //Task<IEnumerable<TC>> FindByAsync(Expression<Func<TC, bool>> predicate);
        //IQueryable<TC> FindOnly(Expression<Func<TC, bool>> predicate);
        TC Find(Guid id);
        Task<TC> FindAsync(Guid id);
        void Add(TC entity);
        void Update(TC entity);
        void UpdateRange(List<TC> entities);
        void Delete(Guid id);
        void Delete(TC entity);
        void Remove(TC entity);
        void InsertUpdateGraph(TC entity);
        void RemoveRange(IEnumerable<TC> lstEntities);
        void AddRange(IEnumerable<TC> lstEntities);
    }
}
