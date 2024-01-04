using MongoDB.Driver;
using System.Linq.Expressions;
namespace Application.Common.Helpers
{
    public class ProjectionHelper
    {
        public static ProjectionDefinition<T> GetProjectionDefinition<T>()
        {
            var fieldNames = GetFieldNames(typeof(T));
            var builder = new ProjectionDefinitionBuilder<T>();

            var projections = fieldNames
                .Select(fieldName => builder.Include(fieldName)).ToList();

            return builder.Combine(projections);
        }

        private static IEnumerable<string> GetFieldNames(Type type)
        {
            return type.GetProperties().Select(property => property.Name).ToList();
        }

        public static ProjectionDefinition<TSource, TDestination> GetProjectionDefinition<TSource, TDestination>()
        {
            var sourceProperties = typeof(TSource).GetProperties();
            var destinationProperties = typeof(TDestination).GetProperties();

            var parameter = Expression.Parameter(typeof(TSource), "x");
            var bindings = destinationProperties.Select(destProp =>
            {
                var sourceProp = sourceProperties.FirstOrDefault(srcProp => srcProp.Name == destProp.Name);
                if (sourceProp != null)
                {
                    var sourcePropExpr = Expression.Property(parameter, sourceProp);
                    var binding = Expression.Bind(destProp, sourcePropExpr);
                    return binding;
                }
                return null;
            }).Where(binding => binding != null);

            var expression = Expression.Lambda<Func<TSource, TDestination>>(
                Expression.MemberInit(Expression.New(typeof(TDestination)), bindings), parameter);

            return Builders<TSource>.Projection.Expression(expression);
        }
    }
}
