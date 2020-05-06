using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Forum.DataAccess.Specification
{
    public class BaseSpecification<T> : ISpecification<T>
    {
        // left empty to evaluate error in PostWithSpecification
        // will use for list view
        public BaseSpecification()
        {
        }

        // will use for detail id
        public BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        public Expression<Func<T, bool>> Criteria { get; }

        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();

        protected void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }
    }
}
