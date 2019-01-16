namespace LightDevNet.DependencyResolver.Resolvers
{
    using System;
    using System.Collections.Generic;

    public class ReflectionResolver : IDependencyResolver
    {
        protected bool IncludeProperties { get; set; }

        protected bool IncludeConstructors { get; set; }

        protected bool IncludeMethods { get; set; }

        public ReflectionResolver Properties(bool properties = true)
        {
            this.IncludeProperties = properties;
            return this;
        }

        public ReflectionResolver Constructors(bool constructors = true)
        {
            this.IncludeConstructors = constructors;
            return this;
        }

        public ReflectionResolver Methods(bool methods = true)
        {
            this.IncludeMethods = methods;
            return this;
        }

        public virtual IEnumerable<object> Resolve(object any)
        {
            var type = any as Type ?? any.GetType();
            if (this.IncludeConstructors)
            {
                foreach (var d in this.ResolveConstructors(type))
                {
                    yield return d;
                }
            }

            if (this.IncludeProperties)
            {
                foreach (var d in this.ResolveProperties(type))
                {
                    yield return d;
                }
            }

            if (this.IncludeMethods)
            {
                foreach (var d in this.ResolveMethods(type))
                {
                    yield return d;
                }
            }
        }

        protected virtual IEnumerable<object> ResolveConstructors(Type type)
        {
            foreach (var c in type.GetConstructors())
            {
                foreach (var p in c.GetParameters())
                {
                    yield return p;
                }
            }
        }

        protected virtual IEnumerable<object> ResolveMethods(Type type)
        {
            foreach (var m in type.GetMethods())
            {
                yield return m.ReturnParameter;
                foreach (var p in m.GetParameters())
                {
                    yield return p;
                }
            }
        }

        protected virtual IEnumerable<object> ResolveProperties(Type type)
        {
            return type.GetProperties();
        }
    }
}