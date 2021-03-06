using ElmahViewer.Data;
using ElmahViewer.Models;

[assembly: WebActivator.PreApplicationStartMethod(typeof(ElmahViewer.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(ElmahViewer.App_Start.NinjectWebCommon), "Stop")]

namespace ElmahViewer.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
            
            RegisterServices(kernel);
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<ICache>().To<Cache>();
            kernel.Bind<IErrorLogFactory>().To<SqlErrorLogFactory>().InRequestScope();
            kernel.Bind<IErrorDetailsViewModelFactory>().To<ErrorViewModelFactory>().InRequestScope();
            kernel.Bind<IErrorListViewModelFactory>().To<ErrorViewModelFactory>().InRequestScope();
            kernel.Bind<IElmahApplicationRepository>().To<ElmahApplicationRepository>().InRequestScope();
        }        
    }
}
