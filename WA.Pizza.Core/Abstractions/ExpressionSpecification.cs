using System.Linq.Expressions;

namespace WA.Pizza.Core.Abstractions
{
	public class ExpressionSpecification<T>
	{
        public Expression<Func<T, bool>> Expression { get; }

        private Func<T, bool>? _expressionFunc;
        private Func<T, bool> ExpressionFunc => _expressionFunc ?? (_expressionFunc = Expression.Compile());

        public ExpressionSpecification(Expression<Func<T, bool>> expression)
        {
            Expression = expression;
        }

        public bool IsSatisfied(T obj)
        {
            bool result = ExpressionFunc(obj);
            return result;
        }
    }
}
