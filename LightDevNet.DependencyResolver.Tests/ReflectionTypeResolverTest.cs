namespace LightDevNet.DependencyResolver.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ReflectionTypeResolverTest
    {
        [TestMethod]
        public void SubListTest()
        {
            var result = this.RunTest<TestClass>();

            Assert.AreEqual("Int32|String|DateTime|List`1|TestSubClass|TestSubClass|DateTime", result);
        }

        [TestMethod]
        public void LinkedTest()
        {
            var result = this.RunTest<LinkedTestClass>();

            Assert.AreEqual("TestClass|Int32|String|DateTime|List`1|TestSubClass|String", result);
        }

        protected virtual string RunTest<T>() where T:new()
        {
            var resolver = new Resolver()
                .Deep(2)
                .UseReflectionType(x => x.Constructors().Properties().Methods());

            var enumerableResult = resolver.Resolve(new T()).ToArray();
            return string.Join('|', enumerableResult.Select(x => ((Type)x.Dependency).Name));
        }

        public class TestClass
        {
            public int IntPr { get; set; }

            public string StrPr { get; set; }

            public DateTime DateTimePr { get; set; }

            public List<TestSubClass> TestSubClassListPr { get; set; }

            public class TestSubClass
            {
                public DateTime? NullableDateTimePr { get; set; }
            }
        }

        public class LinkedTestClass
        {
            public TestClass TestClassPr { get; set; }

            public string StrPr { get; set; }
        }
    }
}