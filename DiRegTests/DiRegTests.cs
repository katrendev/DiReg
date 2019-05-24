using Katren.DiReg;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AutoRegisterTests
{
    public interface IServiceInterface { }

    [DiReg(typeof(IServiceInterface), DiLifetime.Singleton)]
    public class ServiceByInterface: IServiceInterface { }

    [DiReg(DiLifetime.Singleton)]
    public class SingletoneService { }

    [DiReg(DiLifetime.Transient)]
    public class TransientService { }

    [DiReg(DiLifetime.Scoped)]
    public class ScopedService { }


    [TestClass]
    public class DiRegTests
    {
        private static ServiceProvider _provider;
        private static ServiceCollection _services;

        [ClassInitialize]
        public static void TestsInit(TestContext testContext)
        {
            _services = new ServiceCollection();
            _services.AddDiRegClasses(typeof(SingletoneService).Assembly);
            _provider = _services.BuildServiceProvider();
        }

        [ClassCleanup]
        public static void TestsCleanup()
        {
            _provider.Dispose();
            _provider = null;
        }

        [TestMethod]
        public void Singletone_GetTwice_SameInstances()
        {
            IServiceProvider scopedProvider = _provider.CreateScope().ServiceProvider;
            SingletoneService singletone1 = _provider.GetService<SingletoneService>();
            SingletoneService singletone2 = _provider.GetService<SingletoneService>();
            SingletoneService singletone3 = scopedProvider.GetService<SingletoneService>();

            Assert.AreEqual(singletone1, singletone2);
            Assert.AreEqual(singletone2, singletone3);
        }

        [TestMethod]
        public void SingletoneByInterface_GetTwice_SameInstances()
        {
            IServiceProvider scopedProvider = _provider.CreateScope().ServiceProvider;
            IServiceInterface singletone1 = _provider.GetService<IServiceInterface>();
            IServiceInterface singletone2 = _provider.GetService<IServiceInterface>();
            IServiceInterface singletone3 = scopedProvider.GetService<IServiceInterface>();

            Assert.AreEqual(singletone1, singletone2);
            Assert.AreEqual(singletone2, singletone3);
        }

        [TestMethod]
        public void RegisteredByInterfacee_GetByType_ReturnsNull()
        {
            IServiceProvider scopedProvider = _provider.CreateScope().ServiceProvider;
            ServiceByInterface service1 = _provider.GetService<ServiceByInterface>();
            ServiceByInterface service2 = scopedProvider.GetService<ServiceByInterface>();

            Assert.IsNull(service1);
            Assert.IsNull(service2);
        }

        [TestMethod]
        public void Transient_GetTwice_DifferentInstances()
        {
            IServiceProvider scopedProvider = _provider.CreateScope().ServiceProvider;
            TransientService transient1 = _provider.GetService<TransientService>();
            TransientService transient2 = _provider.GetService<TransientService>();
            TransientService transient3 = scopedProvider.GetService<TransientService>();

            Assert.AreNotEqual(transient1, transient2);
            Assert.AreNotEqual(transient1, transient3);
            Assert.AreNotEqual(transient2, transient3);
        }

        [TestMethod]
        public void Scoped_GetService_SameInOneScope()
        {
            IServiceProvider scopedProvider1 = _provider.CreateScope().ServiceProvider;
            IServiceProvider scopedProvider2 = _provider.CreateScope().ServiceProvider;

            ScopedService scoped11 = scopedProvider1.GetService<ScopedService>();
            ScopedService scoped12 = scopedProvider1.GetService<ScopedService>();
            ScopedService scoped21 = scopedProvider2.GetService<ScopedService>();
            ScopedService scoped22 = scopedProvider2.GetService<ScopedService>();

            Assert.AreEqual(scoped11, scoped12);
            Assert.AreEqual(scoped21, scoped22);
            Assert.AreNotEqual(scoped11, scoped22);
        }
    }
}
