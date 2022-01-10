using System;
using System.Linq;
using System.Linq.Expressions;

namespace Dorisoy.Pan.Helper
{
    public class GenericSpecification<T>
    {
        private readonly  Expression<Func<T, bool>> expression;

        public GenericSpecification(Expression<Func<T,bool>> _expression)
        {
            this.expression = _expression;
        }

        public bool IsSatifiedBy(T entity)
        {
          return  this.expression.Compile().Invoke(entity);
        }
    }

    internal sealed class IdentificationSpecification<T> : Specification<T>
    {
        public override Expression<Func<T, bool>> ExpressionTo()
        {
            return x => true;
        }
    }

    public abstract class Specification<T>
    {
        public static readonly Specification<T> All = new IdentificationSpecification<T>();
       
        public bool IsSatified(T entity)
        {
            Func<T, Boolean> predicate = this.ExpressionTo().Compile();
            return predicate.Invoke(entity);
        }
      
        public abstract Expression<Func<T, bool>> ExpressionTo();

        public Specification<T> And(Specification<T> specification)
        {
            if (this == All)
                return specification;
            if (specification == All)
                return this;
            return new AndSpecification<T>(this, specification);
        }
        public Specification<T> Or(Specification<T> specification)
        {
            if (this == All || specification == All)
                return All;
            return new OrSpecification<T>(this, specification);
        }
        public Specification<T> Not(Specification<T> specification)
        {
            return new NotSpecification<T>(this);
        }
    }

    internal sealed class AndSpecification<T> : Specification<T>
    {
        private readonly Specification<T> left;
        private readonly Specification<T> right;
        public AndSpecification(Specification<T> _left, Specification<T> _right)
        {
            left = _left;
            right = _right;
        }
        public override Expression<Func<T, bool>> ExpressionTo()
        {
            Expression<Func<T, bool>> leftPredicate = left.ExpressionTo();
            Expression<Func<T, bool>> rightPredicate = right.ExpressionTo();

            BinaryExpression andExpression = Expression.AndAlso(leftPredicate.Body, rightPredicate.Body);
            return Expression.Lambda<Func<T,bool>>(andExpression, leftPredicate.Parameters.Single());
        }

       }

    internal sealed class OrSpecification<T> : Specification<T>
    {
        private readonly Specification<T> left;
        private readonly Specification<T> right;

        public OrSpecification(Specification<T> _left, Specification<T> _right)
        {
            left = _left;
            right = _right;
        }
        public override Expression<Func<T, bool>> ExpressionTo()
        {
            Expression<Func<T, bool>> leftPredicate = left.ExpressionTo();
            Expression<Func<T, bool>> rightPredicate = right.ExpressionTo();

            BinaryExpression expression = Expression.OrElse(leftPredicate.Body, rightPredicate.Body);

            return Expression.Lambda<Func<T, bool>>(expression, leftPredicate.Parameters.Single());
           
        }
    }

    internal sealed class NotSpecification<T>:Specification<T>
    {
        private readonly Specification<T> specification;
        public NotSpecification(Specification<T> _specification)
        {
            specification = _specification;
        }

        public override Expression<Func<T, bool>> ExpressionTo()
        {
            Expression<Func<T, bool>> predicate= specification.ExpressionTo();

            UnaryExpression expression = Expression.Not(predicate.Body);
            return Expression.Lambda<Func<T, bool>>(expression, predicate.Parameters.Single());

        }
    }

}
