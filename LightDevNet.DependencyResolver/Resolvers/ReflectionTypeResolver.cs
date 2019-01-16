namespace LightDevNet.DependencyResolver.Resolvers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ReflectionTypeResolver : ReflectionResolver
    {
        public override IEnumerable<object> Resolve(object any)
        {
            var type = any as Type ?? any.GetType();
            if (type.IsPrimitive || type == typeof(string) || type == typeof(DateTime))
            {
                return Enumerable.Empty<object>();
            }

            if (type.IsGenericType && type.Namespace.StartsWith("System."))
            {
                return type.GenericTypeArguments.SelectMany(this.ResolveWithGenerics).Distinct();
            }
            else
            {
                var types = new List<Type>();

                if (this.IncludeConstructors)
                {
                    types.AddRange(type.GetConstructors().SelectMany(x => x.GetParameters().Select(p => p.ParameterType).Distinct()));
                }

                if (this.IncludeProperties)
                {
                    types.AddRange(type.GetProperties().Select(p => p.PropertyType).Distinct());
                }

                if (this.IncludeMethods)
                {
                    types.AddRange(type.GetMethods()
                        .Where(x => !x.IsSpecialName && x.DeclaringType != typeof(object))
                        .SelectMany(x => x.GetParameters().Select(p => p.ParameterType)
                        .Distinct()));
                }

                return types.Distinct().SelectMany(this.ResolveWithGenerics).Select(x => Nullable.GetUnderlyingType(x) ?? x).Distinct();
            }
        }

        protected IEnumerable<Type> ResolveWithGenerics(Type type)
        {
            yield return type;
            foreach (var gt in type.GenericTypeArguments.SelectMany(this.ResolveWithGenerics))
            {
                yield return gt;
            }
        }
    }
}