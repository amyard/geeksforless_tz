using Forum.DataAccess.Specification;
using Forum.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Forum.DataAccess
{
    public class SpecificationEvaluator<TEntity> where TEntity : BaseEntity
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecification<TEntity> spec)
        {
            var query = inputQuery;
            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }


            // paggination
            if(spec.IsPagingEnabled)
            {
                query = query.Skip(spec.Skip).Take(spec.Take); 
            }


            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

            return query.OrderByDescending(q => q.Id);
        }
    }
}