namespace LightDevNet.DependencyResolver
{
    using System.Collections.Generic;

    public interface IDependencyResolver
    {
        IEnumerable<object> Resolve(object any);
    }
}