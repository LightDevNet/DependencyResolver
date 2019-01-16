namespace LightDevNet.DependencyResolver
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Resolver : IDependencyResolver
    {
        private static List<Action<Resolver>> configs = new List<Action<Resolver>>();

        private List<Func<Resolver, IDependencyResolver>> dependencyResolverFactories = new List<Func<Resolver, IDependencyResolver>>();

        private IDependencyResolver[] dependencyResolvers;
        private int currentDeep;

        private int maxDeep;
        private bool throwIfNull;
        private bool resolveSelf;

        public Resolver()
        {
            foreach (var c in configs)
            {
                c(this);
            }
        }

        public static void Configure(Action<Resolver> configure)
        {
            configs.Add(configure);
        }

        public Resolver Use<T>(T dependencyResolver) where T : IDependencyResolver
        {
            this.dependencyResolverFactories.Add(x => dependencyResolver);
            return this;
        }

        public Resolver Use(Func<Resolver, IDependencyResolver> dependencyResolverFactory)
        {
            this.dependencyResolverFactories.Add(dependencyResolverFactory);
            return this;
        }

        public Resolver Deep(int deep)
        {
            this.maxDeep = deep;
            return this;
        }

        public Resolver ResolveSelf(bool resolveSelf)
        {
            this.resolveSelf = resolveSelf;
            return this;
        }

        public Resolver ThrowIfNull(bool throwIfNull)
        {
            this.throwIfNull = throwIfNull;
            return this;
        }

        public virtual IEnumerable<Node> Resolve(object any)
        {
            if (this.currentDeep >= this.maxDeep)
            {
                // return
            }
            else if (any == null)
            {
                if (this.throwIfNull)
                {
                    throw new ArgumentNullException();
                }
            }
            else
            {
                if (this.dependencyResolvers == null)
                {
                    if (this.resolveSelf)
                    {
                        yield return new Node(any, this.currentDeep, null);
                    }

                    this.dependencyResolvers = this.dependencyResolverFactories.Select(x => x(this)).ToArray();
                }

                this.currentDeep++;
                foreach (var resolver in this.dependencyResolvers)
                {
                    foreach (var dependency in resolver.Resolve(any))
                    {
                        yield return new Node(dependency, this.currentDeep, resolver);
                        foreach (var childNode in this.Resolve(dependency))
                        {
                            yield return childNode;
                        }
                    }
                }

                this.currentDeep--;
            }
        }

        IEnumerable<object> IDependencyResolver.Resolve(object any)
        {
            return this.Resolve(any);
        }
    }
}