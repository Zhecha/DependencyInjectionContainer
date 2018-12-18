using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer
{
    public class DependencyProvider
    {
        private DependenciesConfiguration configuration;
        private Type currType;
        private ConcurrentStack<Type> stack = new ConcurrentStack<Type>();
        private object instance = null;

        public DependencyProvider(DependenciesConfiguration Configuration)
        {
            configuration = Configuration;
        }
        
        private object ReturnInst(Dependency dependency)
        {
            if(dependency._singleton)
            {
                if (dependency.Getinstance == null)
                {
                    lock (dependency)
                    {
                        if (dependency.Getinstance == null)
                            dependency.Getinstance = CreateInst(dependency);
                    }
                }
                return dependency.Getinstance;
            }
     
            object obj = CreateInst(dependency);
            return obj;
        }

        public IEnumerable<T> ResolveEnum<T>() where T : class
        {
            var depenEnum = configuration.ReturnDependencies(typeof(T));
            if(depenEnum != null)
            {
                IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(typeof(T)));
                foreach (var dp in depenEnum)
                    list.Add(ReturnInst(dp));
                return (IEnumerable<T>)list;
            } else
                return null;
        }

        private object CreateInst(Dependency dependency)
        {
            Type realizType = dependency._realiz;
            if(!stack.Contains(dependency._interface))
            {
                stack.Push(dependency._interface);
                if (realizType.IsGenericTypeDefinition)
                    realizType = realizType.MakeGenericType(currType.GenericTypeArguments);
                ConstructorInfo[] constructors = realizType.GetConstructors().OrderByDescending(z => z.GetParameters().Length).ToArray();
                Type type;
                stack.TryPop(out type);
                if (TryCreateInst(constructors, realizType))
                    return instance;
                else
                    return null;
            }
            return instance;
        }

        private bool TryCreateInst(ConstructorInfo[] infos, Type type)
        {
            bool result = false;
            for(int i = 0; (i <= infos.Count() - 1) && !result; i++)
            {
                try
                {
                    ConstructorInfo constructor = infos[i];
                    ParameterInfo[] parameters = constructor.GetParameters();
                    var paramArray = new object[parameters.Length];
                    for (int j = 0; j < paramArray.Length; j++)
                        paramArray[j] = ReturnInst(configuration.ReturnDependency(parameters[j].ParameterType));
                    instance = Activator.CreateInstance(type, paramArray);
                    return true;
                }
                catch
                {
                    result = false;
                }
            }
            return false;
        }

        public T Resolve<T>() where T : class
        {
            Dependency dependency = configuration.ReturnDependency(typeof(T));
            if ((dependency == null) && (typeof(T).IsGenericType))
                dependency = configuration.ReturnDependency(typeof(T).GetGenericTypeDefinition());
            if(dependency != null)
            {
                if (dependency.isCreated)
                {
                    currType = typeof(T);
                    return (T)ReturnInst(dependency);
                }
            }
            return null;
        }

    }
}
