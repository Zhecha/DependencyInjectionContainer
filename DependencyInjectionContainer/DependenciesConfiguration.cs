using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer
{
    public class DependenciesConfiguration
    {
        private Dictionary<Type, List<Dependency>> pairs;
        
        public DependenciesConfiguration()
        {
            pairs = new Dictionary<Type, List<Dependency>>();
        }

        public IEnumerable<Dependency> ReturnDependencies(Type type)
        {
            List<Dependency> dependencies;
            if (pairs.TryGetValue(type, out dependencies))
                return dependencies;
            return null;
        }

        public Dependency ReturnDependency(Type type)
        {
            List<Dependency> dependencies;
            if (pairs.TryGetValue(type, out dependencies))
                return dependencies.Last();
            return null;
        }

        public void Register(Type Interface, Type realiz, bool Singleton = false)
        {
            List<Dependency> dependencies;
            var dependency = new Dependency(Interface,realiz, Singleton);
            lock (pairs)
            {
                if (!pairs.TryGetValue(Interface, out dependencies))
                {
                    pairs.Add(Interface, new List<Dependency> { dependency });
                    return;
                }
                   
            }

            lock (dependencies)
            {
                if (!dependencies.Contains(dependency))
                    dependencies.Add(dependency);
            }
        }
    }
}
