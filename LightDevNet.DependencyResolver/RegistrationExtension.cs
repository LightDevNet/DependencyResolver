namespace LightDevNet.DependencyResolver
{
    using System;

    using LightDevNet.DependencyResolver.Resolvers;

    public static class RegistrationExtension
    {
        public static Resolver UseReflection(this Resolver resolver, Action<ReflectionResolver> configure)
        {
            return resolver.Use(r =>
            {
                var rr = new ReflectionResolver();
                configure(rr);
                return rr;
            });
        }

        public static Resolver UseReflectionType(this Resolver resolver, Action<ReflectionTypeResolver> configure)
        {
            return resolver.Use(r =>
            {
                var rr = new ReflectionTypeResolver();
                configure(rr);
                return rr;
            });
        }
    }
}