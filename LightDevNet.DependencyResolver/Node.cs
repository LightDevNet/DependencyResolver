namespace LightDevNet.DependencyResolver
{
    public class Node
    {
        public Node(object dependency, int deep, IDependencyResolver dependencyResolver)
        {
            this.Dependency = dependency;
            this.Deep = deep;
            this.DependencyResolver = dependencyResolver;
        }

        public object Dependency { get; protected set; }

        public int Deep { get; protected set; }

        public IDependencyResolver DependencyResolver { get; protected set; }

        public override string ToString()
        {
            return $"{this.Dependency} ({this.Dependency.GetType()}) - {this.DependencyResolver.GetType()}";
        }
    }
}
