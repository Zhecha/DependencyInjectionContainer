using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer
{
    public class Dependency
    {
        public Type _interface;
        public Type _realiz;
        public bool _singleton;
        public object Getinstance { get; set; }
        public bool isCreated { get; private set; }

        public Dependency(Type Interface, Type Realiz, bool Singleton = false)
        {
            bool check = (!Realiz.IsInterface && !Realiz.IsAbstract && (Interface.IsAssignableFrom(Realiz) || Realiz.IsGenericTypeDefinition)) ? true : false;
            if (check)
            {
                _interface = Interface;
                _realiz = Realiz;
                _singleton = Singleton;
                Getinstance = null;
                isCreated = true;
            }
        }
    }
}
