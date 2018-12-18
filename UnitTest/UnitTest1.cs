using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DependencyInjectionContainer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        public interface IClass1 { }
        public abstract class Class1 : IClass1 { }
        public interface IFoo { }
        public class Class : Class1 { }
        public class Foo : IFoo { }
        public class Foo1 : IFoo { }
        public class Bar : IFoo
        {
            public Class Class;
            public Bar(Class1 class1)
            {
                Class = (Class)class1;
            }
        }
        public class GenClass<T>
        {
            T field;
        }
        public interface IService<T> { }
        public class ServiceImpl<T> : IService<T> { }

        [TestMethod]
        public void TestNoSingleton()
        {
            var dependencies = new DependenciesConfiguration();
            dependencies.Register(typeof(IFoo), typeof(Foo), false);
            var provider = new DependencyProvider(dependencies);
            IFoo foo11 = provider.Resolve<IFoo>();
            IFoo foo21 = provider.Resolve<IFoo>();
            Assert.AreNotEqual(foo11,foo21);
        }

        [TestMethod]
        public void TestSingleton()
        {
            var dependencies = new DependenciesConfiguration();
            dependencies.Register(typeof(IFoo), typeof(Foo), true);
            var provider = new DependencyProvider(dependencies);
            IFoo foo1 = provider.Resolve<IFoo>();
            IFoo foo2 = provider.Resolve<IFoo>();
            Assert.AreEqual(foo1,foo2);
        }

        [TestMethod]
        public void TestCountDependencies()
        {
            var dependencies = new DependenciesConfiguration();
            dependencies.Register(typeof(IFoo), typeof(Foo), false);
            dependencies.Register(typeof(IFoo), typeof(Foo1), false);
            var provider = new DependencyProvider(dependencies);
            var objects = provider.ResolveEnum<IFoo>();
            Assert.AreEqual(objects.Count(),2);
        }

        [TestMethod]
        public void TestGenClass()
        {
            var dependencies = new DependenciesConfiguration();
            dependencies.Register(typeof(IFoo), typeof(Foo));
            dependencies.Register(typeof(GenClass<IFoo>), typeof(GenClass<IFoo>));
            var provider = new DependencyProvider(dependencies);
            GenClass<IFoo> obj = provider.Resolve<GenClass<IFoo>>();
            Assert.IsNotNull(obj);
        }
    }
}
